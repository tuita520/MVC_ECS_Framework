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
    //霸体类型
    public enum SkillBodyType
    {
        NULL = 0,//无霸体
        BODY = 1,//有霸体
    }

    //吟唱类型
    public enum SkillSingType
    {
        NULL = 0,//无吟唱
        SING_BREAK = 1,//吟唱可打断
        SING_NO_BREAK = 2,//吟唱不可打断
    }

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
        //  7.回旋镖（德莱文的R）（狐狸的Q）
        //
        RANGE = 3,//范围
        //范围类型
        //  1.以自己为中心（石头人的E）
        //  2.以自己为中心（持续施法）（风女的R）
        //  3.以目标对象为中心（天使的R）
        //  4.以目标对象为中心（持续施法）
        //  5.以目标坐标为中心（泽拉斯的R）
        //  6.以目标坐标为中心（持续施法）（狗头的E）
        //  7.以自己到目标坐标之间的偏移点为中心（刀妹的E）
        //  8.以自己到目标坐标之间的偏移点为中心（持续施法）
        //
        //形状类型
        //  1.矩形
        //  2.扇形
        //  3.圆形
        //

        SUMMON = 4,//召唤
        //召唤物类型
        //  1.随从
        //  2.野怪
        //  
        //召唤方法
        //  1.直接召唤
        //  2.魔法阵持续召唤
        //  3.施法距离，朝向，直接召唤
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

    //结算层打断类型
    public enum SkillAcountInterruptType
    {
        NULL = 0,//无
        HIT = 1,//播放受击
        BLOW_UP = 2,//播放击飞
        DIZZINESS = 3,//播放眩晕
        SLEEP = 4,//播放睡眠
    }

    public enum ActiveSkillUseType
    {
        NULL = 0,//无
        STAR = 1,//准备模式
        SING_RELEASE = 2,//吟唱加释放模式
        RELEASE = 3,//释放模式
    }



    /// <summary>
    /// 被释放技能类
    /// </summary>
    public class BeSkillClass
    {
        public int skillGuid = 0;//技能实体唯一ID
        public int skillID = 0;//技能ID
        public bool initBoo = false;//是否实例化过
        public float releaseObjTime = 0;//技能特效释放时间
        public List<GameObject> beSkillEffectObjList = new List<GameObject>();//受到攻击的技能特效
        public SkillAcountExcel skillAcountData = null;//结算模块数据

        public void Reset()
        {
            skillGuid = 0;
            skillID = 0;
            initBoo = false;
            releaseObjTime = 0;
            beSkillEffectObjList.Clear();
            skillAcountData = null;
        }
    }
    
    //施法者技能属性组件
    public class SkillAttComponent : BaseComponent
    {
        //自己所释放的技能
        public int inputUseSkillID = 0;//输入技能
        public int nowUseSkillId = 0;//正在使用技能
        public float countTime = 0;//累计时间
        public int releaseSkillGuid = 0;//释放出来的技能实体唯一ID
        public bool isAimAvtiveBoo = false;
        public ActiveSkillExcel activeSkillData = null;//技能上层模块
        public List<GameObject> skillEffectObjList = new List<GameObject>();


        //自己所受到的技能
        public List<BeSkillClass> beSkillList = new List<BeSkillClass>();//受到攻击的技能
        

        public void Reset()
        {
            inputUseSkillID = 0;
            nowUseSkillId = 0;
            countTime = 0;
            releaseSkillGuid = 0;
            isAimAvtiveBoo = false;
            activeSkillData = null;
            skillEffectObjList.Clear();

            foreach (BeSkillClass tempClass in beSkillList)
            {
                tempClass.Reset();
                GameWorld.Instance.SkillAttComBeSkillClassPool.Recycle(tempClass);
            }
            beSkillList.Clear();
        }
    }
}
