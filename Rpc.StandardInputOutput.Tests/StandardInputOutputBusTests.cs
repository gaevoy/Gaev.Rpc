using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Rpc.StandardInputOutput.Tests.Client;

namespace Rpc.StandardInputOutput.Tests
{
    [TestFixture("csharp")]
    [TestFixture("nodejs")]
    public class StandardInputOutputBusTests
    {
        private readonly string _consoleApp;

        public StandardInputOutputBusTests(string consoleApp)
        {
            _consoleApp = consoleApp;
        }
        [TestCaseSource(nameof(Payloads))]
        public async Task ShouldSendToChildProcess(object payload)
        {
            // Given
            using (var child = StartChildApp())
            using (var responder = new StandardInputOutputRequestor(child.Process))
            {
                // When
                var response = await responder.Ask(new Ping { Data = payload });

                // Then
                var actual = response as Pong;
                Assert.AreEqual(payload, actual.Data);
                Console.WriteLine(actual.Header);
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

        private ConsoleApp StartChildApp()
        {
            switch (_consoleApp)
            {
                case "csharp": return ConsoleApp.StartCSharp<Program>();
                case "nodejs":
                    var nodejsApp = new Uri(new Uri(typeof(Program).Assembly.CodeBase), "../../../Rpc.StandardInputOutput.Tests.Client/program.js");
                    return ConsoleApp.StartNodeJs(nodejsApp.AbsolutePath);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
