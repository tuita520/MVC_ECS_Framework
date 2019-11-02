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
    //技能范围--------------------------------
    //public enum SkillTargetAreaType
    //{
    //    MONOMER = 1,//单体
    //    ALL = 2,//全体
    //    POINT = 3,//点名，带数量参数
    //    SECTOR = 4,//扇形
    //    RECTANGLE = 5,//矩形
    //    CIRCLE = 6,//圆形
    //    POINT_CIRCLE = 7,//机关点圆形
    //}
    
    //发射模块发射类型
    public enum SkillEmitType
    {
        NULL = 0,//无
        DIRECT = 1,//直接（奶妈的R）（战士的针对性普攻）（金克斯的R）
        FOLLOW = 2,//跟随（女警的R）（ADC的平A）（奶妈的W）
        CIRCLE_ROUND = 3,//回飞镖（纳尔的回旋镖，但只有一次伤害跟回来路上的二次伤害）
        BOUNCE = 4,//反弹
        BOUNCE_NO_RE = 5,//反弹不重复
        RANGE = 6,//范围（卡尔玛的Q）（CS的手雷）
        AUTO_FIND = 7,//自动寻找（闪电链)（暂定熟人的E）（狐狸的W）
        INTERVAL_DIRECT = 8,//间隔直接（奥巴马的R）
    }
    //发射模块角度类型
    public enum SkillEmitAngleType
    {
        NULL = 0,//无
        SELF = 1,//施法者角度
        TARGET = 2,//目标相对于施法者角度
    }
    //移动模块移动目标类型
    public enum SkillMoveTargetType
    {
        NULL = 0,//无
        SELF = 1,//自己
        TARGET = 2,//目标对象
        DAMAGE_ROOT = 3,//伤害源（圆形）
    }
    //移动模块移动类型
    public enum SkillMoveMoveType
    {
        NULL = 0,//无
        SELF = 1,//自己（回城）
        FOLLOW_TARGET = 2,//跟随目标对象（三只手的R）
        MOVE_TO_TARGET = 3,//移动到目标对象（梦魇的R）（正义巨像的R）
        FOR_TARGET_ANGLE = 4,//根据目标对象角度（泰坦的R）
        SELF_ANGLE = 5,//根据施法对象角度（EZ的R）
        MOVE_POS = 6,//移动到目标坐标点（石头人的R）（3C牛头的大招）
        CIRCLE_ROUND = 7,//回旋镖（德莱文的R）（狐狸的Q）
    }
    //范围模块范围类型
    public enum SkillRangeType
    {
        NULL = 0,//无
        SELF = 1,//以自己为中心（石头人的E）
        SELF_CONTINUE = 2,//以自己为中心（持续施法）（风女的R）
        TARGET = 3,//以目标对象为中心（天使的R）
        TARGET_CONTINUE = 4,//以目标对象为中心（持续施法）
        TARGET_POS = 5,//以目标坐标为中心（泽拉斯的R）
        TARGET_POS_CONTINUE = 6,//以目标坐标为中心（持续施法）（狗头的E）
        SELF_TARGET_MIDDLE = 7,//以自己到目标坐标之间的偏移点为中心（刀妹的E）
        SELF_TARGET_MIDDLE_CONTINUE = 8,//以自己到目标坐标之间的偏移点为中心（持续施法）
    }
    //范围模块形状类型
    public enum SkillRangeShapeType
    {
        NULL = 0,//无
        RECTANGLE = 1,//矩形
        SECTOR = 2,//扇形
        CIRCLE = 3,//圆形
    }
    //召唤模块召唤类型
    public enum SkillSummonType
    {
        NULL = 0,//无
        RETINUE = 1,//随从
        WILD = 2,//野怪
    }
    //召唤模型召唤方式
    public enum SkillSummonWayType
    {
        NULL = 0,//无
        DIRECT_POS = 1,//在目标坐标直接召唤
        CIRCLE_CONTINUE = 2,//魔法阵持续召唤
    }
    

    public class SkillComponent : BaseComponent
    {
        public SkillEntityType skillEntityV = SkillEntityType.NULL;//实体模块
        public SkillEmitExcel skillEmitData = null;//发射模块
        public SkillMoveExcel skillMoveData = null;//移动模块
        public SkillRangeExcel skillRangeData = null;//范围模块
        public SkillSummonExcel skillSummonData = null;//召唤模块
        
        public void Reset()
        {
            skillEntityV = SkillEntityType.NULL;
            skillEmitData = null;
            skillMoveData = null;
            skillRangeData = null;
            skillSummonData = null;
        }
    }
}
