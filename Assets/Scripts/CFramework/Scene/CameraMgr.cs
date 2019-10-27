//=====================================================
// - FileName:      CameraMgr.cs
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
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Util;

//=====================================================
// - 1.
// - 2.
// - 3.
// - 4.
// - 5.
// - 6.
//======================================================

namespace Zero.ZeroEngine.SceneFrame
{
    public class CameraMgr : Singleton<CameraMgr>
    {
        public GameObject mainCameraObj;
        public Camera mainCemara;

        public void Init(GameObject canMainCameraObj)
        {
            ZLogger.Info("摄像机管理层初始化");
            mainCameraObj = canMainCameraObj;
            mainCemara = canMainCameraObj.GetComponent<Camera>();
        }

        public void Clear()
        {

        }

        public void AfterInit()
        {

        }

        public void Update(double deltatime)
        {

        }
    }
}
