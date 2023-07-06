using UnityEngine;

namespace NeatWolf.Logging.Samples
{
    [CreateAssetMenu(fileName = "LogWindowOutput", menuName = "Logging/LogWindowOutput", order = 2)]
    public class LogWindowOutput : LogOutput
    {
        public override void Output(string prefix, string message, Color prefixColor, Color messageColor)
        {
            // You might want to do something more sophisticated to handle colors.
            string coloredMessage = $"{ColorToRichText(prefixColor)}{prefix}</color> {ColorToRichText(messageColor)}{message}</color>";
            // Ensure the window is open.
            LogWindow window = (LogWindow)UnityEditor.EditorWindow.GetWindow(typeof(LogWindow), false, "Log Window");
            // Write the message.
            window.AddLog(coloredMessage);
        }

        private static string ColorToRichText(Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
        }
    }

}