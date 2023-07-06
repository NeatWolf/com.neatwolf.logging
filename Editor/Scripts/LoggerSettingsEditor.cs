#if UNITY_EDITOR
using UnityEditor;

namespace NeatWolf.Logging
{
    [CustomEditor(typeof(LoggerSettings))]
    public class LoggerSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawSettingsGUI(serializedObject);
        }

        public static void DrawSettingsGUI(SerializedObject serializedObject)
        {
            // Draw each field manually.
            EditorGUI.BeginChangeCheck();

            // Draw "defaultLogLevel" field.
            SerializedProperty defaultLogLevel = serializedObject.FindProperty("defaultLogLevel");
            EditorGUILayout.PropertyField(defaultLogLevel);

            // Draw "defaultColors" field.
            SerializedProperty defaultColors = serializedObject.FindProperty("defaultColors");
            EditorGUILayout.PropertyField(defaultColors);

            // Draw "defaultMessageColor" field.
            SerializedProperty defaultMessageColor = serializedObject.FindProperty("defaultMessageColor");
            EditorGUILayout.PropertyField(defaultMessageColor);

            // Draw "defaultLogOutput" field.
            SerializedProperty defaultLogOutput = serializedObject.FindProperty("defaultLogOutput");
            EditorGUILayout.PropertyField(defaultLogOutput);

            // Draw "classSpecificSettings" field.
            SerializedProperty classSpecificSettings = serializedObject.FindProperty("classSpecificSettings");
            EditorGUILayout.PropertyField(classSpecificSettings);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif