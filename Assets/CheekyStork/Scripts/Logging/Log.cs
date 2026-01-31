using System.Diagnostics;
using UnityEngine;

namespace CheekyStork.Logging
{
    /// <summary>
    /// This class holds all the different log type calls.
    /// On classes that inherit from UnityEngine.Object class, you can ues this.[LogMethod]. (It will automatically highlight the object in the console.)
    /// On classes that don't inherit from Unity.Object class, you can use Log.[LogMethod].
    /// </summary>
    public static class Log
    {
        #region Unity object class logging without specified context
        [Conditional("CHEEKY_STORK_LOG_LEVEL_FATAL")]
        [HideInCallstack]
        public static void LogFatal(this UnityEngine.Object obj, object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogError, obj, LogLevel.Fatal, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        public static void LogError(this UnityEngine.Object obj, object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogError, obj, LogLevel.Error, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_WARNING")]
        [HideInCallstack]
        public static void LogWarning(this UnityEngine.Object obj, object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogWarning, obj, LogLevel.Warning, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public static void LogDebug(this UnityEngine.Object obj, object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.Log, obj, LogLevel.Debug, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_INFO")]
        [HideInCallstack]
        public static void LogInfo(this UnityEngine.Object obj, object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.Log, obj, LogLevel.Info, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        public static void Assert(this UnityEngine.Object obj, bool condition, object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            if (!condition)
            {
                LogLogicManager.Log(UnityEngine.Debug.LogError, obj, LogLevel.Assert, msg, logTag, extraLogTags);
            }
        }
        #endregion
        
        #region Unity object class logging with specified context
        [Conditional("CHEEKY_STORK_LOG_LEVEL_FATAL")]
        [HideInCallstack]
        public static void LogFatal(this UnityEngine.Object obj, object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogError, contextObject, LogLevel.Fatal, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        public static void LogError(this UnityEngine.Object obj, object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogError, contextObject, LogLevel.Error, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_WARNING")]
        [HideInCallstack]
        public static void LogWarning(this UnityEngine.Object obj, object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogWarning, contextObject, LogLevel.Warning, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public static void LogDebug(this UnityEngine.Object obj, object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.Log, contextObject, LogLevel.Debug, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_INFO")]
        [HideInCallstack]
        public static void LogInfo(this UnityEngine.Object obj, object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.Log, contextObject, LogLevel.Info, msg,logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        public static void Assert(this UnityEngine.Object obj, bool condition, object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            if (!condition)
            {
                LogLogicManager.Log(UnityEngine.Debug.LogError, contextObject, LogLevel.Assert, msg,logTag, extraLogTags);
            }
        }
        #endregion
        
        #region Base class logging without specified context
        [Conditional("CHEEKY_STORK_LOG_LEVEL_FATAL")]
        [HideInCallstack]
        public static void LogFatal(object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogError, LogLevel.Fatal, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        public static void LogError(object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogError, LogLevel.Error, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_WARNING")]
        [HideInCallstack]
        public static void LogWarning(object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogWarning, LogLevel.Warning, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public static void LogDebug(object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.Log, LogLevel.Debug, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_INFO")]
        [HideInCallstack]
        public static void LogInfo(object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.Log, LogLevel.Info, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        public static void Assert(bool condition, object msg, LogTag logTag, params LogTag[] extraLogTags)
        {
            if (!condition)
            {
                LogLogicManager.Log(UnityEngine.Debug.LogError, LogLevel.Assert, msg, logTag, extraLogTags);
            }
        }
        #endregion
        
        #region Base class logging with specified context
        [Conditional("CHEEKY_STORK_LOG_LEVEL_FATAL")]
        [HideInCallstack]
        public static void LogFatal(object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogError, contextObject, LogLevel.Fatal, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        public static void LogError(object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogError, contextObject, LogLevel.Error, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_WARNING")]
        [HideInCallstack]
        public static void LogWarning(object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.LogWarning, contextObject, LogLevel.Warning, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public static void LogDebug(object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.Log, contextObject, LogLevel.Debug, msg, logTag, extraLogTags);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_INFO")]
        [HideInCallstack]
        public static void LogInfo(object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            LogLogicManager.Log(UnityEngine.Debug.Log, contextObject, LogLevel.Info, msg, logTag, extraLogTags);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        public static void Assert(bool condition, object msg, UnityEngine.Object contextObject, LogTag logTag, params LogTag[] extraLogTags)
        {
            if (!condition)
            {
                LogLogicManager.Log(UnityEngine.Debug.LogError, contextObject, LogLevel.Assert, msg, logTag, extraLogTags);
            }
        }
        #endregion
    }
}