//=====================================================
// - FileName:      UIClassHelper.cs
// - Created:       mahuibao
// - UserName:      2019-01-19
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
using Zero.ZeroEngine.Data;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.UI
{
    public class UIClassHelper : Singleton<UIClassHelper>
    {

        /// <summary>
        /// UI类字典
        /// </summary>
        public List<string> UIClassList;
        //model字典
        private List<InterfaceModel> ModelDic;
        //control字典
        private List<InterfaceControl> ControlDic;

        public void Init()
        {
            ZLogger.Info("UI管理层辅助层初始化");
            UIClassList = new List<string>();
            ModelDic = new List<InterfaceModel>();
            ControlDic = new List<InterfaceControl>();

            EventMgr.Instance.AddEventListener(DataConst.DATA_LOAD_COMPLETE, _InitUIClassList);
        }
        public void AfterInit()
        {

        }
        private void _InitUIClassList()
        {
            var iter = DataMgr.Instance.tableUI.DataList.GetEnumerator();
            while (iter.MoveNext())
            {
                UIClassList.Add(iter.Current.name);
            }
        }

        private void _AddModelDic(InterfaceModel addModelObj)
        {
            ModelDic.Add(addModelObj);
        }
        public void InitModelDic()
        {
            if (null != ModelDic)
            {
                var ge = ModelDic.GetEnumerator();
                while (ge.MoveNext())
                {
                    ge.Current.Init();
                }
                ge.Dispose();
            }
        }
        public void ClearModelDic()
        {
            if (null != ModelDic)
            {
                var ge = ModelDic.GetEnumerator();
                while (ge.MoveNext())
                {
                    ge.Current.Clear();
                }
                ge.Dispose();
            }
        }
        private void _AddControlDic(InterfaceControl addControlObj)
        {
            ControlDic.Add(addControlObj);
        }
        public void InitControlDic()
        {
            if (null != ControlDic)
            {
                var ge = ControlDic.GetEnumerator();
                while (ge.MoveNext())
                {
                    ge.Current.Init();
                }
                ge.Dispose();
            }
        }
        public void ClearControlDic()
        {
            if (null != ControlDic)
            {
                var ge = ControlDic.GetEnumerator();
                while (ge.MoveNext())
                {
                    ge.Current.Clear();
                }
                ge.Dispose();
            }
        }

        //public InterfaceView makeViewClass<T>(string className)
        //{
            
        //    T super = new t;
        //    UIClassDic.Add(className, super);
        //    return super;
        //}
    }
}