using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Gaev.Rpc.Rebus.Tests
{
    public class IsolationTestBase
    {
        private List<AppDomain> appsToClean = new List<AppDomain>();
        private List<IDisposable> instancesToDispose = new List<IDisposable>();

        [TearDown]
        public void Clean()
        {
            var toClean = appsToClean;
            appsToClean = new List<AppDomain>();
            var toDispose = instancesToDispose;
            instancesToDispose = new List<IDisposable>();
            Parallel.ForEach(toDispose, i => i.Dispose());
            Parallel.ForEach(toClean, AppDomain.Unload);
        }

        protected object New<T>(AppDomain app = null)
        {
            app = app ?? NewAppDomain();
            var instance = app.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
            if (instance is IDisposable) instancesToDispose.Add((IDisposable)instance);
            return instance;
        }

        protected AppDomain NewAppDomain()
        {
            var app = AppDomain.CreateDomain("TestApp", null, new AppDomainSetup { ApplicationBase = Environment.CurrentDirectory });
            appsToClean.Add(app);
            return app;
        }
    }
}