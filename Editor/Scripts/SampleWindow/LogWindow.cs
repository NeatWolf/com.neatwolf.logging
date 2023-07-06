using UnityEngine;

namespace NeatWolf.Logging.Samples
{
#if UNITY_EDITOR
    using UnityEditor;
    public class LogWindow : EditorWindow
    {
        private string _logText = string.Empty;
        private Vector2 _scrollPosition;

        // Adds a menu named "Log Window" to the Window menu
        [MenuItem("Window/Log Window")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(LogWindow), false, "Log Window");
        }

        public void AddLog(string log)
        {
            _logText += "\n" + log;
        }

        void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            GUILayout.Label(_logText, EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndScrollView();
        }
    }
#endif

}