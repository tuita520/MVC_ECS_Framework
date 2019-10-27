//=====================================================
// - FileName:      ObjectPoolMgr.cs
// - Created:       mahuibao
// - UserName:      2019-06-12
// - Email:         1023276156@qq.com
// - Description:   对象池管理层
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

namespace Zero.ZeroEngine.Common
{
    public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
    {
        //对象池节点
        public Transform RecyclePoolTrs;
        //场景节点
        public Transform SceneTrs;
        //对象池（已经回收在池中的对象）
        protected Dictionary<uint, List<ResourceObj>> m_ObjectPoolDic = new Dictionary<uint, List<ResourceObj>>();
        //暂存ResourceObjDic（所有经过对象池创建的对象GUID存储）
        protected Dictionary<int, ResourceObj> m_ResourceObjDic = new Dictionary<int, ResourceObj>();
        //resourceObj的类对象池
        protected ClassObjectPool<ResourceObj> m_ResourceObjClassPool = null;
        //根据异步的GUID储存ResourceObj，来判断是否正在异步加载
        protected Dictionary<long, ResourceObj> m_AsyncResObjDic = new Dictionary<long, ResourceObj>();

        public void Init(Transform recycleTrs,Transform sceneTrs)
        {
            ZLogger.Info("对象池管理层初始化");
            m_ResourceObjClassPool = GetOrCreateClassPool<ResourceObj>(1000);
            RecyclePoolTrs = recycleTrs;
            RecyclePoolTrs.gameObject.SetActive(false);
            SceneTrs = sceneTrs;

            EventMgr.Instance.AddEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, ClearCache);
        }
        public void Clear()
        {
            EventMgr.Instance.RemoveEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, ClearCache);
        }
        public void AfterInit()
        {

        }
        /// <summary>
        /// 清空对象池
        /// </summary>
        public void ClearCache()
        {
            List<uint> tempList = new List<uint>();
            foreach(uint key in m_ObjectPoolDic.Keys)
            {
                List<ResourceObj> st = m_ObjectPoolDic[key];
                for(int i = st.Count; i >= 0; i--)
                {
                    ResourceObj resObj = st[i];
                    if(!System.Object.ReferenceEquals(resObj.m_CloneObj, null) && resObj.m_ClearScene)
                    {
                        GameObject.Destroy(resObj.m_CloneObj);
                        m_ResourceObjDic.Remove(resObj.m_CloneObj.GetInstanceID());
                        resObj.Reset();
                        m_ResourceObjClassPool.Recycle(resObj);
                    }
                }
                if (st.Count <= 0)
                {
                    tempList.Add(key);
                }
            }

            for(int i = 0; i < tempList.Count; i++)
            {
                uint temp = tempList[i];
                if (m_ObjectPoolDic.ContainsKey(temp))
                {
                    m_ObjectPoolDic.Remove(temp);
                }
            }
            tempList.Clear();
        }
        /// <summary>
        /// 清除某个资源在对象池中所有的对象
        /// </summary>
        /// <param name="crc"></param>
        public void ClearPoolObject(uint crc)
        {
            List<ResourceObj> st = null;
            if (!m_ObjectPoolDic.TryGetValue(crc, out st) || st == null) return;
            for(int i = st.Count - 1; i >= 0; i--)
            {
                ResourceObj resObj = st[i];
                if (resObj.m_ClearScene)
                {
                    st.Remove(resObj);
                    int tempID = resObj.m_CloneObj.GetInstanceID();
                    GameObject.Destroy(resObj.m_CloneObj);
                    resObj.Reset();
                    m_ResourceObjDic.Remove(tempID);
                    m_ResourceObjClassPool.Recycle(resObj);
                    st.Remove(resObj);
                }
            }
            if(st.Count <= 0)
            {
                m_ObjectPoolDic.Remove(crc);
            }
        }

        /// <summary>
        /// 从对象池取对象
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        protected ResourceObj GetObjectFromPool(uint crc)
        {
            List<ResourceObj> st = null;
            if(m_ObjectPoolDic.TryGetValue(crc,out st) && st != null && st.Count > 0)
            {
                //ResourcesMgr的引用计数
                ResourcesMgr.Instance.IncreaseResourceRef(crc);
                ResourceObj resObj = st[0];
                st.RemoveAt(0);
                GameObject obj = resObj.m_CloneObj;
                if (!System.Object.ReferenceEquals(obj, null))
                {
                    resObj.m_Already = false;
#if UNITY_EDITOR
                    if (obj.name.EndsWith("(Recycle)"))
                    {
                        obj.name = obj.name.Replace("(Recycle)", "");
                    }
#endif
                }
                return resObj;
            }
            return null;
        }
        /// <summary>
        /// 取消异步加载
        /// </summary>
        /// <param name="guid"></param>
        public void CancleLoad(long guid)
        {
            ResourceObj resObj = null;
            if(m_AsyncResObjDic.TryGetValue(guid,out resObj)&&ResourcesMgr.Instance.CancleLoad(resObj))
            {
                m_AsyncResObjDic.Remove(guid);
                resObj.Reset();
                m_ResourceObjClassPool.Recycle(resObj);
            }
        }
        /// <summary>
        /// 是否正在异步加载
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool IsIngAsyncLoad(long guid)
        {
            return m_AsyncResObjDic[guid] != null;
        }
        /// <summary>
        /// 该对象是否是对象池创建的
        /// </summary>
        /// <returns></returns>
        public bool IsObjectManagerCreat(GameObject obj)
        {
            ResourceObj tempObj = m_ResourceObjDic[obj.GetInstanceID()];
            return tempObj != null;
        }

        /// <summary>
        /// 预加载
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="count">预加载个数</param>
        /// <param name="clear">跳场景是否清除</param>
        public void PreLoadGameOject(string path, int count = 1, bool clearScene = false)
        {
            List<GameObject> tempGameObjectList = new List<GameObject>();
            for(int i = 0; i < count; i++)
            {
                GameObject obj = InstantiateObject(path, false, clearScene);
                tempGameObjectList.Add(obj);
            }

            for (int i = 0; i < count; i++)
            {
                GameObject obj = tempGameObjectList[i];
                ReleaseObject(obj);
                obj = null;
            }

            tempGameObjectList.Clear();
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="clearScene"></param>
        /// <returns></returns>
        public GameObject InstantiateObject(string path,bool setSceneObj = false,bool clearScene = true)
        {
            uint crc = CRC32.GetCRC32(path);
            ResourceObj resourceObj = GetObjectFromPool(crc);
            if (resourceObj == null)
            {
                resourceObj = m_ResourceObjClassPool.Spawn(true);
                resourceObj.m_Crc = crc;
                resourceObj.m_ClearScene = clearScene;
                //ResourceMgr提供加载方法
                resourceObj = ResourcesMgr.Instance.LoadResource(path, resourceObj);

                if (resourceObj.m_ResItem.m_Obj != null)
                {
                    resourceObj.m_CloneObj = GameObject.Instantiate(resourceObj.m_ResItem.m_Obj) as GameObject;
                }
            }

            if (setSceneObj)
            {
                resourceObj.m_CloneObj.transform.SetParent(SceneTrs, false);
            }

            int tempGuid = resourceObj.m_CloneObj.GetInstanceID();
            if (!m_ResourceObjDic.ContainsKey(tempGuid))
            {
                m_ResourceObjDic.Add(tempGuid, resourceObj);
            }

            return resourceObj.m_CloneObj;
        }
        /// <summary>
        /// 异步对象加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dealFinish"></param>
        /// <param name="priority"></param>
        /// <param name="setSceneObject"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="bClear"></param>
        public long InstantiateObjectAsync(string path,OnAsyncObjFinish dealFinish,LoadResPriority priority,bool setSceneObject = false, 
            object param1 = null, object param2 = null, object param3 = null, bool clearScene = true)
        {
            if (string.IsNullOrEmpty(path)) return 0;

            uint crc = CRC32.GetCRC32(path);
            ResourceObj resObj = GetObjectFromPool(crc);
            if (resObj != null)
            {
                if (setSceneObject)
                {
                    resObj.m_CloneObj.transform.SetParent(SceneTrs, false);
                }

                if (dealFinish != null)
                {
                    dealFinish(path, resObj.m_CloneObj, param1, param2, param3);
                }
                return resObj.m_Guid;
            }
            long guid = ResourcesMgr.Instance.CreatGuid();
            resObj = m_ResourceObjClassPool.Spawn(true);
            resObj.m_Crc = crc;
            resObj.m_SetSceneParent = setSceneObject;
            resObj.m_ClearScene = clearScene;
            resObj.m_DealFinish = dealFinish;
            resObj.m_Param1 = param1;
            resObj.m_Param2 = param2;
            resObj.m_Param3 = param3;
            //调用ResourcesMgr的异步加载接口
            ResourcesMgr.Instance.AsyncLoadResource(path, resObj, OnLoadResourceObjFinish, priority);
            return guid;
        }
        /// <summary>
        /// 资源加载完成回调
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resObj"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        void OnLoadResourceObjFinish(string path, ResourceObj resObj, object param1 = null, object param2 = null, object param3 = null)
        {
            if (resObj == null) return;
            if (resObj.m_ResItem.m_Obj == null)
            {
                ZLogger.Error("异步加载的资源为空：{0}", path);
            }
            else
            {
                resObj.m_CloneObj = GameObject.Instantiate(resObj.m_ResItem.m_Obj) as GameObject;
            }
            //加载完成就从正在加载的异步中移除
            if (m_AsyncResObjDic.ContainsKey(resObj.m_Guid))
            {
                m_AsyncResObjDic.Remove(resObj.m_Guid);
            }

            if(resObj.m_CloneObj != null && resObj.m_SetSceneParent)
            {
                resObj.m_CloneObj.transform.SetParent(SceneTrs, false);
            }

            if (resObj.m_DealFinish != null)
            {
                int tempGuid = resObj.m_CloneObj.GetInstanceID();
                if (!m_ResourceObjDic.ContainsKey(tempGuid))
                {
                    m_ResourceObjDic.Add(tempGuid, resObj);
                }
                resObj.m_DealFinish(path, resObj.m_CloneObj, resObj.m_Param1, resObj.m_Param2, resObj.m_Param3);
            }
        }


        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="maxCacheCount"></param>
        /// <param name="destoryCache"></param>
        /// <param name="recycleParent"></param>
        public void ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destoryCache = false, bool recycleParent = true)
        {
            if (obj == null) return;

            ResourceObj resObj = null;
            int tempGuid = obj.GetInstanceID();
            if (!m_ResourceObjDic.TryGetValue(tempGuid,out resObj))
            {
                ZLogger.Error("{0} 对象不是ObjectManager创建的！", obj.name);
                return;
            }

            if (resObj == null)
            {
                ZLogger.Error("缓存的ResourObj为空");
                return;
            }

            if (resObj.m_Already)
            {
                ZLogger.Error("该对象已经返回对象池了，检查自己是否清空引用");
                return;
            }

#if UNITY_EDITOR
            obj.name += "(Recycle)";
#endif
            List<ResourceObj> st = null;
            if (maxCacheCount == 0)
            {
                m_ResourceObjDic.Remove(tempGuid);
                ResourcesMgr.Instance.ReleaseResource(resObj, destoryCache);
                resObj.Reset();
                m_ResourceObjClassPool.Recycle(resObj);
            }
            else//回到到对象池
            {
                if(!m_ObjectPoolDic.TryGetValue(resObj.m_Crc,out st) || st == null)
                {
                    st = new List<ResourceObj>();
                    m_ObjectPoolDic.Add(resObj.m_Crc, st);
                }

                if (resObj.m_CloneObj)
                {
                    if (recycleParent)
                    {
                        resObj.m_CloneObj.transform.SetParent(RecyclePoolTrs);
                    }
                    else
                    {
                        resObj.m_CloneObj.SetActive(false);
                    }
                }

                if(maxCacheCount <0||st.Count < maxCacheCount)
                {
                    st.Add(resObj);
                    resObj.m_Already = true;
                    //ResourcesMgr做一个引用计数
                    ResourcesMgr.Instance.DecreaseResourceRef(resObj);
                }
                else
                {
                    m_ResourceObjDic.Remove(tempGuid);
                    ResourcesMgr.Instance.ReleaseResource(resObj, destoryCache);
                    resObj.Reset();
                    m_ResourceObjClassPool.Recycle(resObj);
                }
            }

        }

        #region 类对象池使用
        //类对象字典
        protected Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();
        /// <summary>
        /// 创建类对象池，创建完成以后外面可以保存ClassObjectPool<T>,然后调用Spawn和Recycle来创建和回收对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="maxcount"></param>
        /// <returns></returns>
        public ClassObjectPool<T> GetOrCreateClassPool<T>(int maxcount) where T : class, new()
        {
            Type type = typeof(T);
            object outObj = null;
            if(!m_ClassPoolDic.TryGetValue(type,out outObj)||outObj == null)
            {
                ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxcount);
                m_ClassPoolDic.Add(type, newPool);
                return newPool;
            }
            return outObj as ClassObjectPool<T>;
        }
        /// <summary>
        /// 从对象池中取出T对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="maxcount"></param>
        /// <returns></returns>
        public T NewClassObjectFromPool<T>(int maxcount) where T: class,new()
        {
            ClassObjectPool<T> pool = GetOrCreateClassPool<T>(maxcount);
            if(pool==null)
            {
                return null;
            }
            return pool.Spawn(true);
        }
        #endregion

    }
}

