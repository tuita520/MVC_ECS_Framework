//=====================================================
// - FileName:      HealthComponent.cs
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
    public class HealthComponent : BaseComponent
    {
        public int HP = 0;//血条
        public int MP = 0;//魔法值
        public int EP = 0;//耐力条
        public void Reset()
        {
            HP = 0;
            MP = 0;
            EP = 0;
        }
    }
}
