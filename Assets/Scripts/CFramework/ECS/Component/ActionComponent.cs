//=====================================================
// - FileName:      ActionComponent.cs
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
    public enum ActionEnumType
    {
        NULL = 0,
        IDLE = 1,
        NORMAL_ATACK = 2,
    }

    public delegate void OnAnimFinishCB(Animator canAnim, ActionEnumType canActionType, object param1 = null, object param2 = null, object param3 = null);

    //public class ActionCallBackClass
    //{
    //    public OnAnimFinishCB animFinishCB = null;
    //    public object m_Param1, m_Param2, m_Param3 = null;

    //    public void Reset()
    //    {
    //        animFinishCB = null;
    //        m_Param1 = m_Param2 = m_Param3 = null;
    //    }
    //}

    public class ActionComponent : BaseComponent
    {
        public Animator animator;//动画组件
        public ActionEnumType actionName = ActionEnumType.NULL;//动作枚举
        public bool loopBoo = false;//是否循环
        public OnAnimFinishCB animFinishCB = null;//动作播放完回调
        public object m_Param1, m_Param2, m_Param3 = null;//回调参数

        public void Reset()
        {
            animator = null;
            actionName = ActionEnumType.NULL;
            loopBoo = false;
            animFinishCB = null;
            m_Param1 = m_Param2 = m_Param3 = null;
        }
    }
}