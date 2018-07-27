using System;
using System.Collections.Generic;
using System.Linq;

namespace JagiCore
{
    public static class ResultExtensions
    {
        /// <summary>
        /// 判斷結果是否成功或失敗，成功原因：非 null 如果是陣列，不可以是空陣列；主要目的為取代 null 判斷是否成功的方案
        /// 以下型態的成功失敗分布
        ///                 成功      失敗
        /// string = Empty    V
        /// string = null              V
        /// obj = new Class() V
        /// obj = null                 V
        /// array = []                 V
        /// array = null               V 
        /// TODO: 
        ///     要重新思考一下是否針對 empty array or list 額外的判斷，因為畢竟原來的設計只有針對 null
        ///     但因為 Repository 的 Where 條件如果查不到是 empty list，對應 IsFail 比較容易理解
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">可以是任意的數值，但陣列會判斷是否是空值</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Result<T> ToResult<T>(this T value, string errorMessage) where T : class
        {
            if (value == null)
                return Result.Fail<T>(errorMessage);

            if (!(value is string))
            {
                if (value is Array)
                {
                    var array = value as Array;
                    if (array != null && array.Length == 0)
                    {
                        errorMessage = string.IsNullOrEmpty(errorMessage) ? "Empty Array" : errorMessage;
                        return Result.Fail<T>(value, errorMessage);
                    }
                }
                else
                {
                    var enumerable = value as System.Collections.IEnumerable;
                    if (enumerable != null)
                    {
                        var enumableValue = (IEnumerable<object>)value;
                        if (!enumableValue.Any())
                        {
                            errorMessage = string.IsNullOrEmpty(errorMessage) ? "Empty List" : errorMessage;
                            return Result.Fail<T>(value, errorMessage);
                        }
                    }
                }
            }

            return Result.Ok(value);
        }

        public static Result<T> ToResult<T>(this T value) where T : class
        {
            var message = value == null ? "Null object" : string.Empty;
            return ToResult<T>(value, message);
        }

        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
        {
            if (result.IsFailure)
                return result;

            if (!predicate(result.Value))
                return Result.Fail<T>(errorMessage);

            return result;
        }

        public static Result<V> Map<T, V>(this Result<T> result, Func<T, V> func)
        {
            if (result.IsFailure)
                return Result.Fail<V>(result.Error);

            return Result.Ok(func(result.Value));
        }

        public static Result OnSuccess(this Result result, Action action)
        {
            if (result.IsFailure)
                return result;

            action();

            return Result.Ok();
        }

        /// <summary>
        /// 判斷傳入的 Result，如果成功就執行 func 並回傳 func 的 Result。使用方法：
        /// var result = obj.EnsureValidations().OnSuccess(() => DoSomethingReturnResult());
        /// </summary>
        /// <param name="result"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Result OnSuccess(this Result result, Func<Result> func)
        {
            if (result.IsFailure)
                return result;

            return func();
        }

        /// <summary>
        /// 判斷傳入的 Result，如果成功就執行 action 但沒有回傳值。使用方法：
        ///     var result = obj.EnsureValidations().OnSuccess(() => DoSomething());
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="action"></param>
        /// <returns>Result</returns>
        public static Result OnSuccess<T>(this Result<T> result, Action<T> action)
        {
            if (result.IsFailure)
                return result;

            action(result.Value);

            return Result.Ok();
        }

        /// <summary>
        /// 判斷傳入的 Result，如果成功就執行 func 並回傳另外的型態。使用方法：
        ///     Result<string[]> result = EnsureValidations().OnSuccess(value => new string[] { value });
        /// </summary>
        /// <typeparam name="T">Type of Result.Value </typeparam>
        /// <typeparam name="V">return Type of func</typeparam>
        /// <param name="result"></param>
        /// <param name="func">處理 Result.Value 的函數參數</param>
        /// <returns>Result of V type</returns>
        public static Result<V> OnSuccess<T, V>(this Result<T> result, Func<T, V> func)
        {
            if (result.IsFailure)
                return Result.Fail<V>(result.Error);

            return Result.Ok(func(result.Value));
        }

        public static V OnSuccessReturn<T, V>(this Result<T> result, Func<T, V> func)
        {
            if (result.IsFailure)
                return default(V);

            return func(result.Value);
        }

        public static V OnFailureReturn<V>(this Result result, Func<V> func)
        {
            if (result.IsFailure)
                return func();

            return default(V);
        }

        public static Result OnFailure(this Result result, Action action)
        {
            if (result.IsFailure)
            {
                action();
            }

            return result;
        }

        public static Result OnFailure(this Result result, Action<Result> action)
        {
            if (result.IsFailure)
            {
                action(result);
            }

            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Func<Result<T>> func)
        {
            if (result.IsFailure)
                return func();

            return result;
        }

        public static Result OnBoth(this Result result, Action<Result> action)
        {
            action(result);

            return result;
        }

        /// <summary>
        /// 將結果回傳給呼叫端
        /// </summary>
        /// <typeparam name="T">處理的型態</typeparam>
        /// <param name="result"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T OnBoth<T>(this Result result, Func<Result, T> func)
        {
            return func(result);
        }

        /// <summary>
        /// 將 Result<T> 中的 T 轉換成要回傳的 V Type
        /// </summary>
        /// <typeparam name="T">前一個結果的 Type</typeparam>
        /// <typeparam name="V">return Type</typeparam>
        /// <param name="result"></param>
        /// <param name="func">轉換 Result<T> -> V</param>
        /// <returns></returns>
        public static V OnBoth<T, V>(this Result<T> result, Func<Result<T>, V> func)
        {
            return func(result);
        }
    }
}
