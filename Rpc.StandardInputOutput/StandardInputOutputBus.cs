using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
// ReSharper disable MethodSupportsCancellation
#pragma warning disable 4014

namespace Rpc.StandardInputOutput
{
    public class StandardInputOutputBus : IDisposable
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly TextWriter _input;
        private readonly Task _listening;
        private readonly object _inputLock = new object();

        public StandardInputOutputBus(TextWriter input, TextReader output, Action<object, StandardInputOutputBus> receive)
        {
            _input = input;
            _listening = Task.Run(() => ListenStandardOutput(_cancellation.Token, output, receive));
        }

        public StandardInputOutputBus(Action<object, StandardInputOutputBus> receive)
            : this(Console.Out, Console.In, receive) { }

        public StandardInputOutputBus(Process process, Action<object, StandardInputOutputBus> receive)
            : this(process.StandardInput, process.StandardOutput, receive) { }

        public StandardInputOutputBus(int processId, Action<object, StandardInputOutputBus> receive)
            : this(Process.GetProcessById(processId), receive) { }

        public void Dispose()
        {
            _cancellation.Cancel();
            _listening.Wait();
        }

        public void Send(object msg)
        {
            var json = JsonConvert.SerializeObject(msg, JsonSettings);
            lock (_inputLock)
                _input.WriteLine(json);
        }

        private async Task ListenStandardOutput(CancellationToken cancellation, TextReader output, Action<object, StandardInputOutputBus> receive)
        {
            while (true)
                try
                {
                    var json = await output.ReadLineAsync().WithCancellation(cancellation).ConfigureAwait(false);
                    var msg = JsonConvert.DeserializeObject(json, JsonSettings);
                    Task.Run(() => receive(msg, this));
                }
                catch (OperationCanceledException) { return; }
                catch (Exception) { /* ignore it */ }
        }
    }
}