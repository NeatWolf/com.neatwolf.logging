using UnityEngine;

namespace NeatWolf.Logging
{
    public class LogOutput : ScriptableObject
    {
        public virtual void Output(string prefix, string message, Color prefixColor, Color messageColor)
        {
        }
    }
}