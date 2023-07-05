#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace NeatWolf.Logging
{
    [InitializeOnLoad]
    public class LoggerInitializer
    {
        static LoggerInitializer()
        {
            GetOrCreateAsset<Log.LoggerSettings>("LoggerSettings");
            GetOrCreateAsset<Log.UnityConsoleLogOutput>("UnityConsoleLogOutput");
        }

        public static T GetOrCreateAsset<T>(string fileName) where T : ScriptableObject
        {
            var path = $"Assets/Settings/{fileName}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return asset;
        }
    }

    [CustomEditor(typeof(Log.LoggerSettings))]
    public class LoggerSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var loggerSettings = (Log.LoggerSettings)target;

            if (loggerSettings.defaultClassSettings == null)
            {
                loggerSettings.defaultClassSettings = new Log.ClassSettings
                {
                    customLogOutput = LoggerInitializer.GetOrCreateAsset<Log.UnityConsoleLogOutput>("UnityConsoleLogOutput")
                };
            }
        }
    }
}

#endif