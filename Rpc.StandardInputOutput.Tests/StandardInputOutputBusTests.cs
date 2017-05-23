using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rpc.StandardInputOutput.Tests.Client;

namespace Rpc.StandardInputOutput.Tests
{
    public class StandardInputOutputBusTests
    {
        [TestCaseSource(nameof(Payloads))]
        public async Task ShouldSendToChildProcess(object payload)
        {
            // Given
            using (var child = new ConsoleApp<Program>())
            {
                var responder = new StandardInputOutputRequestor(child.Process);

                // When
                var response = await responder.Ask(new Ping { Payload = payload });

                // Then
                var actual = response as Pong;
                Assert.AreEqual(payload, actual.Payload);
            }
        }

        [TestCase(50000)]
        [TestCase(5000)]
        [TestCase(1000)]
        [TestCase(500)]
        public void PerformanceTest(int noOfIterations)
        {
            // Given
            using (var child = new ConsoleApp<Program>())
            {
                var responder = new StandardInputOutputRequestor(child.Process);
                // When
                // warming-up
                Task.WaitAll(Enumerable.Range(0, 10).Select(i => responder.Ask(new Ping { Payload = i })).ToArray());
                GC.Collect();
                GC.Collect();
                Thread.Sleep(100);
                // measure
                Stopwatch t = new Stopwatch();
                t.Start();
                Task.WaitAll(Enumerable.Range(0, noOfIterations).Select(i =>
                {
                    return responder.Ask(new Ping
                    {
                        Payload =
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                    });
                }).ToArray());
                t.Stop();

                // Then
                Console.WriteLine("Messages per second: {0}, run-time: {1}", noOfIterations / t.Elapsed.TotalSeconds,
                    t.Elapsed);
            }
        }

        public static object[] Payloads =
        {
            Guid.NewGuid().ToString(),
            "line 1\r\nline 2\r\nline 3",
            123,
            true,
            DateTime.UtcNow,
            //string.Join("", Enumerable.Range(0, 15000).Select(_ => Guid.NewGuid())) // ~500Kb
        };
    }
}
