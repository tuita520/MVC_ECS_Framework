//=====================================================
// - FileName:      SkillAttComponent.cs
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

    //技能范围--------------------------------
    public enum SkillTargetAreaType
    {
        MONOMER = 1,//单体
        ALL = 2,//全体
        POINT = 3,//点名，带数量参数
        SECTOR = 4,//扇形
        RECTANGLE = 5,//矩形
        CIRCLE = 6,//圆形
        POINT_CIRCLE = 7,//机关点圆形
    }
    

    public enum SkillTargetConditionType
    {

    }

    

    public class SkillAttComponent : MonoBehaviour
    {

    }
}
