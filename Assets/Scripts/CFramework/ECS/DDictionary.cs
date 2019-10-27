//=====================================================
// - FileName:      PackageDic.cs
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

namespace Zero.ZeroEngine.ECS
{
    /// <summary>
    /// 封装好的DIC，提供延迟增删功能
    /// </summary>
    public class DDictionary<K,V> : Dictionary<K, V> where V:BaseECSItem
    {
        bool m_RemoveBoo = false;
        bool m_AddBoo = false;
        Dictionary<K, V> delayAddDic = new Dictionary<K, V>();

        public void DelayAdd(K canKey,V canValue)
        {
            delayAddDic.Add(canKey, canValue);
            m_AddBoo = true;
        }

        public void DelayRemove(V canValue)
        {
            canValue.destroyed = true;
            m_RemoveBoo = true;
        }

        public void DelayRemove(K canKey)
        {
            this[canKey].destroyed = true;
            m_RemoveBoo = true;
        }

        public void ApplyDelayCommands()
        {
            if (m_RemoveBoo)
            {
                List<K> tempList = new List<K>();
                foreach(K tempKey in this.Keys)
                {
                    if (this[tempKey].destroyed)
                    {
                        tempList.Add(tempKey);
                    }
                }
                foreach(K tempKey in tempList)
                {
                    this.Remove(tempKey);
                }
                tempList.Clear();
                m_RemoveBoo = false;
            }

            if (m_AddBoo)
            {
                foreach(K tempKey in delayAddDic.Keys)
                {
                    this.Add(tempKey, delayAddDic[tempKey]);
                }
                delayAddDic.Clear();
                m_AddBoo = false;
            }
        }
    }
}