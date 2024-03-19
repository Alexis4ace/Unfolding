using UnityEngine;

namespace Unfolding.Utils
{
    /// <summary>
    /// This class have a method which allows to set Debug logger availability dynamically.
    /// </summary>
    public static class DebugManager
    {
        /// <summary>
        /// This method set dynamically (just after scene's load) the availability of Debug logger.
        /// </summary>
        /// <remarks>
        /// As this method have RuntimeInitializeOnLoadMethod attribute, we don't have to call it manually.
        /// The purpose of this method is to improve performances when application is deployed,
        /// by forbiding Debug messages (which can slow the application).
        /// </remarks>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void SetDebugLoggerAvailability()
        {
        #if UNITY_EDITOR 
            Debug.unityLogger.logEnabled = true;
        #else
            Debug.unityLogger.logEnabled = false;
        #endif
        }
    }
}