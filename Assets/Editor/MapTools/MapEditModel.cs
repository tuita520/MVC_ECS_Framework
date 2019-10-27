//=====================================================
// - FileName:      MapEditModel.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;

public class MapEditModel :Singleton<MapEditModel>{

    private const string SCENE_PATH = @"Assets/Scene/{0}.unity";

    public int playerGuid = 0;
    public int monsterGuid = 0;
    public int playerGuidCount = 0;
    public int monsterGuidCount = 0;
    public Dictionary<int, bool> playerGuidDic = new Dictionary<int, bool>();
    public Dictionary<int, bool> monsterGuidDic = new Dictionary<int, bool>();

    public MapEditVo curMapVo = new MapEditVo();
    public int curMapId = 0;

    public ClassObjectPool<PlayerBornEditObj> PlayerBornClassPool = new ClassObjectPool<PlayerBornEditObj>(3);
    public ClassObjectPool<MonsterEditObj> MonsterClassPool = new ClassObjectPool<MonsterEditObj>(10);
    public ClassObjectPool<MonsterRefreshEditObj> MonsterRefreshClassPool = new ClassObjectPool<MonsterRefreshEditObj>(10);
    public ClassObjectPool<NpcEditObj> NpcClassPool = new ClassObjectPool<NpcEditObj>(10);
    public ClassObjectPool<TransferEditObj> TransferClassPool = new ClassObjectPool<TransferEditObj>(5);

    public void Init()
    {
        playerGuid = 0;
        playerGuidCount = 0;
        monsterGuid = 0;
        monsterGuidCount = 0;
        playerGuidDic.Clear();
        monsterGuidDic.Clear();

        curMapVo.Reset();
        curMapId = 0;
    }

    public void LoadMapConfig(int canMapID)
    {
        string tempMapConfigName = string.Format("Map{0}EditConfig", canMapID);
        Type classType = Type.GetType(tempMapConfigName);
        var tempMapConfig = Activator.CreateInstance(classType);
        MethodInfo initMechod = classType.GetMethod("Init");
        initMechod.Invoke(tempMapConfig,new object[] { });
        SetPlayerGuidDic();
        SetMonsterGuidDic();
    }

    public void LoadScene(int canSceneID)
    {
        //EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo(); //询问有修改的场景时的 保存当前修改场景
        EditorSceneManager.OpenScene(string.Format(SCENE_PATH, canSceneID));// 打开场景
    }

    void SetPlayerGuidDic()
    {
        playerGuidCount = curMapVo.playerBornEditDic.Count;
        int maxKey = 0;
        foreach (int tempKey in curMapVo.playerBornEditDic.Keys)
        {
            maxKey = tempKey;
        }
        playerGuid = maxKey;
        for (int i = 1; i <= maxKey; i++)
        {
            if (curMapVo.playerBornEditDic.ContainsKey(i))
            {
                playerGuidDic.Add(i, true);
            }
            else
            {
                playerGuidDic.Add(i, false);
            }
        }
    }

    void SetMonsterGuidDic()
    {
        monsterGuidCount = curMapVo.monsterEditDic.Count;
        int maxKey = 0;
        foreach (int tempKey in curMapVo.monsterEditDic.Keys)
        {
            maxKey = tempKey;
        }
        monsterGuid = maxKey;
        for (int i = 1; i <= maxKey; i++)
        {
            if (curMapVo.monsterEditDic.ContainsKey(i))
            {
                monsterGuidDic.Add(i, true);
            }
            else
            {
                monsterGuidDic.Add(i, false);
            }
        }
    }

    public int GetPlayerGuid()
    {
        foreach(int tempKey in playerGuidDic.Keys)
        {
            if (!playerGuidDic[tempKey])
            {
                playerGuidDic[tempKey] = true;
                playerGuidCount++;
                return tempKey;
            }
        }
        playerGuidCount++;
        playerGuid++;
        playerGuidDic.Add(playerGuid, true);
        return playerGuid;
    }

