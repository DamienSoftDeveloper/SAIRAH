using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Storage.Logging
{
    public sealed class FileAssistantLogger : IAssistantLogger
    {
        private readonly string _logDirectory;
        private readonly string _logFilePath;

        public FileAssistantLogger()
        {
            _logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SAIRAH",
                "logs");

            Directory.CreateDirectory(_logDirectory);

            _logFilePath = Path.Combine(
                _logDirectory,
                $"sairah-{DateTime.Now:yyyy-MM-dd}.log");
        }

        public Task LogInfoAsync(string message)
        {
            return WriteAsync("INFO", message);
        }

        public Task LogWarningAsync(string message)
        {
            return WriteAsync("WARNING", message);
        }

        public Task LogErrorAsync(string message, Exception? exception = null)
        {
            string fullMessage = exception == null
                ? message
                : $"{message} | Exception: {exception}";

            return WriteAsync("ERROR", fullMessage);
        }

        private async Task WriteAsync(string level, string message)
        {
            string logEntry =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";

            await File.AppendAllTextAsync(_logFilePath, logEntry);
        }
    }
}
