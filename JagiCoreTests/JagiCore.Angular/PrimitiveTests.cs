using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JagiCore;
using JagiCore.Core;
using Xunit;

namespace JagiCoreTests.JagiCoreAngular
{
    public class PrimitiveTests
    {
        private class Email : ValueObject<Email>
        {
            private readonly string _value;

            private Email(string value)
            {
                _value = value;
            }

            public static Result<Email> Create(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                    return Result.Fail<Email>("Email should not be empty");

                email = email.Trim();

                if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
                    return Result.Fail<Email>("Email is not valid");

                return Result.Ok(new Email(email));
            }

            /// <summary>
            /// 進行型別轉換： 將字串變成 Email
            ///     string emailString = GetEmail();
            ///     Email mail = (Email) emailString;
            /// </summary>
            /// <param name="email"></param>
            public static explicit operator Email(string email)
            {
                return Create(email).Value;
            }

            /// <summary>
            /// 進行型別轉換： 將 Email 變成字串
            ///     Email mail = GetEmail();
            ///     string emailString = email;
            /// </summary>
            /// <param name="email"></param>
            public static implicit operator string (Email email)
            {
                return email._value;
            }
        }

        private class Saleman
        {
            public Saleman(Email email)
            {
                Email = email;
            }

            public Saleman(string email)
            {
                Email = (Email)email;
            }

            private Saleman()
            {

            }

            public int Id { get; private set; }
            public Email Email { get; private set; }
            public Email PrivateEmail { get; set; }
        }

        [Fact]
        public void Test_Create_Salesman_with_email_string()
        {
            var sales = new Saleman("test@example.com");

            Assert.Equal("test@example.com", sales.Email);
        }

        [Fact]
        public void Test_set_sales_private_email()
        {
            Email email = Email.Create("test@example.com").Value;
            var sales = new Saleman(email);

            Assert.Equal(email, sales.Email);

            sales.PrivateEmail = (Email)"test@example.com";

            Assert.Equal(email, sales.PrivateEmail);
        }

        [Fact]
        public void Test_set_null_email()
        {
            var sales = new Saleman("test@example.com");

            Assert.Null(sales.PrivateEmail);

            sales.PrivateEmail = null;
            Assert.Null(sales.PrivateEmail);
        }
    }
}
