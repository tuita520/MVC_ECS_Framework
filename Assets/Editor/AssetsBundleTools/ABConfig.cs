//=====================================================
// - FileName:      ABConfig.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   AB包设置
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ABConfig",menuName ="CreateABConfig",order = 0)]
public class ABConfig : ScriptableObject {

    //数据asset文件夹（打包单个）
    public List<string> m_DataPath = new List<string>();

    //-----------界面层
    //图标图集文件夹（有一个common）
    public List<FileDirABName> m_UIIconFloderPath = new List<FileDirABName>();
    //界面图集文件夹（有一个common）
    public List<FileDirABName> m_UIModuleFloderPath = new List<FileDirABName>();
    //界面prefab文件夹（打包单个）
    public List<string> m_UIPrefabPath = new List<string>();
    //大图片文件夹（打包单个）
    public List<string> m_UITexturePath = new List<string>();

    //-----------场景层
    //场景Obj文件夹（有一个common）
    public List<FileDirABName> m_SceneFloderPath = new List<FileDirABName>();
    //场景scene文件夹（打包单个）
    public List<string> m_SceneUnityPath = new List<string>();

    //-----------角色层
    //角色Obj文件夹
    public List<FileDirABName> m_RoleFloderPath = new List<FileDirABName>();
    //角色prefab文件夹（打包单个）
    public List<string> m_RolePrefabPath = new List<string>();


    //单个文件所在文件夹路径，会遍历这个文件夹下面所有Prefab，所有Prefab的名字不能重复，必须保证名字的唯一性
    public List<string> m_AllPrefabPath = new List<string>();
    
    public List<FileDirABName> m_AllFileDirAB = new List<FileDirABName>();

    [System.Serializable]
    public struct FileDirABName
    {
        public string ABName;
        public string Path;
    }
}