    public int GetMonsterGuid()
    {
        foreach (int tempKey in monsterGuidDic.Keys)
        {
            if (!monsterGuidDic[tempKey])
            {
                monsterGuidDic[tempKey] = true;
                monsterGuidCount++;
                return tempKey;
            }
        }
        monsterGuidCount++;
        monsterGuid++;
        monsterGuidDic.Add(monsterGuid, true);
        return monsterGuid;
    }

    public void RecyclePlayerGuid(int canGuid)
    {
        bool tempBoo;
        if(playerGuidDic.TryGetValue(canGuid,out tempBoo))
        {
            tempBoo = false;
        }
    }

    public void RecycleMonsterGuid(int canGuid)
    {
        bool tempBoo;
        if (monsterGuidDic.TryGetValue(canGuid, out tempBoo))
        {
            tempBoo = false;
        }
    }

    public void ExportMapScriptData(string canCPath1, string canCPath2)
    {
        var u8WithoutBom = new System.Text.UTF8Encoding(false);
        string tempCPath1 = Application.dataPath + canCPath1 + "Map" + curMapId + "EditConfig.cs";
        if (File.Exists(tempCPath1)) File.Delete(tempCPath1);
        StreamWriter csw1;
        csw1 = new StreamWriter(tempCPath1, false, u8WithoutBom);
        csw1.WriteLine("/*=====================================================");
        csw1.WriteLine("* - Editor by tool");
        csw1.WriteLine("* - Don't editor by handself");
        csw1.WriteLine("=====================================================*/");
        csw1.WriteLine("using System;");
        csw1.WriteLine("using UnityEngine;");
        csw1.WriteLine("using System.Collections.Generic;");
        csw1.Write("\n");
        csw1.Write("\n");
        csw1.WriteLine("public class " + "Map" + curMapId + "EditConfig {");
        csw1.WriteLine("\tpublic void Init() {");
        csw1.WriteLine("\t\tMapEditVo tempMapEditVo = MapEditModel.Instance.curMapVo;");
        csw1.WriteLine("\t\ttempMapEditVo.mapId = " + curMapId + ";");
        int tempPlayerCount = 1;
        foreach(PlayerBornEditObj tempPlayerBornObj in curMapVo.playerBornEditDic.Values)
        {
            string tempStr = "tempPlayerObj" + tempPlayerCount.ToString();
            csw1.WriteLine("\t\tPlayerBornEditObj " + tempStr + " = MapEditModel.Instance.PlayerBornClassPool.Spawn(true);");
            csw1.WriteLine("\t\t" + tempStr + ".guid = " + tempPlayerBornObj.guid + ";");
            csw1.WriteLine("\t\t" + tempStr + ".locationV.Set(" + tempPlayerBornObj.locationV.x + "f," + tempPlayerBornObj.locationV.y
                + "f," + tempPlayerBornObj.locationV.z + "f);");
            csw1.WriteLine("\t\ttempMapEditVo.playerBornEditDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempPlayerCount++;
        }
        int tempMonsterCount = 1;
        foreach(MonsterEditObj tempMonsterObj in curMapVo.monsterEditDic.Values)
        {
            string tempStr = "tempMonsterObj" + tempMonsterCount.ToString();
            csw1.WriteLine("\t\tMonsterEditObj " + tempStr + " = MapEditModel.Instance.MonsterClassPool.Spawn(true);");
            csw1.WriteLine("\t\t" + tempStr + ".guid = " + tempMonsterObj.guid + ";");
            csw1.WriteLine("\t\t" + tempStr + ".monsterId =" + tempMonsterObj.monsterId + ";");
            csw1.WriteLine("\t\t" + tempStr + ".locationV.Set(" + tempMonsterObj.locationV.x + "f," + tempMonsterObj.locationV.y
                + "f," + tempMonsterObj.locationV.z + "f);");
            csw1.WriteLine("\t\t" + tempStr + ".rotationV.Set(" + tempMonsterObj.rotationV.x + "f," + tempMonsterObj.rotationV.y
                + "f," + tempMonsterObj.rotationV.z + "f);");
            csw1.WriteLine("\t\ttempMapEditVo.monsterEditDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempMonsterCount++;
        }
        int tempMonsterRefreshCount = 1;
        foreach (MonsterRefreshEditObj tempMonsterRefreshObj in curMapVo.monsterRefreshEditDic.Values)
        {
            string tempStr = "tempMonsterRefreshObj" + tempMonsterRefreshCount.ToString();
            csw1.WriteLine("\t\tMonsterRefreshEditObj " + tempStr + " = MapEditModel.Instance.MonsterRefreshClassPool.Spawn(true);");
            csw1.WriteLine("\t\t" + tempStr + ".guid = " + tempMonsterRefreshObj.guid + ";");
            csw1.WriteLine("\t\t" + tempStr + ".locationV.Set(" + tempMonsterRefreshObj.locationV.x + "f," + tempMonsterRefreshObj.locationV.y
                + "f," + tempMonsterRefreshObj.locationV.z + "f);");
            //csw1.WriteLine("\t\t" + tempStr + ".rotationV.Set(" + tempMonsterRefreshObj.rotationV.x + "f," + tempMonsterRefreshObj.rotationV.y
            //    + "f," + tempMonsterRefreshObj.rotationV.z + "f);");
            csw1.WriteLine("\t\ttempMapEditVo.monsterRefreshEditDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempMonsterRefreshCount++;
        }
        int tempNpcCount = 1;
        foreach (NpcEditObj tempNpcObj in curMapVo.npcEditDic.Values)
        {
            string tempStr = "tempNpcObj" + tempNpcCount.ToString();
            csw1.WriteLine("\t\tNpcEditObj " + tempStr + " = MapEditModel.Instance.NpcClassPool.Spawn(true);");
            csw1.WriteLine("\t\t" + tempStr + ".guid = " + tempNpcObj.guid + ";");
            csw1.WriteLine("\t\t" + tempStr + ".locationV.Set(" + tempNpcObj.locationV.x + "f," + tempNpcObj.locationV.y
                + "f," + tempNpcObj.locationV.z + "f);");
            csw1.WriteLine("\t\t" + tempStr + ".rotationV.Set(" + tempNpcObj.rotationV.x + "f," + tempNpcObj.rotationV.y
                + "f," + tempNpcObj.rotationV.z + "f);");
            csw1.WriteLine("\t\ttempMapEditVo.npcEditDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempNpcCount++;
        }
        int tempTransferCount = 1;
        foreach (TransferEditObj tempTransferObj in curMapVo.transferEditDic.Values)
        {
            string tempStr = "tempTransferObj" + tempTransferCount.ToString();
            csw1.WriteLine("\t\tTransferEditObj " + tempStr + " = MapEditModel.Instance.TransferClassPool.Spawn(true);");
            csw1.WriteLine("\t\t" + tempStr + ".guid = " + tempTransferObj.guid + ";");
            csw1.WriteLine("\t\t" + tempStr + ".locationV.Set(" + tempTransferObj.locationV.x + "f," + tempTransferObj.locationV.y
                + "f," + tempTransferObj.locationV.z + "f);");
            csw1.WriteLine("\t\t" + tempStr + ".rotationV.Set(" + tempTransferObj.rotationV.x + "f," + tempTransferObj.rotationV.y
                + "f," + tempTransferObj.rotationV.z + "f);");
            csw1.WriteLine("\t\t" + tempStr + ".toLocation.Set(" + tempTransferObj.toLocation.x + "f," + tempTransferObj.toLocation.y
                + "f," + tempTransferObj.toLocation.z + "f);");
            csw1.WriteLine("\t\ttempMapEditVo.transferEditDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempTransferCount++;
        }
        csw1.WriteLine("\t\ttempMapEditVo.block = new int[,]{");
        csw1.Write("\n");
        int lineCount = curMapVo.block.GetLength(0);
        int rowCount = curMapVo.block.GetLength(1);
        for(int i = 0; i < lineCount; i++)
        {
            csw1.Write("\t\t\t{");
            for(int j = 0; j < rowCount; j++)
            {
                csw1.Write(curMapVo.block[i,j].ToString());
                if (j != rowCount - 1) csw1.Write(",");
            }
            if (i == lineCount - 1) csw1.Write("}");
            else csw1.Write("},");
            csw1.Write("\n");
        }
        csw1.WriteLine("\t\t");
        csw1.WriteLine("\t\t};");
        csw1.WriteLine("\t}");
        csw1.WriteLine("}");

        csw1.Flush();
        csw1.Close();
        csw1.Dispose();
        Debug.Log(canCPath1+ "Map" + curMapId + "Config.cs file  export complete !");

        string tempCPath2 = Application.dataPath + canCPath2 + "Map" + curMapId + "Config.cs";
        if (File.Exists(tempCPath2)) File.Delete(tempCPath2);
        AssetDatabase.Refresh();
        StreamWriter csw2;
        csw2 = new StreamWriter(tempCPath2, false, u8WithoutBom);
        csw2.WriteLine("/*=====================================================");
        csw2.WriteLine("* - Editor by tool");
        csw2.WriteLine("* - Don't editor by handself");
        csw2.WriteLine("=====================================================*/");
        csw2.WriteLine("using System;");
        csw2.WriteLine("using UnityEngine;");
        csw2.WriteLine("using System.Collections.Generic;");
        csw2.Write("\n");
        csw2.Write("\n");
        csw2.WriteLine("namespace Zero.ZeroEngine.SceneFrame {");
        csw2.WriteLine("\tpublic class " + "Map" + curMapId + "Config : BaseMapConfig {");
        csw2.WriteLine("\t\tpublic override void Init() {");
        csw2.WriteLine("\t\t\tSceneVo tempSceneVo = SceneMgr.Instance.curSceneVo;");
        csw2.WriteLine("\t\t\ttempSceneVo.mapId = " + curMapId + ";");
        int tempPlayerCount2 = 1;
        foreach (PlayerBornEditObj tempPlayerBornObj in curMapVo.playerBornEditDic.Values)
        {
            string tempStr = "tempPlayerObj" + tempPlayerCount2.ToString();
            csw2.WriteLine("\t\t\tPlayerBornObj " + tempStr + " = SceneMgr.Instance.PlayerBornClassPool.Spawn(true);");
            csw2.WriteLine("\t\t\t" + tempStr + ".guid = " + tempPlayerBornObj.guid + ";");
            csw2.WriteLine("\t\t\t" + tempStr + ".locationV.Set(" + tempPlayerBornObj.locationV.x + "f," + tempPlayerBornObj.locationV.y
                + "f," + tempPlayerBornObj.locationV.z + "f);");
            csw2.WriteLine("\t\t\ttempSceneVo.playerBornDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempPlayerCount2++;
        }
        int tempMonsterCount2 = 1;
        foreach (MonsterEditObj tempMonsterObj in curMapVo.monsterEditDic.Values)
        {
            string tempStr = "tempMonsterObj" + tempMonsterCount2.ToString();
            csw2.WriteLine("\t\t\tMonsterObj " + tempStr + " = SceneMgr.Instance.MonsterClassPool.Spawn(true);");
            csw2.WriteLine("\t\t\t" + tempStr + ".guid = " + tempMonsterObj.guid + ";");
            csw2.WriteLine("\t\t\t" + tempStr + ".monsterId =" + tempMonsterObj.monsterId + ";");
            csw2.WriteLine("\t\t\t" + tempStr + ".locationV.Set(" + tempMonsterObj.locationV.x + "f," + tempMonsterObj.locationV.y
                + "f," + tempMonsterObj.locationV.z + "f);");
            csw2.WriteLine("\t\t\t" + tempStr + ".rotationV.Set(" + tempMonsterObj.rotationV.x + "f," + tempMonsterObj.rotationV.y
                + "f," + tempMonsterObj.rotationV.z + "f);");
            csw2.WriteLine("\t\t\ttempSceneVo.monsterDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempMonsterCount2++;
        }
        int tempMonsterRefreshCount2 = 1;
        foreach (MonsterRefreshEditObj tempMonsterRefreshObj in curMapVo.monsterRefreshEditDic.Values)
        {
            string tempStr = "tempMonsterRefreshObj" + tempMonsterRefreshCount2.ToString();
            csw2.WriteLine("\t\t\tMonsterRefreshObj " + tempStr + " = SceneMgr.Instance.MonsterRefreshClassPool.Spawn(true);");
            csw2.WriteLine("\t\t\t" + tempStr + ".guid = " + tempMonsterRefreshObj.guid + ";");
            csw2.WriteLine("\t\t\t" + tempStr + ".locationV.Set(" + tempMonsterRefreshObj.locationV.x + "f," + tempMonsterRefreshObj.locationV.y
                + "f," + tempMonsterRefreshObj.locationV.z + "f);");
            //csw2.WriteLine("\t\t" + tempStr + ".rotationV.Set(" + tempMonsterRefreshObj.rotationV.x + "f," + tempMonsterRefreshObj.rotationV.y
            //    + "f," + tempMonsterRefreshObj.rotationV.z + "f);");
            csw2.WriteLine("\t\t\ttempSceneVo.monsterRefreshDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempMonsterRefreshCount2++;
        }
        int tempNpcCount2 = 1;
        foreach (NpcEditObj tempNpcObj in curMapVo.npcEditDic.Values)
        {
            string tempStr = "tempNpcObj" + tempNpcCount2.ToString();
            csw2.WriteLine("\t\t\tNpcObj " + tempStr + " = SceneMgr.Instance.NpcClassPool.Spawn(true);");
            csw2.WriteLine("\t\t\t" + tempStr + ".guid = " + tempNpcObj.guid + ";");
            csw2.WriteLine("\t\t\t" + tempStr + ".locationV.Set(" + tempNpcObj.locationV.x + "f," + tempNpcObj.locationV.y
                + "f," + tempNpcObj.locationV.z + "f);");
            csw2.WriteLine("\t\t\t" + tempStr + ".rotationV.Set(" + tempNpcObj.rotationV.x + "f," + tempNpcObj.rotationV.y
                + "f," + tempNpcObj.rotationV.z + "f);");
            csw2.WriteLine("\t\t\ttempSceneVo.npcDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempNpcCount2++;
        }
        int tempTransferCount2 = 1;
        foreach (TransferEditObj tempTransferObj in curMapVo.transferEditDic.Values)
        {
            string tempStr = "tempTransferObj" + tempTransferCount2.ToString();
            csw2.WriteLine("\t\t\tTransferObj " + tempStr + " = SceneMgr.Instance.TransferClassPool.Spawn(true);");
            csw2.WriteLine("\t\t\t" + tempStr + ".guid = " + tempTransferObj.guid + ";");
            csw2.WriteLine("\t\t\t" + tempStr + ".locationV.Set(" + tempTransferObj.locationV.x + "f," + tempTransferObj.locationV.y
                + "f," + tempTransferObj.locationV.z + "f);");
            csw2.WriteLine("\t\t\t" + tempStr + ".rotationV.Set(" + tempTransferObj.rotationV.x + "f," + tempTransferObj.rotationV.y
                + "f," + tempTransferObj.rotationV.z + "f);");
            csw2.WriteLine("\t\t\t" + tempStr + ".toLocation.Set(" + tempTransferObj.toLocation.x + "f," + tempTransferObj.toLocation.y
                + "f," + tempTransferObj.toLocation.z + "f);");
            csw2.WriteLine("\t\t\ttempSceneVo.transferDic.Add(" + tempStr + ".guid," + tempStr + ");");
            tempTransferCount2++;
        }
        csw2.WriteLine("\t\t\ttempSceneVo.block = new int[,]{");
        csw2.Write("\n");
        for (int i = 0; i < lineCount; i++)
        {
            csw2.Write("\t\t\t\t{");
            for (int j = 0; j < rowCount; j++)
            {
                csw2.Write(curMapVo.block[i, j].ToString());
                if (j != rowCount - 1) csw2.Write(",");
            }
            if (i == lineCount - 1) csw2.Write("}");
            else csw2.Write("},");
            csw2.Write("\n");
        }
        csw2.WriteLine("\t\t\t");
        csw2.WriteLine("\t\t\t};");
        csw2.WriteLine("\t\t}");
        csw2.WriteLine("\t}");
        csw2.WriteLine("}");

        csw2.Flush();
        csw2.Close();
        csw2.Dispose();
        Debug.Log(canCPath2 + "Map" + curMapId + "Config.cs file  export complete !");
    }

    public void ExportMapLuaData()
    {

    }

    public void ExportMapJavaData()
    {

    }

}
