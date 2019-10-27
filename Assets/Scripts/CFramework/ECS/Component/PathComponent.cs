//=====================================================
// - FileName:      PathComponent.cs
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
using UnityEngine.AI;

namespace Zero.ZeroEngine.ECS
{
    public delegate void OnMovingCB(PathComponent canPathCom, object param1 = null, object param2 = null, object param3 = null);
    public delegate void OnMoveEndCB(PathComponent canPathCom, object param1 = null, object param2 = null, object param3 = null);
    public delegate void OnMoveDistanceCB(PathComponent canPathCom, object param1 = null, object param2 = null, object param3 = null);

    public class PathComponent : BaseComponent
    {
        public float hSpeed = 0f;//平行位移速度（目前仅仅使用这个）
        public float vSpeed = 0f;//跳跃位移速度
        public float rotationSpeed = 10;//旋转速度
        public NavMeshAgent navMeshAgent = null;//navMeshAgent组件
        public GameObject selfObj = null;//对象Obj
        public int targetGuid = 0;//目标唯一ID
        public int followTargetGuid = 0;//跟随目标唯一ID
        public bool isLock = false;//是否锁定，不进行位移
        public bool handMoveBoo = false;//是否手动位移
        public bool agetnMoveBoo = false;//是否根据对象位移
        public Vector3 targetPos = Vector3.zero;//移动到的目标位置
        public Vector3 targetRot = Vector3.zero;//移动到的目标旋转值
        public Matrix4x4 moveDir = Matrix4x4.zero;//手动位移时，移动方向
        public OnMovingCB movingCB = null;//移动中回调
        public object moving_param1, moving_param2, moving_param3 = null;
        public OnMoveEndCB moveEndCB = null;//移动完回调
        public object moveEnd_param1, moveEnd_param2, moveEnd_param3 = null;
        public float moveDistance = 0;//距离目标位置距离判断值
        public OnMoveDistanceCB moveDisCB = null;//到达判断值回调
        public object moveDis_param1, moveDis_param2, moveDis_param3 = null;

        public void Reset()
        {
            hSpeed = 0f;
            vSpeed = 0f;
            rotationSpeed = 10;
            targetGuid = 0;
            followTargetGuid = 0;
            isLock = false;
            handMoveBoo = false;
            agetnMoveBoo = false;
            targetPos = Vector3.zero;
            targetRot = Vector3.zero;
            moveDir = Matrix4x4.zero;
            movingCB = null;
            moving_param1 = moving_param2 = moving_param3 = null;
            moveEndCB = null;
            moveEnd_param1 = moveEnd_param2 = moveEnd_param3 = null;
            moveDistance = 0;
            moveDisCB = null;
            moveDis_param1 = moveDis_param2 = moveDis_param3 = null;
    }
    }
}
