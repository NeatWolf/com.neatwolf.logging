using System;
using System.Diagnostics;
using UnityEngine;

namespace NeatWolf.Logging
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Exception
    }

    public static partial class Log
    {
        private static LogOutput _defaultOutput;
        private static LoggerSettings _loggerSettings;

        [Serializable]
        public class ClassSettings
        {
            public string className;
            public LogLevel logLevel;
            public Color prefixColor = Color.white;
            public Color messageColor = Color.white;
            public LogOutput customLogOutput;
            public bool muted;
        }

        public static void Debug(string message = "")
        {
            OutputLog(message, LogLevel.Debug);
        }

        public static void Info(string message = "")
        {
            OutputLog(message, LogLevel.Info);
        }

        public static void Warning(string message)
        {
            OutputLog(message, LogLevel.Warning);
        }

        public static void Error(string message)
        {
            OutputLog(message, LogLevel.Error);
        }

        public static void Exception(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            OutputLog(exception.ToString(), LogLevel.Exception);
        }

        private static void OutputLog(string message, LogLevel logLevel)
        {
            var trace = new StackTrace(true);
            var frame = trace.GetFrame(1);
            var method = frame.GetMethod();
            var declaringType = method.DeclaringType;
            var fileName = frame.GetFileName();
            var lineNumber = frame.GetFileLineNumber();

            if (_loggerSettings == null)
            {
                _loggerSettings = LoggerSettings.GetOrCreateAsset<LoggerSettings>("LoggerSettings");
            }

            var className = declaringType?.Name;
            var classSettings = _loggerSettings.GetClassSettings(className);

            Color prefixColor = classSettings != null
                ? classSettings.prefixColor
                : _loggerSettings.defaultClassSettings.prefixColor;
            Color messageColor = classSettings != null
                ? classSettings.messageColor
                : _loggerSettings.defaultClassSettings.messageColor;

            string prefix = $"[{logLevel}] {declaringType}.{method.Name} ({fileName}:{lineNumber})";

            var output = classSettings != null && classSettings.customLogOutput != null
                ? classSettings.customLogOutput
                : currentOutput;
            if (output != null) output.Output(prefix, message, prefixColor, messageColor);
        }

        private static LogOutput currentOutput
        {
            get
            {
                if (_defaultOutput == null)
                {
                    _defaultOutput = LoggerSettings.GetOrCreateAsset<UnityConsoleLogOutput>("UnityConsoleLogOutput");
                }

                return _defaultOutput;
            }
        }
    }

    [Serializable]
    public class LogColor
    {
        public Color debug = Color.gray;
        public Color info = Color.white;
        public Color warning = Color.yellow;
        public Color error = Color.red;
        public Color exception = Color.magenta;
    }
}