//=====================================================
// - FileName:      SkillComponent.cs
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
    //技能实体层类型
    public enum SkillEntityType
    {
        NULL = 0,//无

        EMIT = 1,//发射
        //1.直接（奶妈的R）（战士的针对性普攻）
        //2.跟随（女警的R）（ADC的平A）（奶妈的W）
        //3.回飞镖（纳尔的回旋镖，但只有一次伤害跟回来路上的二次伤害）
        //4.反弹
        //5.反弹不重复
        //6.范围（卡尔玛的Q）（CS的手雷）
        //7.自动寻找（闪电链)（暂定熟人的E）（狐狸的W）
        //8.间隔直接（奥巴马的R）
        //
        MOVE = 2,//移动
        //移动目标类型
        //  1.自己
        //  2.目标对象
        //  3.伤害源（圆形）
        //
        //移动类型
        //  1.自己（回城）
        //  2.跟随目标对象（三只手的R）
        //  3.移动到目标对象（梦魇的R）（正义巨像的R）
        //  4.根据目标对象角度（泰坦的R）
        //  5.根据施法对象角度（EZ的R）
        //  6.移动到目标坐标点（石头人的R）（3C牛头的大招）
        //
        RANGE = 3,//范围
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //

        SUMMON = 4,//召唤
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
    }

    //技能目标筛选类型
    public enum SkillTargetType
    {
        NULL = 0,//无需目标
        SELF = 1,//自己
        SELF_TEAMMATE = 2,//队友包括自己
        SELF_NO_TEAMMATE = 3,//队友不包括自己
        FIENDLY = 4,//友方单位
        ENEMY = 5,//敌方单位
        ALL = 6,//所有单位
    }

    //技能触发方式
    public enum SkillTriggerType
    {
        ACTIVE = 1,//主动
        ATAACK = 2,//攻击时
        BE_ATAACK = 3,//被攻击时
        DIE = 4,//死亡时
        USE_ITEM = 5,//使用道具时
        CRASH = 6,//碰撞时
    }

    //瞄准预警类型
    public enum SkillAimType
    {
        RECTANGLE = 1,//矩形（自身锚点）
        RECTANGLE_MOVE = 2,//位移矩形（自身锚点）
        ARROW_POINT = 3,//标记箭头（目标锚点）
        SECTOR_60 = 4,//60°扇形（自身锚点）
        SECTOR_90 = 5,//90°扇形（自身锚点）
        SECTOR_120 = 6,//120°扇形（自身锚点）
        SECTOR_150 = 7,//150°扇形（自身锚点）
        CIRCLE_HALF = 8,//半圆（自身锚点）
        CIRCLE_SELF = 9,//圆（自身锚点）
        CIRCLE_LOCK = 10,//锁定圆（目标锚点）
        CIRCLE_TARGET = 11,//圆（目标锚点）
    }

    //技能施法界面表现类型
    public enum SkillPreviewType
    {
        NOTHING = 0,//无
        RANGE_SECTOR_60 = 1,//范围60°扇形施法
        RANGE_SECTOR_90 = 2,//范围90°扇形形施法
        RANGE_SECTOR_180 = 3,//范围180°扇形形施法
        RANGE_SECTOR_360 = 4,//范围圆形施法
        LOCK_RANGE_ATTACK = 5,//锁定目标，地面圆形施法
        ARROW_SMALL = 6,//窄箭头施法
        ARROW_BIG = 7,//宽箭头施法
        P_RANGE_SECTOR_60 = 8,//指向60°扇形施法
        P_RANGE_SECTOR_90 = 9,//指向90°扇形形施法
        P_RANGE_SECTOR_180 = 10,//指向180°扇形形施法
        P_RANGE_SECTOR_90_LOCK = 11,//指向扇形中心线单选(90°)
    }

    public class SkillComponent : BaseComponent
    {

    }
}
