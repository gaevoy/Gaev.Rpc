using System;
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
