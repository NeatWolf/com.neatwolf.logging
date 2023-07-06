#if UNITY_EDITOR
using UnityEditor;

namespace NeatWolf.Logging
{
    public class LoggerSettingsProvider : SettingsProvider
    {
        private SerializedObject _settings;

        public LoggerSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return LoggerSettings.GetOrCreateAsset<LoggerSettings>("LoggerSettings") != null;
        }

        public override void OnGUI(string searchContext)
        {
            if (_settings == null)
            {
                var loggerSettings = LoggerSettings.GetOrCreateAsset<LoggerSettings>("LoggerSettings");
                _settings = new SerializedObject(loggerSettings);
            }

            LoggerSettingsEditor.DrawSettingsGUI(_settings);
        }

        [SettingsProvider]
        public static SettingsProvider CreateLoggerSettingsProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new LoggerSettingsProvider("Project/Logging", SettingsScope.Project);
                return provider;
            }

            return null;
        }
    }
}
#endif