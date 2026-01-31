using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace CheekyStork.Logging
{
    public class UnityDebugOverride : ILogger 
    {
        private struct OverridePair {

            public UnityDebugOverride Overrider;
            public ILogger Instance;
        }
        
        // Make sure these are the same length as the longest tag. (Not using auto padding for efficiency).
        public const string TagInfo =      "  [INFO]";
        public const string TagWarning =   "  [WARN]";
        public const string TagDebug =     " [DEBUG]";
        public const string TagError =     " [ERROR]";
        public const string TagFatal =     " [FATAL]";
        public const string TagAssert =    "[ASSERT]";
        public const string TagException = "[EXCEPT]";
        
        public const string UntaggedLogSuffix = "[UNTAGGED]";
        public const string EmptyAlignedName = "[          ]";

        public static UnityDebugOverride Instance { get; private set; }
        
        private const string _loggerFieldName = "s_Logger";
        
        private static FieldInfo _loggerField;
        private static List<OverridePair> _activationStack;
        private static bool _isInitialized;
        private static ILogger _defaultLogger;

        private ILogger _logger;
        
        // Required by ILogger interface
        public bool logEnabled { get; set; } = true;
        public ILogHandler logHandler { get; set; }
        public LogType filterLogType { get; set; } = LogType.Log;
        
        public void Initialize()
        {
            if(_isInitialized)
                return;
            
            _activationStack = new List<OverridePair>();
            
            var debugType = typeof(Debug);
            
            _loggerField = debugType.GetField(_loggerFieldName,BindingFlags.Static | BindingFlags.NonPublic);

            if (_loggerField == null)
            {
                Logging.Log.LogError($"Failed to find the logger field for override under the name '{_loggerFieldName}'. Can't override", LogTag.Editor);
            }
            else if (!typeof(ILogger).IsAssignableFrom(_loggerField.FieldType)) 
            {
                Logging.Log.LogError($"Identified logger field '{_loggerFieldName}' is not an ILogger field. Can't override", LogTag.Editor);
                _loggerField = null;
            }
            
            _defaultLogger = Debug.unityLogger;
            
            Instance = this;
            
            logHandler ??= _defaultLogger.logHandler;
            
            _logger = this;
            
            _activationStack.Add(new OverridePair { Overrider = this, Instance = _logger });
            
            UpdateAssignedLoggerInstance();
            
            Application.logMessageReceived += HandleApplicationExceptions;
            
            _isInitialized = true;
        }
        
        private static void UpdateAssignedLoggerInstance() 
        {
            if (_loggerField == null) 
            {
                Logging.Log.LogError("Unity Debug Override doesn't have a reference to the logger field, can't assign override loggers", LogTag.Editor);
                return;
            }

            var prog = _activationStack.Count - 1;
            for (; prog >= 0; --prog) 
            {
                if (_activationStack[prog].Instance != null)
                    break;
            }

            var logger = (prog > -1 ? _activationStack[prog].Instance : _defaultLogger);

            if (logger == null)
            {
                throw new NullReferenceException("Unity Debug Override can't assign a null reference to the active logger");
            }

            try
            {
                _loggerField.SetValue(null, logger);
            } 
            catch (Exception exec) 
            {
                Logging.Log.LogError($"Unity Debug Override failed to update the logger to a '{logger}' object. ERROR: {exec}", LogTag.Editor);
            }
        }
        
        [HideInCallstack]
        public bool IsLogTypeAllowed(LogType logType)
        {
            return true;
        }
        
        [HideInCallstack] 
        public void Log(LogType logType, object message) 
        {
            ProcessLogWithExtraInfo(logType, null, "{0}", GetString(message));
        }

        [HideInCallstack]
        public void Log(LogType logType, object message, UnityEngine.Object context) 
        {
            ProcessLogWithExtraInfo(logType, context, "{0}", GetString(message));
        }

        [HideInCallstack]
        public void Log(LogType logType, string tag, object message) 
        {
            ProcessLogWithExtraInfo(logType, null, "{0}: {1}", tag, GetString(message));
        }

        [HideInCallstack]
        public void Log(LogType logType, string tag, object message, UnityEngine.Object context) 
        {
            ProcessLogWithExtraInfo(logType, context, "{0}: {1}", tag, GetString(message));
        }

        [HideInCallstack]
        public void Log(object message) 
        {
            ProcessLogWithExtraInfo(LogType.Log, null, "{0}", GetString(message));
        }

        [HideInCallstack]
        public void Log(string tag, object message) 
        {
            ProcessLogWithExtraInfo(LogType.Log, null, "{0}: {1}", tag, GetString(message));
        }

        [HideInCallstack]
        public void Log(string tag, object message, UnityEngine.Object context) 
        {
            ProcessLogWithExtraInfo(LogType.Log, context, "{0}: {1}", tag, GetString(message));
        }

        [HideInCallstack]
        public void LogWarning(string tag, object message) 
        {
            ProcessLogWithExtraInfo(LogType.Warning, null, "{0}: {1}", tag, GetString(message));
        }

        [HideInCallstack]
        public void LogWarning(string tag, object message, UnityEngine.Object context) 
        {
            ProcessLogWithExtraInfo(LogType.Warning, context, "{0}: {1}", tag, GetString(message));
        }

        [HideInCallstack]
        public void LogError(string tag, object message) 
        {
            ProcessLogWithExtraInfo(LogType.Error, null, "{0}: {1}", tag, GetString(message));
        }

        [HideInCallstack]
        public void LogError(string tag, object message, UnityEngine.Object context) 
        {
            ProcessLogWithExtraInfo(LogType.Error, context, "{0}: {1}", tag, GetString(message));
        }

        [HideInCallstack]
        public void LogFormat(LogType logType, string format, params object[] args)
        {
            ProcessLogWithExtraInfo(logType, null, format, args);
        }
        
        [HideInCallstack]
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args) 
        {
            ProcessLogWithExtraInfo(logType, context, format, args);
        }
        
        [HideInCallstack]
        public void LogException(Exception exception) 
        {
            ProcessExceptionWithExtraInfo(exception, null);
        }

        [HideInCallstack]
        public void LogException(Exception exception, UnityEngine.Object context) 
        {
            ProcessExceptionWithExtraInfo(exception, context);
        }
        
        [HideInCallstack]
        public void LogWithoutTag(LogType logType, LogLevel logLevel, object message, UnityEngine.Object context)
        {
            ProcessLogWithoutTag(logType, logLevel, context, "{0}", GetString(message));
        }
        
        [HideInCallstack]
        public void LogWithoutTag(LogType logType, LogLevel logLevel, object message) 
        {
            ProcessLogWithoutTag(logType, logLevel, null, "{0}", GetString(message));
        }
        
        [HideInCallstack] 
        private void ProcessLogWithExtraInfo(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            switch (logType)
            {
                case LogType.Log:
                    ProcessSimpleLogWithExtraInfo(logType, context, format, args);
                    break;
                case LogType.Warning:
                    ProcessWarningWithExtraInfo(logType, context, format, args);
                    break;
                case LogType.Error:
                    ProcessErrorWithExtraInfo(logType, context, format, args);
                    break;
                case LogType.Assert:
                    ProcessErrorWithExtraInfo(logType, context, format, args);
                    break;
                case LogType.Exception:
                    ProcessFatalWithExtraInfo(logType, context, format, args);
                    break;
            }
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_INFO")]
        [HideInCallstack] 
        private void ProcessSimpleLogWithExtraInfo(LogType logType, Object context, string format, params object[] args)
        {
            FormatLog(logType, context, format, args);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_WARNING")]
        [HideInCallstack] 
        private void ProcessWarningWithExtraInfo(LogType logType, Object context, string format, params object[] args)
        {
            FormatLog(logType, context, format, args);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_ERROR")]
        [HideInCallstack]
        private void ProcessErrorWithExtraInfo(LogType logType, Object context, string format, params object[] args)
        {
            FormatLog(logType, context, format, args);
        }
        
        [Conditional("CHEEKY_STORK_LOG_LEVEL_FATAL")]
        [HideInCallstack]
        private void ProcessFatalWithExtraInfo(LogType logType, Object context, string format, params object[] args)
        {
            FormatLog(logType, context, format, args);
        }

        [Conditional("CHEEKY_STORK_LOG_LEVEL_FATAL")]
        [HideInCallstack]
        private void ProcessExceptionWithExtraInfo(Exception exception, Object context)
        {
            var alignedName = context == null ? EmptyAlignedName : GetAlignedName(context.name);

            var messageWithTagAtTheEnd = exception.Message;
            var index = messageWithTagAtTheEnd.IndexOf('\n');

            if (index != -1)
            {
                messageWithTagAtTheEnd = messageWithTagAtTheEnd.Insert(index, " " + UntaggedLogSuffix);
            }
            else
            {
                messageWithTagAtTheEnd += " " + UntaggedLogSuffix;
            }
            
            var messageWithExtraInfo = GetCurrentTime() + " " + TagException + " " + alignedName + " " + messageWithTagAtTheEnd;
            
            logHandler.LogFormat(LogType.Exception, context,"{0}\n{1}", messageWithExtraInfo, EnhanceStackTrace(exception.StackTrace));
        }
        
        private void FormatLog(LogType logType, Object context, string format, object[] args)
        {
            var alignedName = context == null ? EmptyAlignedName : GetAlignedName(context.name);

            var messagePrefix = GetCurrentTime() + " " + GetLogTypeTag(logType) + " " + alignedName;

            ProcessLog(logType, context, messagePrefix + " " + format + " " + UntaggedLogSuffix, args);
        }
        
        private static string GetString(object message) 
        {
            return (message != null ? message.ToString() : "Null");
        }
        
        [HideInCallstack]
        private void ProcessLog(LogType logType, Object context, string format, params object[] args)
        {
            logHandler.LogFormat(logType, context, format, args);
        }
        
        private static string GetCurrentTime()
        {
            return $"[{DateTime.Now:HH:mm:ss.fff}]";
        }
        
        private static string GetAlignedName(string name)
        {
            var alignedString = name.Length < 10 ? name.PadRight(10) : name[..10];
            return "[" + alignedString + "]";
        }
        
        private static bool ContainsTimeFormat(string input)
        {
            var regex = new Regex(@"\b\d{2}:\d{2}\b");
            return regex.IsMatch(input);
        }
        
        private static string GetLogTypeTag(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Info => TagInfo,
                LogLevel.Debug => TagDebug,
                LogLevel.Warning => TagWarning,
                LogLevel.Error => TagError,
                LogLevel.Fatal => TagFatal,
                LogLevel.Assert => TagAssert,
                LogLevel.Exception => TagException,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "Invalid log level.")
            };
        }
        
        private static string GetLogTypeTag(LogType logType)
        {
            return logType switch
            {
                LogType.Log => TagInfo,
                LogType.Warning => TagWarning,
                LogType.Error => TagError,
                LogType.Assert => TagAssert,
                LogType.Exception => TagException,
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, "Invalid log type.")
            };
        }
        
        [HideInCallstack]
        private void HandleApplicationExceptions(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Exception) 
                return;

            if (ContainsTimeFormat(logString)) 
                return;

            if (stackTrace == null)
            {
                Logging.Log.LogError("Stack trace is null", LogTag.Editor);
            }
            
            var enhancedStackTrace = EnhanceStackTrace(stackTrace);
            var exception = new Exception(logString + "\n" + enhancedStackTrace);
            
            Debug.LogException(exception);
        }
        
        [HideInCallstack]
        private void ProcessLogWithoutTag (LogType logType, LogLevel logLevel, UnityEngine.Object context, string format, params object[] args)
        {
            if (context == null)
            {
                ProcessLog(logType, context, GetCurrentTime() + " " + GetLogTypeTag(logLevel) + " " + EmptyAlignedName + " " + format , args);
            }
            else
            {
                ProcessLog(logType, context, GetCurrentTime() + " " + GetLogTypeTag(logLevel) + " " + GetAlignedName(context.name) + " " + format, args);
            }
        }
        
        private static string EnhanceStackTrace(string stackTrace)
        {
            if(stackTrace == null)
                return string.Empty;
            
            var enhancedStackTrace = "";
            var lines = stackTrace.Split('\n');

            foreach (var line in lines)
            {
                if(string.IsNullOrEmpty(line)) 
                    continue;
                
                var (leftIndex, middleIndex, rightIndex) = FindScriptFileIndexes(line);
                
                
                if(leftIndex == -1 || middleIndex == -1 || rightIndex == -1)
                {
                    enhancedStackTrace += line + "\n";
                    continue;
                }

                var filePath = line.Substring(leftIndex, middleIndex - leftIndex);
                var properFilePath = filePath.Replace('\\', '/');
                var lineNumberStr = line.Substring(middleIndex + 1, rightIndex - middleIndex);

                int.TryParse(lineNumberStr, out var lineNumber);

                var stringThatShouldBeReplacedWithLink = filePath + ":" + lineNumberStr;
                
                enhancedStackTrace += line.Replace(stringThatShouldBeReplacedWithLink, $"<a href=\"{properFilePath}\" line=\"{lineNumber}\">{properFilePath}:{lineNumber}</a>") + "\n";
            }
            
            return enhancedStackTrace.TrimEnd('\n');
        }
        
        private static (int, int, int) FindScriptFileIndexes(string input)
        {
            const string targetSubstring = ".cs:";
            const string leftOption1 = "in ";
            const string leftOption2 = "at ";

            var scriptFileFormatIndex = input.IndexOf(targetSubstring);
            if (scriptFileFormatIndex == -1) 
                return (-1, -1, -1); // target substring not found

            // Search to the left for "in " or "at "
            var leftIndex = FindLeftIndex(input, scriptFileFormatIndex, leftOption1, leftOption2);
        
            // Search to the right for non-numeric character
            var rightIndex = FindRightIndex(input, scriptFileFormatIndex + targetSubstring.Length);

            var colonIndex = scriptFileFormatIndex + targetSubstring.Length - 1;
            
            return (leftIndex, colonIndex, rightIndex);
        }

        private static int FindLeftIndex(string input, int start, string option1, string option2)
        {
            var indexOption1 = input.LastIndexOf(option1, start, StringComparison.Ordinal);
            var indexOption2 = input.LastIndexOf(option2, start, StringComparison.Ordinal);

            // Choose the option that's closest to start but not past it, and return the index right after it
            if (indexOption1 != -1 && (indexOption1 > indexOption2 || indexOption2 == -1))
            {
                return indexOption1 + option1.Length;
            }
            if (indexOption2 != -1)
            {
                return indexOption2 + option2.Length;
            }

            return -1; // No valid index found
        }

        private static int FindRightIndex(string input, int start)
        {
            var i = start;
            
            while (i < input.Length && char.IsDigit(input[i]))
            {
                i++;
            }

            return i - 1; // The index of the first non-numeric character or end of the string
        }
    }
}