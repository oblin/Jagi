using JagiCore.Helpers;
using JagiCoreTests.CsvSample;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace JagiCoreTests
{
    public class CsvToolsTests
    {
        [Fact]
        public void Read_Csv_To_Book1_List()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book1.csv";
            var csv = CsvTools<Book1>.Default;

            IEnumerable<Book1> books = csv.ReadFromFile(location);

            Assert.Equal(3, books.Count());
            Assert.Equal("yiming", books.First().Name);
            Assert.Equal("測試中", books.Last().Description);
        }

        [Fact]
        public void Read_Csv_To_Book1_List_Empty_Fields()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book1.csv";
            var csv = CsvTools<Book1>.Default;
            List<Book1> books = csv.ReadFromFile(location).ToList();

            Assert.Equal(3, books.Count);
            Assert.Equal(2, books[1].Id);
            Assert.Equal("", books[1].Description);
        }

        [Fact]
        public void Read_Csv_To_Book2_Fail_Column_Date_Not_Mapped()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book1.csv";
            var csv = CsvTools<Book2>.Default;

            Assert.Throws<CsvHelper.ValidationException>(() => csv.ReadFromFile(location));
        }

        [Fact]
        public void Read_Csv_To_Book1_List_With_Custom_Mapping()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book2-date.csv";
            var csv = CsvTools<Book1>.Default;
            IEnumerable<Book1> books = csv.ReadFromFile<Book1Map>(location);

            Assert.Equal(4, books.Count());
            Assert.Equal("yiming", books.First().Name);
            Assert.Equal("", books.Last().Description);
        }

        //[Fact]
        //public void Read_Csv_To_Book2_List_With_Empty_Date()
        //{
        //    string location = Directory.GetCurrentDirectory();
        //    location += @"\CsvSample\Book2-date.csv";
        //    var csv = CsvTools<Book2>.Default;

        //    if (csv.IsMapDefined())
        //        csv.ClearAllRegisteredTypes();

        //    List<Book2> books = csv.ReadFromFile<Book2Map>(location).ToList();

        //    Assert.Equal(4, books.Count);
        //    Assert.Equal("一鳴", books[1].Name);
        //    Assert.Equal("", books[3].Description);
        //    // Date
        //    Assert.Equal("2012/1/2", books[2].Date.ToString("yyyy/M/d"));
        //    // Empty Date
        //    Assert.Equal("0001/01/01", books[3].Date.ToString("yyyy/MM/dd"));
        //}

        [Fact]
        public void Read_Csv_To_Book3_List_With_Null_Date()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book3-null-date.csv";
            var csv = CsvTools<Book3>.Default;
            List<Book3> books = csv.ReadFromFile<Book3Map>(location).ToList();

            // Empty Date
            Assert.Null(books[3].Date);
        }

        [Fact]
        public void Write_Book1_To_File()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book1-write.csv";
            if (File.Exists(location))
                File.Delete(location);

            var records = new List<Book1>
            {
                new Book1 { Id = 1, Name = "one", Description = "測試中文" },
                new Book1 { Id = 2, Name = "two", Description = "" }
            };

            var writer = CsvTools<Book1>.Default;
            writer.WriteToFile(location, records);

            Assert.True(File.Exists(location));

            List<string> lines = new List<string>();
            using (var fs = File.OpenRead(location))
            using (var stream = new StreamReader(fs, Encoding.UTF8))
            {
                while(stream.Peek() >= 0)
                {
                    lines.Add(stream.ReadLine());
                }
            }

            Assert.Equal("Id,Name,Description", lines[0]);
            Assert.Equal("2,two,", lines[2]);
        }

        [Fact]
        public void Write_Book2_To_File_With_Date_Default_Setting()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book2-write.csv";
            if (File.Exists(location))
                File.Delete(location);

            var records = new List<Book2>
            {
                new Book2 { Id = 1, Name = "one", Description = "測試中文", Date = DateTime.Now },
                new Book2 { Id = 2, Name = "two", Description = "", Date = new DateTime(1998, 2, 28) }
            };

            var writer = CsvTools<Book2>.Default;
            writer.WriteToFile(location, records);

            Assert.True(File.Exists(location));

            List<string> lines = new List<string>();
            using (var fs = File.OpenRead(location))
            using (var stream = new StreamReader(fs, Encoding.UTF8))
            {
                while (stream.Peek() >= 0)
                {
                    lines.Add(stream.ReadLine());
                }
            }

            Assert.Equal("Id,Name,Description,Date", lines[0]);
            Assert.Equal("2,two,,1998/2/28 上午 12:00:00", lines[2]);
        }

        [Fact]
        public void Write_Book2_To_File_With_Custom_Map()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book2-write.csv";
            if (File.Exists(location))
                File.Delete(location);

            var records = new List<Book4>
            {
                new Book4 { Id = 1, Name = "one", Description = "測試中文", Date = DateTime.Now },
                new Book4 { Id = 2, Name = "two", Description = "", Date = new DateTime(1998, 2, 28) }
            };

            var writer = CsvTools<Book4>.Default;        
            writer.WriteToFile<Book4DateConvertMap>(location, records);

            Assert.True(File.Exists(location));

            List<string> lines = new List<string>();
            using (var fs = File.OpenRead(location))
            using (var stream = new StreamReader(fs, Encoding.UTF8))
            {
                while (stream.Peek() >= 0)
                {
                    lines.Add(stream.ReadLine());
                }
            }

            Assert.Equal("Id,Name,Description,日期", lines[0]);
            Assert.Equal(3, lines.Count);
            Assert.Equal("2,two,,1998/02/28", lines[2]);
        }

        //[Fact]
        //public void CsvTools_Configuration_Should_Not_Have_Previous_Maps()
        //{
        //    string location = Directory.GetCurrentDirectory();
        //    location += @"\CsvSample\Book3-null-date.csv";

        //    var csv = CsvTools<Book3>.Default;
        //    Assert.False(csv.IsMapDefined());

        //    csv.ReadFromFile(location);

        //    Assert.True(csv.IsMapDefined());
        //}

        [Fact]
        public void CsvTools_Default_And_Create_Should_Not_Have_Same_Maps()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book3-null-date.csv";

            var csv1 = CsvTools<Book3>.Default;
            csv1.ClearAllRegisteredTypes();
            Assert.False(csv1.IsMapDefined());

            csv1.ReadFromFile(location);

            Assert.True(csv1.IsMapDefined());

            //var csv2 = CsvTools<Book2>.Create();
            //Assert.False(csv2.IsMapDefined());
            //Assert.False(csv2.IsMapDefined<Book3>());

            //csv2.ReadFromFile(location);

            //Assert.True(csv2.IsMapDefined<Book2>());
            //Assert.False(csv1.IsMapDefined<Book2>());
        }

        [Fact]
        public void Can_Read_Different_Delimeter()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book1-by-.csv";
            var csv = CsvTools<Book1>.Create("|");

            List<Book1> books = csv.ReadFromFile(location).ToList();

            Assert.Equal(3, books.Count);
            Assert.Equal(2, books[1].Id);
            Assert.Equal("", books[1].Description);
        }

        [Fact]
        public void Can_Write_Different_Delimeter()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book1-write-by-.csv";
            if (File.Exists(location))
                File.Delete(location);

            var records = new List<Book1>
            {
                new Book1 { Id = 1, Name = "one", Description = "測試中文" },
                new Book1 { Id = 2, Name = "two", Description = "" }
            };

            var writer = CsvTools<Book1>.Create("|");
            writer.WriteToFile(location, records);

            Assert.True(File.Exists(location));

            List<string> lines = new List<string>();
            using (var fs = File.OpenRead(location))
            using (var stream = new StreamReader(fs, Encoding.UTF8))
            {
                while (stream.Peek() >= 0)
                {
                    lines.Add(stream.ReadLine());
                }
            }

            Assert.Equal("Id|Name|Description", lines[0]);
            Assert.Equal("2|two|", lines[2]);
        }

        [Fact]
        public void Add_Single_Item()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book1-By-Single.csv";
            if (File.Exists(location))
                File.Delete(location);

            var csv = CsvTools<Book1>.Default;

            var item = new Book1 { Id = 1, Name = "one", Description = "測試中文" };
            csv.WriteToFile(location, item);

            Assert.True(File.Exists(location));

            item = new Book1 { Id = 2, Name = "two", Description = "測試二" };
            csv.WriteToFile(location, item);

            Assert.True(File.ReadLines(location).Count() > 2);
        }

        [Fact]
        public void Add_Multiple_Item()
        {
            string location = Directory.GetCurrentDirectory();
            location += @"\CsvSample\Book1-By-Single.csv";
            if (File.Exists(location))
                File.Delete(location);

            var csv = CsvTools<Book1>.Default;

            var records = new List<Book1>
            {
                new Book1 { Id = 1, Name = "one", Description = "測試中文" },
                new Book1 { Id = 2, Name = "two", Description = "" }
            };

            csv.WriteToFile(location, records);

            Assert.True(File.Exists(location));

            Assert.True(File.ReadLines(location).Count() > 2);
        }
    }
}
