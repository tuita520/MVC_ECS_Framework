//=====================================================
// - FileName:      PositionComponent.cs
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
    public class PositionComponent : BaseComponent
    {
        public Vector3 positionV = Vector3.zero;//位置，方便某些无需物体组件的对象
        public void Reset()
        {
            positionV = Vector3.zero;
        }
    }
}
