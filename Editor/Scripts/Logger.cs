using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NeatWolf.Logging
{
    /// <summary>
    /// Abstract class for different output mediums of logs, using ScriptableObject for default implementation.
    /// </summary>
    public abstract class LogOutputBase : ScriptableObject
    {
        public abstract void LogOutput(string prefix, string message, Color prefixColor, Color messageColor);
    }

    /// <summary>
    /// Outputs logs to Unity's Console.
    /// </summary>
    [CreateAssetMenu(fileName = "UnityConsoleLogOutput", menuName = "ScriptableObjects/UnityConsoleLogOutput", order = 1)]
    public class UnityConsoleLogOutput : LogOutputBase
    {
        public override void LogOutput(string prefix, string message, Color prefixColor, Color messageColor)
        {
            var prefixHtmlColor = ColorUtility.ToHtmlStringRGBA(prefixColor);
            var messageHtmlColor = ColorUtility.ToHtmlStringRGBA(messageColor);
            Debug.Log($"<color=#{prefixHtmlColor}>{prefix}</color><color=#{messageHtmlColor}>{message}</color>");
        }
    }

    /// <summary>
    /// Settings for the logger that can be created as a ScriptableObject.
    /// </summary>
    [CreateAssetMenu(fileName = "LoggerSettings", menuName = "ScriptableObjects/LoggerSettings", order = 1)]
    public class LoggerSettings : ScriptableObject
    {
        public LogLevel defaultLogLevel = LogLevel.Info;

        /// <summary>
        /// Default settings to be used when no class-specific settings are found.
        /// </summary>
        public ClassSettings defaultClassSettings;

        public List<ClassSettings> classSpecificSettings;

        /// <summary>
        /// Settings for a specific class.
        /// </summary>
        [Serializable]
        public class ClassSettings
        {
            public string className;
            public LogLevel logLevel;
            public Color logColor;
            public Color messageColor;
            public string style;

            public LogOutputBase customLogOutput;

            /// <summary>
            /// If true, logs from this class will not be outputted.
            /// </summary>
            public bool muted;
        }

        private Dictionary<string, ClassSettings> _settingsLookup;

        void OnEnable()
        {
            if(classSpecificSettings == null) return;
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
    /// The Logger class is responsible for logging messages with custom settings for each class.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The currently registered output for logging messages.
        /// </summary>
        private static LogOutputBase _currentOutput;

        /// <summary>
        /// Registers the output used for logging.
        /// </summary>
        /// <param name="output">The LogOutput object to use for outputting log messages.</param>
        public static void RegisterLogOutput(LogOutputBase output)
        {
            _currentOutput = output;
        }

        /// <summary>
        /// Static constructor to set the default output to Unity's console.
        /// </summary>
        static Logger()
        {
            _currentOutput = Resources.Load<UnityConsoleLogOutput>("UnityConsoleLogOutput") ?? CreateDefaultLogOutput();
        }

        private static UnityConsoleLogOutput CreateDefaultLogOutput()
        {
            var logOutput = ScriptableObject.CreateInstance<UnityConsoleLogOutput>();
            AssetDatabase.CreateAsset(logOutput, "Assets/UnityConsoleLogOutput.asset");
            AssetDatabase.SaveAssets();
            return logOutput;
        }

        /// <summary>
        /// Logs a message with optional log level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logLevel">The level of the log (default is LogLevel.Info).</param>
        public static void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            OutputLog(message, logLevel);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warn(string message)
        {
            OutputLog(message, LogLevel.Warning);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void Error(string message)
        {
            OutputLog(message, LogLevel.Error);
        }

        /// <summary>
        /// Outputs the log message with the specified log level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logLevel">The level of the log.</param>
        private static void OutputLog(string message, LogLevel logLevel)
        {
            var trace = new StackTrace(true);
            var frame = trace.GetFrame(1);
            var method = frame.GetMethod();
            var declaringType = method.DeclaringType;
            var fileName = frame.GetFileName();
            var lineNumber = frame.GetFileLineNumber();

            var settings = LogSettingsManager.Settings;
            var className = declaringType?.Name;
            var classSettings = GetClassSettings(settings, className);

            Color prefixColor = GetColor(classSettings.logColor, settings.defaultClassSettings.logColor);
            Color messageColor = GetColor(classSettings.messageColor, settings.defaultClassSettings.messageColor);
            string style = GetString(classSettings.style, settings.defaultClassSettings.style);

            string prefix = $"[{logLevel}] {declaringType}.{method.Name} ({fileName}:{lineNumber})";

            (classSettings?.customLogOutput ?? _currentOutput)?.LogOutput(prefix, message, prefixColor, messageColor);
        }

        /// <summary>
        /// Returns the class settings for a given class name, or the default settings if no class-specific settings are found.
        /// </summary>
        /// <param name="settings">The current logger settings.</param>
        /// <param name="className">The name of the class.</param>
        /// <returns>The class settings for the given class name, or the default settings.</returns>
        private static LoggerSettings.ClassSettings GetClassSettings(LoggerSettings settings, string className)
        {
            if (settings == null) return new LoggerSettings.ClassSettings();

            LoggerSettings.ClassSettings classSettings = settings.GetClassSettings(className);
            if (classSettings != null) return classSettings;

            return settings.defaultClassSettings != null ? settings.defaultClassSettings : new LoggerSettings.ClassSettings();
        }

        /// <summary>
        /// Returns the preferred color if it's not the default color, otherwise returns the default color.
        /// </summary>
        /// <param name="preferredColor">The preferred color.</param>
        /// <param name="defaultColor">The default color.</param>
        /// <returns>The preferred color or the default color.</returns>
        private static Color GetColor(Color preferredColor, Color defaultColor)
        {
            if (preferredColor != default(Color)) return preferredColor;

            return defaultColor != default(Color) ? defaultColor : default(Color);
        }

        /// <summary>
        /// Returns the preferred string if it's not empty or null, otherwise returns the default string.
        /// </summary>
        /// <param name="preferredString">The preferred string.</param>
        /// <param name="defaultString">The default string.</param>
        /// <returns>The preferred string or the default string.</returns>
        private static string GetString(string preferredString, string defaultString)
        {
            if (!string.IsNullOrEmpty(preferredString)) return preferredString;

            return !string.IsNullOrEmpty(defaultString) ? defaultString : string.Empty;
        }
    }


    /// <summary>
    /// Class to load LoggerSettings from a ScriptableObject.
    /// </summary>
    public static class LogSettingsManager
    {
        private static LoggerSettings _settings;

        public static LoggerSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.Load<LoggerSettings>("LoggerSettings");
                    if(_settings == null)
                    {
                        _settings = ScriptableObject.CreateInstance<LoggerSettings>();
                        AssetDatabase.CreateAsset(_settings, "Assets/LoggerSettings.asset");
                        AssetDatabase.SaveAssets();
                    }
                }
                return _settings;
            }
        }
    }

    /// <summary>
    /// Enum for different log levels.
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
    }
}