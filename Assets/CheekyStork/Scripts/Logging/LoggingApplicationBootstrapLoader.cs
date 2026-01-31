using UnityEngine;

namespace CheekyStork.Logging
{
    public static class LoggingApplicationBootstrapLoader
    {
        private static UnityDebugOverride _unityDebugOverride;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeUnityDebugOverride()
        {
            _unityDebugOverride = new UnityDebugOverride();
            _unityDebugOverride.Initialize();
        }
    }
}
