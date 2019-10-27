//=====================================================
// - FileName:      MapWindows.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Zero.ZeroEngine.Data;

public class MapWindows : EditorWindow {

    public string _scriptEditPath = "";
    private const string SAVE_EDIT_PATH = "MapWindows._scriptEditPath";

    public string _scriptClientPath = "";
    private const string SAVE_CLIENT_PATH = "MapWindows._scriptClientPath";


    private Vector3 _leftUpStart = Vector3.zero;
    private float _mapAccuracy = 1;
    private int _maplong = 100;
    private int _mapWidth = 100;
    private int _mapHeight = 5;
    private bool _isExportCanMove = false;

    DataTableMap tableMap;
    Dictionary<int, MapExcel> tableMapDic;
    public int mapID = 0;
    public string mapName = "";
    DataTableMonster tableMonster;
    Dictionary<int, MonsterExcel> tableMonsterDic;
    DataTableMonsterRefresh tableMonsterRefresh;
    Dictionary<int, MonsterRefreshExcel> tableMonsterRefreshDic;
    DataTableNpc tableNpc;
    Dictionary<int, NpcExcel> tableNpcDic;
    DataTableTransfer tableTransfer;
    Dictionary<int, TransferExcel> tableTransferDic;
    DataTableModel tableModel;

    private Dictionary<int, string> selectTypeDic = new Dictionary<int, string>();
    private string[] selectTypeArr = new string[] { "Null", "PlayerBorn", "Monster", "MonsterRefresh", "Npc", "Transfer" };
    private int selectTypeIndex = 0;

    private Dictionary<int, GameObject> playerBornGoDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> monsterGoDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> monsterRefreshGoDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> npcGoDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> transferGoDic = new Dictionary<int, GameObject>();
    

    const string PLAYER_BORN_PREFIX = "PlayerBorn|";
    const string MONSTER_PREFIX = "Monster|";
    const string MONSTER_REFRESH_PREFIX = "MonsterRefresh|";
    const string NPC_PREFIX = "NPC|";
    const string TRANSFER_PREFIX = "Transfer|";

    private bool _isInitBoo = false;

    void OnEnable()
    {
        selectTypeDic.Add(0, "Null");
        selectTypeDic.Add(1, "PlayerBorn");
        selectTypeDic.Add(2, "Monster");
        selectTypeDic.Add(3, "MonsterRefresh");
        selectTypeDic.Add(4, "Npc");
        selectTypeDic.Add(5, "Transfer");

        _scriptEditPath = EditorPrefs.GetString(SAVE_EDIT_PATH, _scriptEditPath);
        _scriptClientPath = EditorPrefs.GetString(SAVE_CLIENT_PATH, _scriptClientPath);

        tableMap = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Resources/Data/DataTableMap.asset") as DataTableMap;
        tableMap.Init();
        tableMapDic = tableMap.GetInfo();

        tableMonster = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Resources/Data/DataTableMonster.asset") as DataTableMonster;
        tableMonster.Init();
        tableMonsterDic = tableMonster.GetInfo();

        tableMonsterRefresh = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Resources/Data/DataTableMonsterRefresh.asset") as DataTableMonsterRefresh;
        tableMonsterRefresh.Init();
        tableMonsterRefreshDic = tableMonsterRefresh.GetInfo();

        tableNpc = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Resources/Data/DataTableNpc.asset") as DataTableNpc;
        tableNpc.Init();
        tableNpcDic = tableNpc.GetInfo();

        tableTransfer = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Resources/Data/DataTableTransfer.asset") as DataTableTransfer;
        tableTransfer.Init();
        tableTransferDic = tableTransfer.GetInfo();

        tableModel = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Resources/Data/DataTableModel.asset") as DataTableModel;
        tableModel.Init();

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
    }

    private void OnGUI()
    {
        TopGui();
        MiddleGui();
        BottomGui();
    }

