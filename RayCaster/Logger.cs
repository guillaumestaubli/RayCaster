using RayCaster.Ressources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCaster
{
    static class Logger
    {
        private static bool _logsWritten = false;

        public static bool LogsWritten
        {
            get
            {
                return _logsWritten;
            }
        }

        public static void WriteLog(string level, string message, string? location = null, Exception? ex = null)
        {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            StreamWriter? sw = null;

            switch (level)
            {
                case LogLevel.DEBUG:
                    sw = new StreamWriter(@"logs\debug.log", true, Encoding.UTF8);
                    break;
                case LogLevel.INFO:
                    sw = new StreamWriter(@"logs\info.log", true, Encoding.UTF8);
                    break;
                case LogLevel.WARN:
                    sw = new StreamWriter(@"logs\warning.log", true, Encoding.UTF8);
                    break;
                case LogLevel.ERROR:
                    sw = new StreamWriter(@"logs\error.log", true, Encoding.UTF8);
                    break;
                default:
                    sw = new StreamWriter(@"logs\other.log", true, Encoding.UTF8);
                    break;
            }

            string log = $"{DateTime.Now.ToString()} | {level} | {message}";

            if (location != null)
                log += $" | {location}";

            if (ex != null)
                log += $" | {ex.ToString()}";

            sw.WriteLine(log);

            sw.Flush();
            sw.Close();
            sw.Dispose();

            _logsWritten = true;
        }
    }
}
