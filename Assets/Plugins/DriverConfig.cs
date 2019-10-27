//=====================================================
// - FileName:      DriverConfig.cs
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
namespace Zero.Plugins.Base
{
    public static class DriverConfig
    {
        /// <summary>
        /// 默认帧数
        /// </summary>
        public static int defaultFrame = 60;
        /// <summary>
        /// 待机帧数
        /// </summary>
        public static int lowFrame = 10;
        /// <summary>
        /// 游戏设定分辨率宽度
        /// </summary>
        public static readonly int GameResolutionWidth = 1920;
        /// <summary>
        /// 游戏设定分辨率高度
        /// </summary>
        public static readonly int GameResolutionHeight = 1080;

        public static byte[] urlListContent = null;
        //public static //edit
        public static bool isCheckUpdate = false;
        
    }
}
