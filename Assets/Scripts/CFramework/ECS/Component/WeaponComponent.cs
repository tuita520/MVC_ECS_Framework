//=====================================================
// - FileName:      WeaponComponent.cs
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
    public class WeaponComponent : BaseComponent
    {
        public int rightWeapon = 0;//右边武器模型ID
        public GameObject rigetWeaponObj = null;
        public int leftWeapon = 0;//左边武器模型ID
        public GameObject leftWeaponObj = null;
        public void Reset()
        {
            rightWeapon = 0;
            rigetWeaponObj = null;
            leftWeapon = 0;
            leftWeaponObj = null;
        }
    }
}
