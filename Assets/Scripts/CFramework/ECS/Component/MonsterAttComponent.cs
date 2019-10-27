//=====================================================
// - FileName:      MonsterBelongComponent.cs
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
    public class MonsterAttComponent : BaseComponent
    {
        public int monsterID = 0;//怪物ID
        public int monsterBelongID = 0;//怪物所属刷新点ID

        public void Reset()
        {
            monsterID = 0;
            monsterBelongID = 0;
        }
    }
}
