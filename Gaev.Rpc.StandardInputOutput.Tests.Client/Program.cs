using System;
using System.Threading;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Gaev.Rpc.StandardInputOutput.Tests.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var cancellation = new CancellationTokenSource();
            Console.CancelKeyPress += delegate { cancellation.Cancel(); };

            using (var responder = new StandardInputOutputResponder())
            {
                responder.On<Ping>(async msg => new Pong
                {
                    Header = "Hello from C#",
                    Data = msg.Data
                });
                responder.Start();
                cancellation.Token.WaitHandle.WaitOne();
            }
        }
    }

}
