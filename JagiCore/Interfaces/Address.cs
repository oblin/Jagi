using System.Collections.Generic;

namespace JagiCore.Interfaces
{
    public class Address : IEntity
    {
        public int Id { get; set; }

        public string Zip { get; set; }

        public string County { get; set; }

        public string Realm { get; set; }

        public string Street { get; set; }
    }

    public class AddressQueryResult
    {
        public AddressQueryResult()
        {
            Zips = Counties = Realms = Streets = (new List<string>()).ToArray();
        }

        public string[] Zips { get; set; }
        public string[] Counties { get; set; }
        public string[] Realms { get; set; }
        public string[] Streets { get; set; }
    }
}
