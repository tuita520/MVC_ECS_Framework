//=====================================================
// - FileName:      GameWorld.cs
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
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Data;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.ECS
{
    public class GameWorld : Singleton<GameWorld>
    {
        public Transform sceneTrs;
        public Transform recycleTrs;

        //--------对象数量-------目前:7
        private ClassObjectPool<MainRoleEntity> MainRoleETClassPool = new ClassObjectPool<MainRoleEntity>(1);
        private ClassObjectPool<MonsterEntity> MonsterETClassPool = new ClassObjectPool<MonsterEntity>(100);
        private ClassObjectPool<MonsterRefreshEntity> MonsterRefreshETClassPool = new ClassObjectPool<MonsterRefreshEntity>(20);
        private ClassObjectPool<NpcRoleEntity> NpcRoleETClassPool = new ClassObjectPool<NpcRoleEntity>(20);
        private ClassObjectPool<PlayerBornEntity> PlayerBornETClassPool = new ClassObjectPool<PlayerBornEntity>(5);
        private ClassObjectPool<SkillEntity> SkillETClassPool = new ClassObjectPool<SkillEntity>(200);
        private ClassObjectPool<TransferEntity> TransferETClassPool = new ClassObjectPool<TransferEntity>(10);

        //--------对象数量-------目前:7
        public MainRoleEntity mainRoleET;
        public DDictionary<int, MonsterEntity> MonsterETDic = new DDictionary<int, MonsterEntity>();
        public DDictionary<int, MonsterRefreshEntity> MonsterRefreshETDic = new DDictionary<int, MonsterRefreshEntity>();
        public DDictionary<int, NpcRoleEntity> NpcETDic = new DDictionary<int, NpcRoleEntity>();
        public DDictionary<int, PlayerBornEntity> PlayerBornETDic = new DDictionary<int, PlayerBornEntity>();
        public DDictionary<int, SkillEntity> SkillETDic = new DDictionary<int, SkillEntity>();
        public DDictionary<int, TransferEntity> TransferETDic = new DDictionary<int, TransferEntity>();

        //--------对象数量-------目前:7
        private List<MainRoleEntity> MainRoleETRecycleList = new List<MainRoleEntity>();
        private List<MonsterEntity> MonsterETRecycleList = new List<MonsterEntity>();
        private List<MonsterRefreshEntity> MonsterRefreshETRecycleList = new List<MonsterRefreshEntity>();
        private List<NpcRoleEntity> NpcRoleETRecycleList = new List<NpcRoleEntity>();
        private List<PlayerBornEntity> PlayerBornETRecycleList = new List<PlayerBornEntity>();
        private List<SkillEntity> SkillETRecycleList = new List<SkillEntity>();
        private List<TransferEntity> TransferETRecycleList = new List<TransferEntity>();


        ////--------组件数量-------目前:17
        //public DDictionary<int, ActionComponent> ActionComDic = new DDictionary<int, ActionComponent>();//
        //public DDictionary<int, AngleComponent> AngleComDic = new DDictionary<int, AngleComponent>();//
        //public DDictionary<int, GameObjectComponent> GameObjectComDic = new DDictionary<int, GameObjectComponent>();//
        //public DDictionary<int, HealthComponent> HealthComDic = new DDictionary<int, HealthComponent>();//
        
        //public DDictionary<int, MonsterBelongComponent> MonsterBelongComDic = new DDictionary<int, MonsterBelongComponent>();//
        //public DDictionary<int, MonsterIDComponent> MonsterIDComDic = new DDictionary<int, MonsterIDComponent>();//
        //public DDictionary<int, NameComponent> NameComDic = new DDictionary<int, NameComponent>();//
        //public DDictionary<int, NpcAttComponent> NpcIDComDic = new DDictionary<int, NpcAttComponent>();//

        //public DDictionary<int, PathComponent> PathComDic = new DDictionary<int, PathComponent>();//
        //public DDictionary<int, PositionComponent> PositionComDic = new DDictionary<int, PositionComponent>();//
        //public DDictionary<int, RoleComponent> RoleComDic = new DDictionary<int, RoleComponent>();//
        //public DDictionary<int, SpeedRotComponent> RotationComDic = new DDictionary<int, SpeedRotComponent>();//
        //public DDictionary<int, RefreshAttComponent> RefreshComDic = new DDictionary<int, RefreshAttComponent>();//

        //public DDictionary<int, SizeComponent> SizeComDic = new DDictionary<int, SizeComponent>();//
        //public DDictionary<int, SpeedComponent> SpeedComDic = new DDictionary<int, SpeedComponent>();//
        //public DDictionary<int, TransferAttComponent> TransferIDComDic = new DDictionary<int, TransferAttComponent>();//
        //public DDictionary<int, WeaponComponent> WeaponComDic = new DDictionary<int, WeaponComponent>();//
        

        //public void Init(Transform canSceneTrs,Transform canRecycleTrs)
        //{
        //    ZLogger.Info("世界角色中心层初始化");
        //    sceneTrs = canSceneTrs;
        //    recycleTrs = canRecycleTrs;
        //    recycleTrs.gameObject.SetActive(false);

        //    //--------系统数量-------目前:1
        //    GameObjectSystem.Instance.Init();
        //}

        //public void Clear()
        //{

        //}

        //public void AfterInit()
        //{

        //}

        //public void Update(double deltatime)
        //{
        //    //--------系统数量-------目前:0
        //    GameObjectSystem.Instance.Update(deltatime);


        //    //--------对象数量-------目前:7
        //    //mainRoleET
        //    //MonsterETDic.ApplyDelayCommands();
        //    //MonsterRefreshETDic.ApplyDelayCommands();
        //    //NpcETDic.ApplyDelayCommands();
        //    //PlayerBornETDic.ApplyDelayCommands();
        //    //SkillETDic.ApplyDelayCommands();
        //    //TransferETDic.ApplyDelayCommands();

        //    //--------组件数量-------目前:17
        //    ActionComDic.ApplyDelayCommands();
        //    AngleComDic.ApplyDelayCommands();
        //    GameObjectComDic.ApplyDelayCommands();
        //    HealthComDic.ApplyDelayCommands();
            
        //    MonsterBelongComDic.ApplyDelayCommands();
        //    MonsterIDComDic.ApplyDelayCommands();
        //    NameComDic.ApplyDelayCommands();
        //    NpcIDComDic.ApplyDelayCommands();

        //    PathComDic.ApplyDelayCommands();
        //    PositionComDic.ApplyDelayCommands();
        //    RefreshComDic.ApplyDelayCommands();
        //    RoleComDic.ApplyDelayCommands();
        //    RotationComDic.ApplyDelayCommands();

        //    SizeComDic.ApplyDelayCommands();
        //    SpeedComDic.ApplyDelayCommands();
        //    TransferIDComDic.ApplyDelayCommands();
        //    WeaponComDic.ApplyDelayCommands();

            
        //}

        ///// <summary>
        ///// 创建主角色
        ///// </summary>
        //public void CreateMainRoleEntity(int canGuid, float canAngle, int canHP, int canMP, int canEP, int canModelID, string canName, Vector3 canPosition, int canRoleType, Vector3 canRotation,
        //    Vector3 canSize, float canHSpeed, float canVSpeed, int canRightWeapon, int canLeftWeapon,
        //    ActionEnumType canActionEnum, bool canActionLoopBoo = true, OnAnimFinishCB canAnimFinishCB = null, object param1 = null, object param2 = null, object param3 = null)
        //{
        //    if(mainRoleET == null)
        //    {
        //        mainRoleET = MainRoleETClassPool.Spawn(true);
        //    }
        //    mainRoleET.guidID = canGuid;

        //    mainRoleET.actionCom.actionName = canActionEnum;
        //    mainRoleET.actionCom.loopBoo = canActionLoopBoo;
        //    if (canAnimFinishCB != null)
        //    {
        //        mainRoleET.actionCom.animFinishCB = canAnimFinishCB;
        //        if (param1 != null)
        //        {
        //            mainRoleET.actionCom.m_Param1 = param1;
        //        }
        //        if (param2 != null)
        //        {
        //            mainRoleET.actionCom.m_Param2 = param2;
        //        }
        //        if (param3 != null)
        //        {
        //            mainRoleET.actionCom.m_Param3 = param3;
        //        }
        //    }
        //    ActionComDic.DelayAdd(mainRoleET.guidID, mainRoleET.actionCom);//
        //    mainRoleET.angleCom.angleV = canAngle;
        //    AngleComDic.DelayAdd(mainRoleET.guidID, mainRoleET.angleCom);//
        //    mainRoleET.healthCom.HP = canHP;
        //    mainRoleET.healthCom.MP = canMP;
        //    mainRoleET.healthCom.EP = canEP;
        //    HealthComDic.DelayAdd(mainRoleET.guidID, mainRoleET.healthCom);//
        //    mainRoleET.nameCom.targetName = canName;
        //    NameComDic.DelayAdd(mainRoleET.guidID, mainRoleET.nameCom);//




        //    mainRoleET.positionCom.positionV = canPosition;
        //    PositionComDic.DelayAdd(mainRoleET.guidID, mainRoleET.positionCom);//
        //    mainRoleET.roleCom.roleType = canRoleType;
        //    RoleComDic.DelayAdd(mainRoleET.guidID, mainRoleET.roleCom);//
        //    mainRoleET.rotationCom.rotationV = canRotation;
        //    RotationComDic.DelayAdd(mainRoleET.guidID, mainRoleET.rotationCom);//
        //    mainRoleET.sizeCom.sizeV = canSize;
        //    SizeComDic.DelayAdd(mainRoleET.guidID, mainRoleET.sizeCom);//

        //    mainRoleET.speedCom.hSpeed = canHSpeed;
        //    mainRoleET.speedCom.vSpeed = canVSpeed;
        //    SpeedComDic.DelayAdd(mainRoleET.guidID, mainRoleET.speedCom);//
        //    mainRoleET.weaponCom.rightWeapon = canRightWeapon;
        //    mainRoleET.weaponCom.leftWeapon = canLeftWeapon;
        //    WeaponComDic.DelayAdd(mainRoleET.guidID, mainRoleET.weaponCom);//

        //    ModelExcel tempModelData = DataMgr.Instance.tableModel.GetInfoById(canModelID);
        //    mainRoleET.gameObjCom.modelID = canModelID;
        //    ObjectPoolMgr.Instance.InstantiateObjectAsync(tempModelData.path, LoadMainRoleCallback, LoadResPriority.RES_MIDDLE, true);
        //}

        //private void LoadMainRoleCallback(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        //{
        //    if (mainRoleET.gameObjCom.parentObj == null)
        //    {
        //        GameObject tempGo = new GameObject();
        //        mainRoleET.gameObjCom.parentObj = tempGo;
        //        mainRoleET.gameObjCom.parentObjTrs = tempGo.transform;
        //        mainRoleET.gameObjCom.parentObjTrs.SetParent(sceneTrs);
        //    }
        //    else
        //    {
        //        mainRoleET.gameObjCom.parentObjTrs.SetParent(sceneTrs);
        //    }
        //    mainRoleET.gameObjCom.parentObj.name = "MainRole(" + mainRoleET.guidID.ToString() + ")";
        //    GameObject tempObj = obj as GameObject;
        //    tempObj.transform.SetParent(mainRoleET.gameObjCom.parentObj.transform);
        //    mainRoleET.gameObjCom.cloneObj = tempObj;
        //    GameObjectComDic.DelayAdd(mainRoleET.guidID, mainRoleET.gameObjCom);//

        //    Animator tempAnimator = tempObj.GetComponent<Animator>();
        //    mainRoleET.actionCom.animator = tempAnimator;
        //}

        ///// <summary>
        ///// 创建怪物
        ///// </summary>
        //public void CreateMonsterEntity(int canGuid, float canAngle, int canHP, int canMP, int canEP, int canModelID, int canMonsterBelongID, int canMonsterID, string canName, Vector3 canPosition,
        //    int canRoleType, Vector3 canRotation, Vector3 canSize, float canHSpeed, float canVSpeed, float canMoveAngle, int canRightWeapon, int canLeftWeapon,
        //    ActionEnumType canActionEnum, bool canActionLoopBoo = true, OnAnimFinishCB canAnimFinishCB = null, object param1 = null, object param2 = null, object param3 = null)
        //{
        //    MonsterEntity tempMonsterET = MonsterETClassPool.Spawn(true);
        //    tempMonsterET.guidID = canGuid;
            
        //    tempMonsterET.actionCom.actionName = canActionEnum;
        //    tempMonsterET.actionCom.loopBoo = canActionLoopBoo;
        //    if (canAnimFinishCB != null)
        //    {
        //        tempMonsterET.actionCom.animFinishCB = canAnimFinishCB;
        //        if (param1 != null)
        //        {
        //            tempMonsterET.actionCom.m_Param1 = param1;
        //        }
        //        if (param2 != null)
        //        {
        //            tempMonsterET.actionCom.m_Param2 = param2;
        //        }
        //        if (param3 != null)
        //        {
        //            tempMonsterET.actionCom.m_Param3 = param3;
        //        }
        //    }
        //    ActionComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.actionCom);//
        //    tempMonsterET.angleCom.angleV = canAngle;
        //    AngleComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.angleCom);//
        //    tempMonsterET.healthCom.HP = canHP;
        //    tempMonsterET.healthCom.MP = canMP;
        //    tempMonsterET.healthCom.EP = canEP;
        //    HealthComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.healthCom);//
        //    tempMonsterET.monsterBelongCom.monsterBelongID = canMonsterBelongID;
        //    MonsterBelongComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.monsterBelongCom);//

        //    tempMonsterET.monsterIDCom.monsterID = canMonsterID;
        //    MonsterIDComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.monsterIDCom);//
        //    tempMonsterET.nameCom.targetName = canName;
        //    NameComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.nameCom);//



        //    tempMonsterET.positionCom.positionV = canPosition;
        //    PositionComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.positionCom);//
        //    tempMonsterET.roleCom.roleType = canRoleType;
        //    RoleComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.roleCom);//

        //    tempMonsterET.rotationCom.rotationV = canRotation;
        //    RotationComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.rotationCom);//
        //    tempMonsterET.sizeCom.sizeV = canSize;
        //    SizeComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.sizeCom);//
        //    tempMonsterET.speedCom.hSpeed = canHSpeed;
        //    tempMonsterET.speedCom.vSpeed = canVSpeed;
        //    SpeedComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.speedCom);//
        //    tempMonsterET.weaponCom.rightWeapon = canRightWeapon;
        //    tempMonsterET.weaponCom.leftWeapon = canLeftWeapon;
        //    WeaponComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.weaponCom);//

        //    MonsterETDic.Add(tempMonsterET.guidID, tempMonsterET);
        //    ModelExcel tempModelData = DataMgr.Instance.tableModel.GetInfoById(canModelID);
        //    tempMonsterET.gameObjCom.modelID = canModelID;
        //    ObjectPoolMgr.Instance.InstantiateObjectAsync(tempModelData.path, LoadMonsterRoleCallback, LoadResPriority.RES_MIDDLE, true, tempMonsterET.guidID);
        //}

        //private void LoadMonsterRoleCallback(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        //{
        //    MonsterEntity tempMonsterET;
        //    int tempGuid = (int)param1;
        //    if(MonsterETDic.TryGetValue(tempGuid,out tempMonsterET))
        //    {
        //        if (tempMonsterET.gameObjCom.parentObj == null)
        //        {
        //            GameObject tempGo = new GameObject();
        //            tempMonsterET.gameObjCom.parentObj = tempGo;
        //            tempMonsterET.gameObjCom.parentObjTrs = tempGo.transform;
        //            tempMonsterET.gameObjCom.parentObjTrs.SetParent(sceneTrs);
        //        }
        //        else
        //        {
        //            tempMonsterET.gameObjCom.parentObjTrs.SetParent(sceneTrs);
        //        }
        //        tempMonsterET.gameObjCom.parentObj.name = "MonsterRole(" + tempMonsterET.guidID.ToString() + ")";
        //        GameObject tempObj = obj as GameObject;
        //        tempObj.transform.SetParent(tempMonsterET.gameObjCom.parentObj.transform);
        //        tempMonsterET.gameObjCom.cloneObj = tempObj;
        //        GameObjectComDic.DelayAdd(tempMonsterET.guidID, tempMonsterET.gameObjCom);//

        //        Animator tempAnimator = tempObj.GetComponent<Animator>();
        //        tempMonsterET.actionCom.animator = tempAnimator;
        //    }
        //    else
        //    {
        //        ZLogger.Error("创建Monster回调出错");
        //    }
        //}

        ///// <summary>
        ///// 创建怪物刷新点
        ///// </summary>
        //public void CreateMonsterRefreshEntity(int canGuid, string canName, Vector3 canPosition, int canRefreshID)
        //{
        //    MonsterRefreshEntity tempMonsterRefreshET = MonsterRefreshETClassPool.Spawn(true);
        //    tempMonsterRefreshET.guidID = canGuid;

        //    tempMonsterRefreshET.nameCom.targetName = canName;
        //    NameComDic.DelayAdd(tempMonsterRefreshET.guidID, tempMonsterRefreshET.nameCom);//
        //    tempMonsterRefreshET.positionCom.positionV = canPosition;
        //    PositionComDic.DelayAdd(tempMonsterRefreshET.guidID, tempMonsterRefreshET.positionCom);//
        //    tempMonsterRefreshET.refreshCom.refreshID = canRefreshID;
        //    MonsterRefreshExcel tempData = DataMgr.Instance.tableMonsterRefresh.GetInfoById(canRefreshID);
        //    if (tempData != null)
        //    {
        //        tempMonsterRefreshET.refreshCom.refreshMaxCount = tempData.att[3];
        //        tempMonsterRefreshET.refreshCom.refreshTimer = tempData.refreshT;
        //    }
        //    RefreshComDic.DelayAdd(tempMonsterRefreshET.guidID, tempMonsterRefreshET.refreshCom);//

        //    MonsterRefreshETDic.Add(tempMonsterRefreshET.guidID, tempMonsterRefreshET);
        //}

        ///// <summary>
        ///// 创建NPC
        ///// </summary>
        //public void CreateNpcRoleEntity(int canGuid, float canAngle, int canHP, int canMP, int canEP, int canModelID, string canName, int canNpcID, Vector3 canPosition,
        //    int canRoleType, Vector3 canRotation, Vector3 canSize, float canHSpeed, float canVSpeed, float canMoveAngle, int canRightWeapon, int canLeftWeapon,
        //    ActionEnumType canActionEnum, bool canActionLoopBoo = true, OnAnimFinishCB canAnimFinishCB = null, object param1 = null, object param2 = null, object param3 = null)
        //{
        //    NpcRoleEntity tempNpcRoleET = NpcRoleETClassPool.Spawn(true);
        //    tempNpcRoleET.guidID = canGuid;

        //    tempNpcRoleET.actionCom.actionName = canActionEnum;
        //    tempNpcRoleET.actionCom.loopBoo = canActionLoopBoo;
        //    if (canAnimFinishCB != null)
        //    {
        //        tempNpcRoleET.actionCom.animFinishCB = canAnimFinishCB;
        //        if (param1 != null)
        //        {
        //            tempNpcRoleET.actionCom.m_Param1 = param1;
        //        }
        //        if (param2 != null)
        //        {
        //            tempNpcRoleET.actionCom.m_Param2 = param2;
        //        }
        //        if (param3 != null)
        //        {
        //            tempNpcRoleET.actionCom.m_Param3 = param3;
        //        }
        //    }
        //    ActionComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.actionCom);//
        //    tempNpcRoleET.angleCom.angleV = canAngle;
        //    AngleComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.angleCom);//
        //    tempNpcRoleET.healthCom.HP = canHP;
        //    tempNpcRoleET.healthCom.MP = canMP;
        //    tempNpcRoleET.healthCom.EP = canEP;
        //    HealthComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.healthCom);//
        //    tempNpcRoleET.nameCom.targetName = canName;
        //    NameComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.nameCom);//

        //    tempNpcRoleET.npcIDCom.npcID = canNpcID;
        //    NpcIDComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.npcIDCom);//



        //    tempNpcRoleET.positionCom.positionV = canPosition;
        //    PositionComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.positionCom);//
        //    tempNpcRoleET.roleCom.roleType = canRoleType;
        //    RoleComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.roleCom);//
        //    tempNpcRoleET.rotationCom.rotationV = canRotation;
        //    RotationComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.rotationCom);//

        //    tempNpcRoleET.sizeCom.sizeV = canSize;
        //    SizeComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.sizeCom);//
        //    tempNpcRoleET.speedCom.hSpeed = canHSpeed;
        //    tempNpcRoleET.speedCom.vSpeed = canVSpeed;
        //    SpeedComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.speedCom);//
        //    tempNpcRoleET.weaponCom.rightWeapon = canRightWeapon;
        //    tempNpcRoleET.weaponCom.leftWeapon = canLeftWeapon;
        //    WeaponComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.weaponCom);//

        //    NpcETDic.Add(tempNpcRoleET.guidID, tempNpcRoleET);
        //    ModelExcel tempModelData = DataMgr.Instance.tableModel.GetInfoById(canModelID);
        //    tempNpcRoleET.gameObjCom.modelID = canModelID;
        //    ObjectPoolMgr.Instance.InstantiateObjectAsync(tempModelData.path, LoadNpcRoleCallBack, LoadResPriority.RES_MIDDLE, true, tempNpcRoleET.guidID);
        //}

        //private void LoadNpcRoleCallBack(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        //{
        //    NpcRoleEntity tempNpcRoleET;
        //    int tempGuid = (int)param1;
        //    if (NpcETDic.TryGetValue(tempGuid, out tempNpcRoleET))
        //    {
        //        if (tempNpcRoleET.gameObjCom.parentObj == null)
        //        {
        //            GameObject tempGo = new GameObject();
        //            tempNpcRoleET.gameObjCom.parentObj = tempGo;
        //            tempNpcRoleET.gameObjCom.parentObjTrs = tempGo.transform;
        //            tempNpcRoleET.gameObjCom.parentObjTrs.SetParent(sceneTrs);
        //        }
        //        else
        //        {
        //            tempNpcRoleET.gameObjCom.parentObjTrs.SetParent(sceneTrs);
        //        }
        //        tempNpcRoleET.gameObjCom.parentObj.name = "NpcRole(" + tempNpcRoleET.guidID.ToString() + ")";
        //        GameObject tempObj = obj as GameObject;
        //        tempObj.transform.SetParent(tempNpcRoleET.gameObjCom.parentObj.transform);
        //        tempNpcRoleET.gameObjCom.cloneObj = tempObj;
        //        GameObjectComDic.DelayAdd(tempNpcRoleET.guidID, tempNpcRoleET.gameObjCom);//

        //        Animator tempAnimator = tempObj.GetComponent<Animator>();
        //        tempNpcRoleET.actionCom.animator = tempAnimator;
        //    }
        //    else
        //    {
        //        ZLogger.Error("创建NpcRole回调出错");
        //    }
        //}

        ///// <summary>
        ///// 创建玩家出生点
        ///// </summary>
        //public void CreatePlayerBornEntity(int canGuid,string canName,Vector3 canPosition)
        //{
        //    PlayerBornEntity tempPlayerBornET = PlayerBornETClassPool.Spawn(true);
        //    tempPlayerBornET.guidID = canGuid;

        //    tempPlayerBornET.nameCom.targetName = canName;
        //    NameComDic.DelayAdd(tempPlayerBornET.guidID, tempPlayerBornET.nameCom);//
        //    tempPlayerBornET.positionCom.positionV = canPosition;
        //    PositionComDic.DelayAdd(tempPlayerBornET.guidID, tempPlayerBornET.positionCom);//
        //}

        ///// <summary>
        ///// 创建技能对象
        ///// </summary>
        //public void CreateSkillEntity()
        //{

        //}

        ///// <summary>
        ///// 创建传送点
        ///// </summary>
        //public void CreateTransferEntity(int canGuid, int canModelID, string canName, Vector3 canPosition, Vector3 canRotation, Vector3 canSize, int canTransferID)
        //{
        //    TransferEntity tempTransferET = TransferETClassPool.Spawn(true);
        //    tempTransferET.guidID = canGuid;
            
        //    tempTransferET.nameCom.targetName = canName;
        //    NameComDic.DelayAdd(tempTransferET.guidID, tempTransferET.nameCom);//
        //    tempTransferET.positionCom.positionV = canPosition;
        //    PositionComDic.DelayAdd(tempTransferET.guidID, tempTransferET.positionCom);//
        //    tempTransferET.rotationCom.rotationV = canRotation;
        //    RotationComDic.DelayAdd(tempTransferET.guidID, tempTransferET.rotationCom);//
        //    tempTransferET.sizeCom.sizeV = canSize;
        //    SizeComDic.DelayAdd(tempTransferET.guidID, tempTransferET.sizeCom);//

        //    tempTransferET.transferIDCom.transferID = canTransferID;
        //    TransferIDComDic.DelayAdd(tempTransferET.guidID, tempTransferET.transferIDCom);//

        //    TransferETDic.Add(tempTransferET.guidID, tempTransferET);
        //    ModelExcel tempModelData = DataMgr.Instance.tableModel.GetInfoById(canModelID);
        //    tempTransferET.gameObjCom.modelID = canModelID;
        //    ObjectPoolMgr.Instance.InstantiateObjectAsync(tempModelData.path, LoadTransferCallBack, LoadResPriority.RES_MIDDLE, true, tempTransferET.guidID);
        //}

        //private void LoadTransferCallBack(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        //{
        //    TransferEntity tempTransferET;
        //    int tempGuid = (int)param1;
        //    if (TransferETDic.TryGetValue(tempGuid, out tempTransferET))
        //    {
        //        if (tempTransferET.gameObjCom.parentObj == null)
        //        {
        //            GameObject tempGo = new GameObject();
        //            tempTransferET.gameObjCom.parentObj = tempGo;
        //            tempTransferET.gameObjCom.parentObjTrs = tempGo.transform;
        //            tempTransferET.gameObjCom.parentObjTrs.SetParent(sceneTrs);
        //        }
        //        else
        //        {
        //            tempTransferET.gameObjCom.parentObjTrs.SetParent(sceneTrs);
        //        }
        //        tempTransferET.gameObjCom.parentObj.name = "Transfer(" + tempTransferET.guidID.ToString() + ")";
        //        GameObject tempObj = obj as GameObject;
        //        tempObj.transform.SetParent(tempTransferET.gameObjCom.parentObj.transform);
        //        tempTransferET.gameObjCom.cloneObj = tempObj;
        //        GameObjectComDic.DelayAdd(tempTransferET.guidID, tempTransferET.gameObjCom);//
        //    }
        //    else
        //    {
        //        ZLogger.Error("创建Transfer回调出错");
        //    }
        //}

    }
}
