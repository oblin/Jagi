using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using JagiCore.Services;
using JagiCore.Interfaces;
using NSubstitute;
using JagiCore;
using System.Data;

namespace JagiCoreTests
{
    public class AddressServiceTests
    {
        [Fact]
        public void Empty_Cache_Not_Found_County_Realm_By_Zip()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new AddressService(memoryCache);
            var result = service.GetByZip("235");

            Assert.True(result.IsFailure);
        }
        
        [Fact]
        public void Can_Generate_Empty_Cache_By_Address()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new AddressService(memoryCache);
            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "" });

            var result = service.GetByZip("235");
            Assert.True(result.IsSuccess);

            var counties = result.Value.Counties;
            Assert.Equal(1, counties.Length);
            Assert.Equal("新北市", counties[0]);

            var realms = result.Value.Realms;
            Assert.Equal(1, realms.Length);
            Assert.Equal("中和區", realms[0]);

            var streets = result.Value.Streets;
            Assert.Equal(0, streets.Length);
        }

        [Fact]
        public void Can_Add_Item_To_Cache()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new AddressService(memoryCache);
            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "" });

            var result = service.GetByZip("235");
            Assert.True(result.IsSuccess);

            var counties = result.Value.Counties;
            Assert.Equal(1, counties.Length);
            Assert.Equal("新北市", counties[0]);

            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中正路" });

            result = service.GetByZip("235");

            counties = result.Value.Counties;
            Assert.Equal(1, counties.Length);
            Assert.Equal("新北市", counties[0]);

            counties = result.Value.Counties;
            Assert.Equal(1, counties.Length);
            Assert.Equal("新北市", counties[0]);

            var streets = result.Value.Streets;
            Assert.Equal(1, streets.Length);
            Assert.Equal("中正路", streets[0]);
        }

        [Fact]
        public void Get_Items_From_Cache_By_County()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new AddressService(memoryCache);
            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中正路" });
            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中山路" });
            service.Add(new Address { County = "台北市", Zip = "100", Realm = "中正區", Street = "八德路1段" });
            service.Add(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路4段" });

            var result = service.GetByCounty("新北市");
            Assert.True(result.IsSuccess);
            var streets = result.Value.Streets;
            Assert.Equal(2, streets.Length);
            Assert.Equal("中正路", streets[0]);

            var zips = result.Value.Zips;
            Assert.Equal(1, zips.Length);
            Assert.Equal("235", zips[0]);

            result = service.GetByCounty("台北市");
            Assert.True(result.IsSuccess);
            streets = result.Value.Streets;
            Assert.Equal(2, streets.Length);
            Assert.Equal("八德路1段", streets[0]);

            zips = result.Value.Zips;
            Assert.Equal(2, zips.Length);
            Assert.Equal("100", zips[0]);
        }

        [Fact]
        public void Get_Items_From_Cache_By_Realm()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new AddressService(memoryCache);
            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中正路" });
            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中山路" });
            service.Add(new Address { County = "台北市", Zip = "100", Realm = "中正區", Street = "八德路1段" });
            service.Add(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路4段" });

            var result = service.GetByRealm("中和區");
            Assert.True(result.IsSuccess);
            var streets = result.Value.Streets;
            Assert.Equal(2, streets.Length);
            Assert.Equal("中正路", streets[0]);

            var zips = result.Value.Zips;
            Assert.Equal(1, zips.Length);
            Assert.Equal("235", zips[0]);
        }

        [Fact]
        public void Can_Remove_Item_If_Have_Other_Streets()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new AddressService(memoryCache);
            service.Add(new Address { County = "台北市", Zip = "100", Realm = "中正區", Street = "八德路1段" });
            service.Add(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路4段" });
            service.Add(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路6段" });

            service.Remove(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路4段" });

            var result = service.GetByCounty("台北市");
            var zips = result.Value.Zips;
            Assert.Equal(2, zips.Length);
            Assert.Equal("100", zips[0]);
            Assert.Equal("110", zips[1]);

            var streets = result.Value.Streets;
            Assert.Equal(2, streets.Length);
            Assert.Equal("八德路1段", streets[0]);
            Assert.Equal("信義路6段", streets[1]);

            result = service.GetByRealm("信義區");
            Assert.Equal(1, result.Value.Counties.Length);
            Assert.Equal(1, result.Value.Zips.Length);
            Assert.Equal(1, result.Value.Streets.Length);
        }

        [Fact]
        public void Can_Remove_Item_If_No_Street()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new AddressService(memoryCache);
            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中正路" });
            service.Add(new Address { County = "台北市", Zip = "100", Realm = "中正區", Street = "八德路1段" });
            service.Add(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路4段" });
            service.Add(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路6段" });

            service.Remove(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中正路" });

            var result = service.GetByCounty("新北市");
            Assert.True(result.IsFailure);

            result = service.GetByZip("235");
            Assert.True(result.IsFailure);

            // 額外確認未刪除的項目還是有值
            result = service.GetByZip("110");
            Assert.Equal(1, result.Value.Zips.Length);
            Assert.Equal(2, result.Value.Streets.Length);
        }

        [Fact]
        public void Update_Item_By_Add_And_Remove()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new AddressService(memoryCache);
            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "興南路1段" });
            service.Add(new Address { County = "台北市", Zip = "100", Realm = "中正區", Street = "八德路1段" });
            service.Add(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路4段" });
            service.Add(new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路6段" });

            service.Remove(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "興南路1段" });

            var result = service.GetByCounty("新北市");
            Assert.True(result.IsFailure);

            service.Add(new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "興南路2段" });
            result = service.GetByCounty("新北市");
            Assert.Equal(1, result.Value.Zips.Length);
            Assert.Equal(1, result.Value.Streets.Length);
            Assert.Equal("興南路2段", result.Value.Streets[0]);
        }

        [Fact]
        public void Use_Repository_Can_Create_Cache()
        {
            var repo = Substitute.For<IRepository<Address>>();
            repo.GetAll().Returns(FakeAddress());

            var service = AddressService.Create(repo);

            var result = service.GetByRealm("中和區");
            Assert.True(result.IsSuccess);
            var streets = result.Value.Streets;
            Assert.Equal(2, streets.Length);
            Assert.Equal("中正路", streets[0]);

            var zips = result.Value.Zips;
            Assert.Equal(1, zips.Length);
            Assert.Equal("235", zips[0]);
        }

        private Result<IEnumerable<Address>> FakeAddress()
        {
            List<Address> addresses = new List<Address>
            {
                new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中正路" },
                new Address { County = "新北市", Zip = "235", Realm = "中和區", Street = "中山路" },
                new Address { County = "台北市", Zip = "100", Realm = "中正區", Street = "八德路1段" },
                new Address { County = "台北市", Zip = "110", Realm = "信義區", Street = "信義路4段" }
            };

            return addresses.ToResult<IEnumerable<Address>>("OK");
        }

    }
}
