using System;
using UnityEngine;

namespace CheekyStork.Logging
{
    public static class LogLogicManager
    {
        private static Type _classType;
        
        [HideInCallstack]
        public static void Log(Action<string, UnityEngine.Object> logMethod, UnityEngine.Object obj, LogLevel logLevel, object msg, LogTag logTag, params LogTag[] logTags)
        {
            var tags = MeldTags(logTag, logTags);
            var objectName = GetSendersGOName(obj);
            
            DisplayString(logMethod, obj, msg, objectName, logLevel, tags);
        }
        
        [HideInCallstack]
        public static void Log(Action<string> logMethod, LogLevel logLevel, object msg, LogTag logTag, params LogTag[] logTags)
        {
            var tags = MeldTags(logTag, logTags);
            
            DisplayString(logMethod, msg, logLevel, tags);
        }

        private static string MeldTags(LogTag logTag, LogTag[] logTags)
        {
            var tags = "[" + logTag.ToString().ToUpper() + "]";
            
            foreach (var tag in logTags)
            {
                tags += "[" + tag.ToString().ToUpper() + "]";
            }

            return tags;
        }
        
        private static string GetSendersGOName(UnityEngine.Object obj)
        {
            var name = string.Empty;

            if (obj != null)
            {
                name = obj.name;
            }

            return name;
        }
        
        [HideInCallstack]
        private static void DisplayString(Action<string, UnityEngine.Object> logMethod, UnityEngine.Object contextObj, object msg, string name, LogLevel logLevel, string tags)
        {
            if (UnityDebugOverride.Instance != null)
            {
                if (logMethod == UnityEngine.Debug.Log)
                {
                    UnityDebugOverride.Instance.LogWithoutTag(LogType.Log, logLevel,  string.Join("; ", msg) + " {" + name + "} " + tags, contextObj);
                }
                else if (logMethod == UnityEngine.Debug.LogWarning)
                {
                    UnityDebugOverride.Instance.LogWithoutTag(LogType.Warning, logLevel,  string.Join("; ", msg) + " {" + name + "} " + tags, contextObj);
                }
                else if (logMethod == UnityEngine.Debug.LogError)
                {
                    UnityDebugOverride.Instance.LogWithoutTag(LogType.Error, logLevel,  string.Join("; ", msg) + " {" + name + "} " + tags, contextObj);
                }
            }
            else
            {
                logMethod(string.Concat("[", logLevel, "] ", string.Join("; ", msg), " {", name, "} ", tags), contextObj);
            }
        }
        
        [HideInCallstack]
        private static void DisplayString(Action<string> logMethod, object msg, LogLevel logLevel, string tags)
        {
            if (UnityDebugOverride.Instance != null)
            {
                if (logMethod == UnityEngine.Debug.Log)
                {
                    UnityDebugOverride.Instance.LogWithoutTag(LogType.Log, logLevel, string.Join("; ", msg) + " " + tags);
                }
                else if (logMethod == UnityEngine.Debug.LogWarning)
                {
                    UnityDebugOverride.Instance.LogWithoutTag(LogType.Warning, logLevel, string.Join("; ", msg) + " " + tags);
                }
                else if (logMethod == UnityEngine.Debug.LogError)
                {
                    UnityDebugOverride.Instance.LogWithoutTag(LogType.Error, logLevel, string.Join("; ", msg) + " " + tags);
                }
            }
            else
            {
                logMethod(string.Concat("[", logLevel, "] ", string.Join("; ", msg), " " + tags));
            }
        }
    }
}