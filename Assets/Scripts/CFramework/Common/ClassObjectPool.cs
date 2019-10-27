//=====================================================
// - FileName:      ClassObjectPool.cs
// - Created:       mahuibao
// - UserName:      2019-06-12
// - Email:         1023276156@qq.com
// - Description:   对象池
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero.ZeroEngine.Common
{
    public class ClassObjectPool<T> where T : class, new()
    {
        //池
        private readonly Stack<T> m_Pool = new Stack<T>();
        //最大对象个数 <=0表示不限个数
        public int m_MaxCount = 0;
        //没有回收的对象个数
        public int m_NoRecycleCount = 0;
        //池剩余个数
        public int m_NoPopCount { get { return m_Pool.Count; } }

        public ClassObjectPool(int maxCount)
        {
            m_MaxCount = maxCount;
            for (int i = 0; i < maxCount; i++)
            {
                m_Pool.Push(new T());
            }
        }

        /// <summary>
        /// 从池里面取类对象
        /// </summary>
        /// <param name="createIfPoolEmpty">如果为空是否new一个</param>
        /// <returns></returns>
        public T Spawn(bool createIfPoolEmpty)
        {
            if (m_Pool.Count > 0)
            {
                T rtn = m_Pool.Pop();
                if(rtn == null)
                {
                    if(createIfPoolEmpty)
                    {
                        rtn = new T();
                    }
                }
                m_NoRecycleCount++;
                return rtn;
            }
            else
            {
                if(createIfPoolEmpty)
                {
                    T rtn = new T();
                    m_NoRecycleCount++;
                    return rtn;
                }
            }
            return null;
        }
        /// <summary>
        /// 回收类对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Recycle(T obj)
        {
            if (obj == null)
                return false;

            m_NoRecycleCount--;

            if(m_Pool.Count >= m_MaxCount && m_MaxCount> 0)
            {
                obj = null;
                return false;
            }

            m_Pool.Push(obj);
            return true;
        }
    }
}

