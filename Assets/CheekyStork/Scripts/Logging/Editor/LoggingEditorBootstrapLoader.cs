using UnityEditor;

namespace CheekyStork.Logging
{
    [InitializeOnLoad]
    public static class LoggingEditorBootstrapLoader
    {
        private static UnityDebugOverride _unityDebugOverride;
        
        static LoggingEditorBootstrapLoader ()
        {
            _unityDebugOverride = new UnityDebugOverride();
            _unityDebugOverride.Initialize();
        }
    }
}
