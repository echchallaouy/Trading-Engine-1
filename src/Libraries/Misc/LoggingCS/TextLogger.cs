using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TradingEngineServer.Logging
{
    public interface ITextLogger : ILogger, IDisposable
    {

    }

    public class TextLogger : AbstractLogger, ITextLogger
    {
        public TextLogger(IOptions<LoggerConfiguration> settings) : base()
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            if (_settings.LoggerType != LoggerType.Text)
                throw new InvalidOperationException($"Instantiating Incorrect LoggerType ({_settings.LoggerType})");

            var now = DateTime.Now;
            string logDirectory = Path.Combine(_settings.TextLoggerConfiguration.Directory, $"{now:MM-dd-yyyy}");
            string baseLogName = $"{_settings.TextLoggerConfiguration.Filename}-{now:HH_mm_ss}";
            string logName = Path.ChangeExtension(baseLogName, _settings.TextLoggerConfiguration.FilenameExtension);
            string fullLogName = Path.Combine(logDirectory, logName);
            Directory.CreateDirectory(logDirectory);
            _ = Task.Run(() => LogAsync(fullLogName, _logBlock, _logTaskCancellationSource.Token));
        }

        private static async Task LogAsync(string fullLogPath, BufferBlock<LogInformation> logQueue, CancellationToken token)
        {
            using var fs = new FileStream(fullLogPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            using var sr = new StreamWriter(fs) { AutoFlush = true };
            try
            {
                while (true)
                {
                    var item = await logQueue.ReceiveAsync(token).ConfigureAwait(false);
                    var log = FormatLogInformation(item);
                    await sr.WriteLineAsync(log).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            { }
        }

        private static string FormatLogInformation(LogInformation logInformation)
        {
            return $"[{logInformation.LogTime:yyyy-MM-dd HH:mm:ss.fffffff}] " +
                $"[{logInformation.ThreadName,-30}:{logInformation.ThreadId:000}] " +
                $"[{logInformation.Module}] [{logInformation.LogLevel}] {logInformation.Message}";
        }

        protected override void Log(LogLevel logLevel, string module, string message)
        {
            _logBlock.Post(new LogInformation(logLevel, DateTime.Now,
                Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name, message, module));
        }

        ~TextLogger()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (_disposed)
                    return;
                _disposed = true;
            }

            if (disposing)
            {
                // Clear managed resources
                _logTaskCancellationSource.Cancel();
                _logTaskCancellationSource.Dispose();
            }

            // Clear unmanaged resources.
        }

        // PRIVATE //
        private readonly LoggerConfiguration _settings;

        // PRIVATE //
        private bool _disposed = false;
        private readonly CancellationTokenSource _logTaskCancellationSource = new CancellationTokenSource();
        private readonly BufferBlock<LogInformation> _logBlock = new BufferBlock<LogInformation>();
        private readonly object _lock = new object();
    }
}
