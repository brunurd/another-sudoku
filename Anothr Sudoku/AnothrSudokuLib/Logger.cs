using System;
using System.Collections.Generic;
using System.Text;
using Godot;

namespace AnothrSudokuLib
{
    public class Logger
    {
        public struct LogData
        {
            public readonly string message;
            public readonly List<Detail> details;
            public readonly DateTime localTime;
            public readonly DateTime utcTime;
            public readonly LogLevel level;

            public LogData(string message, List<Detail> details, LogLevel level)
            {
                this.message = message;
                this.details = details;
                localTime = DateTime.Now;
                utcTime = DateTime.UtcNow;
                this.level = level;
            }

            public override string ToString()
            {
                var str = new StringBuilder($"\n[{level} - {utcTime}]\n{message}\n");
                str.Append("{ ");
                foreach (var detail in details)
                {
                    str.Append($" {detail}");
                    if (details.IndexOf(detail) < details.Count - 1) {
                        str.Append(',');
                    }
                }
                str.Append(" }\n");
                return str.ToString();
            }
        }

        public struct Detail
        {
            public readonly string key;
            public readonly Variant value;

            public Detail(string key, Variant value)
            {
                this.key = key;
                this.value = value;
            }

            public override string ToString()
            {
                return $"{key}: {value}";
            }
        }

        public enum LogLevel
        {
            Error = 1,
            Warn = 2,
            Info = 3,
            Debug = 4
        }

        private static Logger _logger = null;
        private static Action<LogData> _transports = null;
        private readonly List<LogData> _logs = new List<LogData>();

        public static Logger Instance {
            get {
                if (_logger == null)
                {
                    _logger = new Logger();
                }
                return _logger;
            }
        }

        public static Action<LogData> Transports {
            get {
                if (_transports == null)
                {
                    _transports = new Action<LogData>((log) => {
                        if (log.level <= LogLevel.Debug) {
                            GD.Print(log);
                        }
                    });
                }
                return _transports;
            }
        }

        public static void Log(string message, LogLevel level, params Detail[] details)
        {
            Instance.AnyLog(message, level, details);
        }

        public static void Log(string message, params Detail[] details)
        {
            Log(message, LogLevel.Info, details);
        }

        public static void Log(params Variant[] args)
        {
            var message = args.Length >= 1 ? args[0].ToString() : "";
            var details = new List<Detail>();
            for(var i = 0; i < args.Length; i++) {
                details.Add(new Detail(i.ToString(), args[i]));
            }
            Instance.AnyLog(message, LogLevel.Debug, details.ToArray());
        }

        public static void AddTransport(Action<LogData> callback, LogLevel level) {
            _transports += (log) => {
                if (log.level <= level) {
                    callback(log);
                }
            };
        }

        public void LogError(string message, params Detail[] details)
        {
            AnyLog(message, LogLevel.Error, details);
        }

        public void LogWarn(string message, params Detail[] details)
        {
            AnyLog(message, LogLevel.Warn, details);
        }

        public void LogInfo(string message, params Detail[] details)
        {
            AnyLog(message, LogLevel.Info, details);
        }

        public void LogDebug(string message, params Detail[] details)
        {
            AnyLog(message, LogLevel.Debug, details);
        }

        public void AnyLog(string message, LogLevel level, params Detail[] details)
        {
            if (OS.HasFeature("release") && level >= LogLevel.Debug) {
                return;
            }

            var log = new LogData(
                message,
                details: new List<Detail>(details == null ? new Detail[]{} : details),
                level
            );
            _logger._logs.Add(log);
            Transports?.Invoke(log);
        }
    }
}