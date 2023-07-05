using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeatWolf.Logging
{
#if UNITY_EDITOR
    using UnityEditor;

    public static class LoggingMenu
    {
        [MenuItem("Assets/Add to LoggerSettings", true)]
        private static bool ValidateAddToLoggerSettings()
        {
            // Only allows the option to be available when a ScriptableObject is selected.
            return Selection.activeObject != null && Selection.activeObject is MonoScript;
        }

        [MenuItem("Assets/Add to LoggerSettings")]
        private static void AddToLoggerSettings()
        {
            MonoScript script = Selection.activeObject as MonoScript;
            if (script == null) return;

            string className = script.GetClass().FullName;

            var settings = LoggerSettings.GetOrCreateAsset<LoggerSettings>("LoggerSettings");

            if (!settings.classSpecificSettings.Exists(x => x.className == className))
            {
                var newClassSettings = new Log.ClassSettings
                {
                    className = className
                };
            
                settings.classSpecificSettings.Add(newClassSettings);

                EditorUtility.SetDirty(settings); // Mark the LoggerSettings asset as dirty to make sure the changes are saved.
            }
            else
            {
                Debug.Log($"ClassSettings for class {className} already exists.");
            }
        }
    }
#endif

}
