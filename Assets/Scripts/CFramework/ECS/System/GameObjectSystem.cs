//=====================================================
// - FileName:      GameObjectSystem.cs
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
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.ECS
{
    public class GameObjectSystem : Singleton<GameObjectSystem>
    {
        public GameWorld world;

        public void Init()
        {
            ZLogger.Info("GameObject系统层初始化");
            world = GameWorld.Instance;
        }

        public void Clear()
        {

        }

        public void AfterInit()
        {

        }

        public void Update(double deltatime)
        {
            //foreach (GameObjectComponent tempGameObjCom in world.GameObjectComDic.Values)
            //{
            //    if (tempGameObjCom.cloneObj != null)
            //    {
            //        if (tempGameObjCom.setPositionBoo)
            //        {
            //            PositionComponent tempPositionCom;
            //            if (world.PositionComDic.TryGetValue(tempGameObjCom.baseEntity.guidID, out tempPositionCom))
            //            {
            //                tempGameObjCom.parentObjTrs.localPosition = tempPositionCom.positionV;
            //            }
            //            tempGameObjCom.setPositionBoo = true;
            //        }
            //        if (tempGameObjCom.setRotationBoo)
            //        {
            //            SpeedRotComponent tempRotationCom;
            //            if (world.RotationComDic.TryGetValue(tempGameObjCom.baseEntity.guidID, out tempRotationCom))
            //            {
            //                tempGameObjCom.parentObjTrs.localRotation = Quaternion.Euler(tempRotationCom.rotationV);
            //            }
            //            tempGameObjCom.setRotationBoo = true;
            //        }
            //        if (tempGameObjCom.setSizeBoo)
            //        {
            //            SizeComponent tempSizeCom;
            //            if (world.SizeComDic.TryGetValue(tempGameObjCom.baseEntity.guidID, out tempSizeCom))
            //            {
            //                tempGameObjCom.parentObjTrs.localScale = tempSizeCom.sizeV;
            //            }
            //            tempGameObjCom.setSizeBoo = true;
            //        }
            //    }
            //}

        }
    }
}