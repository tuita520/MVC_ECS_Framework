//=====================================================
// - FileName:      DataMgr.cs
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
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Util;

//=====================================================
// - 1.
// - 2.
// - 3.
// - 4.
// - 5.
// - 6.
//======================================================

namespace Zero.ZeroEngine.Data
{
    public class DataConst
    {
        public const string DATA_LOAD_COMPLETE = "DATA_LOAD_COMPLETE";
    }
    public class DataMgr : Singleton<DataMgr>
    {
        public const int DataCount = 1;
        public const string DataPath = "Assets/Resources/Data/";

        //------------------与数据表一一对应关系
        public DataTableMap tableMap;
        public DataTableModel tableModel;
        public DataTableMonster tableMonster;
        public DataTableMonsterRefresh tableMonsterRefresh;
        public DataTableNpc tableNpc;
        public DataTableTransfer tableTransfer;
        public DataTableUi tableUI;

        private Dictionary<string, DataBase> m_DataDic = new Dictionary<string, DataBase>();


        public void Init()
        {
            ZLogger.Info("数据管理层初始化");
            m_DataDic.Clear();

            EventMgr.Instance.AddEventListener(ABMgrConst.AB_CONFIG_LOAD_COMPLETE, ExtractData);
        }
        public void Clear()
        {
            EventMgr.Instance.RemoveEventListener(ABMgrConst.AB_CONFIG_LOAD_COMPLETE, ExtractData);
        }
        public void AfterInit()
        {

        }
        public void ExtractData()
        {
            EventMgr.Instance.TriggerEvent(GlobalEvent.DATA_LOAD_PROGRESS, "数据资源初始化...开始...", 3, 0);

            tableMap = ResourcesMgr.Instance.LoadResource<DataTableMap>(DataPath + "DataTableMap.asset");
            tableMap.Init();
            m_DataDic.Add("DataTableMap", tableMap);

            tableModel = ResourcesMgr.Instance.LoadResource<DataTableModel>(DataPath + "DataTableModel.asset");
            tableModel.Init();
            m_DataDic.Add("DataTableModel", tableModel);

            tableMonster = ResourcesMgr.Instance.LoadResource<DataTableMonster>(DataPath + "DataTableMonster.asset");
            tableMonster.Init();
            m_DataDic.Add("DataTableMonster", tableMonster);

            tableMonsterRefresh = ResourcesMgr.Instance.LoadResource<DataTableMonsterRefresh>(DataPath + "DataTableMonsterRefresh.asset");
            tableMonsterRefresh.Init();
            m_DataDic.Add("DataTableMonsterRefresh", tableMonsterRefresh);

            tableNpc = ResourcesMgr.Instance.LoadResource<DataTableNpc>(DataPath + "DataTableNpc.asset");
            tableNpc.Init();
            m_DataDic.Add("DataTableNpc", tableNpc);

            tableTransfer = ResourcesMgr.Instance.LoadResource<DataTableTransfer>(DataPath + "DataTableTransfer.asset");
            tableTransfer.Init();
            m_DataDic.Add("DataTableTransfer", tableTransfer);

            EventMgr.Instance.TriggerEvent(GlobalEvent.DATA_LOAD_PROGRESS, "数据资源初始化中...", 3, 30);



            EventMgr.Instance.TriggerEvent(GlobalEvent.DATA_LOAD_PROGRESS, "数据资源初始化中...", 3, 60);



            tableUI = ResourcesMgr.Instance.LoadResource<DataTableUi>(DataPath + "DataTableUi.asset");
            tableUI.Init();
            m_DataDic.Add("DataTableUi", tableUI);

            EventMgr.Instance.TriggerEvent(GlobalEvent.DATA_LOAD_PROGRESS, "数据资源初始化中...", 3, 90);


            EventMgr.Instance.TriggerEvent(DataConst.DATA_LOAD_COMPLETE);
            EventMgr.Instance.TriggerEvent(GlobalEvent.DATA_LOAD_PROGRESS, "数据资源初始化完成!!!", 3, 100);
            
        }

        public void ClearCache()
        {
            List<string> tempList = new List<string>();
            foreach(string tempStr in m_DataDic.Keys)
            {
                m_DataDic[tempStr].Clear();
                tempList.Add(tempStr);
            }
            foreach(string tempStr in tempList)
            {
                m_DataDic.Remove(tempStr);
                ResourcesMgr.Instance.ReleaseResource(DataPath + tempStr, true);
            }
            m_DataDic.Clear();
            tempList.Clear();
        }
    }
}
