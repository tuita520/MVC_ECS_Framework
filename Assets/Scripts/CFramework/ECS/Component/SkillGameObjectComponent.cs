//=====================================================
// - FileName:      SkillGameObjectComponent.cs
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
    public class SkillGameObjectComponent : BaseComponent
    {
        public GameObject parentObj = null;//对象Obj
        public Transform parentObjTrs = null;//对象Trs
        public GameObject cloneObj = null;//读取资源实例化的对象，位于上面节点之下
        public bool isClearing = false;//是否在清理中
        public int modelID = 0;//模型ID
        public bool isTriggerBoo = true;//是否是触发器
        public Collider selfCollider = null;//触发器或碰撞体
        //public Rigidbody selfRigidbody = null;//刚体组件

        //public GameObject topPointObj = null;//头顶节点
        //public GameObject buttomPointObj = null;//脚底节点
        //public GameObject bodyObj = null;//身体节点
        //public Renderer bodyMat = null;//身体材质
        //public GameObject wingPointObj = null;//翅膀节点
        //public GameObject weaponLeftPointObj = null;//左手武器节点
        //public GameObject weaponRightPointObj = null;//右手武器节点

        //public GameObject centerBone = null;//中心骨骼，作为受击点或发射默认点
        public float hitRadius = 0f;//碰撞半径
        public float hitLong = 0f;//碰撞长度
        public float hitWidth = 0f;//碰撞宽度
        public float hitHeight = 0f;//碰撞高度

        public void Reset()
        {
            if (parentObj != null)
            {
                parentObj.name += "(Recycle)";
                parentObjTrs.SetParent(GameWorld.Instance.recycleTrs);
                parentObjTrs.localPosition = Vector3.zero;
                parentObjTrs.localRotation = Quaternion.identity;
                parentObjTrs.localScale = Vector3.one;
            }
            cloneObj = null;
            isClearing = false;
            modelID = 0;
            isTriggerBoo = true;
            if (selfCollider != null)
            {
                selfCollider.isTrigger = isTriggerBoo;
            }
            //if (selfRigidbody != null)
            //{
            //    selfRigidbody = null;
            //}
            //if (topPointObj != null)
            //{
            //    topPointObj = null;
            //}
            //if (buttomPointObj != null)
            //{
            //    buttomPointObj = null;
            //}
            //if (bodyObj != null)
            //{
            //    bodyObj = null;
            //}
            //if (bodyMat != null)
            //{
            //    bodyMat = null;
            //}
            //if (wingPointObj != null)
            //{
            //    wingPointObj = null;
            //}
            //if (weaponLeftPointObj != null)
            //{
            //    weaponLeftPointObj = null;
            //}
            //if (weaponRightPointObj != null)
            //{
            //    weaponRightPointObj = null;
            //}
            //if (centerBone != null)
            //{
            //    centerBone = null;
            //}
            hitRadius = 0f;
            hitLong = 0f;
            hitWidth = 0f;
            hitHeight = 0f;
        }

    }
}
