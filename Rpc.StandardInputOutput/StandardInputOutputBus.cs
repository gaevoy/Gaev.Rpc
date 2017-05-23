using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        private readonly SemaphoreSlim _inputLock = new SemaphoreSlim(1, 1);

        public StandardInputOutputBus(TextWriter input, TextReader output, Func<object, StandardInputOutputBus, Task> receive)
        {
            _input = input;
            _listening = Task.Run(() => ListenStandardOutput(_cancellation.Token, output, receive));
        }

        public StandardInputOutputBus(Func<object, StandardInputOutputBus, Task> receive)
            : this(Console.Out, Console.In, receive) { }

        public StandardInputOutputBus(Process process, Func<object, StandardInputOutputBus, Task> receive)
            : this(process.StandardInput, process.StandardOutput, receive) { }

        public StandardInputOutputBus(int processId, Func<object, StandardInputOutputBus, Task> receive)
            : this(Process.GetProcessById(processId), receive) { }

        public void Dispose()
        {
            _cancellation.Cancel();
            _listening.Wait();
        }


        public async Task Send(object msg)
        {
            await _inputLock.WaitAsync();
            try
            {
                await _input.WriteLineAsync(JsonConvert.SerializeObject(msg, JsonSettings)).ConfigureAwait(false);
            }
            finally
            {
                _inputLock.Release();
            }
        }


        private async Task ListenStandardOutput(CancellationToken cancellation, TextReader output, Func<object, StandardInputOutputBus, Task> receive)
        {
            while (true)
                try
                {
                    var json = await output.ReadLineAsync().WithCancellation(cancellation).ConfigureAwait(false);
                    var msg = JsonConvert.DeserializeObject(json, JsonSettings);
                    await receive(msg, this).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { return; }
                catch (Exception) { /* ignore it */ }
        }
    }
}