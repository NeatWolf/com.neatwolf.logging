using UnityEngine;

namespace NeatWolf.Logging
{
    [CreateAssetMenu(fileName = "UnityConsoleLogOutput", menuName = "Logging/UnityConsoleLogOutput", order = 1)]
    public class UnityConsoleLogOutput : LogOutput
    {
        public override void Output(string prefix, string message, Color prefixColor, Color messageColor)
        {
            var prefixHtmlColor = ColorUtility.ToHtmlStringRGBA(prefixColor);
            var messageHtmlColor = ColorUtility.ToHtmlStringRGBA(messageColor);
            Debug.Log(
                $"<color=#{prefixHtmlColor}>{prefix}</color><color=#{messageHtmlColor}>{message}</color>");
        }
    }
}