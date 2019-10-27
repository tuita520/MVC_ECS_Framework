//=====================================================
// - FileName:      BaseEntity.cs
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


    public class BaseEntity : BaseECSItem
    {
        public int guidID = 0;//唯一ID
        public int roleType = 0;//大类型
        public int roleSubtype = 0;//子类型

        public virtual void Reset()
        {
            guidID = 0;
            roleType = 0;
            roleSubtype = 0;
        }
    }
}
