using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plagiarism.Worker
{
    public static class Logger
    {
        // Will be switched to Enterprise Library Logging
        // or something else.

        // Should be thread-safe.
        private static LoggerImpl Impl = null;
        public static string Separator = new String('=', 50);

        public static void Init(string logDirectory)
        {
            Impl = new LoggerImpl(logDirectory);
        }

        public static void Dump(TextWriter writer, bool onlyErrors = false)
        {
            Impl.Dump(writer, onlyErrors);
        }

        public static bool IsDebugLevelEnabled
        {
            get
            {
                return Impl.IsDebug;
            }
            set
            {
                Impl.IsDebug = value;
            }
        }

        // DEBUG

        public static void Debug(string line)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Debug, line);
        }
        public static void Debug(string format, object arg0)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Debug, String.Format(format, arg0));
        }
        public static void Debug(string format, object arg0, object arg1)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Debug, String.Format(format, arg0, arg1));
        }

        // INFO

        public static void Info(string line)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Info, line);
        }
        public static void InfoPositive(string prefix, string main, string suffix = null)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Info, suffix, prefix, main, true);
        }
        public static void InfoNegative(string prefix, string main, string suffix = null)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Info, suffix, prefix, main, false);
        }
        public static void Info(string format, object arg0)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Info, String.Format(format, arg0));
        }
        public static void Info(string format, object arg0, object arg1)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Info, String.Format(format, arg0, arg1));
        }
        public static void Info(string format, object arg0, object arg1, object arg2)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Info, String.Format(format, arg0, arg1, arg2));
        }

        // ERROR

        public static void Error(string line)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Error, line);
        }
        public static void Error(string format, object arg0)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Error, String.Format(format, arg0));
        }
        public static void Error(string format, object arg0, object arg1)
        {
            Impl.WriteMessage(LoggerImpl.MessageLevel.Error, String.Format(format, arg0, arg1));
        }

        private sealed class LoggerImpl : IDisposable
        {
            public const string LogFileName = "log.txt";

            private readonly Object SyncObject = new Object();

            public bool IsDebug = true;
            private readonly bool EnableConsole = false;
            private readonly bool EnableLogFile = true;

            private readonly LinkedList<string> RecentRecords = new LinkedList<string>();
            private readonly LinkedList<string> RecentErrors = new LinkedList<string>();

            private readonly StreamWriter LogFile;

            public enum MessageLevel
            {
                Debug,
                Info,
                Error
            }

            private void WriteConsoleLogLine(DateTime ts, MessageLevel level, string prefix, string positiveOrNegativeMain, string suffix, bool isPositive)
            {
                // prefix is always gray
                // main is red or green
                // suffix is gray for info, dark gray for debug and cyan for error

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(ts.ToString("HH:mm:ss"));
                Console.ResetColor();
                switch (level)
                {
                    case MessageLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case MessageLevel.Info:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case MessageLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                }
                Console.Write(" ");
                Console.Write(level.ToString().Substring(0, 1));
                Console.Write(":");
                Console.ResetColor();
                if (prefix != null)
                {
                    Console.Write(" ");
                    Console.Write(prefix);
                }
                if (positiveOrNegativeMain != null)
                {
                    Console.Write(" ");
                    Console.ForegroundColor = isPositive ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write(positiveOrNegativeMain);
                    Console.ResetColor();
                }

                if (suffix != null)
                {
                    Console.Write(" ");
                    switch (level)
                    {
                        case MessageLevel.Debug:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            break;
                        case MessageLevel.Error:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                    }
                    Console.Write(suffix);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }

            private string BuildTextLogLine(DateTime ts, MessageLevel level, string prefix, string positiveOrNegativeMain, string suffix)
            {
                string res = String.Empty;
                if (prefix != null)
                {
                    res += " " + prefix;
                }
                if (positiveOrNegativeMain != null)
                {
                    res += " " + positiveOrNegativeMain;
                }
                if (suffix != null)
                {
                    res += " " + suffix;
                }

                return String.Format("{0} {1,7}:{2}", ts.ToString("yyyyMMdd HH:mm:ss"), level, res);
            }
            
            public void WriteMessage(MessageLevel level, string suffix, string prefix = null, string positiveOrNegativeMain = null, bool isPositive = true)
            {
                if (level == MessageLevel.Debug && !IsDebug)
                {
                    return;
                }

                DateTime ts = DateTime.Now;
                lock (SyncObject)
                {
                    try
                    {
                        if (EnableConsole)
                        {
                            WriteConsoleLogLine(ts, level, prefix, positiveOrNegativeMain, suffix, isPositive);
                        }

                        if (EnableLogFile)
                        {
                            string line = BuildTextLogLine(ts, level, prefix, positiveOrNegativeMain, suffix);
                            LogFile.WriteLine(line);
                        }
                    }
                    catch
                    {
                        //throw;
                    }
                }
            }

            public LoggerImpl(string logDirectory)
            {
                EnableConsole = Environment.UserInteractive;
                EnableLogFile = false;
                try
                {
                    string logPath = Path.Combine(logDirectory, LogFileName);
                    LogFile = new StreamWriter(logPath, true, Encoding.UTF8);
                    LogFile.AutoFlush = true;
                    EnableLogFile = true;
                }
                catch
                {
                }
            }

            public void Dump(TextWriter writer, bool onlyErrors)
            {
                lock (SyncObject)
                {
                    foreach (var rec in (!onlyErrors ? RecentRecords : RecentErrors))
                    {
                        writer.WriteLine(rec);
                    }
                }
            }

            public void Dispose()
            {
                if (LogFile != null)
                {
                    LogFile.Dispose();
                }
            }
        }
    }
}
