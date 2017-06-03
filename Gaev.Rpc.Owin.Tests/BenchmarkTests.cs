using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
// ReSharper disable CoVariantArrayConversion
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Gaev.Rpc.Owin.Tests
{
    [TestFixture, Explicit]
    public class BenchmarkTests
    {
        //[TestCase(50000)]
        [TestCase(5000)]
        [TestCase(1000)]
        [TestCase(500)]
        public void PerformanceTest(int noOfIterations)
        {
            // Given
            var baseUrl = "http://localhost:9009/";
            var responder = new OwinResponder();
            responder.On<Ping>(async req => new Pong { Data = req.Data });
            var requestor = new OwinRequestor(baseUrl);
            using (WebApp.Start(baseUrl, responder.Start))
            {
                // When
                // warming-up
                Task.WaitAll(Enumerable.Range(0, 10).Select(i => requestor.Ask(new Ping { Data = i })).ToArray());
                GC.Collect();
                GC.Collect();
                Thread.Sleep(100);
                // measure
                Stopwatch t = new Stopwatch();
                t.Start();
                Task.WaitAll(Enumerable.Range(0, noOfIterations).Select(i =>
                {
                    return requestor.Ask(new Ping
                    {
                        Data =
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                    });
                }).ToArray());
                t.Stop();

                // Then
                Console.WriteLine("Messages per second: {0}, run-time: {1}", noOfIterations / t.Elapsed.TotalSeconds,
                    t.Elapsed);
            }
        }
    }
}
