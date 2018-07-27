using System;
using System.Collections.Generic;

namespace JagiCore
{
    /// <summary>
    /// 執行結果可以透過 Result Type 提供三個結果：
    /// 1. 成功或失敗： Boolean
    /// 2. 失敗錯誤訊息：可以指定字串
    /// 3. 回傳值：成功：可以是任意型態，或者沒有，失敗：不允許呼叫，會出現 InvalidOperationException
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }

        public string Error { get; }

        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && error != string.Empty)
                throw new InvalidOperationException();
            if (!isSuccess && error == string.Empty)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// 指定錯誤訊息（可用 error id in ErrorDictionary）或者直接傳入錯誤訊息
        /// </summary>
        /// <param name="errorIdOrMessage">error id in ErrorDictionary or Message</param>
        /// <returns></returns>
        public static Result Fail(string errorIdOrMessage)
        {
            return new Result(false, errorIdOrMessage);
        }

        /// <summary>
        /// 指定錯誤訊息（可用 error id in ErrorDictionary）或者直接傳入錯誤訊息
        /// </summary>
        /// <typeparam name="T">該型態的預設值（各種型態都是 null）</typeparam>
        /// <param name="errorIdOrMessage">error id in ErrorDictionary or Message</param>
        /// <returns></returns>
        public static Result<T> Fail<T>(string errorIdOrMessage)
        {
            return Fail<T>(default(T), errorIdOrMessage);
        }

        public static Result<T> Fail<T>()
        {
            return Fail<T>($"type {typeof(T)} not found");
        }

        /// <summary>
        /// 指定錯誤的回傳值，主要用途在於 IEnumerable type，錯誤時候使用者可以指定是 empty list，而不是預設的 Null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="errorIdOrMessage"></param>
        /// <returns></returns>
        public static Result<T> Fail<T>(T value, string errorIdOrMessage)
        {
            return new Result<T>(value, false, errorIdOrMessage);
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.IsFailure)
                    return result;
            }

            return Ok();
        }

        //public void OnFailure(Action p)
        //{
        //    throw new NotImplementedException();
        //}
    }


    public class Result<T> : Result
    {
        private readonly T _value;
        public T Value
        {
            get
            {
                // 這段有爭議，如果失敗傳回可以允許使用者指定 default value
                // 例如：IEnumerable 指定 empty list
                //if (!IsSuccess)
                //    throw new InvalidOperationException();

                return _value;
            }
        }

        protected internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }
    }
}
