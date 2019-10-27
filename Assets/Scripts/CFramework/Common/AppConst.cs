//=====================================================
// - FileName:      AppConst.cs
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

namespace Zero.ZeroEngine.Common
{
    public class AppConst
    {
        public const string AppName = "TestFramework"; //应用程序名称

        public const bool DebugMode = true; //调试模式-用于内部测试(发布手机版本时，改成false后再编译资源)

        public const int GameFrameRate = 30; //游戏帧频

        public const string AssetDir = "StreamingAssets";//素材目录 

#if UNITY_ANDROID
        public const string plat = "android/";
#elif (UNITY_IPHONE || UNITY_IOS)
        public const string plat = "ios/";
#else
        public const string plat = "window/";
#endif

        public static string host
        {
            get
            {
                //GameName = "xxx";
                //PlatId = "";
                //webIP = "192.168.0.200";//"sdk.171game.com";
                //webPort = "8000";
                //DebugEngine = true;
                return "http://192.168.0.8/data/";
            }
        }
        public static string WebUrl = host + plat;    //各平台资源地址
    }
}
