//=====================================================
// - FileName:      ResourcesMgr.cs
// - Created:       mahuibao
// - UserName:      2019-01-09
// - Email:         1023276156@qq.com
// - Description:   资源管理层
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.SceneFrame;
using Zero.ZeroEngine.Util;

//=====================================================
// - 1.有个crc与resouritem中间类的字典，存储已经加载好的资源，免得重复加载资源，也方便从缓存中重复取资源，所存储的resouritem类来自下层，不需要类对象池
// - 2.加载资源又分同步跟异步，异步里还需要针对objectPoolMgr再分两种
// - 3.异步加载，就需要有个异步中间类，负责存储异步所需要的信息以及所需要的回调内容，异步中间类有类对象池
// - 4.异步中间类中，需要存储异步完成后的回调，所以要有个回调类，回调类中存储两种，一种加载资源完成后回调，一种针对objectPoolMgr加载完成后回调。回调类也有对象池
// - 5.上层有个objectPoolMgr管理层，就需要有个中间类，来与之做交互。
// - 6.加载资源，在第一次加载的时候会放入缓存中，放入缓存的时候会做引用计数增加的操作，然后释放资源的时候会做引用计数-1的操作，所以加载非对象资源，可以加载好，
// -   在外面存储好，不要再重复进入resourcesMgr加载，因为从缓存加载会增加引用计数，所以外部控制好加载卸载操作。。。加载对象资源就在更上一层ObjectPoolMgr中写好了。
// - 7.释放资源，会进入销毁，销毁资源会检查引用计数，不为零，返回，反之，进入下一步，如果设置了销毁，就会直接销毁，没设置，就会放入缓存引用计数为零列表，在后面才销毁。
// - 8.预加载资源，目前只有同步，就相当于，加载一遍，然后释放掉，结果资源存储了引用计数为零的列表里，但AB包依旧还在。
// - 9.针对SceneMgr，增加了一个缓存池，并且书写了专门的同步加载，释放资源，释放AB包，添加到缓存，从缓存中获取等函数。
// - 10.
//======================================================

namespace Zero.ZeroEngine.Common
{
    //异步优先级
    public enum LoadResPriority
    {
        
        RES_HIGHT = 0,//最高优先级
        RES_MIDDLE,//一般优先级
        RES_SLOW,//低优先级
        RES_NUM,//数量
    }
    //resMgr与ObjMgr中间类
    public class ResourceObj
    {
        //路径对应crc
        public uint m_Crc = 0;
        //存ResourceItem
        public ResourceItem m_ResItem = null;
        //实例化出来的Gameobject
        public GameObject m_CloneObj = null;
        //切换场景是否清除
        public bool m_ClearScene = true;
        //储存异步的GUID
        public int m_Guid = 0;
        //是否已经放回对象池
        public bool m_Already = false;
        //------------------------------异步用途
        //是否放到场景节点下面
        public bool m_SetSceneParent = false;
        //实例化资源加载完成回调
        public OnAsyncObjFinish m_DealFinish = null;
        //异步参数
        public object m_Param1, m_Param2, m_Param3 = null;

        public void Reset()
        {
            m_Crc = 0;
            m_CloneObj = null;
            m_ClearScene = true;
            m_Guid = 0;
            m_ResItem = null;
            m_Already = false;
            m_SetSceneParent = false;
            m_DealFinish = null;
            m_Param1 = m_Param2 = m_Param3 = null;
        }

    }
    /// <summary>
    /// 异步中间类
    /// </summary>
    public class AsynvLoadResParam
    {
        public List<AsyncCallBack> m_CallBackList = new List<AsyncCallBack>();
        public uint m_Crc;
        public string m_Path;
        public bool m_SpriteBoo;
        public LoadResPriority m_Priority = LoadResPriority.RES_SLOW;

        public void Reset()
        {
            m_CallBackList.Clear();
            m_Crc = 0;
            m_Path = "";
            m_SpriteBoo = false;
            m_Priority = LoadResPriority.RES_SLOW;
        }
    }
    /// <summary>
    /// 异步回调类
    /// </summary>
    public class AsyncCallBack
    {
        //加载完成的回调(针对ObjectPoolMgr)
        public OnAsyncFinish m_ObjDealFinish = null;
        //ObjectPoolMgr对应的中间类
        public ResourceObj m_ResObj = null;
        //-----------------------------------------------------
        //加载完成的回调
        public OnAsyncObjFinish m_ResDealFinish = null;
        
