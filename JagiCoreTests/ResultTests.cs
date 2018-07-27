using JagiCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class ResultTests
    {
        [Fact]
        public void Simple_Failed_Result_Type()
        {
            var obj = new MyClass(null);

            Assert.Throws<NullReferenceException>(() => obj.LengthWithException());

            var result = obj.LengthWithResult();

            Assert.False(result.IsSuccess);
            Assert.Equal("建構式不可以是空字串", result.Error);
            // 當 result.IsFailed 非 null 數字就是 0
            Assert.Equal(0, result.Value);
        }

        [Fact]
        public void Test_Results()
        {
            string value = string.Empty;
            Assert.True(value.ToResult().IsSuccess);
            Assert.Empty(value.ToResult().Value);

            value = null;
            Assert.False(value.ToResult().IsSuccess);
            Assert.Null(value.ToResult().Value);

            var obj = new TestResultClass();
            Assert.True(obj.ToResult().IsSuccess);
            obj = null;
            Assert.False(obj.ToResult().IsSuccess);

            int[] array = new int[] { };
            Assert.False(array.ToResult().IsSuccess);
        }

        [Fact]
        public void Simple_Success_Result_Type()
        {
            var obj = new MyClass("Test");

            var result = obj.LengthWithResult();

            Assert.True(result.IsSuccess);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public void Use_Client_Error_Dictionary()
        {
            Dictionary<string, string> errorMap = new Dictionary<string, string>
            {
                { "NotFound", "找不到指定物件" },
                { "Null", "傳入 Null 字串" }
            };

            var obj = new MyClass(null);
            var result = obj.LengthWithErrorMap();

            Assert.False(result.IsSuccess);
            Assert.Equal("傳入 Null 字串", result.Error);
        }

        [Fact]
        public void Use_Railway_Orient_Approach()
        {
            var obj = new MyClass(null);
            var result1 = obj.LengthWithErrorMap();
            if (result1.IsFailure)
                Assert.False(result1.IsSuccess);

            var result2 = obj.LengthWithResult();
            if (result2.IsFailure)
                Assert.False(result2.IsSuccess);

            // 上面透過 ResultExtension 可以簡化為：
            Assert.NotEqual("OK",
                Result.Combine(result1, result2)
                    .OnSuccess(() => Assert.True(false))
                    .OnFailure(() => Assert.True(true))
                    .OnBoth(result => result.IsSuccess ? "OK" : result.Error));
        }

        [Fact]
        public void Test_On_Success_Deal_With_Result_Value()
        {
            var obj = new MyClass("Test");

            Assert.True(
                obj.LengthWithErrorMap()                // 執行成功，回傳 Test 長度： 4
                    .OnSuccess(i => Assert.Equal(4, i)) // OnSuccess 可以傳入 int，因此 i == 4
                        .IsSuccess);                    // 因為 LengthWithErrorMap() 成功，因此是 IsSuccess 這跟 OnSucces 裡面執行的是否成功無關
        }

        [Fact]
        public void Test_Ensure_Result_And_Message()
        {
            var obj = new MyClass("Micky");

            Assert.True(obj.StatementValidations().IsSuccess);
            Assert.True(obj.EnsureValidations().IsSuccess);

            obj = new MyClass("Jack");

            var stateResult = obj.StatementValidations();
            Assert.True(stateResult.IsFailure);
            Assert.Equal("Can not less than 5", stateResult.Error);

            var ensureResult = obj.EnsureValidations();
            Assert.True(ensureResult.IsFailure);
            Assert.Equal("Can not less than 5", ensureResult.Error);
        }

        [Fact]
        public void Map_Result_Can_Convert_To_Other_Type()
        {
            var obj = new MyClass("Micky");
            Assert.True(obj.EnsureValidations().IsSuccess);

            Result<string[]> result = obj.EnsureValidations().Map(value => new string[] { value });
            Assert.Equal(1, result.Value.Length);
            Assert.Equal("Micky", result.Value[0]);
        }

        [Fact]
        public void Convert_Result_On_Success_To_Other_Type()
        {
            var obj = new MyClass("Micky");
            Assert.True(obj.EnsureValidations().IsSuccess);

            Result<string[]> result = obj.EnsureValidations().OnSuccess(value => new string[] { value });
            Assert.Equal(1, result.Value.Length);
            Assert.Equal("Micky", result.Value[0]);
        }

        [Fact]
        public void Execute_Action_On_Success_Return_Result_Only()
        {
            var obj = new MyClass("Micky");
            Assert.True(obj.EnsureValidations().IsSuccess);

            var result = obj.EnsureValidations().OnSuccess(() => DoSomething());
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Execute_Function_On_Success_Return_Result_Only()
        {
            var obj = new MyClass("Micky");
            Assert.True(obj.EnsureValidations().IsSuccess);

            var result = obj.EnsureValidations().OnSuccess(() => DoSomethingReturnResult());
            Assert.True(result.IsFailure);
            Assert.Equal("Failed", result.Error);
        }

        [Fact]
        public void Test_IEnumerable_Empty_Result()
        {
            var list = new List<MyClass>();
            var result = list.ToResult<IEnumerable<MyClass>>();
            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Test_IEnumerable_OK_Result()
        {
            var list = new List<MyClass>();
            list.Add(new MyClass("TEST"));
            var result = list.ToResult<IEnumerable<MyClass>>();
            Assert.False(result.IsFailure);
        }

        private void DoSomething()
        {
        }

        private Result DoSomethingReturnResult()
        {
            return Result.Fail("Failed");
        }
    }

    public class TestResultClass
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int GetId() => Id;
    }

    public class MyClass
    {
        private readonly string _name;
        private const string CANNOT_BE_EMPTY = "Can not be empty";
        private const string CANNOT_LESS_THAN_5 = "Can not less than 5";
        private dynamic _errorCodes;

        public MyClass(string name)
        {
            _name = name;
            Dictionary<string, string> errorMap = new Dictionary<string, string>
            {
                { "NotFound", "找不到指定物件" },
                { "Null", "傳入 Null 字串" }
            };
            _errorCodes = ErrorCodes.Create(errorMap);
        }

        public int LengthWithException()
        {
            return _name.Length;
        }

        public Result<string> GetResult()
        {
            return _name.ToResult();
        }

        public Result<int> LengthWithResult()
        {
            if (string.IsNullOrEmpty(_name))
                return Result.Fail<int>("建構式不可以是空字串");

            return Result.Ok(_name.Length);
        }

        public Result<int> LengthWithErrorMap()
        {
            if (string.IsNullOrEmpty(_name))
                return Result.Fail<int>(_errorCodes.Null);

            return Result.Ok(_name.Length);
        }

        internal Result<string> StatementValidations()
        {
            if (string.IsNullOrEmpty(_name))
                return Result.Fail<string>(CANNOT_BE_EMPTY);

            if (_name.Length < 5)
                return Result.Fail<string>(CANNOT_LESS_THAN_5);

            return Result.Ok(_name);
        }

        internal Result<string> EnsureValidations()
        {
            return _name.ToResult(CANNOT_BE_EMPTY)
                .Ensure(n => !string.IsNullOrEmpty(n), CANNOT_BE_EMPTY)
                .Ensure(n => n.Length >= 5, CANNOT_LESS_THAN_5);
        }
    }
}
