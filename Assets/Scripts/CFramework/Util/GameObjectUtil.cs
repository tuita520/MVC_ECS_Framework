//=====================================================
// - FileName:      GameObjectUtil.cs
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


namespace Zero.ZeroEngine.Util
{
    /// <summary>
    /// GameObject扩展类：获取某个组件，无则添加
    /// </summary>
    public static class GameObjectUtil
    {
        public static T GetOrCreatComPonent<T>(this GameObject obj) where T : MonoBehaviour
        {
            T t = obj.GetComponent<T>();
            if (t == null)
                t = obj.AddComponent<T>();
            return t;
        }
    }
}

