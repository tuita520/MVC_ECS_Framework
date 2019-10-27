//=====================================================
// - FileName:      Zlogger.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero.Plugins.Base;

namespace Zero.ZeroEngine.Util
{
    public class ZLogger
    {
        static ZLogger()
        {
#if GAME_PUBLISH && !DEEP_DEBUG_RESMANAGER
            BaseLogger.SetFilterMsgCallback(HashString);
#endif
        }

        public static void SetShowStackState(bool showStack)
        {
            BaseLogger.SetShowStackState(showStack);
        }
        public static void SetWriteState(bool writeStack)
        {
            BaseLogger.SetWriteState(writeStack);
        }

#if !GAME_PUBLISH
        public static List<string> infoFilterList = new List<string>();
        //目前必须是开始包含过滤字符串
        private static bool IsInInfoFilterList(string str)
        {
            //无设置则全部输出
            int infoFilterListCount = infoFilterList.Count;
            if (infoFilterListCount!=0)
            {
                for (int i = 0; i < infoFilterListCount; ++i)
                {
                    if (str.IndexOf(infoFilterList[i])==0)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }
#endif
        public static void Release()
        {
            BaseLogger.Release();
        }
        public static void RegisterLogMsgCallback(BaseLogger.LogMsgCallback lmCB)
        {
            BaseLogger.RegisterLogMsgCallback(lmCB);
        }
        public static void UnRegisterLogMsgCallback(BaseLogger.LogMsgCallback lmCB)
        {
            BaseLogger.UnRegisterLogMsgCallback(lmCB);
        }
        public static void Test (bool comparison,object message = null,params object[] argv)
        {
            if (comparison)
                return;
            if(LogLevel.DEBUG== (BaseLogger.currentLogLevels & LogLevel.DEBUG))
            {
                string messageFormat = "";
                if(message != null)
                {
                    messageFormat = BaseLogger.LoggerStringFormat(message.ToString(), argv);
                }
                else
                {
                    messageFormat = "comparison not pass!!";
                }
                BaseLogger.Debug("Test + " + messageFormat + " #");
            }
        }
        public static void ArrayDebug(params object[] values)
        {
            if(LogLevel.DEBUG == (BaseLogger.currentLogLevels & LogLevel.DEBUG))
            {
                string msg = "Array: ";
                for(int i =0;i<values.Length;++i)
                {
                    msg += values[i] + ((i == values.Length - 1 && i != 0) ? ", " : "");
                }
                BaseLogger.Debug(msg);
            }
        }
        public static void Debug(object message,params object[] argv)
        {
            BaseLogger.Debug(message, argv);
        }
        public static void Info(object message, params object[] argv)
        {
            if(LogLevel.INFO == (BaseLogger.currentLogLevels & LogLevel.INFO))
            {
#if !GAME_PUBLISH
                if (!IsInInfoFilterList(message.ToString()))
                {
                    return;
                }
#endif
                BaseLogger.Info(message, argv);
            }
        }
        public static void Warning(object message, params object[] argv)
        {
            BaseLogger.Warning(message, argv);
        }
        public static void Error(object message, params object[] argv)
        {
            BaseLogger.Error(message, argv);
        }
        public static void Critical(object message, params object[] argv)
        {
            BaseLogger.Critical(message, argv);
        }
        public static void Except(object message, params object[] argv)
        {
            BaseLogger.Except(message, argv);
        }
        public static void Assert(bool comparison,object message = null, params object[] argv)
        {
            BaseLogger.Assert(comparison, message, argv);
        }

        public static void SNError(object message,params object[] argv)
        {
            BaseLogger.Warning(message, argv);
        }

        private static int MAX_CACHE_QUEUE_NUM = 10;
        private static Queue<string> hashqueue = null;
        private static bool HashString(string str)
        {
            if (hashqueue == null)
                hashqueue = new Queue<string>();
            if(hashqueue.Contains(str))
            {
                return true;
            }
            hashqueue.Enqueue(str);
            while (hashqueue.Count > MAX_CACHE_QUEUE_NUM) hashqueue.Dequeue();
            return false;
        }
    }
}
