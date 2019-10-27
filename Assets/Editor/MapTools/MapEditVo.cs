//=====================================================
// - FileName:      MapEditVo.cs
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

public class MapEditVo {
    public int mapId = 0;
    public int[,] block = null;

    public Dictionary<int, PlayerBornEditObj> playerBornEditDic = new Dictionary<int, PlayerBornEditObj>();
    public Dictionary<int, MonsterEditObj> monsterEditDic = new Dictionary<int, MonsterEditObj>();
    public Dictionary<int, MonsterRefreshEditObj> monsterRefreshEditDic = new Dictionary<int, MonsterRefreshEditObj>();
    public Dictionary<int, NpcEditObj> npcEditDic = new Dictionary<int, NpcEditObj>();
    public Dictionary<int, TransferEditObj> transferEditDic = new Dictionary<int, TransferEditObj>();

    public void Reset()
    {
        mapId = 0;
        block = null;
        playerBornEditDic.Clear();
        monsterEditDic.Clear();
        monsterRefreshEditDic.Clear();
        npcEditDic.Clear();
        transferEditDic.Clear();
    }
}

public class SceneEditObjVo
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

public class PlayerBornEditObj : SceneEditObjVo
{
}

public class MonsterEditObj : SceneEditObjVo
{
    public int monsterId = 0;
    public override void Reset()
    {
        base.Reset();
        monsterId = 0;
    }
}

public class MonsterRefreshEditObj : SceneEditObjVo
{
}

public class NpcEditObj : SceneEditObjVo
{
}

public class TransferEditObj : SceneEditObjVo
{
    public Vector3 toLocation = new Vector3();
    public override void Reset()
    {
        base.Reset();
        toLocation = Vector3.zero;
    }
}
