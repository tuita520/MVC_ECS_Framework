//=====================================================
// - FileName:      UIEventListener.cs
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
using UnityEngine.EventSystems;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.UI
{
    public enum UIEventEnum
    {
        ON_CLICK,
        ON_DOWN,
        ON_ENTER,
        ON_EXIT,
        ON_UP,
        ON_SELECT,
        ON_UPDATESELECT,
    }

    public class UIEventListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;

        static public bool AddEventListener(GameObject go,UIEventEnum tempEnum, VoidDelegate tempDelegate)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null) listener = go.AddComponent<UIEventListener>();
            switch (tempEnum)
            {
                case UIEventEnum.ON_CLICK:
                    listener.onClick = tempDelegate;
                    return true;
                case UIEventEnum.ON_DOWN:
                    listener.onDown = tempDelegate;
                    return true;
                case UIEventEnum.ON_ENTER:
                    listener.onEnter = tempDelegate;
                    return true;
                case UIEventEnum.ON_EXIT:
                    listener.onExit = tempDelegate;
                    return true;
                case UIEventEnum.ON_UP:
                    listener.onUp = tempDelegate;
                    return true;
                case UIEventEnum.ON_SELECT:
                    listener.onSelect = tempDelegate;
                    return true;
                case UIEventEnum.ON_UPDATESELECT:
                    listener.onUpdateSelect = tempDelegate;
                    return true;
            }
            return false;
        }
        static public bool RemoveEventListener(GameObject go, UIEventEnum tempEnum)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null)
            {
                ZLogger.Error("GameObject {0} don't have UIEventListener", go.name);
                return false;
            }
            switch (tempEnum)
            {
                case UIEventEnum.ON_CLICK:
                    if (listener.onClick != null)
                        listener.onClick = null;
                    return true;
                case UIEventEnum.ON_DOWN:
                    if (listener.onDown != null)
                        listener.onDown = null;
                    return true;
                case UIEventEnum.ON_ENTER:
                    if (listener.onEnter != null)
                        listener.onEnter = null;
                    return true;
                case UIEventEnum.ON_EXIT:
                    if (listener.onExit != null)
                        listener.onExit = null;
                    return true;
                case UIEventEnum.ON_UP:
                    if (listener.onUp != null)
                        listener.onUp = null;
                    return true;
                case UIEventEnum.ON_SELECT:
                    if (listener.onSelect != null)
                        listener.onSelect = null;
                    return true;
                case UIEventEnum.ON_UPDATESELECT:
                    if (listener.onUpdateSelect != null)
                        listener.onUpdateSelect = null;
                    return true;
            }
            return false;
        }
        static public bool RemoveAllEventListener(GameObject go)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null)
            {
                ZLogger.Error("GameObject {0} don't have UIEventListener", go.name);
                return false;
            }
            if (listener.onClick != null)
                listener.onClick = null;
            if (listener.onDown != null)
                listener.onDown = null;
            if (listener.onEnter != null)
                listener.onEnter = null;
            if (listener.onExit != null)
                listener.onExit = null;
            if (listener.onUp != null)
                listener.onUp = null;
            if (listener.onSelect != null)
                listener.onSelect = null;
            if (listener.onUpdateSelect != null)
                listener.onUpdateSelect = null;
            return true;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null) onClick(gameObject);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null) onDown(gameObject);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null) onEnter(gameObject);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null) onExit(gameObject);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null) onUp(gameObject);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(gameObject);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelect != null) onUpdateSelect(gameObject);
        }
    }
}
