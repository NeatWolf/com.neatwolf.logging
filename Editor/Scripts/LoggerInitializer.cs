#if UNITY_EDITOR
using UnityEditor;

namespace NeatWolf.Logging
{
    [InitializeOnLoad]
    public static class LoggerInitializer
    {
        static LoggerInitializer()
        {
            LoggerSettings.GetOrCreateAsset<LoggerSettings>("LoggerSettings");
            LoggerSettings.GetOrCreateAsset<UnityConsoleLogOutput>("UnityConsoleLogOutput");
        }
    }
}
#endif