using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rebus.Rpc.Tests
{
    public class IsolationTestBase
    {
        private List<AppDomain> appsToClean = new List<AppDomain>();

        [TearDown]
        public void Clean()
        {
            var toClean = appsToClean;
            appsToClean = new List<AppDomain>();
            Parallel.ForEach(toClean, AppDomain.Unload);
        }

        protected object New<T>(AppDomain app = null)
        {
            app = app ?? NewAppDomain();
            return app.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }

        protected AppDomain NewAppDomain()
        {
            var app = AppDomain.CreateDomain("TestApp", null, new AppDomainSetup { ApplicationBase = Environment.CurrentDirectory });
            appsToClean.Add(app);
            return app;
        }
    }
}