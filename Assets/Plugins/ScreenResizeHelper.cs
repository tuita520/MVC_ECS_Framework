//=====================================================
// - FileName:      ScreenResizeHelper.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero.Plugins.Base
{
    /// <summary>
    /// 屏幕缩放辅助类
    /// </summary>
    public class ScreenResizeHelper
    {
        /// <summary>
        /// 屏幕缩放委托
        /// </summary>
        public delegate void OnScreenResize();

        private static int _width = 0;
        /// <summary>
        /// 屏幕缩放后宽度
        /// </summary>
        public static int width{ get { return _width; } }
        private static int _height = 0;
        /// <summary>
        /// 屏幕缩放后高度
        /// </summary>
        public static int height { get { return _height; } }

        private static OnScreenResize _onScreenResize;

        public static void Init()
        {
            _width = Screen.width;
            _height = Screen.height;

            _onScreenResize = null;
        }

        public static void AddCallback(OnScreenResize osr)
        {
            if(_onScreenResize == null)
            {
                _onScreenResize = osr;
            }
            else
            {
                Delegate[] ds = _onScreenResize.GetInvocationList();
                int dsCount = ds.Length;
                for(int i=0;i<dsCount;++i)
                {
                    if(ds[i].Equals(osr))
                    {
                        BaseLogger.Warning("ScreenResizeHelper  AddCallback  duplicate ({0})", osr);
                    }
                }
                _onScreenResize -= osr;
            }
        }

        public static void RemoveCallback(OnScreenResize osr)
        {
            if (_onScreenResize != null)
                _onScreenResize -= osr;
        }

        public static void ClearupCallback()
        {
            if(_onScreenResize != null)
            {
                OnScreenResize newOnScreenResize = null;
                Delegate[] ds = _onScreenResize.GetInvocationList();
                int dsCount = ds.Length;
                if(dsCount >0)
                {
                    newOnScreenResize = ds[0] as OnScreenResize;
                }
                for(int i =0;i<dsCount;++i)
                {
                    newOnScreenResize += (ds[i] as OnScreenResize);
                }

                _onScreenResize = newOnScreenResize;
            }
        }

        public static void RemoveAllCallback()
        {
            _onScreenResize = null;
        }

        public static void Update()
        {
            int curWidth = Screen.width;
            int curHeight = Screen.height;
            if(curWidth != _width || curHeight != _height)
            {
                _width = curWidth;
                _height = curHeight;

                if (_onScreenResize != null)
                {
                    _onScreenResize();
                }
            }
        }

        public static bool isFitWidth
        {
            get
            {
                return (((float)_width / _height) <= ((float)DriverConfig.GameResolutionWidth/DriverConfig.GameResolutionHeight));
            }
        }

        public static Vector2 ScreenSize()
        {
            if (isFitWidth)
                return new Vector2(DriverConfig.GameResolutionWidth, ((float)_height / _width) * DriverConfig.GameResolutionWidth);
            else
                return new Vector2(((float)_width / _height) * DriverConfig.GameResolutionWidth, DriverConfig.GameResolutionHeight);
        }
    }
}
