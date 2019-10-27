//=====================================================
// - FileName:      Singleton.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   普通单例
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zero.ZeroEngine.Core
{
    public class Singleton<T> : IDisposable where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }

        public virtual void Dispose()
        {

        }
    }
}

