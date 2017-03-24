using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;

namespace QLog
{
    internal class QLogger: ILogger
    {
        private string logFilePath;
        private string logFormat;
        private FileStream logFile;
        private int bufferSize;
        private int singleFileSize;
        private int bufferIndex;
        private byte[] buffer;
        private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
        private int currentLogFileSize;
        private Task logTask = null;

        public QLogger(Config config)
        {
            logFilePath = config.FileName;
            bufferSize = config.BufferSize;
            singleFileSize = config.SingleFileSize;
            logFormat = config.Format.Replace("$level", "0").Replace("$date", "1").Replace("$message", "2");
            StartLogTask();
        }

        //DEBUG,INFO,WARN,ERROR,FATAL,TRACE
        public void Debug(string message)
        {
            logQueue.Enqueue(string.Format(logFormat, "DEBUG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message) + Environment.NewLine);
        }
        public void Info(string message)
        {
            logQueue.Enqueue(string.Format(logFormat, "INFO", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message) + Environment.NewLine);
        }
        public void Trace(string message)
        {
            logQueue.Enqueue(string.Format(logFormat, "TRACE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message) + Environment.NewLine);
        }
        public void Warn(string message)
        {
            logQueue.Enqueue(string.Format(logFormat, "WARN", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message) + Environment.NewLine);
        }
        public void Error(string message)
        {
            logQueue.Enqueue(string.Format(logFormat, "ERROR", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message) + Environment.NewLine);
        }
        public void Fatal(string message)
        {
            logQueue.Enqueue(string.Format(logFormat, "FATAL", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message) + Environment.NewLine);
        }

        private void StartLogTask()
        {
            logTask = Task.Factory.StartNew(() =>
            {
                string logInfo;
                buffer = new byte[bufferSize];
                while (true)
                {
                    if (logQueue.TryDequeue(out logInfo))
                    {
                        var data = Encoding.UTF8.GetBytes(logInfo);
                        currentLogFileSize += data.Length;
                        if (bufferIndex + data.Length > bufferSize)
                        {
                            FlushToDisk();
                        }
                        data.CopyTo(buffer, bufferIndex);
                        bufferIndex += data.Length;
                    }
                    else
                    {
                        FlushToDisk();
                    }
                }
            });
        }

        private string CreateFile()
        {
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }
            string filePath = Path.Combine(logFilePath, DateTime.Now.ToString("yyyyMMddHHmmssff") + ".log");
            File.Create(filePath).Close();
            return filePath;
        }

        private void FlushToDisk()
        {
            if (logFile == null)
            {
                logFile = new FileStream(CreateFile(), FileMode.Append);
            }
            else
            {
                if (currentLogFileSize >= singleFileSize)
                {
                    logFile.Close();
                    logFile = new FileStream(CreateFile(), FileMode.Append);
                    currentLogFileSize = 0;
                }
            }

            logFile.Write(buffer, 0, buffer.Count(item => item > 0));
            logFile.Flush();
            bufferIndex = 0;
            buffer = new byte[bufferSize];
        }
    }
}
