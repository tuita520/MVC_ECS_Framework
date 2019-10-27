//=====================================================
// - FileName:      SceneMgr.cs
// - Created:       mahuibao
// - UserName:      2019-01-13
// - Email:         1023276156@qq.com
// - Description:   场景管理层
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Data;
using Zero.ZeroEngine.UI;
using Zero.ZeroEngine.Util;

//=====================================================
// - 1.
// - 2.
// - 3.
// - 4.
// - 5.
// - 6.
//======================================================

namespace Zero.ZeroEngine.SceneFrame
{
    /// <summary>
    /// 场景管理层
    /// </summary>
    public class SceneMgr : Singleton<SceneMgr>
    {
        //永久持有的场景列表（这里负责记录，然而存储加载部分依旧写在资源管理层）
        private List<uint> holdSceneList = new List<uint>();
        //缓存中的场景列表
        private Queue<uint> sceneList = new Queue<uint>();
        //场景资源路径
        private const string SCENE_PATH = "Assets/Scene/";
        //最大缓存值
        private int MaxCacheCount = 3;
        //切换的地图ID
        private int switchMapID = 0;
        //切换的场景ID
        private int switchSceneID = 0;
        //是否运行切换场景的协程
        private bool switchAsyncBoo = false;
        //协程名
        public const string sceneLoadCor = "sceneLoadCor";
        //当前地图ID
        public int curMapID = 0;
        //当前场景ID
        public int curSceneID = 0;
        //场景切换锁
        public bool switchLockBoo = false;

        //当前场景数据
        public SceneVo curSceneVo = new SceneVo();

        public ClassObjectPool<PlayerBornObj> PlayerBornClassPool = new ClassObjectPool<PlayerBornObj>(3);
        public ClassObjectPool<MonsterObj> MonsterClassPool = new ClassObjectPool<MonsterObj>(10);
        public ClassObjectPool<MonsterRefreshObj> MonsterRefreshClassPool = new ClassObjectPool<MonsterRefreshObj>(10);
        public ClassObjectPool<NpcObj> NpcClassPool = new ClassObjectPool<NpcObj>(10);
        public ClassObjectPool<TransferObj> TransferClassPool = new ClassObjectPool<TransferObj>(5);

        //缓存的地图数据字典
        public Dictionary<int, BaseMapConfig> MapConfigDic = new Dictionary<int, BaseMapConfig>();

        public void Init()
        {
            ZLogger.Info("场景管理层初始化");

            CoroutineMgr.Instance.StartCoroutine(sceneLoadCor, SceneLoadAsyncCor());
        }
        public void AfterInit()
        {

        }

        public void ClearCache()
        {
            foreach (uint tempCrc in holdSceneList)
            {
                ResourcesMgr.Instance.ReleaseResourceScene(tempCrc);
            }
            foreach (uint tempCrc in sceneList)
            {
                ResourcesMgr.Instance.ReleaseResourceScene(tempCrc);
            }
            holdSceneList.Clear();
            sceneList.Clear();
            MapConfigDic.Clear();
        }

        public void LoadScene(int mapID)
        {
            switchLockBoo = true;
            EventMgr.Instance.TriggerEvent(SceneConst.SWITCH_SCENE_STAR);
            UIMgr.Instance.OpenView<LoadingView>("LoadingView");

            EventMgr.Instance.TriggerEvent<string, int>(SceneConst.SWITCH_SCENE_PROGRESS, "开始加载场景", 5);

            MapExcel tempMapData = DataMgr.Instance.tableMap.GetInfoById(mapID);
            switchMapID = mapID;

            string scenePath = SCENE_PATH + tempMapData.mapResId.ToString() + ".unity";
            uint crc = CRC32.GetCRC32(scenePath);

            if (holdSceneList.Contains(crc) || sceneList.Contains(crc))
            {
                switchSceneID = tempMapData.mapResId;
            }
            else
            {
                ResourcesMgr.Instance.LoadResourceScene(scenePath);
                switchSceneID = tempMapData.mapResId;
            }
            EventMgr.Instance.TriggerEvent<string, int>(SceneConst.SWITCH_SCENE_PROGRESS, "开始加载场景", 15);
            switchAsyncBoo = true;
        }

        void CacheSceneList(int sceneID, bool holdBoo)
        {
            string scenePath = SCENE_PATH + sceneID + ".unity";
            uint crc = CRC32.GetCRC32(scenePath);
            if (holdBoo)
            {
                if (!holdSceneList.Contains(crc))
                {
                    holdSceneList.Add(crc);
                }
            }
            else
            {
                if (!sceneList.Contains(crc))
                {
                    if (sceneList.Count >= MaxCacheCount)
                    {
                        uint tempCrc = sceneList.Dequeue();
                        ResourcesMgr.Instance.ReleaseResourceScene(tempCrc);
                        sceneList.Enqueue(crc);
                    }
                    else
                    {
                        sceneList.Enqueue(crc);
                    }
                }
            }
        }

        IEnumerator SceneLoadAsyncCor()
        {
            while (true)
            {
                if (!switchAsyncBoo)
                {
                    yield return null;
                }
                else
                {
                    if (switchSceneID != 0)
                    {
                        EventMgr.Instance.TriggerEvent(SceneConst.SWITCH_SCENE_STAR_LOAD);
                        yield return null;

                        AsyncOperation op = SceneManager.LoadSceneAsync(switchSceneID);
                        while (!op.isDone)
                        {
                            EventMgr.Instance.TriggerEvent<string, int>(SceneConst.SWITCH_SCENE_PROGRESS, "开始加载场景", (int)(op.progress * 65) + 15);
                            yield return null;
                        }
                        if (op.isDone)
                        {
                            CacheSceneList(switchSceneID, false);

                            LoadMapConfig();
                            EventMgr.Instance.TriggerEvent<string, int>(SceneConst.SWITCH_SCENE_PROGRESS, "开始加载场景", 85);
                            yield return null;
                            EventMgr.Instance.TriggerEvent(SceneConst.SWITCH_SCENE_LOAD_COMPLETE);

                            switchAsyncBoo = false;
                            curSceneID = switchSceneID;
                            curMapID = switchMapID;
                            switchSceneID = 0;
                            switchMapID = 0;

                            if (UIMgr.Instance.GetViewIsOpenByName("LoadingView"))
                            {
                                UIMgr.Instance.CloseViewByName("LoadingView");
                            }
                        }
                    }
                    else
                    {
                        ZLogger.Error("SceneMgr switch scene error:{0}", switchSceneID);
                    }
                }
            }
        }

        void LoadMapConfig()
        {
            BaseMapConfig tempMapConfig;
            if (MapConfigDic.TryGetValue(switchSceneID, out tempMapConfig))
            {
                tempMapConfig.Init();
            }
            else
            {
                switch (switchSceneID)
                {
                    case 1001:
                        tempMapConfig = new Map1001Config() as BaseMapConfig;
                        break;
                    case 1002:
                        break;
                }
                tempMapConfig.Init();
                MapConfigDic.Add(switchSceneID, tempMapConfig);
            }

        }


        public bool MapIsRoad(int canX, int canZ)
        {
            if(curSceneVo.block[canX,canZ] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
