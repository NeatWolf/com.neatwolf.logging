using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NeatWolf.Logging
{
    [CreateAssetMenu(fileName = "LoggerSettings", menuName = "Settings/LoggerSettings", order = 1)]
    public class LoggerSettings : ScriptableObject
    {
        public LogLevel defaultLogLevel = LogLevel.Info;
        public LogColor defaultColors;
        public Log.ClassSettings defaultClassSettings;
        public List<Log.ClassSettings> classSpecificSettings;

        private Dictionary<string, Log.ClassSettings> _settingsLookup;

        void OnEnable()
        {
            if (defaultClassSettings == null)
            {
                defaultClassSettings = new Log.ClassSettings();
            }
            if (classSpecificSettings == null)
            {
                classSpecificSettings = new List<Log.ClassSettings>();
            }
            _settingsLookup = classSpecificSettings.ToDictionary(s => s.className);
        }

        public Log.ClassSettings GetClassSettings(string className)
        {
            Log.ClassSettings settings;
            if (_settingsLookup != null && _settingsLookup.TryGetValue(className, out settings) && !settings.muted)
            {
                return settings;
            }
            return null;
        }
            

        public static T GetOrCreateAsset<T>(string fileName) where T : ScriptableObject
        {
#if UNITY_EDITOR
            var path = $"Assets/Settings/{fileName}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null)
            {
                asset = CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return asset;
#else
        return Resources.Load<T>(fileName);
#endif
        }
            
    }
}