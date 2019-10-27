//=====================================================
// - FileName:      USpriteMgr.cs
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
    public class USpriteObj
    {
        public GameObject m_Obj = null;
        public AssetList m_SpriteList = null;
        public Dictionary<string, Object> m_SpriteDic = new Dictionary<string, Object>();
        public bool m_ClearScene = true;

        public void Reset()
        {
            m_Obj = null;
            m_SpriteList = null;
            m_SpriteDic.Clear();
            m_ClearScene = true;
        }
    }

    public class USpriteAsyncObj
    {
        public List<GameObject> m_ObjList = new List<GameObject>();
        public string m_IconName = string.Empty;
        public void Reset()
        {
            m_ObjList.Clear();
            m_IconName = string.Empty;
        }
    }

    public class USpriteMgr : Singleton<USpriteMgr>
    {
        //USpritePrefab节点（用来存放预置）
        public Transform USpriteRecyclePoolTrs;
        //缓存已经加载好的USprite预置以及资源
        public Dictionary<uint, USpriteObj> USpriteAssetDic = new Dictionary<uint, USpriteObj>();
        //USprite资源类对象池
        protected ClassObjectPool<USpriteObj> USpriteClassPool = new ClassObjectPool<USpriteObj>(20);
        //USprite回调类对象池
        protected ClassObjectPool<USpriteAsyncObj> USpriteAsyncObjClassPool = new ClassObjectPool<USpriteAsyncObj>(30);
        //正在异步列表
        private Dictionary<uint, List<USpriteAsyncObj>> USpriteAsyncDic = new Dictionary<uint, List<USpriteAsyncObj>>();

        public void Init(Transform canObjTrs)
        {
            ZLogger.Info("USprite层初始化");
            USpriteRecyclePoolTrs = canObjTrs;
            USpriteRecyclePoolTrs.gameObject.SetActive(false);

            EventMgr.Instance.AddEventListener(ABMgrConst.AB_CONFIG_LOAD_COMPLETE, LoadHoldUSprite);
            EventMgr.Instance.AddEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, ClearCache);
        }
        public void Clear()
        {
            EventMgr.Instance.RemoveEventListener(ABMgrConst.AB_CONFIG_LOAD_COMPLETE, LoadHoldUSprite);
            EventMgr.Instance.RemoveEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, ClearCache);
        }

        public void AfterInit()
        {
            
        }
        /// <summary>
        /// 加载部分需要一直持有的USprite
        /// </summary>
        public void LoadHoldUSprite()
        {
            EventMgr.Instance.TriggerEvent(GlobalEvent.USPRITE_LOAD_PROGRESS, "图标资源初始化...开始...", 1, 0);
            _LoadSprite(SpriteName.CommonSprite,false);
            EventMgr.Instance.TriggerEvent(GlobalEvent.USPRITE_LOAD_PROGRESS, "图标资源初始化中...", 1, 20);

            EventMgr.Instance.TriggerEvent(GlobalEvent.USPRITE_LOAD_PROGRESS, "图标资源初始化完成!!!", 1, 100);
        }

        public void ClearCache()
        {
            List<uint> tempClearKeyList = new List<uint>();
            foreach(uint tempKey in USpriteAssetDic.Keys)
            {
                USpriteObj tempUSpriteObj = USpriteAssetDic[tempKey];
                if (tempUSpriteObj.m_ClearScene)
                {
                    tempClearKeyList.Add(tempKey);
                }
            }
            foreach(uint tempKey in tempClearKeyList)
            {
                USpriteObj tempUSpriteObj = USpriteAssetDic[tempKey];
                if (USpriteAssetDic.TryGetValue(tempKey, out tempUSpriteObj) && tempUSpriteObj != null)
                {
                    ObjectPoolMgr.Instance.ReleaseObject(tempUSpriteObj.m_Obj,0,true);
                    tempUSpriteObj.Reset();
                    USpriteClassPool.Recycle(tempUSpriteObj);
                    USpriteAssetDic.Remove(tempKey);
                }
            }
            tempClearKeyList.Clear();
        }

        public void SetSpriteClearScene(string spritePath, bool clearSceneCan)
        {
            uint tempCrc = CRC32.GetCRC32(spritePath);
            USpriteObj tempUSpriteObj;
            if (USpriteAssetDic.TryGetValue(tempCrc, out tempUSpriteObj) && tempUSpriteObj != null)
            {
                tempUSpriteObj.m_ClearScene = clearSceneCan;
            }
        }

        public Sprite GetSprite(string spritePath, string iconName)
        {
            uint tempCrc = CRC32.GetCRC32(spritePath);
            Sprite returnSprite = null;
            USpriteObj tempUSpriteObj;
            if(USpriteAssetDic.TryGetValue(tempCrc,out tempUSpriteObj) && tempUSpriteObj != null)
            {
                Object tempObj;
                if(tempUSpriteObj.m_SpriteDic.TryGetValue(iconName,out tempObj)&& tempObj != null)
                {
                    Texture2D tempTexture = tempObj as Texture2D;
                    Sprite tempSprite = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f));
                    return tempSprite;
                }
            }
            else
            {
                tempUSpriteObj = USpriteClassPool.Spawn(true);
                tempUSpriteObj.m_Obj = ObjectPoolMgr.Instance.InstantiateObject(spritePath, false, false);
                tempUSpriteObj.m_Obj.transform.SetParent(USpriteRecyclePoolTrs);
                tempUSpriteObj.m_SpriteList = tempUSpriteObj.m_Obj.GetComponent<AssetList>();
                for (int i = 1, n = tempUSpriteObj.m_SpriteList.list.Count; i < n; i++)
                {
                    Texture2D tempTexture = tempUSpriteObj.m_SpriteList.list[i] as Texture2D;
                    Sprite tempSprite = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f));
                    tempUSpriteObj.m_SpriteDic.Add(tempTexture.name, tempUSpriteObj.m_SpriteList.list[i]);
                    if (tempTexture.name == iconName)
                    {
                        returnSprite = tempSprite;
                    }
                }
                USpriteAssetDic.Add(tempCrc, tempUSpriteObj);
            }
            return returnSprite;
        }

        private void _LoadSprite(string spritePath,bool clearScene)
        {
            uint tempCrc = CRC32.GetCRC32(spritePath);
            USpriteObj tempUSpriteObj;
            if (USpriteAssetDic.TryGetValue(tempCrc, out tempUSpriteObj) && tempUSpriteObj != null)
            {
                return;
            }
            else
            {
                tempUSpriteObj = USpriteClassPool.Spawn(true);
                tempUSpriteObj.m_Obj = ObjectPoolMgr.Instance.InstantiateObject(spritePath, false, false);
                tempUSpriteObj.m_Obj.transform.SetParent(USpriteRecyclePoolTrs);
                tempUSpriteObj.m_SpriteList = tempUSpriteObj.m_Obj.GetComponent<AssetList>();
                tempUSpriteObj.m_ClearScene = clearScene;
                for (int i = 1, n = tempUSpriteObj.m_SpriteList.list.Count; i < n; i++)
                {
                    Texture2D tempTexture = tempUSpriteObj.m_SpriteList.list[i] as Texture2D;
                    tempUSpriteObj.m_SpriteDic.Add(tempTexture.name, tempUSpriteObj.m_SpriteList.list[i]);
                }
                USpriteAssetDic.Add(tempCrc, tempUSpriteObj);
            }
        }

        public void SetSprite(string spritePath, string iconName, GameObject canObj)
        {
            uint tempCrc = CRC32.GetCRC32(spritePath);
            USpriteObj tempUSpriteObj;
            if (USpriteAssetDic.TryGetValue(tempCrc, out tempUSpriteObj) && tempUSpriteObj != null)
            {
                Object tempObj;
                if (tempUSpriteObj.m_SpriteDic.TryGetValue(iconName, out tempObj) && tempObj != null)
                {
                    Texture2D tempTexture = tempObj as Texture2D;
                    Sprite tempSprite = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f));
                    Image tempImage = (canObj as GameObject).GetComponent<Image>();
                    tempImage.sprite = tempSprite;
                }
            }
            else
            {
                List<USpriteAsyncObj> tempUSpriteAsyncList;
                if(USpriteAsyncDic.TryGetValue(tempCrc,out tempUSpriteAsyncList) && tempUSpriteAsyncList != null)
                {
                    bool tempHaveBoo = false;
                    foreach(USpriteAsyncObj tempUSpriteAsyncObj in tempUSpriteAsyncList)
                    {
                        if (tempUSpriteAsyncObj.m_IconName.Equals(iconName))
                        {
                            tempHaveBoo = true;
                            tempUSpriteAsyncObj.m_ObjList.Add(canObj);
                        }
                    }
                    if (!tempHaveBoo)
                    {
                        USpriteAsyncObj tempUSpriteAsyncObj = USpriteAsyncObjClassPool.Spawn(true);
                        tempUSpriteAsyncObj.m_IconName = iconName;
                        tempUSpriteAsyncObj.m_ObjList.Add(canObj);
                        tempUSpriteAsyncList.Add(tempUSpriteAsyncObj);
                    }
                }
                else
                {
                    tempUSpriteAsyncList = new List<USpriteAsyncObj>();
                    USpriteAsyncObj tempUSpriteAsyncObj = USpriteAsyncObjClassPool.Spawn(true);
                    tempUSpriteAsyncObj.m_IconName = iconName;
                    tempUSpriteAsyncObj.m_ObjList.Add(canObj);
                    tempUSpriteAsyncList.Add(tempUSpriteAsyncObj);
                    USpriteAsyncDic.Add(tempCrc,tempUSpriteAsyncList);
                    ObjectPoolMgr.Instance.InstantiateObjectAsync(spritePath, OnSetLoadFinish, LoadResPriority.RES_SLOW, false, null, null, null, false);
                }
            }
        }

        void OnSetLoadFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        {
            uint tempCrc = CRC32.GetCRC32(path);
            USpriteObj tempUSpriteObj;
            tempUSpriteObj = USpriteClassPool.Spawn(true);
            tempUSpriteObj.m_Obj = obj as GameObject;
            tempUSpriteObj.m_Obj.transform.SetParent(USpriteRecyclePoolTrs);
            tempUSpriteObj.m_SpriteList = tempUSpriteObj.m_Obj.GetComponent<AssetList>();
            for (int i = 1, n = tempUSpriteObj.m_SpriteList.list.Count; i < n; i++)
            {
                Texture2D tempTexture = tempUSpriteObj.m_SpriteList.list[i] as Texture2D;
                tempUSpriteObj.m_SpriteDic.Add(tempTexture.name, tempUSpriteObj.m_SpriteList.list[i]);
            }
            USpriteAssetDic.Add(tempCrc, tempUSpriteObj);

            List<USpriteAsyncObj> tempUSpriteAsyncList;
            if (!USpriteAsyncDic.TryGetValue(tempCrc, out tempUSpriteAsyncList) && tempUSpriteAsyncList == null)
            {
                ZLogger.Error("USpriteMgr中，设置界面USprite出错，{0}", path);
                return;
            }
            foreach (USpriteAsyncObj tempUSpriteAsyncObj in tempUSpriteAsyncList)
            {
                for(int i = 0, n = tempUSpriteAsyncObj.m_ObjList.Count; i < n; i++)
                {
                    SetSprite(path, tempUSpriteAsyncObj.m_IconName, tempUSpriteAsyncObj.m_ObjList[i]);
                }
                tempUSpriteAsyncObj.Reset();
                USpriteAsyncObjClassPool.Recycle(tempUSpriteAsyncObj);
            }
            tempUSpriteAsyncList.Clear();
            USpriteAssetDic.Remove(tempCrc);
        }

    }
}
