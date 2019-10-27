//=====================================================
// - FileName:      AssetBundleMgr.cs
// - Created:       mahuibao
// - UserName:      2019-06-12
// - Email:         1023276156@qq.com
// - Description:   AB包管理层
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Util;

//=====================================================
// - 1.AB包是不会重复加载的，所有使用一个AB包类，存储加载好的AB包，以及有多少个引用了此AB包的计数器，AB包类需要有个类对象池
// - 2.加载好的AB包使用一个字典存储，方便查找以及防止重复加载
// - 3.AB包的配置表需要提前加载，加载完知道所有的依赖关系
// - 4.配置表的信息也是使用一个AB与资源的中间类字典存储
// - 5.与资源管理层的中间类，结构是根据两边需求所编写的
// - 6.中间类的个数是固定的，而且都是一直持有的，不需要类对象池
//======================================================

namespace Zero.ZeroEngine.Common
{
    public class ABMgrConst
    {
        public const string AB_CONFIG_LOAD_COMPLETE = "AB_CONFIG_LOAD_COMPLETE";
    }

    public class AssetBundleItem
    {
        public AssetBundle assetBundle = null;
        public int RefCount;

        public void Reset()
        {
            assetBundle = null;
            RefCount = 0;
        }
    }

    public class ResourceItem
    {
        //资源路径的CRC
        public uint m_Crc = 0;
        //该资源的文件名
        public string m_AssetName = string.Empty;
        //该资源所在的AssetBundle名字
        public string m_ABName = string.Empty;
        //该资源所依赖的AssetBundle
        public List<string> m_DependenAssetBundle = null;
        //该资源加载完的AB包
        public AssetBundle m_AssetBundel = null;

        //--------------------------------以下针对资源ResourcesMgr中使用
        //资源对象
        public Object m_Obj = null;
        //资源唯一标识
        public int m_Guid = 0;
        //资源最后所使用的时间
        public float m_LastUseTime = 0.0f;
        //引用计数
        protected int m_RefCount = 0;
        //是否跳场景，清掉
        public bool m_ClearScene = true;

        public int RefCount
        {
            get { return m_RefCount; }
            set
            {
                m_RefCount = value;
                if (m_RefCount < 0)
                {
                    ZLogger.Error("refcount < 0 {0} , {1}", m_RefCount, (m_Obj != null ? m_Obj.name : "name is null"));
                }
            }
        }

    }
    public class AssetBundleMgr : Singleton<AssetBundleMgr> {

        protected string m_ABConfigABName = "assetbundleconfig";
        //资源关系依赖配置表，可以根据CRC找到对应资源块
        protected Dictionary<uint, ResourceItem> m_ResourceItemDic = new Dictionary<uint, ResourceItem>();
        //储存已加载的ab包，key为CRC
        protected Dictionary<uint, AssetBundleItem> m_AssetBundleItemDic = new Dictionary<uint, AssetBundleItem>();
        //AssetBundleItem类对象池
        protected ClassObjectPool<AssetBundleItem> m_AssetBundleItemPool = ObjectPoolMgr.Instance.GetOrCreateClassPool<AssetBundleItem>(500);

        protected string ABLoadPath
        {
            get
            {
                if (AppConst.DebugMode)
                {

                    return Application.dataPath + "/AssetBundle/" + AppConst.plat;
                }
                else
                {
                    return UtilTool.DataPath;
                }
            }
        }

