using System;
using System.Diagnostics;

namespace Rpc.StandardInputOutput.Tests
{
    public class ConsoleApp<T> : IDisposable
    {
        private bool _disposed;
        public ConsoleApp()
        {
            var appPath = new Uri(typeof(T).Assembly.CodeBase).AbsolutePath;
            Process = new Process
            {
                StartInfo = new ProcessStartInfo(appPath)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    ErrorDialog = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };
            Process.Start();
        }
        public Process Process { get; }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                try
                {
                    Process.Kill();
                }
                catch { }
            }
            GC.SuppressFinalize(this);
        }

        ~ConsoleApp()
        {
            Dispose();
        }
    }
}