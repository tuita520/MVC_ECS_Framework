//=====================================================
// - FileName:      LoginMgr.cs
// - Created:       mahuibao
// - UserName:      2019-01-09
// - Email:         1023276156@qq.com
// - Description:   登录管理层
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.Login
{
    /// <summary>
    /// 登录管理层
    /// </summary>
    public class LoginMgr : Singleton<LoginMgr>
    {

        public void Init()
        {
            ZLogger.Info("登录管理层初始化");
        }
        public void AfterInit()
        {

        }
        public void EnterLogin()
        {
            //异步加载登录场景
            //ResourcesMgr.Instance.AsyncLoadScene(LoginDefine.LOGIN_SCENE_PATH);
            //并显示加载的进度条
            //加载完成之后再打开注册登录界面
        }
    }
}