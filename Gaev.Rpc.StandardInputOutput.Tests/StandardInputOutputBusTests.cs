using System;
using System.Threading.Tasks;
using Gaev.Rpc.StandardInputOutput.Tests.Client;
using NUnit.Framework;

namespace Gaev.Rpc.StandardInputOutput.Tests
{
    [TestFixture("csharp")]
    [TestFixture("nodejs")]
    public class StandardInputOutputBusTests
    {
        private readonly string _childAppType;

        public StandardInputOutputBusTests(string childAppType)
        {
            _childAppType = childAppType;
        }

        [TestCaseSource(nameof(Payloads))]
        public async Task ShouldSendToChildProcess(object expectedData)
        {
            // Given
            using (var childApp = StartChildApp())
            using (var requestor = new StandardInputOutputRequestor(childApp.Process))
            {
                // When
                var response = await requestor.Ask(new Ping { Data = expectedData });

                // Then
                var actual = response as Pong;
                Assert.AreEqual(expectedData, actual.Data);
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
            switch (_childAppType)
            {
                case "csharp": return ConsoleApp.StartCSharp<Program>();
                case "nodejs":
                    var nodejsApp = new Uri(new Uri(typeof(Program).Assembly.CodeBase), "../../../Gaev.Rpc.StandardInputOutput.Tests.Client/program.js");
                    return ConsoleApp.StartNodeJs(nodejsApp.AbsolutePath);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
