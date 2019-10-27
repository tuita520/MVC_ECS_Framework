//=====================================================
// - FileName:      BaseCoroutineHelper.cs
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
using Zero.Plugins.Base;
using Zero.ZeroEngine.Core;

namespace Zero.ZeroEngine.Util
{
    public class CoroutineMgr : SingletonMono<CoroutineMgr>
    {
        sealed class CoroutineCore : MonoBehaviour
        {
            void OnDestroy()
            {
                StopAllCoroutines();
            }
        }

        //private static string COROUTINEHELPER_GONAME = "CoroutineHelper";
        private GameObject _coroutineHelperObj = null;
        private static string DEFAULT_COROUTINEHELPER = "MainCoroutine";
        private Dictionary<string, CoroutineCore> _cores = new Dictionary<string, CoroutineCore>();

        public void Init(GameObject canGo)
        {
            ZLogger.Info("协程管理层初始化");
            _coroutineHelperObj = canGo;
        }

        private CoroutineMgr()
        {
            //_coroutineHelperObj = new GameObject(COROUTINEHELPER_GONAME);
            //UnityEngine.Object.DontDestroyOnLoad(_coroutineHelperObj);
        }

        public Coroutine StartCoroutine(string coreName, IEnumerator routine)
        {
            CoroutineCore curCore = GetOrCreateCoroutineCore(coreName);
            return curCore.StartCoroutine(routine);
        }
        public void StopCoroutine(string coreName, Coroutine routine)
        {
            CoroutineCore curCore = GetCoroutineCore(coreName);
            if (curCore != null)
            {
                curCore.StopCoroutine(routine);
            }
            else
            {
                BaseLogger.Error("you stopCoroutine with error coreName:{0}", coreName);
            }
        }

        public void StopCoroutine(string coreName, IEnumerator routine)
        {
            CoroutineCore curCore = GetCoroutineCore(coreName);
            if (curCore != null)
            {
                curCore.StopCoroutine(routine);
                
            }
            else
            {
                BaseLogger.Error("you stopCoroutine with error coreName:{0}", coreName);
            }
        }

        public void StopALLCoroutines(string coreName)
        {
            CoroutineCore curCore = GetCoroutineCore(coreName);
            if (curCore != null)
            {
                curCore.StopAllCoroutines();
            }
            else
            {
                BaseLogger.Error("Not have this coreName:{0}", coreName);
            }
        }

        public new void StopAllCoroutines()
        {
            var iter = _cores.GetEnumerator();
            while (iter.MoveNext())
            {
                iter.Current.Value.StopAllCoroutines();
            }
        }

        public new Coroutine StartCoroutine(IEnumerator routine)
        {
            return StartCoroutine(DEFAULT_COROUTINEHELPER, routine);
        }

        public new void StopCoroutine(Coroutine routine)
        {
            StopCoroutine(DEFAULT_COROUTINEHELPER, routine);
        }

        public new void StopCoroutine(IEnumerator routine)
        {
            StopCoroutine(DEFAULT_COROUTINEHELPER, routine);
        }

        public void DestroyCoroutineCore(string coreName)
        {
            if (_cores.ContainsKey(coreName))
            {
                CoroutineCore curCore = _cores[coreName];
                curCore.StopAllCoroutines();
                GameObject.DestroyImmediate(curCore.gameObject);
            }
        }

        private CoroutineCore GetCoroutineCore(string coreName)
        {
            CoroutineCore curCC = null;
            _cores.TryGetValue(coreName, out curCC);
            return curCC;
        }

        private CoroutineCore CreateCoroutineCore(string coreName)
        {
            GameObject curCoreObj = new GameObject(coreName);
            curCoreObj.transform.parent = _coroutineHelperObj.transform;
#if UNITY_5 || UNITY_2017

#else
    UnityEngine.Object.DontDestroyOnLoad(curCoreObj);
#endif
            CoroutineCore curCore = curCoreObj.AddComponent<CoroutineCore>();
            _cores.Add(coreName, curCore);
            return curCore;
        }

        private CoroutineCore GetOrCreateCoroutineCore(string coreName)
        {
            CoroutineCore curCore = GetCoroutineCore(coreName);
            if (curCore != null)
            { }
            else
            {
                curCore = CreateCoroutineCore(coreName);
            }
            return curCore;
        }
    }
}

