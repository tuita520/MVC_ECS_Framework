//=====================================================
// - FileName:      RoleComponent.cs
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
    public class RoleComponent : BaseComponent
    {
        public bool isMainRoleBoo = false;//是否主角
        public bool isEnemyBoo = false;//是否敌人
        public bool isTeammateBoo = false;//是否队友
        public int teamID = 0;//队伍ID
        
        public void Reset()
        {
            isMainRoleBoo = false;
            isEnemyBoo = false;
            isTeammateBoo = false;
            teamID = 0;
        }
    }
}
