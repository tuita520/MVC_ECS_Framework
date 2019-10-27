//=====================================================
// - FileName:      Main.cs
// - Created:       #mahuibao#
// - UserName:      #2019-01-09#
// - Email:         1023276156@qq.com
// - Description:   游戏启动入口
// -  (C) Copyright 2018 - 2018
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero.Plugins.Base;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Data;
using Zero.ZeroEngine.ECS;
using Zero.ZeroEngine.Login;
using Zero.ZeroEngine.SceneFrame;
using Zero.ZeroEngine.UI;
using Zero.ZeroEngine.Util;
/// <summary>
/// 游戏启动主入口
/// </summary>
public class Main : SingletonMono<Main> {

    private GameObject tempObj;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        ZLogger.Info("<游戏启动初始化");
        this.Init();
    }

    void Start () {
        //ObjectPoolMgr.Instance.PreLoadGameOject("Assets/Resources/UI/Prefabs/Main/UIRoot.prefab", 20);
    }
    void OnLoadFinish(string path, Object obj, object pram1 = null, object param2 = null, object param3 = null)
    {
        tempObj = obj as GameObject;
    }

    private void Init()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Application.targetFrameRate = AppConst.GameFrameRate;

        //DataTableUi testAsset2 = Resources.Load<DataTableUi>("Data/DataTableUi");
        //ScriptableObject tempObj = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Resources/Data/DataTableUi.asset");
        //DataTableUi testAsset = tempObj as DataTableUi;
        //foreach (UiExcel gd in testAsset.DataList)
        //{
        //    Debug.Log(gd.id);
        //    Debug.Log(gd.name);
        //}
        //testAsset.Init();
        //UiExcel tempData = testAsset.GetInfoById(1);
        //Debug.Log(tempData.id);

        //事件管理层
        EventMgr.Instance.Init();
        //协程管理层
        UtilTool.Add<CoroutineMgr>(gameObject);
        GameObject tempGo0 = GameObject.Find("CoroutinePoolTrs");
        DontDestroyOnLoad(tempGo0);
        CoroutineMgr.Instance.Init(tempGo0);
        //资源更新初始化管理层
        UpdateCheckMgr.Instance.Init();
        //AB包资源管理层
        AssetBundleMgr.Instance.Init();
        //资源管理层
        ResourcesMgr.Instance.Init(this);
        //对象池管理层
        GameObject tempGo1 = GameObject.Find("RecyclePoolTrs");
        DontDestroyOnLoad(tempGo1);
        GameObject tempGo2 = GameObject.Find("SceneTrs");
        DontDestroyOnLoad(tempGo2);
        ObjectPoolMgr.Instance.Init(tempGo1.transform, tempGo2.transform);
        //数据管理层
        DataMgr.Instance.Init();
        //屏幕辅助缩放类
        ScreenResizeHelper.Init();
        //UI管理层
        UtilTool.Add<UIMgr>(gameObject);
        //gameObject.GetOrCreatComPonent<UIMgr>();
        UIMgr.Instance.Init();
        //UI管理辅助层
        UIClassHelper.Instance.Init();
        //UISprite管理层
        GameObject tempGo3 = GameObject.Find("USpriteRecyclePoolTrs");
        DontDestroyOnLoad(tempGo3);
        USpriteMgr.Instance.Init(tempGo3.transform);
        //UTexture管理层
        UTextureMgr.Instance.Init();
        //登录管理层
        LoginMgr.Instance.Init();
        //音效管理层
        GameObject tempGo4 = GameObject.Find("SoundStageTrs");
        DontDestroyOnLoad(tempGo4);
        AudioMgr.Instance.Init(tempGo4.transform);
        //场景管理层
        SceneMgr.Instance.Init();
        //摄像机管理层
        GameObject tempGo6 = GameObject.Find("MainCamera");
        DontDestroyOnLoad(tempGo6);
        CameraMgr.Instance.Init(tempGo6);

        //世界角色中心层
        GameObject tempGo5 = GameObject.Find("GameWorldRecyclePoolTrs");
        DontDestroyOnLoad(tempGo5);
        //GameWorld.Instance.Init(tempGo2.transform, tempGo5.transform);

        _AfterInit();
    }

    private void _AfterInit()
    {
        EventMgr.Instance.AddEventListener(GlobalEvent.INIT_COMPLETE, InitCompleteStar);
        _LoadStarUIView();

        //EventMgr.Instance.AfterInit();
        //CoroutineHelper.Instance.AfterInit();
        UpdateCheckMgr.Instance.AfterInit();
        //AssetBundleMgr.Instance.AfterInit();
        //ResourcesMgr.Instance.AfterInit();
        //ObjectPoolMgr.Instance.AfterInit();
        //DataMgr.Instance.AfterInit();
        //UIMgr.Instance.AfterInit();
        //UIClassHelper.Instance.AfterInit();
        //USpriteMgr.Instance.AfterInit();
        //UTextureMgr.Instance.AfterInit();
        //LoginMgr.Instance.AfterInit();
        //AudioMgr.Instance.AfterInit();
        //SceneMgr.Instance.AfterInit();
        //GameWorld.Instance.AfterInit();
    }

    void InitCompleteStar()
    {
        starView.CloseView();
        GameObject.Destroy(starUiPrefab);
        starView = null;
        UIMgr.Instance.OpenView<MainStarView>("MainStarView", 0);
    }

    private GameObject starUiPrefab;
    private StarUIMainView starView;
    private double testTime;

    private void _LoadStarUIView()
    {
        starUiPrefab = Instantiate((GameObject)Resources.Load("UI/StarUIModel/StarUIPre"));
        starUiPrefab.transform.SetParent(UIMgr.Instance.middleLayer.transform, false);
        //starUiPrefab.transform.position = Vector2(-910, -450);
        starView = new StarUIMainView();
        starView.Init(starUiPrefab);
    }


    void Update () {
        //testTime = Time.deltaTime + testTime;
        //if (testTime >= 3.0)
        //{
        //    EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "(此过程不消耗任何流量，请放心等待)首次进入游戏,初始化中...", (int)(0.2 * 100f));
        //    testTime = 0;
        //}

        if (Input.GetKeyDown(KeyCode.A))
        {
            ObjectPoolMgr.Instance.ReleaseObject(tempObj);
            tempObj = null;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ObjectPoolMgr.Instance.InstantiateObjectAsync("Assets/Resources/UI/Prefabs/Main/UIRoot.prefab", OnLoadFinish, LoadResPriority.RES_HIGHT,true);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ObjectPoolMgr.Instance.ReleaseObject(tempObj,0,true);
            tempObj = null;
        }


        UIMgr.Instance.Update();
    }

    private void LateUpdate()
    {
        UIMgr.Instance.LateUpdate();
    }

    private void OnDestroy()
    {
        
    }
}
