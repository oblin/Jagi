using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore
{
    public static class Disposable
    {
        /// <summary>
        /// 使用 function 表達式執行 using 動作，最終仍然會呼叫 Dispose()
        /// 使用範例：
        /// var testDispose = Disposable.Using(
        ///     () => new Dispose(),
        ///     dispose => dispose);
        /// </summary>
        /// <typeparam name="TDisposable">第一個物件建立方式必須要是 IDispose</typeparam>
        /// <typeparam name="TResult">使用第一個物件建立後的執行方案</typeparam>
        /// <param name="factory">建立物件方式</param>
        /// <param name="map">處理物件方式</param>
        /// <returns></returns>
        public static TResult Using<TDisposable, TResult>(
            Func<TDisposable> factory,
            Func<TDisposable, TResult> map)
            where TDisposable : IDisposable
        {
            using (var disposable = factory())
            {
                return map(disposable);
            }
        }
    }

    public static class FunctionalHelpers
    {
        /// <summary>
        /// 函數傳入 Tee 的值，可以被其他函數處理，並且回傳
        /// 使用範例：傳入 Tee 裡面，跟回傳給外面是一樣的字串
        /// string hello = dispose.Greeting("Mac")
        ///     .Tee(d => Assert.Equal("Hello Mac", d));
        ///     Assert.Equal("Hello Mac", hello);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">傳入的物件型態</param>
        /// <param name="action">處理傳入物件型態的函數</param>
        /// <returns>傳回原先的 taget 物件不做變更</returns>
        public static T Tee<T>(this T target, Action<T> action)
        {
            action(target);
            return target;
        }

        /// <summary>
        /// 指定處理 TSource 的函數，並且回傳函數處理完畢後的類型
        /// Map 會將 source type 變成 result type
        /// </summary>
        /// <typeparam name="TSource">傳入的 source type</typeparam>
        /// <typeparam name="TResult">處理完畢後的型態</typeparam>
        /// <param name="this"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static TResult FMap<TSource, TResult>(this TSource @this, Func<TSource, TResult> fn)
        {
            return fn(@this);
        }
    }
}
