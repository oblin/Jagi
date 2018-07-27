using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JagiCore.Core;
using Xunit;

namespace JagiCoreTests
{
    public class ValueObjectTests
    {
        private class Address : ValueObject<Address>
        {
            private readonly string _state;
            private readonly string _address1;
            private readonly string _city;

            public Address(string address1, string city, string state)
            {
                _state = state;
                _address1 = address1;
                _city = city;
            }

            public string Address1 => _address1;
            public string City => _city;
            public string State => _state;
        }
        private class ExpandedAddress : Address
        {
            private readonly string _address2;

            public ExpandedAddress(string address1, string address2, string city, string state)
                : base(address1, city, state)
            {
                _address2 = address2;
            }

            public string Address2 => _address2;
        }

        private class ExpandedAddress2 : Address
        {
            public ExpandedAddress2(string address1, string city, string state)
                : base(address1, city, state)
            {
            }

            public string Address2 { get; set; }
        }

        private class ExpandedAddress3 : Address
        {
            public ExpandedAddress3(string address1, string city, string state)
                : base(address1, city, state)
            {
            }

            private string Address2 { get; set; }
        }

        [Fact]
        public void AddressEqualsWorksWithIdenticalAddresses()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address1", "Austin", "TX");

            Assert.True(address.Equals(address2));
        }

        [Fact]
        public void AddressEqualsWorksWithNonIdenticalAddresses()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address2", "Austin", "TX");

            Assert.False(address.Equals(address2));

            // Is Symmetric
            Assert.False(address2.Equals(address));
        }

        [Fact]
        public void AddressEqualsWorksWithNulls()
        {
            Address address = new Address(null, "Austin", "TX");
            Address address2 = new Address("Address2", "Austin", "TX");

            Assert.False(address.Equals(address2));
        }

        [Fact]
        public void AddressEqualsIsReflexive()
        {
            Address address = new Address("Address1", "Austin", "TX");

            Assert.True(address.Equals(address));
        }

        [Fact]
        public void AddressEqualsIsTransitive()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address1", "Austin", "TX");
            Address address3 = new Address("Address1", "Austin", "TX");

            Assert.True(address.Equals(address2));
            Assert.True(address2.Equals(address3));
            Assert.True(address.Equals(address3));
        }

        [Fact]
        public void AddressOperatorsWork()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address1", "Austin", "TX");
            Address address3 = new Address("Address2", "Austin", "TX");

            Assert.True(address == address2);
            Assert.True(address2 != address3);
        }

        [Fact]
        public void DerivedTypesBehaveCorrectly()
        {
            Address address = new Address("Address1", "Austin", "TX");
            ExpandedAddress address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            Assert.False(address.Equals(address2));
            Assert.False(address == address2);

            ExpandedAddress address3 = new ExpandedAddress("Address1", null, "Austin", "TX");
            Assert.False(address.Equals(address3));

            // 只要有任何一個屬性不同都會視為不同
            ExpandedAddress2 address4 = new ExpandedAddress2("Address1", "Austin", "TX");
            Assert.False(address.Equals(address4));

            // 就算所有屬性一樣，但因為已經繼承，所以還是會視為不同
            ExpandedAddress3 address5 = new ExpandedAddress3("Address1", "Austin", "TX");
            Assert.False(address.Equals(address5));
        }

        [Fact]
        public void EqualValueObjectsHaveSameHashCode()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address1", "Austin", "TX");

            Assert.Equal(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void TransposedValuesGiveDifferentHashCodes()
        {
            Address address = new Address(null, "Austin", "TX");
            Address address2 = new Address("TX", "Austin", null);

            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void UnequalValueObjectsHaveDifferentHashCodes()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address2", "Austin", "TX");

            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void TransposedValuesOfFieldNamesGivesDifferentHashCodes()
        {
            Address address = new Address("_city", null, null);
            Address address2 = new Address(null, "_address1", null);

            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void DerivedTypesHashCodesBehaveCorrectly()
        {
            ExpandedAddress address = new ExpandedAddress("Address99999", "Apt 123", "New Orleans", "LA");
            ExpandedAddress address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }
    }
}
