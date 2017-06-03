using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Gaev.Rpc.Owin.Tests
{
    public class RpcResponderTests
    {
        [TestCaseSource(nameof(Payloads))]
        public async Task ShouldSendAndRespond(object expectedData)
        {
            // Given
            var baseUrl = "http://localhost:9009/";
            var responder = new OwinResponder();
            responder.On<Ping>(async req => new Pong { Data = req.Data });
            var requestor = new OwinRequestor(baseUrl);
            using (WebApp.Start(baseUrl, responder.Start))
            {
                // When
                var response = await requestor.Ask(new Ping { Data = expectedData }) as Pong;

                // Then
                Assert.AreEqual(expectedData, response?.Data);
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

    public class Ping
    {
        public object Data { get; set; }
    }

    public class Pong
    {
        public object Data { get; set; }
    }
}
