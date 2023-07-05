#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace NeatWolf.Logging
{
    /// <summary>
    /// Class responsible for logging in Unity's Console with optional colors, log levels and custom outputs.
    /// </summary>
    public static class Log
    {
        private static LogOutput _defaultOutput;
        private static LoggerSettings _loggerSettings;

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Exception
        }

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

        [Serializable]
        public class LogColor
        {
            public Color debug = Color.gray;
            public Color info = Color.white;
            public Color warning = Color.yellow;
            public Color error = Color.red;
            public Color exception = Color.magenta;
        }

        [CreateAssetMenu(fileName = "LoggerSettings", menuName = "Settings/LoggerSettings", order = 1)]
        public class LoggerSettings : ScriptableObject
        {
            public LogLevel defaultLogLevel = LogLevel.Info;
            public LogColor defaultColors;
            public ClassSettings defaultClassSettings;
            public List<ClassSettings> classSpecificSettings;

            private Dictionary<string, ClassSettings> _settingsLookup;

            void OnEnable()
            {
                if (classSpecificSettings == null) return;
                _settingsLookup = classSpecificSettings.ToDictionary(s => s.className);
            }

            public ClassSettings GetClassSettings(string className)
            {
                ClassSettings settings;
                if (_settingsLookup != null && _settingsLookup.TryGetValue(className, out settings) && !settings.muted)
                {
                    return settings;
                }
                return null;
            }
        }

        /// <summary>
        /// Abstract class for different output mediums of logs, using ScriptableObject for default implementation.
        /// </summary>
        public abstract class LogOutput : ScriptableObject
        {
            public abstract void Output(string prefix, string message, Color prefixColor, Color messageColor);
        }

        /// <summary>
        /// Outputs logs to Unity's Console.
        /// </summary>
        [CreateAssetMenu(fileName = "UnityConsoleLogOutput", menuName = "ScriptableObjects/UnityConsoleLogOutput", order = 1)]
        public class UnityConsoleLogOutput : LogOutput
        {
            public override void Output(string prefix, string message, Color prefixColor, Color messageColor)
            {
                var prefixHtmlColor = ColorUtility.ToHtmlStringRGBA(prefixColor);
                var messageHtmlColor = ColorUtility.ToHtmlStringRGBA(messageColor);
                UnityEngine.Debug.Log($"<color=#{prefixHtmlColor}>{prefix}</color><color=#{messageHtmlColor}>{message}</color>");
            }
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

            var settings = LoggerInitializer.GetOrCreateAsset<LoggerSettings>("LoggerSettings");
            var className = declaringType?.Name;
            var classSettings = settings.GetClassSettings(className);

            Color prefixColor = classSettings != null ? classSettings.prefixColor : settings.defaultClassSettings.prefixColor;
            Color messageColor = classSettings != null ? classSettings.messageColor : settings.defaultClassSettings.messageColor;

            string prefix = $"[{logLevel}] {declaringType}.{method.Name} ({fileName}:{lineNumber})";

            var output = classSettings != null && classSettings.customLogOutput != null ? classSettings.customLogOutput : currentOutput;
            if(output != null) output.Output(prefix, message, prefixColor, messageColor);
        }

        private static LogOutput currentOutput
        {
            get
            {
                if (_defaultOutput == null)
                {
                    _defaultOutput = LoggerInitializer.GetOrCreateAsset<UnityConsoleLogOutput>("UnityConsoleLogOutput");
                }

                return _defaultOutput;
            }
        }
    }
}