//=====================================================
// - FileName:      NameComponent.cs
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

namespace Zero.ZeroEngine.ECS
{
    public class NameComponent : BaseComponent
    {
        public string targetName = string.Empty;//名字
        public bool isActiveBoo = false;//是否显示出名字
        public void Reset()
        {
            targetName = string.Empty;
            isActiveBoo = false;
        }
    }
}