    void TopGui()
    {
        EditorGUILayout.BeginHorizontal();
        _scriptEditPath = EditorGUILayout.TextField("script_Editor临时目录:", _scriptEditPath);
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            if (path != null)
            {
                _scriptEditPath = path.Substring(path.IndexOf("Assets") + 6).Replace("\\", "/") + "/";
                Debug.LogWarning("Select _scriptEditPath folder =  " + _scriptEditPath);
                EditorPrefs.SetString(SAVE_EDIT_PATH, _scriptEditPath);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        _scriptClientPath = EditorGUILayout.TextField("script 客户端导出目录:", _scriptClientPath);
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            if (path != null)
            {
                _scriptClientPath = path.Substring(path.IndexOf("Assets") + 6).Replace("\\", "/") + "/";
                Debug.LogWarning("Select _scriptClientPath folder =  " + _scriptClientPath);
                EditorPrefs.SetString(SAVE_CLIENT_PATH, _scriptClientPath);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUI.color = Color.white;
        GUILayout.Label("提示-------------------------------------------------------------------------:");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("地图ID", GUILayout.Width(100)))
        {
            GenericMenu mapMenu = new GenericMenu();
            CreateMapMenu(mapMenu);
            mapMenu.DropDown(new Rect(100, 0, 0, 16));
        }
        EditorGUILayout.LabelField("当前地图ID:", mapID.ToString());
        EditorGUILayout.LabelField("当前地图名字:", mapName);
        EditorGUILayout.EndHorizontal();
    }
    void CreateMapMenu(GenericMenu tempMenu)
    {
        if (tableMapDic.Count <= 0)
        {
            Debug.LogError("地图表数据出错");
            return;
        }
        foreach (int tempKey in tableMapDic.Keys)
        {
            string tempMapName = tableMapDic[tempKey].name;
            int tempMapId = tableMapDic[tempKey].map_id;
            tempMenu.AddItem(new GUIContent(tempMapName + "(id=" + tempMapId + ")"), false, OnMpaSelectCallBack, tempMapId);
        }
    }

    void OnMpaSelectCallBack(object userData)
    {
        int tempMapId = (int)userData;
        if (tempMapId == 0)
        {
            Debug.LogError("地图ID为0");
            return;
        }
        if (tempMapId == mapID) return;
        mapID = tempMapId;
        MapExcel tempMapData = tableMap.GetInfoById(mapID);
        mapName = tempMapData.name;
        
        MapEditModel.Instance.Init();
        MapEditModel.Instance.curMapId = mapID;
        MapEditModel.Instance.LoadScene(tempMapData.mapResId);
        string tempCPath1 = Application.dataPath + _scriptEditPath + "Map" + mapID + "EditConfig.cs";
        if (File.Exists(tempCPath1))
        {
            MapEditModel.Instance.LoadMapConfig(mapID);
        }

        ClearAllGoDic();
        _isInitBoo = false;
        InitAllGo();

        this.Repaint();
    }

    void ClearAllGoDic()
    {
        playerBornGoDic.Clear();
        monsterGoDic.Clear();
        monsterRefreshGoDic.Clear();
        npcGoDic.Clear();
        transferGoDic.Clear();
    }

    void MiddleGui()
    {
        if (mapID == 0)
            return;
        _leftUpStart = EditorGUILayout.Vector3Field("地图开始左上角:", _leftUpStart);
        _mapAccuracy = EditorGUILayout.FloatField("地图格子精度:", _mapAccuracy);
        _maplong = EditorGUILayout.IntField("地图可行走长度:", _maplong);
        _mapWidth = EditorGUILayout.IntField("地图可行走宽度:", _mapWidth);
        _mapHeight = EditorGUILayout.IntField("地图可行走高度:", _mapHeight);

        if (GUILayout.Button("检测当前场景可行走区域"))
        {
            ExportMapCanMoveData();
        }
        if (GUILayout.Button("导出当前地图数据（Script）"))
        {
            if (_isExportCanMove) MapEditModel.Instance.ExportMapScriptData(_scriptEditPath,_scriptClientPath);
            else Debug.LogError("未导出可行走区域数据");
        }
        if (GUILayout.Button("导出当前地图数据（Lua）"))
        {
            if (_isExportCanMove) MapEditModel.Instance.ExportMapLuaData();
            else Debug.LogError("未导出可行走区域数据");
        }
        if (GUILayout.Button("导出当前地图数据（Java）"))
        {
            if (_isExportCanMove) MapEditModel.Instance.ExportMapJavaData();
            else Debug.LogError("未导出可行走区域数据");
        }

        GUILayout.Label("提示-------------------------------------------------------------------------:");

        EditorGUILayout.BeginHorizontal();
        selectTypeIndex = EditorGUILayout.Popup(selectTypeIndex, selectTypeArr, GUILayout.Width(100));
        EditorGUILayout.LabelField("选择类型:", selectTypeDic[selectTypeIndex]);
        if (GUILayout.Button("Create"))
        {
            try
            {
                switch (selectTypeIndex)
                {
                    case 0:
                        return;
                    case 1:
                        CreatePlayerBornGo();
                        break;
                    case 2:
                        CreateMonsterGo();
                        break;
                    case 3:
                        CreateMonsterRefreshGo();
                        break;
                    case 4:
                        CreateNpcGo();
                        break;
                    case 5:
                        CreateTransferGo();
                        break;
                }
            }
            catch
            {
                Debug.LogError("地图编辑器出错了");
            }
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("刷新下面数据"))
        {
            this.Repaint();
        }
    }
    void BottomGui()
    {
        if (mapID == 0)
            return;
        if (selectTypeIndex == 0)
            return;
        switch (selectTypeIndex)
        {
            case 0:
                return;
            case 1:
                OnGuiPlayerBorn();
                break;
            case 2:
                OnGuiMonster();
                break;
            case 3:
                OnGuiMonsterRefresh();
                break;
            case 4:
                OnGuiNpc();
                break;
            case 5:
                OnGuiTransfer();
                break;
        }
    }
    private Vector2 _scroll_pos;
    void OnGuiPlayerBorn()
    {
        if (MapEditModel.Instance.curMapVo == null) return;
        _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
        foreach (PlayerBornEditObj tempObj in MapEditModel.Instance.curMapVo.playerBornEditDic.Values)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                DeletePlayerBorn(tempObj.guid);
            }
            EditorGUILayout.LabelField("唯一ID:", tempObj.guid.ToString());
            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                RefreshPlayerBorn(tempObj.guid);
            }
            EditorGUILayout.EndHorizontal();
            tempObj.locationV = EditorGUILayout.Vector3Field("      位置:", tempObj.locationV);
            //EditorGUILayout.Vector3Field("      旋转:", tempObj.rotationV);
        }
        EditorGUILayout.EndScrollView();
    }
    void OnGuiMonster()
    {
        if (MapEditModel.Instance.curMapVo == null) return;
        _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
        foreach (MonsterEditObj tempObj in MapEditModel.Instance.curMapVo.monsterEditDic.Values)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                DeleteMonster(tempObj.guid);
            }
            EditorGUILayout.LabelField("唯一ID:", tempObj.guid.ToString());
            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                RefreshMonster(tempObj.guid);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("怪物ID", GUILayout.Width(100)))
            {
                GenericMenu monsterMenu = new GenericMenu();
                CreateMonsterMenu(monsterMenu, tempObj.guid);
                monsterMenu.DropDown(new Rect(100, 0, 0, 16));
            }
            EditorGUILayout.LabelField("当前怪物ID:", tempObj.monsterId.ToString());
            EditorGUILayout.EndHorizontal();
            tempObj.locationV = EditorGUILayout.Vector3Field("      位置:", tempObj.locationV);
            tempObj.rotationV = EditorGUILayout.Vector3Field("      旋转:", tempObj.rotationV);
        }
        EditorGUILayout.EndScrollView();
    }

    void CreateMonsterMenu(GenericMenu tempMenu, int tempGuid)
    {
        if (tableMonsterDic.Count <= 0)
        {
            Debug.LogError("怪物表数据出错");
            return;
        }
        foreach (int tempKey in tableMonsterDic.Keys)
        {
            string tempName = tableMonsterDic[tempKey].name;
            int tempId = tableMonsterDic[tempKey].id;
            int[] tempArr = new int[] { tempGuid, tempId };
            tempMenu.AddItem(new GUIContent(tempName + "(id=" + tempId + ")"), false, OnMonsterSelectCallBack, tempArr);
        }
    }
    void OnMonsterSelectCallBack(object userData)
    {
        int[] tempData = (int[])userData;
        if (tempData[0] == 0)
        {
            Debug.LogError("怪物ID为0");
            return;
        }
        MonsterEditObj tempObj;
        if (MapEditModel.Instance.curMapVo.monsterEditDic.TryGetValue(tempData[0], out tempObj))
        {
            tempObj.monsterId = tempData[1];
            GameObject tempGo;
            if(monsterGoDic.TryGetValue(tempData[0],out tempGo))
            {
                DeleteAllChild(tempGo);
                GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableMonsterDic[tempObj.monsterId].model).path)) as GameObject;
                tempModel.transform.SetParent(tempGo.transform, false);
                tempGo.name = MONSTER_PREFIX + tempObj.guid + "|" + tempObj.monsterId;
            }
            this.Repaint();
        }
    }

    void OnGuiMonsterRefresh()
    {
        if (MapEditModel.Instance.curMapVo == null) return;
        _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
        foreach (MonsterRefreshEditObj tempObj in MapEditModel.Instance.curMapVo.monsterRefreshEditDic.Values)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                DeleteMonsterRefresh(tempObj.guid);
            }
            EditorGUILayout.LabelField("唯一ID:", tempObj.guid.ToString());
            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                RefreshMonsterRefresh(tempObj.guid);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新点ID", GUILayout.Width(100)))
            {
                GenericMenu monsterRefreshMenu = new GenericMenu();
                CreateMonsterRefreshMenu(monsterRefreshMenu, tempObj.guid);
                monsterRefreshMenu.DropDown(new Rect(100, 0, 0, 16));
            }
            EditorGUILayout.LabelField("当前刷新点ID:", tempObj.guid.ToString());
            EditorGUILayout.EndHorizontal();
            tempObj.locationV = EditorGUILayout.Vector3Field("      位置:", tempObj.locationV);
            //tempObj.rotationV = EditorGUILayout.Vector3Field("      旋转:", tempObj.rotationV);
        }
        EditorGUILayout.EndScrollView();
    }
    void CreateMonsterRefreshMenu(GenericMenu tempMenu, int tempGuid)
    {
        if (tableMonsterRefreshDic.Count <= 0)
        {
            Debug.LogError("怪物刷新点表数据出错");
            return;
        }
        foreach (int tempKey in tableMonsterRefreshDic.Keys)
        {
            int tempId = tableMonsterRefreshDic[tempKey].id;
            int[] tampArr = new int[] { tempGuid, tempId };
            tempMenu.AddItem(new GUIContent("(id=" + tempId + ")"), false, OnMonsterRefreshSelectCallBack, tampArr);
        }
    }
    void OnMonsterRefreshSelectCallBack(object userData)
    {
        int[] tempData = (int[])userData;
        if (tempData[1] == 0)
        {
            Debug.LogError("怪物刷新点ID为0");
            return;
        }
        if (MapEditModel.Instance.curMapVo.monsterRefreshEditDic.ContainsKey(tempData[1]))
        {
            Debug.LogError("怪物刷新点已经添加过了，请重新选择");
        }
        MonsterRefreshEditObj tempObj;
        if (MapEditModel.Instance.curMapVo.monsterRefreshEditDic.TryGetValue(tempData[0], out tempObj))
        {
            tempObj.guid = tempData[1];
            MapEditModel.Instance.curMapVo.monsterRefreshEditDic.Remove(tempData[0]);
            GameObject tempGo;
            if(monsterRefreshGoDic.TryGetValue(tempData[0],out tempGo))
            {
                monsterRefreshGoDic.Remove(tempData[0]);
                monsterRefreshGoDic.Add(tempObj.guid, tempGo);
                tempGo.name = MONSTER_REFRESH_PREFIX + tempObj.guid;
            }
            MapEditModel.Instance.curMapVo.monsterRefreshEditDic.Add(tempObj.guid, tempObj);
            this.Repaint();
        }
    }


    void OnGuiNpc()
    {
        if (MapEditModel.Instance.curMapVo == null) return;
        _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
        foreach (NpcEditObj tempObj in MapEditModel.Instance.curMapVo.npcEditDic.Values)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                DeleteNpc(tempObj.guid);
            }
            EditorGUILayout.LabelField("唯一ID:", tempObj.guid.ToString());
            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                RefreshNpc(tempObj.guid);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("NPC_ID", GUILayout.Width(100)))
            {
                GenericMenu NpcMenu = new GenericMenu();
                CreateNpcMenu(NpcMenu, tempObj.guid);
                NpcMenu.DropDown(new Rect(100, 0, 0, 16));
            }
            EditorGUILayout.LabelField("当前NPC_ID:", tempObj.guid.ToString());
            EditorGUILayout.EndHorizontal();
            tempObj.locationV = EditorGUILayout.Vector3Field("      位置:", tempObj.locationV);
            tempObj.rotationV = EditorGUILayout.Vector3Field("      旋转:", tempObj.rotationV);
        }
        EditorGUILayout.EndScrollView();
    }
    void CreateNpcMenu(GenericMenu tempMenu, int tempGuid)
    {
        if (tableNpcDic.Count <= 0)
        {
            Debug.LogError("NPC表数据出错");
            return;
        }
        foreach (int tempKey in tableNpcDic.Keys)
        {
            string tempName = tableNpcDic[tempKey].name;
            int tempId = tableNpcDic[tempKey].id;
            int[] tempArr = new int[] { tempGuid, tempId };
            tempMenu.AddItem(new GUIContent(tempName + "(id=" + tempId + ")"), false, OnNpcSelectCallback, tempArr);
        }
    }
    void OnNpcSelectCallback(object userData)
    {
        int[] tempData = (int[])userData;
        if (tempData[1] == 0)
        {
            Debug.LogError("NPC_ID为0");
            return;
        }
        if (MapEditModel.Instance.curMapVo.npcEditDic.ContainsKey(tempData[1]))
        {
            Debug.LogError("NPC已经添加过了，请重新选择");
        }
        NpcEditObj tempObj;
        if (MapEditModel.Instance.curMapVo.npcEditDic.TryGetValue(tempData[0], out tempObj))
        {
            tempObj.guid = tempData[1];
            MapEditModel.Instance.curMapVo.npcEditDic.Remove(tempData[0]);
            GameObject tempGo;
            if(npcGoDic.TryGetValue(tempData[0],out tempGo))
            {
                npcGoDic.Remove(tempData[0]);
                DeleteAllChild(tempGo);
                GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableNpcDic[tempObj.guid].model).path)) as GameObject;
                tempModel.transform.SetParent(tempGo.transform, false);
                tempGo.name = NPC_PREFIX + tempObj.guid;
                npcGoDic.Add(tempObj.guid, tempGo);
            }
            MapEditModel.Instance.curMapVo.npcEditDic.Add(tempObj.guid, tempObj);
            this.Repaint();
        }
    }

    void OnGuiTransfer()
    {
        if (MapEditModel.Instance.curMapVo == null) return;
        _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
        foreach (TransferEditObj tempObj in MapEditModel.Instance.curMapVo.transferEditDic.Values)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                DeleteTransfer(tempObj.guid);
            }
            EditorGUILayout.LabelField("唯一ID:", tempObj.guid.ToString());
            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                RefreshTransfer(tempObj.guid);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("传送点ID", GUILayout.Width(100)))
            {
                GenericMenu TransferMenu = new GenericMenu();
                CreateTransferMenu(TransferMenu, tempObj.guid);
                TransferMenu.DropDown(new Rect(100, 0, 0, 16));
            }
            EditorGUILayout.LabelField("当前传送点ID:", tempObj.guid.ToString());
            EditorGUILayout.EndHorizontal();
            tempObj.locationV = EditorGUILayout.Vector3Field("      位置:", tempObj.locationV);
            tempObj.rotationV = EditorGUILayout.Vector3Field("      旋转:", tempObj.rotationV);
            tempObj.toLocation = EditorGUILayout.Vector3Field("      传送位置:", tempObj.toLocation);
        }
        EditorGUILayout.EndScrollView();
    }
    void CreateTransferMenu(GenericMenu tempMenu, int tempGuid)
    {
        if (tableTransferDic.Count <= 0)
        {
            Debug.LogError("传送点表数据出错");
            return;
        }
        foreach (int tempKey in tableTransferDic.Keys)
        {
            string tempName = tableTransferDic[tempKey].name;
            int tempId = tableTransferDic[tempKey].id;
            int[] tempArr = new int[] { tempGuid, tempId };
            tempMenu.AddItem(new GUIContent(tempName + "(id=" + tempId + ")"), false, OnTransferSelectCallback, tempArr);
        }
    }
    void OnTransferSelectCallback(object userData)
    {
        int[] tempData = (int[])userData;
        if (tempData[1] == 0)
        {
            Debug.LogError("传送点ID为0");
            return;
        }
        if (MapEditModel.Instance.curMapVo.transferEditDic.ContainsKey(tempData[1]))
        {
            Debug.LogError("传送点已经添加过了，请重新选择");
        }
        TransferEditObj tempObj;
        if (MapEditModel.Instance.curMapVo.transferEditDic.TryGetValue(tempData[0], out tempObj))
        {
            tempObj.guid = tempData[1];
            MapEditModel.Instance.curMapVo.transferEditDic.Remove(tempData[0]);
            GameObject tempGo;
            if (transferGoDic.TryGetValue(tempData[0], out tempGo))
            {
                transferGoDic.Remove(tempData[0]);
                DeleteAllChild(tempGo);
                GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableTransferDic[tempObj.guid].model).path)) as GameObject;
                tempModel.transform.SetParent(tempGo.transform, false);
                tempGo.name = TRANSFER_PREFIX + tempObj.guid;
                transferGoDic.Add(tempObj.guid, tempGo);
            }
            MapEditModel.Instance.curMapVo.transferEditDic.Add(tempObj.guid, tempObj);
            this.Repaint();
        }
    }

    void InitAllGo()
    {
        foreach (PlayerBornEditObj tempObj in MapEditModel.Instance.curMapVo.playerBornEditDic.Values)
        {
            InitPlayerBornGo(tempObj);
        }
        foreach (MonsterEditObj tempObj in MapEditModel.Instance.curMapVo.monsterEditDic.Values)
        {
            InitMonsterGo(tempObj);
        }
        foreach (MonsterRefreshEditObj tempObj in MapEditModel.Instance.curMapVo.monsterRefreshEditDic.Values)
        {
            InitMonsterRefreshGo(tempObj);
        }
        foreach (NpcEditObj tempObj in MapEditModel.Instance.curMapVo.npcEditDic.Values)
        {
            InitNpcGo(tempObj);
        }
        foreach (TransferEditObj tempObj in MapEditModel.Instance.curMapVo.transferEditDic.Values)
        {
            InitTransferGo(tempObj);
        }
        _isInitBoo = true;
    }

    void CreatePlayerBornGo()
    {
        PlayerBornEditObj tempObj =  MapEditModel.Instance.PlayerBornClassPool.Spawn(true);
        tempObj.guid = MapEditModel.Instance.GetPlayerGuid();
        MapEditModel.Instance.curMapVo.playerBornEditDic.Add(tempObj.guid, tempObj);
        GameObject tempGo = new GameObject();
        tempGo.name = PLAYER_BORN_PREFIX + tempObj.guid;
        playerBornGoDic.Add(tempObj.guid, tempGo);
        this.Repaint();
    }

    void InitPlayerBornGo(PlayerBornEditObj canPlayerObj)
    {
        GameObject tempGo = new GameObject();
        tempGo.name = PLAYER_BORN_PREFIX + canPlayerObj.guid;
        tempGo.transform.localPosition = canPlayerObj.locationV;
        //tempGo.transform.localRotation = Quaternion.Euler(canPlayerObj.rotationV.x, canPlayerObj.rotationV.y, canPlayerObj.rotationV.z);
        playerBornGoDic.Add(canPlayerObj.guid, tempGo);
    }

    void CreateMonsterGo()
    {
        MonsterEditObj tempObj = MapEditModel.Instance.MonsterClassPool.Spawn(true);
        tempObj.guid = MapEditModel.Instance.GetMonsterGuid();
        foreach (int tempKey in tableMonsterDic.Keys)
        {
            tempObj.monsterId = tableMonsterDic[tempKey].id;
            break;
        }
        MapEditModel.Instance.curMapVo.monsterEditDic.Add(tempObj.guid, tempObj);
        GameObject tempGo = new GameObject();
        tempGo.name = MONSTER_PREFIX + tempObj.guid + "|" + tempObj.monsterId;
        GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableMonsterDic[tempObj.monsterId].model).path)) as GameObject;
        tempModel.transform.SetParent(tempGo.transform,false);
        monsterGoDic.Add(tempObj.guid, tempGo);
        this.Repaint();
    }

    void InitMonsterGo(MonsterEditObj canMonsterObj)
    {
        GameObject tempGo = new GameObject();
        tempGo.name = MONSTER_PREFIX + canMonsterObj.guid + "|" + canMonsterObj.monsterId;
        GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableMonsterDic[canMonsterObj.monsterId].model).path)) as GameObject;
        tempModel.transform.SetParent(tempGo.transform, false);
        tempGo.transform.localPosition = canMonsterObj.locationV;
        tempGo.transform.localRotation = Quaternion.Euler(canMonsterObj.rotationV.x, canMonsterObj.rotationV.y, canMonsterObj.rotationV.z);
        monsterGoDic.Add(canMonsterObj.guid, tempGo);
    }

    void CreateMonsterRefreshGo()
    {
        MonsterRefreshEditObj tempObj = MapEditModel.Instance.MonsterRefreshClassPool.Spawn(true);
        foreach (int tempKey in tableMonsterRefreshDic.Keys)
        {
            if (MapEditModel.Instance.curMapVo.monsterRefreshEditDic.ContainsKey(tableMonsterRefreshDic[tempKey].id))
            {
                Debug.LogError("测试刷新点ID用于创建，不可用于重复使用，此地图已经含有测试刷新点");
                MapEditModel.Instance.MonsterRefreshClassPool.Recycle(tempObj);
                return;
            }
            else
            {
                tempObj.guid = tableMonsterRefreshDic[tempKey].id;
                break;
            }
        }
        MapEditModel.Instance.curMapVo.monsterRefreshEditDic.Add(tempObj.guid, tempObj);
        GameObject tempGo = new GameObject();
        tempGo.name = MONSTER_REFRESH_PREFIX + tempObj.guid;
        monsterRefreshGoDic.Add(tempObj.guid, tempGo);
        this.Repaint();
    }

    void InitMonsterRefreshGo(MonsterRefreshEditObj canMonsterRefreshObj)
    {
        GameObject tempGo = new GameObject();
        tempGo.name = MONSTER_REFRESH_PREFIX + canMonsterRefreshObj.guid;
        tempGo.transform.localPosition = canMonsterRefreshObj.locationV;
        //tempGo.transform.localRotation = Quaternion.Euler(canMonsterRefreshObj.rotationV.x, canMonsterRefreshObj.rotationV.y, canMonsterRefreshObj.rotationV.z);
        monsterRefreshGoDic.Add(canMonsterRefreshObj.guid, tempGo);
    }

    void CreateNpcGo()
    {
        NpcEditObj tempObj = MapEditModel.Instance.NpcClassPool.Spawn(true);
        foreach (int tempKey in tableNpcDic.Keys)
        {
            if (MapEditModel.Instance.curMapVo.npcEditDic.ContainsKey(tableNpcDic[tempKey].id))
            {
                Debug.LogError("测试NPC ID用于创建，不可用于重复使用，此地图已经含有测试NPC");
                MapEditModel.Instance.NpcClassPool.Recycle(tempObj);
                return;
            }
            else
            {
                tempObj.guid = tableNpcDic[tempKey].id;
                break;
            }
        }
        MapEditModel.Instance.curMapVo.npcEditDic.Add(tempObj.guid, tempObj);
        GameObject tempGo = new GameObject();
        tempGo.name = NPC_PREFIX + tempObj.guid;
        GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableNpcDic[tempObj.guid].model).path)) as GameObject;
        tempModel.transform.SetParent(tempGo.transform, false);
        npcGoDic.Add(tempObj.guid, tempGo);
        this.Repaint();
    }

    void InitNpcGo(NpcEditObj canNpcObj)
    {
        GameObject tempGo = new GameObject();
        tempGo.name = NPC_PREFIX + canNpcObj.guid;
        GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableNpcDic[canNpcObj.guid].model).path)) as GameObject;
        tempModel.transform.SetParent(tempGo.transform, false);
        tempGo.transform.localPosition = canNpcObj.locationV;
        tempGo.transform.localRotation = Quaternion.Euler(canNpcObj.rotationV.x, canNpcObj.rotationV.y, canNpcObj.rotationV.z);
        npcGoDic.Add(canNpcObj.guid, tempGo);
    }

    void CreateTransferGo()
    {
        TransferEditObj tempObj = MapEditModel.Instance.TransferClassPool.Spawn(true);
        foreach (int tempKey in tableTransferDic.Keys)
        {
            if (MapEditModel.Instance.curMapVo.transferEditDic.ContainsKey(tableTransferDic[tempKey].id))
            {
                Debug.LogError("测试传送点ID用于创建，不可用于重复使用，此地图已经含有测试传送点");
                MapEditModel.Instance.TransferClassPool.Recycle(tempObj);
                return;
            }
            else
            {
                tempObj.guid = tableTransferDic[tempKey].id;
                break;
            }
        }
        MapEditModel.Instance.curMapVo.transferEditDic.Add(tempObj.guid, tempObj);
        GameObject tempGo = new GameObject();
        tempGo.name = TRANSFER_PREFIX + tempObj.guid;
        GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableTransferDic[tempObj.guid].model).path)) as GameObject;
        tempModel.transform.SetParent(tempGo.transform, false);
        transferGoDic.Add(tempObj.guid, tempGo);
        this.Repaint();
    }

    void InitTransferGo(TransferEditObj canTransferObj)
    {
        GameObject tempGo = new GameObject();
        tempGo.name = TRANSFER_PREFIX + canTransferObj.guid;
        GameObject tempModel = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(tableModel.GetInfoById(tableTransferDic[canTransferObj.guid].model).path)) as GameObject;
        tempModel.transform.SetParent(tempGo.transform, false);
        tempGo.transform.localPosition = canTransferObj.locationV;
        tempGo.transform.localRotation = Quaternion.Euler(canTransferObj.rotationV.x, canTransferObj.rotationV.y, canTransferObj.rotationV.z);
        transferGoDic.Add(canTransferObj.guid, tempGo);
    }

    void RefreshPlayerBorn(int canGuid)
    {
        GameObject tempGo;
        if(playerBornGoDic.TryGetValue(canGuid, out tempGo))
        {
            PlayerBornEditObj tempObj;
            if(MapEditModel.Instance.curMapVo.playerBornEditDic.TryGetValue(canGuid,out tempObj))
            {
                tempGo.transform.localPosition = tempObj.locationV;
                tempGo.transform.localRotation = Quaternion.Euler(tempObj.rotationV);
            }
        }
    }
    void DeletePlayerBorn(int canGuid)
    {
        GameObject tempGo;
        if (playerBornGoDic.TryGetValue(canGuid, out tempGo))
        {
            PlayerBornEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.playerBornEditDic.TryGetValue(canGuid, out tempObj))
            {
                MapEditModel.Instance.curMapVo.playerBornEditDic.Remove(canGuid);
                tempObj.Reset();
                MapEditModel.Instance.PlayerBornClassPool.Recycle(tempObj);
            }
            playerBornGoDic.Remove(canGuid);
            MapEditModel.Instance.RecyclePlayerGuid(canGuid);
            GameObject.DestroyImmediate(tempGo);
            this.Repaint();
        }
    }

    void RefreshMonster(int canGuid)
    {
        GameObject tempGo;
        if (monsterGoDic.TryGetValue(canGuid, out tempGo))
        {
            MonsterEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.monsterEditDic.TryGetValue(canGuid, out tempObj))
            {
                tempGo.transform.localPosition = tempObj.locationV;
                tempGo.transform.localRotation = Quaternion.Euler(tempObj.rotationV);
            }
        }
    }
    void DeleteMonster(int canGuid)
    {
        GameObject tempGo;
        if (monsterGoDic.TryGetValue(canGuid, out tempGo))
        {
            MonsterEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.monsterEditDic.TryGetValue(canGuid, out tempObj))
            {
                MapEditModel.Instance.curMapVo.monsterEditDic.Remove(canGuid);
                tempObj.Reset();
                MapEditModel.Instance.MonsterClassPool.Recycle(tempObj);
            }
            monsterGoDic.Remove(canGuid);
            MapEditModel.Instance.RecycleMonsterGuid(canGuid);
            GameObject.DestroyImmediate(tempGo);
            this.Repaint();
        }
    }

    void RefreshMonsterRefresh(int canGuid)
    {
        GameObject tempGo;
        if (monsterRefreshGoDic.TryGetValue(canGuid, out tempGo))
        {
            MonsterRefreshEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.monsterRefreshEditDic.TryGetValue(canGuid, out tempObj))
            {
                tempGo.transform.localPosition = tempObj.locationV;
                //tempGo.transform.localRotation = Quaternion.Euler(tempObj.rotationV);
            }
        }
    }
    void DeleteMonsterRefresh(int canGuid)
    {
        GameObject tempGo;
        if (monsterRefreshGoDic.TryGetValue(canGuid, out tempGo))
        {
            MonsterRefreshEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.monsterRefreshEditDic.TryGetValue(canGuid, out tempObj))
            {
                MapEditModel.Instance.curMapVo.monsterRefreshEditDic.Remove(canGuid);
                tempObj.Reset();
                MapEditModel.Instance.MonsterRefreshClassPool.Recycle(tempObj);
            }
            monsterRefreshGoDic.Remove(canGuid);
            GameObject.DestroyImmediate(tempGo);
            this.Repaint();
        }
    }

    void RefreshNpc(int canGuid)
    {
        GameObject tempGo;
        if (npcGoDic.TryGetValue(canGuid, out tempGo))
        {
            NpcEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.npcEditDic.TryGetValue(canGuid, out tempObj))
            {
                tempGo.transform.localPosition = tempObj.locationV;
                tempGo.transform.localRotation = Quaternion.Euler(tempObj.rotationV);
            }
        }
    }
    void DeleteNpc(int canGuid)
    {
        GameObject tempGo;
        if (npcGoDic.TryGetValue(canGuid, out tempGo))
        {
            NpcEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.npcEditDic.TryGetValue(canGuid, out tempObj))
            {
                MapEditModel.Instance.curMapVo.npcEditDic.Remove(canGuid);
                tempObj.Reset();
                MapEditModel.Instance.NpcClassPool.Recycle(tempObj);
            }
            npcGoDic.Remove(canGuid);
            GameObject.DestroyImmediate(tempGo);
            this.Repaint();
        }
    }

    void RefreshTransfer(int canGuid)
    {
        GameObject tempGo;
        if (transferGoDic.TryGetValue(canGuid, out tempGo))
        {
            TransferEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.transferEditDic.TryGetValue(canGuid, out tempObj))
            {
                tempGo.transform.localPosition = tempObj.locationV;
                tempGo.transform.localRotation = Quaternion.Euler(tempObj.rotationV);
            }
        }
    }
    void DeleteTransfer(int canGuid)
    {
        GameObject tempGo;
        if (transferGoDic.TryGetValue(canGuid, out tempGo))
        {
            TransferEditObj tempObj;
            if (MapEditModel.Instance.curMapVo.transferEditDic.TryGetValue(canGuid, out tempObj))
            {
                MapEditModel.Instance.curMapVo.transferEditDic.Remove(canGuid);
                tempObj.Reset();
                MapEditModel.Instance.TransferClassPool.Recycle(tempObj);
            }
            transferGoDic.Remove(canGuid);
            GameObject.DestroyImmediate(tempGo);
            this.Repaint();
        }
    }

    void DeleteAllChild(GameObject canGo)
    {
        int tempCount = canGo.transform.childCount;
        for(int i = 0; i < tempCount; i++)
        {
            DestroyImmediate(canGo.transform.GetChild(0).gameObject);
        }
    }

    void ExportMapCanMoveData()
    {
        int[,] tempBlockArr = new int[_mapWidth, _maplong];
        for (int j = 0; j < _maplong; j++)
        {
            for (int i = 0; i < _mapWidth; i++)
            {
                int tempBlock = tempBlockArr[i, j];
                NavMeshHit tempHit;
                for (float k = 0; k < _mapHeight; k+=0.5f)
                {
                    if (NavMesh.SamplePosition(_leftUpStart + new Vector3(i * _mapAccuracy, k, j * _mapAccuracy), out tempHit, 0.25f, NavMesh.AllAreas))
                    {
                        tempBlock = 1;
                        tempBlockArr[i, j] = 1;
                        break;
                    }
                }
                Debug.DrawRay(_leftUpStart + new Vector3(i * _mapAccuracy, 0, j * _mapAccuracy), Vector3.up * 4, tempBlock == 1 ? Color.green : Color.red, 100.0f);
            }
            EditorUtility.DisplayProgressBar("可行走区域检测","检测中", j * 1.0f / (float)_maplong);
        }
        EditorUtility.ClearProgressBar();
        MapEditModel.Instance.curMapVo.block = tempBlockArr;
        _isExportCanMove = true;
    }

    private void OnDestroy()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");

        tableMap.Clear();
        tableMap = null;
        tableMapDic = null;

        tableMonster.Clear();
        tableMonster = null;
        tableMonsterDic = null;

        tableMonsterRefresh.Clear();
        tableMonsterRefresh = null;
        tableMonsterRefreshDic = null;

        tableNpc.Clear();
        tableNpc = null;
        tableNpcDic = null;

        tableTransfer.Clear();
        tableTransfer = null;
        tableTransferDic = null;

        tableModel.Clear();
        tableModel = null;
    }
}
