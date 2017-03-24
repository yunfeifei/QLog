using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLog
{
    public class Config
    {
        public string FileName { get; set; }
        public string LogLevel { get; set; }//日志级别   0:Debug 1:Trace  2:Info  3:Warn  4:Error  5:Fatal
        public int BufferSize { get; set; }
        public int SingleFileSize { get; set; }
        public string Format { get; set; }

        public Config()
        {
            FileName = "logs";
            LogLevel = "0,1,2,3,4,5";
            BufferSize = 1024;
            SingleFileSize = 10 * 1024 * 1024;
            Format = "[{$level}][{$date}][{$message}]";
        }
    }
}
