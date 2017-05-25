using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Gaev.Rpc.StandardInputOutput.Tests.Client;
using NUnit.Framework;

namespace Gaev.Rpc.StandardInputOutput.Tests
{
    [TestFixture, Explicit]
    public class BenchmarkTests
    {
        [Test]
        public void BenchmarkTest()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            var summary = BenchmarkRunner.Run<StandardInputOutputBenchmark>(ManualConfig
                .CreateEmpty()
                .With(NullLogger.Instance)
                .With(DefaultColumnProviders.Target)
                .With(DefaultColumnProviders.Params)
                .With(StatisticColumn.Mean)
                .With(StatisticColumn.StdDev)
                .With(StatisticColumn.OperationsPerSecond)
                .With(StatisticColumn.P80)
                .With(StatisticColumn.P95)
                .With(MemoryDiagnoser.Default.GetColumnProvider())
            );
            var logger = new AccumulationLogger();
            AsciiDocExporter.Default.ExportToLog(summary, logger);
            Console.WriteLine(logger.GetLog());
        }

        [MemoryDiagnoser]
        [ShortRunJob]
        //[AllStatisticsColumn]
        public class StandardInputOutputBenchmark
        {
            private ConsoleApp child;
            private StandardInputOutputRequestor responder;

            public StandardInputOutputBenchmark()
            {
                child = ConsoleApp.StartCSharp<Program>();
                responder = new StandardInputOutputRequestor(child.Process);
            }

            [Params(1, 100, 1000)]
            public int Iterations { get; set; }

            [Benchmark]
            public Task Int()
            {
                if (Iterations == 1)
                    return responder.Ask(new Ping { Data = 123 });
                return Task.WhenAll(Enumerable.Range(0, Iterations).Select(e => responder.Ask(new Ping { Data = e })));
            }
        }

        [TestCase(50000)]
        [TestCase(5000)]
        [TestCase(1000)]
        [TestCase(500)]
        public void PerformanceTest(int noOfIterations)
        {
            // Given
            using (var child = ConsoleApp.StartCSharp<Program>())
            using (var responder = new StandardInputOutputRequestor(child.Process))
            {
                // When
                // warming-up
                Task.WaitAll(Enumerable.Range(0, 10).Select(i => responder.Ask(new Ping { Data = i })).ToArray());
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
