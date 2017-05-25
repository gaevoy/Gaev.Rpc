using System;
using System.Diagnostics;

namespace Gaev.Rpc.StandardInputOutput.Tests
{
    public class ConsoleApp : IDisposable
    {
        private bool _disposed;
        public ConsoleApp(string appPath, string arguments = null)
        {
            Process = new Process
            {
                StartInfo = new ProcessStartInfo(appPath, arguments)
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

        public static ConsoleApp StartCSharp<T>()
        {
            return new ConsoleApp(new Uri(typeof(T).Assembly.CodeBase).AbsolutePath);
        }
        public static ConsoleApp StartNodeJs(string appPath)
        {
            var testPath = new Uri(typeof(ConsoleApp).Assembly.CodeBase);
            var nodejsPath = new Uri(testPath, "../../../packages/Node.js.5.3.0/node.exe");
            return new ConsoleApp(nodejsPath.AbsolutePath, appPath);
        }
    }
}