using System;
using System.IO;

namespace TinyUrl.Api.services
{
    public class DevLogger : IAppLogger
    {
        private readonly string _logFilePath;

        public DevLogger(string logFilePath = "logs.txt")
        {
            _logFilePath = logFilePath;
            var dir = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public void Log(string message)
        {
            File.AppendAllText(_logFilePath, $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
        }
    }
}