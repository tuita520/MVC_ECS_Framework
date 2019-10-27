//=====================================================
// - FileName:      MoveSystem.cs
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
    public class MoveSystem : Singleton<MoveSystem>
    {
        public GameWorld world;

        public void Init()
        {
            ZLogger.Info("Move系统层初始化");
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
            //foreach (SpeedComponent tempSpeedCom in world.SpeedComDic.Values)
            //{
            //    AngleComponent tempAngleCom;
            //    PositionComponent tempPositionCom;
            //    if (world.AngleComDic.TryGetValue(tempSpeedCom.baseEntity.guidID, out tempAngleCom) && world.PositionComDic.TryGetValue(tempSpeedCom.baseEntity.guidID, out tempPositionCom))
            //    {
            //        Quaternion rot = Quaternion.Euler(0, tempAngleCom.angleV, 0);
            //        Matrix4x4 mat = new Matrix4x4();
            //        mat.SetTRS(Vector3.zero, rot, Vector3.one);
            //        Vector4 dir =  mat.GetColumn(2).normalized;
            //    }
            //}
        }
    }
}