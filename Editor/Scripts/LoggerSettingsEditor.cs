#if UNITY_EDITOR
using UnityEditor;

namespace NeatWolf.Logging
{
    [CustomEditor(typeof(LoggerSettings))]
    public class LoggerSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // DrawDefaultInspector();

            var loggerSettings = (LoggerSettings)target;

            // Draw each field manually.

            EditorGUI.BeginChangeCheck();

            // Draw "defaultLogLevel" field.
            SerializedProperty defaultLogLevel = serializedObject.FindProperty("defaultLogLevel");
            EditorGUILayout.PropertyField(defaultLogLevel);

            // Draw "defaultColors" field.
            SerializedProperty defaultColors = serializedObject.FindProperty("defaultColors");
            EditorGUILayout.PropertyField(defaultColors);

            // Draw "classSpecificSettings" field.
            SerializedProperty classSpecificSettings = serializedObject.FindProperty("classSpecificSettings");
            EditorGUILayout.PropertyField(classSpecificSettings);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (loggerSettings.defaultClassSettings == null)
            {
                loggerSettings.defaultClassSettings = new Log.ClassSettings
                {
                    customLogOutput = LoggerSettings.GetOrCreateAsset<UnityConsoleLogOutput>("UnityConsoleLogOutput")
                };
            }
        }
    }
}
#endif