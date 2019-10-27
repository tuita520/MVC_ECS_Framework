//=====================================================
// - FileName:      AssetBundleConfig.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   AB包配置
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace Zero.ZeroEngine.Common
{
    [System.Serializable]
    public class AssetBundleConfig
    {

        [XmlElement("ABList")]
        public List<ABBase> ABList { get; set; }
    }
    [System.Serializable]
    public class ABBase
    {
        [XmlAttribute("Path")]
        public string Path { get; set; }
        [XmlAttribute("Crc")]
        public uint Crc { get; set; }
        [XmlAttribute("ABName")]
        public string ABName { get; set; }
        [XmlAttribute("AssetName")]
        public string AssetName { get; set; }
        [XmlAttribute("ABDependce")]
        public List<string> ABDependce { get; set; }
    }
}
