using JagiCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class FunctionalHelperTests
    {
        [Fact]
        public void Test_IDisposable_Had_Been_Call_Using_Mock()
        {
            var dispose = Substitute.For<Dispose>();
            using (var disposable = (IDisposable)dispose)
            {

            }
            ((IDisposable)dispose).Received().Dispose();
        }

        [Fact]
        public void Test_IDisposable_Had_Been_Call_Using_Constructor()
        {
            Dispose testDispose;
            using (var disposable = new Dispose())
            {
                testDispose = disposable;
                Assert.False(disposable.DisposeBeenCalled);
            }
            Assert.True(testDispose.DisposeBeenCalled);
        }

        [Fact]
        public void Test_Using_Will_Call_Dispose()
        {
            var testDispose = Disposable.Using(
                () => new Dispose(),
                dispose => dispose);
            Assert.True(testDispose.DisposeBeenCalled);
        }

        [Fact]
        public void Test_Using_With_Parameter_Constructor()
        {
            var dispose = Substitute.For<Dispose>();
            string result = Disposable.Using(
                () => dispose,
                d => d.Greeting());

            Assert.Equal("Hello", result);
            ((IDisposable)dispose).Received().Dispose();
        }

        [Fact]
        public void Tee_Can_Execute_Two_Action()
        {
            var dispose = new Dispose();

            string hello = dispose.Greeting("Mac")
                .Tee(d => Assert.Equal("Hello Mac", d));
            Assert.Equal("Hello Mac", hello);
        }
    }

    public class Dispose : IDisposable
    {
        public Dispose() : this(false) { }

        public string Greeting() { return "Hello"; }

        public string Greeter(string greeter)
        {
            return greeter;
        }

        public string Greeting(string greeter)
        {
            return Greeting() + " " + Greeter(greeter);
        }

        public Dispose(bool defaultDispose)
        {
            this.DisposeBeenCalled = defaultDispose;
        }

        public bool DisposeBeenCalled { get; set; }

        void IDisposable.Dispose()
        {
            DisposeBeenCalled = true;
        }
    }
}
