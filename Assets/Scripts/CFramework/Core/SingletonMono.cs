//=====================================================
// - FileName:      SingletonMono.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   MOMO单例
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.Core
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(obj);
                    instance = obj.GetOrCreatComPonent<T>();
                }
                return instance;
            }
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void Dispose()
        {

        }
    }
}
