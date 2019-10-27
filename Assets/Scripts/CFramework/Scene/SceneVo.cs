//=====================================================
// - FileName:      SceneVo.cs
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

namespace Zero.ZeroEngine.SceneFrame
{
    public class SceneVo
    {
        public int mapId = 0;
        public int[,] block = null;

        public Dictionary<int, PlayerBornObj> playerBornDic = new Dictionary<int, PlayerBornObj>();
        public Dictionary<int, MonsterObj> monsterDic = new Dictionary<int, MonsterObj>();
        public Dictionary<int, MonsterRefreshObj> monsterRefreshDic = new Dictionary<int, MonsterRefreshObj>();
        public Dictionary<int, NpcObj> npcDic= new Dictionary<int, NpcObj>();
        public Dictionary<int, TransferObj> transferDic = new Dictionary<int, TransferObj>();

        public void Reset()
        {
            mapId = 0;
            block = null;
            playerBornDic.Clear();
            monsterDic.Clear();
            monsterRefreshDic.Clear();
            npcDic.Clear();
            transferDic.Clear();
        }

    }
    public class BaseObj
    {
        public int guid = 0;
        public Vector3 locationV = new Vector3();
        public Vector3 rotationV = new Vector3();

        public virtual void Reset()
        {
            guid = 0;
            locationV = Vector3.zero;
            rotationV = Vector3.zero;
        }
    }
    /// <summary>
    /// 场景出生点，唯一ID为地图ID*10+数字1开始
    /// </summary>
    public class PlayerBornObj : BaseObj
    {
    }
    /// <summary>
    /// 场景特殊不刷新怪，唯一ID为地图ID*100+数字1开始
    /// </summary>
    public class MonsterObj : BaseObj
    {
        //怪物表对应ID
        public int monsterId = 0;

        public override void Reset()
        {
            base.Reset();
            monsterId = 0;
        }
    }
    /// <summary>
    /// 场景刷新点，唯一ID对应刷新怪点表中的ID
    /// </summary>
    public class MonsterRefreshObj : BaseObj
    {
    }
    /// <summary>
    /// 场景NPC，唯一ID对应NPC表中的ID
    /// </summary>
    public class NpcObj : BaseObj
    {
    }
    /// <summary>
    /// 场景传送点，唯一ID对应传送点表中的ID
    /// </summary>
    public class TransferObj : BaseObj
    {
        public Vector3 toLocation = new Vector3();

        public override void Reset()
        {
            base.Reset();
            toLocation = Vector3.zero;
        }
    }
}

