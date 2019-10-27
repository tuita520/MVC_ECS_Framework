//=====================================================
// - FileName:      UTextureMgr.cs
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
using UnityEngine.UI;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.SceneFrame;
using Zero.ZeroEngine.Util;

//=====================================================
// - 1.
// - 2.
// - 3.
// - 4.
// - 5.
// - 6.
//======================================================

namespace Zero.ZeroEngine.UI
{
    public class UTextureObj
    {
        public Texture2D m_Obj = null;
        public bool m_ClearScene = true;

        public void Reset()
        {
            m_Obj = null;
            m_ClearScene = true;
        }
    }

    public class UTextureMgr : Singleton<UTextureMgr>
    {
        public const string TEXTURE_PATH = "Assets/Resources/UI/Image/";

        //缓存已经加载好的UTexture
        public Dictionary<uint, UTextureObj> UTextureAssetDic = new Dictionary<uint, UTextureObj>();
        //UTexture资源类对象池
        protected ClassObjectPool<UTextureObj> UTextureClassPool = new ClassObjectPool<UTextureObj>(20);
        //正在异步列表（里面包括了全部需要设置Texture的对象）（其实资源加载中，异步加载已经做了回调的累加，加载完资源后会回调所有，所以可以选择不在这里设置的）
        private Dictionary<uint, List<GameObject>> UTextureAsyncDic = new Dictionary<uint, List<GameObject>>();

        public void Init()
        {
            ZLogger.Info("UTexture层初始化");

            EventMgr.Instance.AddEventListener(ABMgrConst.AB_CONFIG_LOAD_COMPLETE, LoasHoldTexture);
            EventMgr.Instance.AddEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, ClearCache);
        }
        public void Clear()
        {
            EventMgr.Instance.RemoveEventListener(ABMgrConst.AB_CONFIG_LOAD_COMPLETE, LoasHoldTexture);
            EventMgr.Instance.RemoveEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, ClearCache);
        }
        public void AfterInit()
        {

        }
        /// <summary>
        /// 加载部分需要一直持有的Texture
        /// </summary>
        public void LoasHoldTexture()
        {
            EventMgr.Instance.TriggerEvent(GlobalEvent.UTEXTURE_LOAD_PROGRESS, "图片资源初始化...开始...", 2, 0);
            //_LoadTexture();
            EventMgr.Instance.TriggerEvent(GlobalEvent.UTEXTURE_LOAD_PROGRESS, "图片资源初始化中...", 2, 60);

            EventMgr.Instance.TriggerEvent(GlobalEvent.UTEXTURE_LOAD_PROGRESS, "图片资源初始化完成!!!", 2, 100);
        }

        public void ClearCache()
        {
            List<uint> tempClearKeyList = new List<uint>();
            foreach(uint tempKey in UTextureAssetDic.Keys)
            {
                UTextureObj tempUTextureObj = UTextureAssetDic[tempKey];
                if (tempUTextureObj.m_ClearScene)
                {
                    tempClearKeyList.Add(tempKey);
                }
            }
            foreach(uint tempKey in tempClearKeyList)
            {
                UTextureObj tempUTextureObj;
                if (UTextureAssetDic.TryGetValue(tempKey, out tempUTextureObj) && tempUTextureObj != null)
                {
                    ResourcesMgr.Instance.ReleaseResource(tempUTextureObj.m_Obj, true);
                    tempUTextureObj.Reset();
                    UTextureClassPool.Recycle(tempUTextureObj);
                    UTextureAssetDic.Remove(tempKey);
                }
            }
            tempClearKeyList.Clear();
        }

        public void SetTextureClearScene(string pathCan, bool clearScene)
        {
            string tempPath = TEXTURE_PATH + pathCan;
            uint tempCrc = CRC32.GetCRC32(tempPath);
            UTextureObj tempUTextureObj;
            if (UTextureAssetDic.TryGetValue(tempCrc, out tempUTextureObj) && tempUTextureObj != null)
            {
                tempUTextureObj.m_ClearScene = clearScene;
            }
        }

        public Texture2D GetTexture(string pathCan)
        {
            return _LoadTexture(pathCan,false);
        }

        private Texture2D _LoadTexture(string pathCan, bool clearScene)
        {
            string tempPath = TEXTURE_PATH + pathCan;
            uint tempCrc = CRC32.GetCRC32(tempPath);
            UTextureObj tempUTextureObj;
            if (UTextureAssetDic.TryGetValue(tempCrc, out tempUTextureObj) && tempUTextureObj != null)
            {
                return tempUTextureObj.m_Obj;
            }
            else
            {
                tempUTextureObj = UTextureClassPool.Spawn(true);
                tempUTextureObj.m_Obj = ResourcesMgr.Instance.LoadResource<Texture2D>(tempPath);
                tempUTextureObj.m_ClearScene = clearScene;
                UTextureAssetDic.Add(tempCrc, tempUTextureObj);
                return tempUTextureObj.m_Obj;
            }
        }

        public void SetTexture(string pathCan,GameObject objCan,bool clearScene = true)
        {
            string tempPath = TEXTURE_PATH + pathCan;
            uint tempCrc = CRC32.GetCRC32(tempPath);
            UTextureObj tempUTextureObj;
            if (UTextureAssetDic.TryGetValue(tempCrc, out tempUTextureObj) && tempUTextureObj != null)
            {
                RawImage tempRawImage = objCan.GetComponent<RawImage>();
                tempRawImage.texture = tempUTextureObj.m_Obj;
            }
            else
            {
                List<GameObject> tempObjList;
                if(UTextureAsyncDic.TryGetValue(tempCrc,out tempObjList) && tempObjList != null)
                {
                    tempObjList.Add(objCan);
                }
                else
                {
                    tempObjList = new List<GameObject>();
                    tempObjList.Add(objCan);
                    UTextureAsyncDic.Add(tempCrc, tempObjList);
                    ResourcesMgr.Instance.AsyncLoadResource(tempPath, OnSetLoadFinish, LoadResPriority.RES_SLOW, false, clearScene);
                }
            }
        }
        void OnSetLoadFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        {
            uint tempCrc = CRC32.GetCRC32(path);
            UTextureObj tempUTextureObj;
            tempUTextureObj = UTextureClassPool.Spawn(true);
            tempUTextureObj.m_Obj = obj as Texture2D;
            tempUTextureObj.m_ClearScene = (bool)param1;
            UTextureAssetDic.Add(tempCrc, tempUTextureObj);
            List<GameObject> tempObjList;
            if (UTextureAsyncDic.TryGetValue(tempCrc, out tempObjList) && tempObjList != null)
            {
                for(int i = 0, n = tempObjList.Count; i < n; i++)
                {
                    RawImage tempRawImage = tempObjList[i].GetComponent<RawImage>();
                    tempRawImage.texture = tempUTextureObj.m_Obj;
                }
            }
            tempObjList.Clear();
            UTextureAsyncDic.Remove(tempCrc);
        }
    }
}
