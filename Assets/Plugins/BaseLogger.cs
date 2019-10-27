//=====================================================
// - FileName:      BaseLogger.cs
// - Created:       mahuibao
// - UserName:      2019-01-30
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace Zero.Plugins.Base
{
    [Flags]
    public enum LogLevel
    {
        NONE = 0,
        DEBUG = 1,
        INFO = 2,
        WARRING = 4,
        ERROR = 8,
        EXCEPT = 16,
        CRITICAL = 32,
    }
    /// <summary>
    /// 基础的输出类
    /// </summary>
    public class BaseLogger
    {
        //过滤本身产生的 Log 继续回调处理
        public static string LOGGER_PREFIX = "['Logger']";

        public static LogLevel currentLogLevels = LogLevel.DEBUG | LogLevel.INFO | LogLevel.WARRING | LogLevel.ERROR | LogLevel.EXCEPT | LogLevel.CRITICAL;

        public delegate bool FilterMsgCallback(string msg);
        private static FilterMsgCallback _filterMsgCallback = null;
        public delegate void LogMsgCallback(string msg, LogLevel level);
        private static LogMsgCallback _logMsgCallback = null;
        private static bool _isShowStack = true;
        private static bool _isWriter = true;
        private static LogWriter _logWriter = null;

        static BaseLogger()
        {
            Init();
#if UNITY_5 || UNITY_2017
            Application.logMessageReceived += ProcessExceptionReport;
#else
            Application.RegisterLogCallback(new Application.LogCallback(ProcessExceptionReport));
#endif
        }

        public static void SetShowStackState(bool showStack)
        {
            _isShowStack = showStack;
        }
        public static void SetWriteState(bool writeStack)
        {
            _isWriter = writeStack;
        }
        public static void Init()
        {
            if (_logWriter == null)
            {
                _logWriter = new LogWriter();
            }
        }
        public static void Release()
        {

#if UNITY_5 || UNITY_2017
            Application.logMessageReceived -= ProcessExceptionReport;
#endif
            _filterMsgCallback = null;
            _logMsgCallback = null;

            _logWriter.Release();
            _logWriter = null;
        }

        public static void SetFilterMsgCallback(FilterMsgCallback fmCB)
        {
            _filterMsgCallback = fmCB;
        }
        public static void RegisterLogMsgCallback(LogMsgCallback lmCB)
        {
            if (_logMsgCallback == null)
            {
                _logMsgCallback = lmCB;
            }
            else
            {
                Delegate[] ds = _logMsgCallback.GetInvocationList();
                int dsCount = ds.Length;
                for (int i = 0; i < dsCount; i++)
                {
                    if (ds[i].Equals(lmCB))
                    {
                        Warning("BaseLogger RegisterLogMsgCallback duplicate ({0})", lmCB);
                        return;
                    }
                }
                _logMsgCallback += lmCB;
            }
        }
        public static void UnRegisterLogMsgCallback(LogMsgCallback lmCB)
        {
            if (_logMsgCallback == null)
            {
                _logMsgCallback -= lmCB;
            }
        }

        public static void Debug(object message,params object[] argv)
        {
            if(LogLevel.DEBUG == (currentLogLevels & LogLevel.DEBUG))
            {
                string messageFormat = LoggerStringFormat(message, argv);
                Log(string.Concat(" [ DEBUG ]: ", _isShowStack ? GetStackInfo() : "", messageFormat), LogLevel.DEBUG);
            }
        }
        public static void Info(object message, params object[] argv)
        {
            if (LogLevel.INFO == (currentLogLevels & LogLevel.INFO))
            {
                string messageFormat = LoggerStringFormat(message, argv);
                Log(string.Concat(" [ INFO ]: ", _isShowStack ? GetStackInfo() : "", messageFormat), LogLevel.INFO);
            }
        }
        public static void Warning(object message, params object[] argv)
        {
            if (LogLevel.WARRING == (currentLogLevels & LogLevel.WARRING))
            {
                string messageFormat = LoggerStringFormat(message, argv);
                Log(string.Concat(" [ WARRING ]: ", _isShowStack ? GetStackInfo() : "", messageFormat), LogLevel.WARRING);
            }
        }
        public static void Error(object message, params object[] argv)
        {
            if (LogLevel.ERROR == (currentLogLevels & LogLevel.ERROR))
            {
                string messageFormat = LoggerStringFormat(message, argv);
                Log(string.Concat(" [ ERROR ]:", _isShowStack ? GetStackInfo() : "", messageFormat), LogLevel.ERROR);
            }
        }
        public static void Critical(object message, params object[] argv)
        {
            if (LogLevel.CRITICAL == (currentLogLevels & LogLevel.CRITICAL))
            {
                string messageFormat = LoggerStringFormat(message, argv);
                Log(string.Concat(" [ CRITICAL ]:", _isShowStack ? GetStackInfo() : "", messageFormat), LogLevel.CRITICAL);
            }
        }
        public static void Except(object message, params object[] argv)
        {
            if (LogLevel.EXCEPT == (currentLogLevels & LogLevel.EXCEPT))
            {
                string messageFormat = LoggerStringFormat(message, argv);
                Log(string.Concat(" [ EXCEPT ]:", _isShowStack ? GetStackInfo() : "", messageFormat), LogLevel.EXCEPT);
            }
        }
        public static void Assert(bool comparison,object message = null,params object[] argv)
        {
            if (comparison)
                return;
            if(LogLevel.ERROR == (currentLogLevels & LogLevel.ERROR))
            {
                string messageFormat = "";
                if(message  != null)
                {
                    messageFormat = LoggerStringFormat(message.ToString(), argv);
                }
                else
                {
                    messageFormat = "comparison not pass!!";
                }
                Error("Assert {0} #", messageFormat);
            }
        }

        public static string LoggerStringFormat(object message, params object[] argv)
        {
            string logMsg = "logMsg";
            try
            {
                if (argv == null || argv.Length == 0)
                {
                    logMsg = message.ToString();
                }
                else
                {
                    logMsg = string.Format(message.ToString(), argv);
                }
            }
            catch(Exception ex)
            {
                logMsg = ex.Message;
            }
            return logMsg;
        }

        private static string GetStacksInfo()
        {
            StringBuilder sb = new StringBuilder();
            StackTrace st = new StackTrace(true);
            StackFrame[] sf = st.GetFrames();
            for (int i = 2;i<sf.Length;++i)
            {
                sb.AppendLine(sf[i].ToString());
            }
            return sb.ToString();
        }
        private static string GetStackInfo()
        {
            StackTrace st = new StackTrace(true);
            if(st.FrameCount >2 )
            {
                StackFrame sf = st.GetFrame(2);
                var method = sf.GetMethod();
                return string.Format("{0},{1}():", method.ReflectedType.Name, method.Name);
            }
            return "StackInfo";
        }

        private static void Log(string message, LogLevel level, bool needUEDebugOutputLog = true)
        {
            //回调每次都进行，过滤调整在记录文件前
            if (_logMsgCallback != null)
                _logMsgCallback(message, level);
            if(_filterMsgCallback != null)
            {
                if (_filterMsgCallback(message))
                    return;
            }
            if (_logWriter != null)
            {
                string msg = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
                if(_isWriter)
                {
                    _logWriter.WriteLog(msg, level, needUEDebugOutputLog);
                }
                else
                {
                    _logWriter.SafeUnityEngineDebugOutput(message, level);
                }
            }
        }

        private static void ProcessExceptionReport(string message, string stackTrace, LogType type)
        {
            if (message.IndexOf(LOGGER_PREFIX) == 0)
                return;
            LogLevel logLevel = LogLevel.DEBUG;
            switch(type)
            {
                case LogType.Assert:
                    logLevel = LogLevel.DEBUG;
                    break;
                case LogType.Log:
                    logLevel = LogLevel.INFO;
                    break;
                case LogType.Warning:
                    logLevel = LogLevel.WARRING;
                    break;
                case LogType.Error:
                    logLevel = LogLevel.ERROR;
                    break;
                case LogType.Exception:
                    logLevel = LogLevel.EXCEPT;
                    break;
                default:
                    break;
            }

            if(logLevel == (currentLogLevels & logLevel))
            {
                Log(string.Concat(" [SYS_", logLevel, "]:", message, "\n", stackTrace), logLevel, false);
            }
        }


        class LogWriter
        {
            private string _logPath = UnityEngine.Application.persistentDataPath + "/";
            private string _logFileNamePattern = "log_game*.txt";
            private static string _logFileFormatStr = "yyMMddHHmmssff";
            private string _logFileName = "log_name{0}.txt";

#if UNITY_STANDALONE
            private int _levelLogFireNum = 10;
#else
            private int _levelLogFireNum = 3;
#endif
            private string _logFilePath;
            private FileStream _fs;
            private StreamWriter _sw;
            private Action<string, LogLevel, bool> _logWriter;
            private readonly static object _locker = new object();

            public LogWriter()
            {
                if (!Directory.Exists(_logPath))
                {
                    Directory.CreateDirectory(_logPath);
                }
                //_logFilePath = string.Concat(_logPath, string.Format(_logFileName, "game"));
                ClearLogFile(_levelLogFireNum - 1);
                _logFilePath = string.Concat(_logPath, string.Format(_logFileName, DateTime.Now.ToString(_logFileFormatStr)));
                try
                {
                    _logWriter = Write;
                    _fs = new FileStream(_logFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    _sw = new StreamWriter(_fs, Encoding.UTF8);
                }
                catch(Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.Message);
                }
            }

            public void Release()
            {
                lock (_locker)
                {
                    if(_sw != null)
                    {
                        _sw.Close();
                        _sw.Dispose();
                    }
                    if(_fs != null)
                    {
                        _fs.Close();
                        _fs.Dispose();
                    }
                }
            }
            public string GetCurLogName()
            {
                return _logFilePath;
            }
            public void WriteLog(string msg,LogLevel level,bool needUEDebugOutputLog)
            {
                if(Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _logWriter(msg, level, needUEDebugOutputLog);
                }
                else
                {
                    _logWriter.Invoke(msg, level, needUEDebugOutputLog);
                }
            }
            public void SafeUnityEngineDebugOutput(string msg,LogLevel level)
            {
                try
                {
                    UnityEngineDebugOutput(msg, level);
                }
                catch(Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.Message);
                }
            }
            public void UnityEngineDebugOutput(string msg,LogLevel level)
            {
                switch(level)
                {
                    case LogLevel.DEBUG:
                    case LogLevel.INFO:
                        UnityEngine.Debug.Log(string.Concat(BaseLogger.LOGGER_PREFIX, msg));
                        break;
                    case LogLevel.WARRING:
                        UnityEngine.Debug.LogWarning(string.Concat(BaseLogger.LOGGER_PREFIX, msg));
                        break;
                    case LogLevel.ERROR:
                    case LogLevel.EXCEPT:
                    case LogLevel.CRITICAL:
                        UnityEngine.Debug.LogError(string.Concat(BaseLogger.LOGGER_PREFIX, msg));
                        break;
                    default:
                        break;
                }
            }
            private void Write(string msg,LogLevel level,bool needUEDebugOutputLog)
            {
                lock(_locker)
                {
                    try
                    {
                        if(needUEDebugOutputLog)
                        {
                            UnityEngineDebugOutput(msg, level);
                        }
                        if(_sw!=null)
                        {
                            _sw.WriteLine(msg);
                            _sw.Flush();
                        }
                    }
                    catch(Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex.Message);
                    }
                }
            }
            private void ClearLogFile(int leaveFilesNum)
            {
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(_logPath, _logFileNamePattern);
                }
                catch(Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.Message);
                }
                if (files!=null)
                {
                    int filesNum = files.Length;
                    if (filesNum> leaveFilesNum)
                    {
                        Array.Sort(files);

                        int delFilesNum = filesNum - leaveFilesNum;
                        for (int i = 0; i < delFilesNum; i++)
                        {
                            try
                            {
                                File.Delete(files[i]);
                            }
                            catch(Exception ex)
                            {
                                UnityEngine.Debug.LogWarning(string.Format("delete {0} failed ({1})",files[i],ex.Message));
                            }
                        }
                    }
                }
            }
        }
    }
}
