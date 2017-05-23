using System;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Rpc.StandardInputOutput.Tests.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var cancellation = new CancellationTokenSource();
            Console.CancelKeyPress += delegate { cancellation.Cancel(); };

            using (var responder = new StandardInputOutputResponder())
            {
                responder.On<Ping>(async msg => new Pong { Payload = msg.Payload });
                cancellation.Token.WaitHandle.WaitOne();
            }
        }
    }

    public class Ping
    {
        public object Payload { get; set; }
    }

    public class Pong
    {
        public object Payload { get; set; }
    }
}