        public void Init()
        {
            ZLogger.Info("AB包管理层初始化");
            EventMgr.Instance.AddEventListener(UpdataConst.LOADER_ALL_COMPLETED, LoadAssetBundleConfig);
        }
        public void AfterInit()
        {
            
        }
        /// <summary>
        /// 加载AB配置表
        /// </summary>
        /// <returns></returns>
        public void LoadAssetBundleConfig()
        {
#if UNITY_EDITOR
            if (AppConst.DebugMode)
                EventMgr.Instance.TriggerEvent(ABMgrConst.AB_CONFIG_LOAD_COMPLETE);
                return;
#endif
            m_ResourceItemDic.Clear();
            string configPath = ABLoadPath + m_ABConfigABName;
            AssetBundle configAB = AssetBundle.LoadFromFile(configPath);
            TextAsset textAsset = configAB.LoadAsset<TextAsset>(m_ABConfigABName);
            if(textAsset == null)
            {
                ZLogger.Error("AssetBundleConfgi is no exist");
            }

            MemoryStream stream = new MemoryStream(textAsset.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            AssetBundleConfig config = (AssetBundleConfig)bf.Deserialize(stream);
            stream.Close();

            for (int i=0;i< config.ABList.Count; i++)
            {
                ABBase abBase = config.ABList[i];
                ResourceItem item = new ResourceItem();
                item.m_Crc = abBase.Crc;
                item.m_AssetName = abBase.AssetName;
                item.m_ABName = abBase.ABName;
                item.m_DependenAssetBundle = abBase.ABDependce;
                if (m_ResourceItemDic.ContainsKey(item.m_Crc))
                {
                    ZLogger.Error("重复的CRC : {0}  ab包名： {1}", item.m_AssetName, item.m_ABName);
                }
                else
                {
                    m_ResourceItemDic.Add(item.m_Crc, item);
                }
            }
            EventMgr.Instance.TriggerEvent(ABMgrConst.AB_CONFIG_LOAD_COMPLETE);
            return;
        }
        /// <summary>
        /// 根据路径的CRC加载中间类resourceItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public ResourceItem LoadResourceAssetBundle(uint crc)
        {
            ResourceItem item = null;
            if(!m_ResourceItemDic.TryGetValue(crc,out item)||item == null)
            {
                ZLogger.Error("LoadResourceAssetBundle can not find crc {0}  in assetbundleconfig", crc.ToString());
                return item;
            }
            if(item.m_AssetBundel!=null)
            {
                return item;
            }
            item.m_AssetBundel = LoadAssetBundle(item.m_ABName);

            if(item.m_DependenAssetBundle!=null)
            {
                for(int i = 0; i < item.m_DependenAssetBundle.Count; i++)
                {
                    LoadAssetBundle(item.m_DependenAssetBundle[i]);
                }
            }
            return item;
        }
        /// <summary>
        /// 加载单个assetbundle根据名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string name)
        {
            AssetBundleItem item = null;
            uint crc = CRC32.GetCRC32(name);

            if(!m_AssetBundleItemDic.TryGetValue(crc,out item))
            {
                AssetBundle assetBundle = null;
                string fullPath = ABLoadPath + name;
                assetBundle = AssetBundle.LoadFromFile(fullPath);
                if (assetBundle == null)
                {
                    ZLogger.Error(" load assetbundle Error : {0}", fullPath);
                }

                item = m_AssetBundleItemPool.Spawn(true);
                item.assetBundle = assetBundle;
                item.RefCount++;
                m_AssetBundleItemDic.Add(crc, item);
            }
            else
            {
                item.RefCount++;
            }
            return item.assetBundle;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="item"></param>
        public void ReleaseAsset(ResourceItem item)
        {
            if(item == null)
            {
                return;
            }
            if (item.m_DependenAssetBundle != null && item.m_DependenAssetBundle.Count > 0)
            {
                for(int i = 0; i < item.m_DependenAssetBundle.Count; i++)
                {
                    UnLoadAssetBundle(item.m_DependenAssetBundle[i]);
                }
            }
            UnLoadAssetBundle(item.m_ABName);
            item.m_AssetBundel = null;
        }
        /// <summary>
        /// 释放AB包
        /// </summary>
        /// <param name="name"></param>
        private void UnLoadAssetBundle(string name)
        {
            AssetBundleItem item = null;
            uint crc = CRC32.GetCRC32(name);
            if(m_AssetBundleItemDic.TryGetValue(crc,out item)&& item != null)
            {
                item.RefCount--;
                if (item.RefCount <= 0)
                {
                    item.assetBundle.Unload(true);
                    item.Reset();
                    m_AssetBundleItemPool.Recycle(item);
                    m_AssetBundleItemDic.Remove(crc);
                }
            }
        }
        /// <summary>
        /// 根据CRC查找resourceItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public ResourceItem FindResourceItem(uint crc)
        {
            ResourceItem item = null;
            m_ResourceItemDic.TryGetValue(crc, out item);
            return item;
        }
    }
}