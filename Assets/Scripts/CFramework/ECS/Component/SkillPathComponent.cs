//=====================================================
// - FileName:      SkillPathComponent.cs
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
    public class SkillPathComponent : BaseComponent
    {
        public int targetGuid = 0;//目标唯一ID
        public int followGuid = 0;//跟随目标唯一ID
        public Vector3 starPos = Vector3.zero;//开始位置
        public Vector3 targetPos = Vector3.zero;//目标位置
        public float distance = 0;//距离
        public bool autoDestroyBoo = false;//是否自动销毁
        public float releaseTime = 0;//释放时间
        public float stayTime = 0;//停留时间（移动模块）
        public bool catchDestroyBoo = false;//到达后是否立即销毁（移动模块）
        public int moveSpeed = 0;//水平移动速度

        public void Reset()
        {

        }
    }
}