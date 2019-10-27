//=====================================================
// - FileName:      UpdataConst.cs
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

namespace Zero.ZeroEngine.Common
{
    public class UpdataConst
    {
        // 加载显示进度UI （数据不是真实的）
        // 当前正在下载的"文件名" (随机进度值，如果还是当前文件名，再加上一个随机值作为进度)
        public const string LOADER_PROGRESS = "LOADER_PROGRESS";
        // 完成"文件名" 
        public const string LOADER_COMPLETED = "LOADER_COMPLETED";
        // 全部加载完成
        public const string LOADER_ALL_COMPLETED = "LOADER_ALL_COMPLETED";
    }
}