        //回调参数
        public object m_Param1 = null, m_Param2 = null, m_Param3 = null;

        public void Reset()
        {
            m_ObjDealFinish = null;
            m_ResObj = null;
            m_ResDealFinish = null;
            m_Param1 = m_Param2 = m_Param3 = null;
        }
    }

    //资源完成加载回调
    public delegate void OnAsyncObjFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null);

    //实例化对象加载完成回调（这个是针对objectPoolMgr使用的，其他地方一般不用到）（因为加载资源，objectPoolMgr也有要在加载完后完成的事情）
    public delegate void OnAsyncFinish(string path, ResourceObj resObj, object param1 = null, object param2 = null, object param3 = null);

    /// <summary>
    /// 资源管理层
    /// </summary>
    public class ResourcesMgr : Singleton<ResourcesMgr>
    {
        protected long m_Guid = 0;
        //缓存使用中的资源列表
        public Dictionary<uint, ResourceItem> AssetDic { get; set; } = new Dictionary<uint, ResourceItem>();
        //缓存引用计数为零的资源列表，达到缓存最大的时候释放这个列表里面最早没用的资源
        protected CMapList<ResourceItem> m_NoRefrenceAssetMapList = new CMapList<ResourceItem>();
        //中间类的类对象池
        protected ClassObjectPool<AsynvLoadResParam> m_AsyncLoadResParamPool = new ClassObjectPool<AsynvLoadResParam>(50);
        //回调类的类对象池
        protected ClassObjectPool<AsyncCallBack> m_AsyncCallBackPool = new ClassObjectPool<AsyncCallBack>(100);
        //Mono脚本
        //protected MonoBehaviour m_StarMono;
        //protected Coroutine m_LoadCoroutine;

        //正在异步加载的资源列表
        protected List<AsynvLoadResParam>[] m_LoadingAssetList = new List<AsynvLoadResParam>[(int)LoadResPriority.RES_NUM];
        //正在异步加载的DIC
        protected Dictionary<uint, AsynvLoadResParam> m_LodingAssetDic = new Dictionary<uint, AsynvLoadResParam>();
        //最长连续卡着加载资源的时间，单位微秒
        private const long MAXLOADRESTIME = 200000;
        //最大缓存个数
        private const int MAXCACHECOUNT = 500;

        public const string resourcesLoadCor = "ResourcesLoadCor";

        //-----以下为SceneMgr使用
        //缓存已经加载完的场景资源
        public Dictionary<uint, ResourceItem> SceneAssetDic { get; set; } = new Dictionary<uint, ResourceItem>();

        public void Init(MonoBehaviour mono)
        {
            ZLogger.Info("资源管理层初始化");
            for(int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
            {
                m_LoadingAssetList[i] = new List<AsynvLoadResParam>();
            }
            //m_StarMono = mono;
            //m_StarMono.StartCoroutine(AsyncLoadCor());
            CoroutineMgr.Instance.StartCoroutine(resourcesLoadCor, AsyncLoadCor());

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
        /// 创建唯一的GUID
        /// </summary>
        public long CreatGuid()
        {
            return m_Guid++;
        }
        /// <summary>
        ///  清空缓存
        /// </summary>
        public void ClearCache()
        {
            List<ResourceItem> tempList = new List<ResourceItem>();
            foreach(ResourceItem item in AssetDic.Values)
            {
                if (item.m_ClearScene)
                {
                    tempList.Add(item);
                }
            }
            foreach(ResourceItem item in tempList)
            {
                DestoryResourceItem(item, true);
            }
            tempList.Clear();
        }
        /// <summary>
        /// 取消异步加载资源
        /// </summary>
        public bool CancleLoad(ResourceObj resObj)
        {
            AsynvLoadResParam para = null;
            if(m_LodingAssetDic.TryGetValue(resObj.m_Crc,out para) && m_LoadingAssetList[(int)para.m_Priority].Contains(para))
            {
                //清除异步加载中间类中记录的异步回调list，将回调类先重置，后回收，然后移除
                for(int i = para.m_CallBackList.Count; i >= 0; i--)
                {
                    AsyncCallBack tempCallBack = para.m_CallBackList[i];
                    if (tempCallBack != null && resObj == tempCallBack.m_ResObj)
                    {
                        tempCallBack.Reset();
                        m_AsyncCallBackPool.Recycle(tempCallBack);
                        para.m_CallBackList.Remove(tempCallBack);
                    }
                }
                //进行完上一步后，继续重置异步加载中间类，先重置，然后将分级列表中，dic中移除，回收异步加载中间类
                if (para.m_CallBackList.Count <= 0)
                {
                    para.Reset();
                    m_LoadingAssetList[(int)para.m_Priority].Remove(para);
                    m_AsyncLoadResParamPool.Recycle(para);
                    m_LodingAssetDic.Remove(resObj.m_Crc);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 根据ResObj增加引用计数
        /// </summary>
        public int IncreaseResourceRef(ResourceObj resObj, int count = 1)
        {
            return resObj != null ? IncreaseResourceRef(resObj.m_Crc, count) : 0;
        }
        /// <summary>
        /// 根据path增加引用计数
        /// </summary>
        public int IncreaseResourceRef(uint crc = 0, int count = 1)
        {
            ResourceItem item = null;
            if(!AssetDic.TryGetValue(crc,out item) || item == null)
            {
                return 0;
            }
            item.RefCount += count;
            item.m_LastUseTime = Time.realtimeSinceStartup;
            return item.RefCount;
        }
        /// <summary>
        /// 根据ResourceObj减少引用计数
        /// </summary>
        public int DecreaseResourceRef(ResourceObj resObj,int count = 1)
        {
            return resObj != null ? DecreaseResourceRef(resObj.m_Crc, count) : 0;
        }
        /// <summary>
        /// 根据路径减少引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DecreaseResourceRef(uint crc, int count = 1)
        {
            ResourceItem item = null;
            if (!AssetDic.TryGetValue(crc, out item) || item == null)
            {
                return 0;
            }
            item.RefCount -= count;
            return item.RefCount;
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        public void PreLoadRes(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            uint crc = CRC32.GetCRC32(path);
            ResourceItem item = GetCacheResourceItem(crc,0);
            if (item != null)
            {
                return;
            }

            Object obj = null;
#if UNITY_EDITOR
            if (AppConst.DebugMode)
            {
                item = AssetBundleMgr.Instance.FindResourceItem(crc);
                if (item != null && item.m_Obj != null)
                {
                    obj = item.m_Obj as Object;
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleMgr.Instance.LoadResourceAssetBundle(crc);
                if (item != null && item.m_AssetBundel != null)
                {
                    if (item.m_Obj != null)
                    {
                        obj = item.m_Obj;
                    }
                    else
                    {
                        obj = item.m_AssetBundel.LoadAsset<Object>(item.m_AssetName);
                    }
                }
            }

            CacheResource(path, ref item, crc, obj);
            //跳场景不清空缓存
            item.m_ClearScene = false;
            ReleaseResource(obj, false);
        }

        /// <summary>
        /// 同步加载资源，针对给ObjectPoolMgr接口
        /// </summary>
        public ResourceObj LoadResource(string path,ResourceObj resObj)
        {
            if (resObj == null)
            {
                return null;
            }
            uint crc = resObj.m_Crc == 0 ? CRC32.GetCRC32(path) : resObj.m_Crc;

            ResourceItem item = GetCacheResourceItem(crc);
            if (item != null)
            {
                resObj.m_ResItem = item;
                return resObj;
            }

            Object obj = null;
#if UNITY_EDITOR
            if (AppConst.DebugMode)
            {
                item = AssetBundleMgr.Instance.FindResourceItem(crc);
                if (item!=null && item.m_Obj != null)
                {
                    obj = item.m_Obj as Object;
                }
                else
                {
                    if(item == null)
                    {
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleMgr.Instance.LoadResourceAssetBundle(crc);
                if (item != null && item.m_AssetBundel != null)
                {
                    if (item.m_Obj != null)
                    {
                        obj = item.m_Obj as Object;
                    }
                    else
                    {
                        obj = item.m_AssetBundel.LoadAsset<Object>(item.m_AssetName);
                    }
                }
            }

            CacheResource(path, ref item, crc, obj);
            resObj.m_ResItem = item;
            item.m_ClearScene = resObj.m_ClearScene;

            return resObj;
        }

        /// <summary>
        /// 同步资源加载，外部直接调用，仅加载不需要实例化的资源，例如textur，音频等等
        /// </summary>
        public T LoadResource<T>(string path)where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            uint crc = CRC32.GetCRC32(path);
            ResourceItem item = GetCacheResourceItem(crc);
            if(item != null)
            {
                return item.m_Obj as T;
            }

            T obj = null;
#if UNITY_EDITOR
            if (AppConst.DebugMode)
            {
                item = AssetBundleMgr.Instance.FindResourceItem(crc);
                if (item != null && item.m_Obj != null)
                {
                    obj = item.m_Obj as T;
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<T>(path);
                }
            }
#endif
            if(obj == null)
            {
                item = AssetBundleMgr.Instance.LoadResourceAssetBundle(crc);
                if(item!=null&&item.m_AssetBundel != null)
                {
                    if (item.m_Obj != null)
                    {
                        obj = item.m_Obj as T;
                    }
                    else
                    {
                        obj = item.m_AssetBundel.LoadAsset<T>(item.m_AssetName);
                    }
                }
            }

            CacheResource(path,ref item,crc,obj);

            return obj;
        }
        /// <summary>
        /// 同步资源加载，针对给SceneMgr使用
        /// </summary>
        public void LoadResourceScene(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            uint crc = CRC32.GetCRC32(path);
            ResourceItem item = GetCacheResourceItemScene(crc);
            if (item != null)
            {
                return;
            }
#if UNITY_EDITOR
            if (AppConst.DebugMode)
            {
                item = AssetBundleMgr.Instance.FindResourceItem(crc);
                if (item == null)
                {
                    item = new ResourceItem();
                    item.m_Crc = crc;
                }
            }
#endif
            if (item == null)
            {
                item = AssetBundleMgr.Instance.LoadResourceAssetBundle(crc);
            }

            CacheResourceScene(path, ref item, crc);
        }
        /// <summary>
        /// 根据ResourceObj卸载资源,针对给ObjectPoolMgr接口
        /// </summary>
        public bool ReleaseResource(ResourceObj resObj, bool destoryObj = false)
        {
            if (resObj == null)
            {
                return false;
            }
            ResourceItem item = null;
            if (!AssetDic.TryGetValue(resObj.m_Crc, out item) || item == null)
            {
                ZLogger.Error("AssetDic里不存在该资源：{0} ,可能释放了多次", resObj.m_CloneObj.name);
            }
            GameObject.Destroy(resObj.m_CloneObj);
            item.RefCount--;
            DestoryResourceItem(item, destoryObj);
            return true;
        }

        /// <summary>
        /// 不需要实例化的资源的卸载，根据对象
        /// </summary>
        public bool ReleaseResource(Object obj,bool destoryObj = false)
        {
            if (obj == null)
            {
                return false;
            }

            ResourceItem item = null;
            foreach(ResourceItem res in AssetDic.Values)
            {
                if (res.m_Guid == obj.GetInstanceID())
                {
                    item = res;
                }
            }

            if(item == null)
            {
                ZLogger.Error("AssetDic里不存在该资源：{0} ,可能释放了多次", obj.name);
                return false;
            }
            item.RefCount--;
            DestoryResourceItem(item,destoryObj);
            return true;
        }
        /// <summary>
        /// 不需要实例化的资源的卸载，根据路径
        /// </summary>
        public bool ReleaseResource(string path, bool destoryObj = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            uint crc = CRC32.GetCRC32(path);
            ResourceItem item = null;
            if(!AssetDic.TryGetValue(crc,out item)||item == null)
            {
                ZLogger.Error("AssetDic里不存在该资源：{0} ,可能释放了多次", path);
            }
            item.RefCount--;
            DestoryResourceItem(item, destoryObj);
            return true;
        }
        /// <summary>
        /// 场景资源卸载，根据CRC，针对SceneMgr
        /// </summary>
        public bool ReleaseResourceScene(uint crc)
        {
            ResourceItem item = null;
            if (!SceneAssetDic.TryGetValue(crc, out item) || item == null)
            {
                ZLogger.Error("AssetDic里不存在该资源：{0} ,可能释放了多次", crc);
            }
            item.RefCount--;
            DestoryResourceItemScene(item, true);
            return true;
        }
        /// <summary>
        /// 缓存加载的资源
        /// </summary>
        void CacheResource(string path,ref ResourceItem item,uint crc,Object obj,int addrefcount = 1)
        {
            if (m_NoRefrenceAssetMapList.Find(item))
            {
                m_NoRefrenceAssetMapList.Remove(item);
            }

            //缓存太多，清除最早没有使用的资源
            WashOut();

            if (item == null)
            {
                ZLogger.Error("ResourceItem is null, path : {0}", path);
            }
            if (obj == null)
            {
                ZLogger.Error("ResourceLoad Fail : {0}", path);
            }
            item.m_Obj = obj;
            item.m_Guid = obj.GetInstanceID();
            item.m_LastUseTime = Time.realtimeSinceStartup;
            item.RefCount += addrefcount;
            ResourceItem oldItem = null;
            if(AssetDic.TryGetValue(item.m_Crc,out oldItem))
            {
                AssetDic[item.m_Crc] = item;
            }
            else
            {
                AssetDic.Add(item.m_Crc, item);
            }
        }
        /// <summary>
        /// 缓存加载的场景资源，针对SceneMgr
        /// </summary>
        void CacheResourceScene(string path, ref ResourceItem item, uint crc, int addrefcount = 1)
        {
            if (item == null)
            {
                ZLogger.Error("ResourceItem is null, path : {0}", path);
            }
            item.m_LastUseTime = Time.realtimeSinceStartup;
            item.RefCount += addrefcount;
            ResourceItem oldItem = null;
            if (SceneAssetDic.TryGetValue(item.m_Crc, out oldItem))
            {
                SceneAssetDic[item.m_Crc] = item;
            }
            else
            {
                SceneAssetDic.Add(item.m_Crc, item);
            }
        }
        /// <summary>
        /// 缓存太多，清除最早没有使用的资源
        /// </summary>
        protected void WashOut()
        {
            //当大于缓存个数时，我们来进行清除最早没用的资源，释放部分缓存
            while (m_NoRefrenceAssetMapList.Size() >= MAXCACHECOUNT)
            {
                for(int i = 0; i < MAXCACHECOUNT / 2; i++)
                {
                    ResourceItem item = m_NoRefrenceAssetMapList.Back();
                    DestoryResourceItem(item, true);
                }
            } 
        }
        /// <summary>
        /// 回收一个资源
        /// </summary>
        protected void DestoryResourceItem(ResourceItem item,bool destoryCache = false)
        {
            if (item == null || item.RefCount > 0)
            {
                return;
            }
            if (!destoryCache)
            {
                m_NoRefrenceAssetMapList.InsertToHead(item);
                return;
            }
            if (!AssetDic.Remove(item.m_Crc))
            {
                return;
            }
            m_NoRefrenceAssetMapList.Remove(item);

            //释放assetbundel引用
            AssetBundleMgr.Instance.ReleaseAsset(item);
            //清空资源对应的对象池
            ObjectPoolMgr.Instance.ClearPoolObject(item.m_Crc);

            if (item.m_Obj != null)
            {
                item.m_Obj = null;
#if UNITY_EDITOR
                Resources.UnloadUnusedAssets();
#endif
            }
        }
        /// <summary>
        /// 回收一个资源，针对SceneMgr
        /// </summary>
        protected void DestoryResourceItemScene(ResourceItem item, bool destoryCache = true)
        {
            if (item == null || item.RefCount > 0)
            {
                ZLogger.Error("针对场景的释放，不应该有引用大于0的情况");
                return;
            }
            if (!SceneAssetDic.Remove(item.m_Crc))
            {
                return;
            }
            //释放assetbundel引用
            AssetBundleMgr.Instance.ReleaseAsset(item);
            if (item.m_Obj != null)
            {
                item.m_Obj = null;
#if UNITY_EDITOR
                Resources.UnloadUnusedAssets();
#endif
            }
        }

        /// <summary>
        /// 编辑器下加载
        /// </summary>
#if UNITY_EDITOR
        protected T LoadAssetByEditor<T>(string path) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif
        /// <summary>
        /// 从资源池获取缓存资源
        /// </summary>
        ResourceItem GetCacheResourceItem(uint crc,int addrefcount = 1)
        {
            ResourceItem item = null;
            if(AssetDic.TryGetValue(crc,out item))
            {
                if (item != null)
                {
                    item.RefCount += addrefcount;
                    item.m_LastUseTime = Time.realtimeSinceStartup;
                }
            }
            return item;
        }
        /// <summary>
        /// 从资源池获取缓存资源，针对SceneMgr使用
        /// </summary>
        ResourceItem GetCacheResourceItemScene(uint crc, int addrefcount = 1)
        {
            ResourceItem item = null;
            if (SceneAssetDic.TryGetValue(crc, out item))
            {
                if (item != null)
                {
                    item.RefCount += addrefcount;
                    item.m_LastUseTime = Time.realtimeSinceStartup;
                }
            }
            return item;
        }
        /// <summary>
        /// 异步加载资源（仅仅是不需要实例化的资源，例如texture，音频等等）
        /// </summary>
        public void AsyncLoadResource(string path, OnAsyncObjFinish dealFinish,LoadResPriority priority, bool isSprite = false, object param1= null,
            object param2=null,object param3=null,uint crc = 0)
        {
            if(crc == 0)
            {
                crc = CRC32.GetCRC32(path);
            }

            ResourceItem item = GetCacheResourceItem(crc);
            if(item != null)
            {
                if (dealFinish != null)
                {
                    dealFinish(path,item.m_Obj,param1,param2,param3);
                }
                return;
            }

            //判断是否在加载中
            AsynvLoadResParam para = null;
            if(!m_LodingAssetDic.TryGetValue(crc,out para) || para == null)
            {
                para = m_AsyncLoadResParamPool.Spawn(true);
                para.m_Crc = crc;
                para.m_Path = path;
                para.m_SpriteBoo = isSprite;
                para.m_Priority = priority;
                m_LodingAssetDic.Add(crc, para);
                m_LoadingAssetList[(int)priority].Add(para);
            }

            //往回调列表里加回调
            //这里加的是不针对的回调
            AsyncCallBack callBack = m_AsyncCallBackPool.Spawn(true);
            callBack.m_ResDealFinish = dealFinish;
            callBack.m_Param1 = param1;
            callBack.m_Param2 = param2;
            callBack.m_Param3 = param3;
            para.m_CallBackList.Add(callBack);
        }
        /// <summary>
        /// 针对ObjectPoolMgr异步加载接口
        /// </summary>
        public void AsyncLoadResource(string path,ResourceObj resObj,OnAsyncFinish dealFinish,LoadResPriority priority)
        {
            ResourceItem item = GetCacheResourceItem(resObj.m_Crc);
            if (item != null)
            {
                resObj.m_ResItem = item;
                if (dealFinish != null)
                {
                    dealFinish(path, resObj);
                }
                return;
            }

            //判断是否在加载中
            AsynvLoadResParam para = null;
            if (!m_LodingAssetDic.TryGetValue(resObj.m_Crc, out para) || para == null)
            {
                para = m_AsyncLoadResParamPool.Spawn(true);
                para.m_Crc = resObj.m_Crc;
                para.m_Path = path;
                para.m_Priority = priority;
                m_LodingAssetDic.Add(resObj.m_Crc, para);
                m_LoadingAssetList[(int)priority].Add(para);
            }

            //往回调列表里加回调
            //这里加的是针对的回调
            AsyncCallBack callBack = m_AsyncCallBackPool.Spawn(true);
            callBack.m_ObjDealFinish = dealFinish;
            callBack.m_ResObj = resObj;
            para.m_CallBackList.Add(callBack);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        IEnumerator AsyncLoadCor()
        {
            List<AsyncCallBack> callBackList = null;
            //上一次yield的时间
            long lastYieldTime = System.DateTime.Now.Ticks;
            while (true)
            {
                bool haveYieldBoo = false;
                for(int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
                {
                    if (m_LoadingAssetList[(int)LoadResPriority.RES_HIGHT].Count > 0)
                    {
                        i = (int)LoadResPriority.RES_HIGHT;
                    }
                    else if (m_LoadingAssetList[(int)LoadResPriority.RES_MIDDLE].Count > 0)
                    {
                        i = (int)LoadResPriority.RES_MIDDLE;
                    }

                    List<AsynvLoadResParam> loadingList = m_LoadingAssetList[i];
                    if (loadingList.Count <= 0) continue;
                    AsynvLoadResParam loadingItem = loadingList[0];
                    loadingList.RemoveAt(0);
                    callBackList = loadingItem.m_CallBackList;

                    Object obj = null;
                    ResourceItem item = null;
#if UNITY_EDITOR
                    if (AppConst.DebugMode)
                    {
                        if (loadingItem.m_SpriteBoo)
                        {
                            obj = LoadAssetByEditor<Sprite>(loadingItem.m_Path);
                        }
                        else
                        {
                            obj = LoadAssetByEditor<Object>(loadingItem.m_Path);
                        }
                        //模拟异步加载
                        yield return new WaitForSeconds(0.5f);

                        item = AssetBundleMgr.Instance.FindResourceItem(loadingItem.m_Crc);
                        if (item == null)
                        {
                            item = new ResourceItem();
                            item.m_Crc = loadingItem.m_Crc;
                        }
                    }
#endif
                    if(obj == null)
                    {
                        item = AssetBundleMgr.Instance.LoadResourceAssetBundle(loadingItem.m_Crc);
                        if (item != null && item.m_AssetBundel != null)
                        {
                            AssetBundleRequest abRequest = null;
                            if (loadingItem.m_SpriteBoo)
                            {
                                abRequest = item.m_AssetBundel.LoadAssetAsync<Sprite>(item.m_AssetName);
                            }
                            else
                            {
                                abRequest = item.m_AssetBundel.LoadAssetAsync(item.m_AssetName);
                            }
                            yield return abRequest;
                            if (abRequest.isDone)
                            {
                                obj = abRequest.asset;
                            }
                            lastYieldTime = System.DateTime.Now.Ticks;
                        }
                    }
                    CacheResource(loadingItem.m_Path, ref item, loadingItem.m_Crc, obj, callBackList.Count);

                    for(int j = 0; j < callBackList.Count; j++)
                    {
                        AsyncCallBack callBack = callBackList[j];

                        if (callBack != null && callBack.m_ObjDealFinish != null && callBack.m_ResObj != null)
                        {
                            ResourceObj tempResObj = callBack.m_ResObj;
                            tempResObj.m_ResItem = item;
                            callBack.m_ObjDealFinish(loadingItem.m_Path, tempResObj, tempResObj.m_Param1, tempResObj.m_Param2, tempResObj.m_Param3);
                            callBack.m_ObjDealFinish = null;
                            tempResObj = null;
                        }

                        if (callBack != null && callBack.m_ResDealFinish != null)
                        {
                            callBack.m_ResDealFinish(loadingItem.m_Path,obj,callBack.m_Param1,callBack.m_Param2,callBack.m_Param3);
                            callBack.m_ResDealFinish = null;
                        }
                        callBack.Reset();
                        m_AsyncCallBackPool.Recycle(callBack);
                    }

                    obj = null;
                    callBackList.Clear();
                    m_LodingAssetDic.Remove(loadingItem.m_Crc);

                    loadingItem.Reset();
                    m_AsyncLoadResParamPool.Recycle(loadingItem);

                    if (System.DateTime.Now.Ticks - lastYieldTime > MAXLOADRESTIME)
                    {
                        yield return null;
                        lastYieldTime = System.DateTime.Now.Ticks;
                        haveYieldBoo = true;
                    }
                }

                if(!haveYieldBoo || System.DateTime.Now.Ticks - lastYieldTime > MAXLOADRESTIME)
                {
                    lastYieldTime = System.DateTime.Now.Ticks;
                    yield return null;
                }
                
            }
        }

    }




#region 封装双向链表
    public class CMapList<T> where T : class, new()
    {
        DoubleLinkedList<T> m_DLink = new DoubleLinkedList<T>();
        Dictionary<T, DoubleLinkedListNode<T>> m_FindMap = new Dictionary<T, DoubleLinkedListNode<T>>();

        ~CMapList()
        {
            Clear();
        }
        /// <summary>
        /// 清空列表
        /// </summary>
        public void Clear()
        {
            while (m_DLink.Tail != null)
            {
                Remove(m_DLink.Tail.t);
            }
        }

        /// <summary>
        /// 插入一个节点到表头
        /// </summary>
        /// <param name="t"></param>
        public void InsertToHead(T t)
        {
            DoubleLinkedListNode<T> node = null;
            if (m_FindMap.TryGetValue(t, out node) && node != null)
            {
                m_DLink.AddToHeader(node);
                return;
            }
            m_DLink.AddToHeader(t);
            m_FindMap.Add(t, m_DLink.Head);
        }
        /// <summary>
        /// 从表位弹出一个节点
        /// </summary>
        public void Pop()
        {
            if(m_DLink.Tail != null)
            {
                Remove(m_DLink.Tail.t);
            }
        }
        /// <summary>
        /// 删除某个节点
        /// </summary>
        /// <param name="t"></param>
        public void Remove(T t)
        {
            DoubleLinkedListNode<T> node = null;
            if(!m_FindMap.TryGetValue(t,out node) || node == null)
            {
                return;
            }
            m_DLink.RemoveNode(node);
            m_FindMap.Remove(t);
        }
        /// <summary>
        /// 获取到尾部节点
        /// </summary>
        /// <returns></returns>
        public T Back()
        {
            return m_DLink.Tail == null ? null : m_DLink.Tail.t;
        }
        /// <summary>
        /// 返回节点个数
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return m_FindMap.Count;
        }
        /// <summary>
        /// 查找是否存在该节点
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Find(T t)
        {
            DoubleLinkedListNode<T> node = null;
            if (!m_FindMap.TryGetValue(t, out node) || node == null) return false;
            return true;
        }
        /// <summary>
        /// 刷新某个节点，把节点移动到头部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Reflesh(T t)
        {
            DoubleLinkedListNode<T> node = null;
            if (!m_FindMap.TryGetValue(t, out node) || node == null) return false;

            m_DLink.MoveToHead(node);
            return true;
        }

    }
#endregion

#region 双向链表
    /// <summary>
    /// 双向链表结构节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleLinkedListNode<T> where T : class, new()
    {
        //前一个节点
        public DoubleLinkedListNode<T> prev = null;
        //后一个节点
        public DoubleLinkedListNode<T> next = null;
        //当前节点
        public T t = null;
    }
    public class DoubleLinkedList<T> where T:class, new()
    {
        //表头
        public DoubleLinkedListNode<T> Head = null;
        //表尾
        public DoubleLinkedListNode<T> Tail = null;
        //双向链表结构类对象池
        protected ClassObjectPool<DoubleLinkedListNode<T>> m_DoubleLinkedNodePool 
            = ObjectPoolMgr.Instance.GetOrCreateClassPool<DoubleLinkedListNode<T>>(500);
        //个数
        protected int m_Count = 0;

        public int Count
        {
            get { return m_Count; }
        }
        /// <summary>
        /// 添加一个节点到头部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DoubleLinkedListNode<T> AddToHeader(T t)
        {
            DoubleLinkedListNode<T> pList = m_DoubleLinkedNodePool.Spawn(true);
            pList.next = null;
            pList.prev = null;
            pList.t = t;
            return AddToHeader(pList);
        }
        /// <summary>
        /// 添加一个节点到头部
        /// </summary>
        /// <param name="pNode"></param>
        /// <returns></returns>
        public DoubleLinkedListNode<T> AddToHeader(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null) return null;

            pNode.prev = null;
            if(Head == null)
            {
                Head = Tail = pNode;
            }
            else
            {
                pNode.next = Head;
                pNode.prev = pNode;
                Head = pNode;
            }
            m_Count++;
            return Head;
        }

        /// <summary>
        /// 添加一个节点到尾部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DoubleLinkedListNode<T> AddToTail(T t)
        {
            DoubleLinkedListNode<T> pList = m_DoubleLinkedNodePool.Spawn(true);
            pList.next = null;
            pList.prev = null;
            pList.t = t;
            return AddToTail(pList);
        }
        /// <summary>
        /// 添加一个节点到尾部
        /// </summary>
        /// <param name="pNode"></param>
        /// <returns></returns>
        public DoubleLinkedListNode<T> AddToTail(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null) return null;

            pNode.next = null;
            if(Tail == null)
            {
                Head = Tail = pNode;
            }
            else
            {
                pNode.prev = Tail;
                pNode.next = pNode;
                Tail = pNode;
            }
            m_Count++;
            return Tail;
        }
        /// <summary>
        /// 移除某个节点
        /// </summary>
        /// <param name="pNode"></param>
        public void RemoveNode(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null) return;
            if(pNode == Head)
            {
                Head = pNode.next;
            }
            if(pNode == Tail)
            {
                Tail = pNode.prev;
            }
            //以下便于理解，解释下，一般头部指向的前一个为null，尾部指向的后一个为null
            if(pNode.prev != null)
            {
                pNode.prev.next = pNode.next;
            }

            if(pNode.next != null)
            {
                pNode.next.prev = pNode.prev ;
            }

            pNode.next = pNode.prev = null;
            pNode.t = null;
            m_DoubleLinkedNodePool.Recycle(pNode);
            m_Count--;
        }
        /// <summary>
        /// 把某个节点移动到头部
        /// </summary>
        /// <param name="pNode"></param>
        public void MoveToHead(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null || pNode == Head) return;

            if (pNode.prev == null && pNode.next == null) return;

            if (pNode == Tail)
            {
                Tail = pNode.prev;
            }
            if (pNode.prev != null)
            {
                pNode.prev.next = pNode.next;
            }
            if (pNode.next != null)
            {
                pNode.next.prev = pNode.prev;
            }

            pNode.prev = null;
            pNode.next = Head;
            Head.prev = pNode;
            Head = pNode;
            if(Tail == null)//这是链表只有一个的时候的
            {
                Tail = Head;
            }
        }
    }
#endregion
}
