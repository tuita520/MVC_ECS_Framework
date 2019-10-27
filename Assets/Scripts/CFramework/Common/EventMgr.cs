//=====================================================
// - FileName:      EventMgr.cs
// - Created:       mahuibao
// - UserName:      2019-01-13
// - Email:         1023276156@qq.com
// - Description:   事件管理层
// -  (C) Copyright 2018 - 2018
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.Common
{
    public delegate void EventCallBack();
    public delegate void EventCallBack<T>(T arg1);
    public delegate void EventCallBack<T, U>(T arg1,U arg2);
    public delegate void EventCallBack<T, U, V>(T arg1, U arg2, V arg3);
    public delegate void EventCallBack<T, U, V, W>(T arg1, U arg2, V arg3, W arg4);
    public delegate void EventCallBack<T, U, V, W, Z>(T arg1, U arg2, V arg3, W arg4, Z arg5);

    /// <summary>
    /// 事件管理层
    /// </summary>
    public class EventMgr : Singleton<EventMgr>
    {
        private static EventController m_eventController = new EventController();
        public static Dictionary<string, Delegate> TheRouter
        {
            get { return m_eventController.TheRouter; }
        }
        public void Init()
        {
            ZLogger.Info("事件管理层初始化");
        }
        public void AfterInit()
        {

        }
        /// <summary>
        /// 标记为永久注册事件
        /// </summary>
        /// <param name="eventType"></param>
        public void MarkAsPermanent(string eventType)
        {
            m_eventController.MarkAsPermanent(eventType);
        }
        /// <summary>
        /// 清除非永久性注册事件
        /// </summary>
        public void Cleanup()
        {
            m_eventController.CleanUp();
        }

        #region 增加监听器
        /// <summary>
        /// 增加监听器，不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(string eventType, EventCallBack handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        /// <summary>
        /// 增加监听器，1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(string eventType, EventCallBack<T> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        /// <summary>
        /// 增加监听器，2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(string eventType, EventCallBack<T, U> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        /// <summary>
        /// 增加监听器，3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(string eventType, EventCallBack<T, U, V> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        /// <summary>
        /// 增加监听器，4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(string eventType, EventCallBack<T, U, V, W> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        public void AddEventListener(string eventType, EventCallBack<object> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        public void AddEventListener(string eventType, EventCallBack<object, object> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        #endregion

        #region 移除监听器
        /// <summary>
        /// 移除监听器，不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(string eventType, EventCallBack handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        /// <summary>
        /// 移除监听器，1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(string eventType, EventCallBack<T> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        /// <summary>
        /// 移除监听器，2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(string eventType, EventCallBack<T, U> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        /// <summary>
        /// 移除监听器，1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(string eventType, EventCallBack<T, U, V> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        /// <summary>
        /// 移除监听器，1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(string eventType, EventCallBack<T, U, V, W> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        public void RemoveEventListener(string eventType, EventCallBack<object> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        public void RemoveEventListener(string eventType, EventCallBack<object, object> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 触发事件，不带参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent(string eventType)
        {
            m_eventController.TriggerEvent(eventType);
        }
        /// <summary>
        /// 触发事件，带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T>(string eventType, T arg1)
        {
            m_eventController.TriggerEvent(eventType, arg1);
        }
        /// <summary>
        /// 触发事件，带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2);
        }
        /// <summary>
        /// 触发事件，带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2, arg3);
        }
        /// <summary>
        /// 触发事件，带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2, arg3, arg4);
        }
        public void TriggerEvent(string eventType, object arg1)
        {
            m_eventController.TriggerEvent(eventType, arg1);
        }
        public void TriggerEvent<T, U, V, W>(string eventType, object arg1, object arg2)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2);
        }
        #endregion
    }
    /// <summary>
    /// 事件处理类
    /// </summary>
    public class EventController
    {
        //普通事件
        private Dictionary<string, Delegate> m_theRouter = new Dictionary<string, Delegate>();

        public Dictionary<string, Delegate> TheRouter
        {
            get { return m_theRouter; }
        }

        //永久注册的事件列表
        private List<string> m_permanentEvents = new List<string>();
        /// <summary>
        /// 标记为永久注册事件
        /// </summary>
        /// <param name="eventType"></param>
        public void MarkAsPermanent(string eventType)
        {
            m_permanentEvents.Add(eventType);
        }
        /// <summary>
        /// 判断是否已经包含事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool ContainsEvent(string eventType)
        {
            return m_theRouter.ContainsKey(eventType);
        }
        /// <summary>
        /// 清除非永久性注册的事件
        /// </summary>
        public void CleanUp()
        {
            List<string> eventToRemove = new List<string>();
            var iter = m_theRouter.GetEnumerator();
            while (iter.MoveNext())
            {
                bool wasFound = false;
                int permanentEvnetsCount = m_permanentEvents.Count;
                for (int i = 0; i < permanentEvnetsCount; i++)
                {
                    if (iter.Current.Key == m_permanentEvents[i])
                    {
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound) eventToRemove.Add(iter.Current.Key);
            }
            int eventToRemoveCount = eventToRemove.Count;
            for (int i = 0; i < eventToRemoveCount; i++)
            {
                m_theRouter.Remove(eventToRemove[i]);
            }
        }
        /// <summary>
        /// 处理增加监听前的事项，检测参数等等
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingAdded"></param>
        private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                m_theRouter.Add(eventType, null);
            }

            Delegate d = m_theRouter[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new Exception(string.Format("Try to add not correct event {0} ，Current type is{1}, adding type is {2} "
                    , eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }
        private bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                return false;
            }
            Delegate d = m_theRouter[eventType];
            if ((d != null) && (d.GetType() != listenerBeingRemoved.GetType()))
            {
                throw new Exception(string.Format("Remove listener {0}\" failed, Current type is {1}, removing type is{2}"
                    , eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
            else
                return true;
        }

        /// <summary>
        /// 移除监听之后的处理，删掉事件
        /// </summary>
        /// <param name="eventType"></param>
        private void OnListenerRemoved(string eventType)
        {
            if (m_theRouter.ContainsKey(eventType) && m_theRouter[eventType] == null)
            {
                m_theRouter.Remove(eventType);
            }
        }

        #region 增加监听器
        /// <summary>
        /// 增加监听器，不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(string eventType, EventCallBack handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (EventCallBack)m_theRouter[eventType] + handler;
        }
        /// <summary>
        /// 增加监听器，1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(string eventType, EventCallBack<T> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (EventCallBack<T>)m_theRouter[eventType] + handler;
        }
        /// <summary>
        /// 增加监听器，2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(string eventType, EventCallBack<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (EventCallBack<T, U>)m_theRouter[eventType] + handler;
        }
        /// <summary>
        /// 增加监听器，3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(string eventType, EventCallBack<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (EventCallBack<T, U, V>)m_theRouter[eventType] + handler;
        }
        /// <summary>
        /// 增加监听器，4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(string eventType, EventCallBack<T, U, V, W> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (EventCallBack<T, U, V, W>)m_theRouter[eventType] + handler;
        }

        public void AddEventListener(string eventType, EventCallBack<object> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (EventCallBack<object>)m_theRouter[eventType] + handler;
        }
        public void AddEventListener(string eventType, EventCallBack<object, object> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (EventCallBack<object, object>)m_theRouter[eventType] + handler;
        }

        #endregion

        #region 移除监听器
        /// <summary>
        /// 移除监听器，不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(string eventType, EventCallBack handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (EventCallBack)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        /// <summary>
        /// 移除监听器，1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(string eventType, EventCallBack<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (EventCallBack<T>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        /// <summary>
        /// 移除监听器，2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(string eventType, EventCallBack<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (EventCallBack<T, U>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        /// <summary>
        /// 移除监听器，3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(string eventType, EventCallBack<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (EventCallBack<T, U, V>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        /// <summary>
        /// 移除监听器，4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(string eventType, EventCallBack<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (EventCallBack<T, U, V, W>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        public void RemoveEventListener(string eventType, EventCallBack<object> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (EventCallBack<object>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        public void RemoveEventListener(string eventType, EventCallBack<object, object> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (EventCallBack<object, object>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 触发事件，不带参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent(string eventType)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callBacks = d.GetInvocationList();
            for (int i = 0; i < callBacks.Length; i++)
            {
                EventCallBack callBack = callBacks[i] as EventCallBack;
                if (null == callBack.Target)
                {
                    continue;
                }
                if (callBack == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error:types of parameters not match", eventType));
                }
                try
                {
                    callBack();
                }
                catch (Exception ex)
                {
                    ZLogger.Except(ex);
                }
            }
        }
        /// <summary>
        /// 触发事件，带1个参数触发
        /// </summary>
        public void TriggerEvent<T>(string eventType, T arg1)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callBacks = d.GetInvocationList();
            for (int i = 0; i < callBacks.Length; i++)
            {
                EventCallBack<T> callBack = callBacks[i] as EventCallBack<T>;
                if (null == callBack.Target)
                {
                    continue;
                }
                if (callBack == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error:types of parameters not match", eventType));
                }
                try
                {
                    callBack(arg1);
                }
                catch (Exception ex)
                {
                    ZLogger.Except(ex);
                }
            }
        }
        /// <summary>
        /// 触发事件，带2个参数触发
        /// </summary>
        public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callBacks = d.GetInvocationList();
            for (int i = 0; i < callBacks.Length; i++)
            {
                EventCallBack<T, U> callBack = callBacks[i] as EventCallBack<T, U>;
                if (null == callBack.Target)
                {
                    continue;
                }
                if (callBack == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error:types of parameters not match", eventType));
                }
                try
                {
                    callBack(arg1, arg2);
                }
                catch (Exception ex)
                {
                    ZLogger.Except(ex);
                }
            }
        }
        /// <summary>
        /// 触发事件，带3个参数触发
        /// </summary>
        public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callBacks = d.GetInvocationList();
            for (int i = 0; i < callBacks.Length; i++)
            {
                EventCallBack<T, U, V> callBack = callBacks[i] as EventCallBack<T, U, V>;
                if (null == callBack.Target)
                {
                    continue;
                }
                if (callBack == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error:types of parameters not match", eventType));
                }
                try
                {
                    callBack(arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    ZLogger.Except(ex);
                }
            }
        }
        /// <summary>
        /// 触发事件，带4个参数触发
        /// </summary>
        public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callBacks = d.GetInvocationList();
            for (int i = 0; i < callBacks.Length; i++)
            {
                EventCallBack<T, U, V, W> callBack = callBacks[i] as EventCallBack<T, U, V, W>;
                if (null == callBack.Target)
                {
                    continue;
                }
                if (callBack == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error:types of parameters not match", eventType));
                }
                try
                {
                    callBack(arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    ZLogger.Except(ex);
                }
            }
        }

        public void TriggerEvent(string eventType, object arg1)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callBacks = d.GetInvocationList();
            for (int i = 0; i < callBacks.Length; i++)
            {
                EventCallBack<object> callBack = callBacks[i] as EventCallBack<object>;
                if (null == callBack.Target)
                {
                    continue;
                }
                if (callBack == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error:types of parameters not match", eventType));
                }
                try
                {
                    callBack(arg1);
                }
                catch (Exception ex)
                {
                    ZLogger.Except(ex);
                }
            }
        }
        public void TriggerEvent(string eventType, object arg1, object arg2)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callBacks = d.GetInvocationList();
            for (int i = 0; i < callBacks.Length; i++)
            {
                EventCallBack<object, object> callBack = callBacks[i] as EventCallBack<object, object>;
                if (null == callBack.Target)
                {
                    continue;
                }
                if (callBack == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error:types of parameters not match", eventType));
                }
                try
                {
                    callBack(arg1, arg2);
                }
                catch (Exception ex)
                {
                    ZLogger.Except(ex);
                }
            }
        }

        #endregion
    }
}