//=====================================================
// - FileName:      ABBuildTools.cs
// - Created:       mahuibao
// - UserName:      2019-06-12
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Util;

//先打包文件夹，再打包预置，是有判断包含关系的
//


public class ABBuildTools {

    public static string m_BundleTargetPath = Application.dataPath + "/../AssetBundle/" +
        (EditorUserBuildSettings.activeBuildTarget.ToString().Contains("windows") ? "windows" : EditorUserBuildSettings.activeBuildTarget.ToString().ToLower());
    //配置好的AB包配置表路径
    private static string ABCONFIGPATH = "Assets/Editor/AssetsBundleTools/ABConfig.asset";

    //单个Prefab的ab包（前期为空，需要先找到预制，然后找到它所有的依赖，添加进去）
    private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();

    //key是ab包名，value是路径，所有文件夹ab包dic
    private static Dictionary<string, string> m_AllFileDic = new Dictionary<string, string>();

    //过滤的List（用于过滤已经添加好的floder路径，以及prefab路径，以及prefab依赖的资源的路径）
    private static List<string> m_AllFileAB = new List<string>();
    
    //存储所有有效的路径（用于在写入xml、二进制文件时候，筛选出需要的floder路径或者prefab路径，其余不要）
    private static List<string> m_ConfigFil = new List<string>();

    // 应该程序版本号(c#代码或dll或者scene改动（不包含edtior下的），相应改动此处序号)
    // 这个打包时会被打入为 v1.v2.v3.v4 中的v1 表示游戏里要安装最新版本
    private static string appVersion = "1";

    //用于遍历打包出来的所有AB包，然后写入版本控制中
    private static List<string> tempFiles = new List<string>();

    public static void Build()
    {
        m_AllFileDic.Clear();
        m_AllFileAB.Clear();
        m_AllPrefabDir.Clear();
        m_ConfigFil.Clear();
        ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);

        //获取 数据asset文件夹（打包单个）
        string[] allStrData = AssetDatabase.FindAssets("DataTable l", abConfig.m_DataPath.ToArray());
        for (int i = 0; i < allStrData.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allStrData[i]);
            string[] p1 = path.Split(new string[] { "/" }, StringSplitOptions.None);
            string tempName1 = p1[p1.Length - 1];
            string[] p2 = tempName1.Split(new string[] { "." }, StringSplitOptions.None);
            string tempName2 = p2[0];
            EditorUtility.DisplayProgressBar("查找Data_Asset", "Data_Asset:" + path, i * 1.0f / allStrData.Length);
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                List<string> allDependPath = new List<string>();
                allDependPath.Add(path);
                if (m_AllPrefabDir.ContainsKey(tempName2.ToLower()))
                {
                    Debug.LogError("存在相同名字的Data_Asset！ 名字：" + tempName2);
                }
                m_AllPrefabDir.Add("Data/" + tempName2, allDependPath);
            }
        }

        //获取 图标图集文件夹（有一个common）
        foreach (ABConfig.FileDirABName fileDir in abConfig.m_UIIconFloderPath)
        {
            if (m_AllFileDic.ContainsKey(fileDir.ABName))
            {
                Debug.LogError("图标图集文件夹   AB包配置名字重复，请检查");
            }
            else
            {
                m_AllFileDic.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }

        //获取 界面图集文件夹（有一个common）
        foreach (ABConfig.FileDirABName fileDir in abConfig.m_UIModuleFloderPath)
        {
            if (m_AllFileDic.ContainsKey(fileDir.ABName))
            {
                Debug.LogError("界面图集文件夹   AB包配置名字重复，请检查");
            }
            else
            {
                m_AllFileDic.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }

        //获取 界面prefab文件夹（打包单个）
        string[] allStrUIPrefab = AssetDatabase.FindAssets("t:Prefab", abConfig.m_UIPrefabPath.ToArray());
        for (int i = 0; i < allStrUIPrefab.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allStrUIPrefab[i]);
            EditorUtility.DisplayProgressBar("查找UI Prefab", "UI Prefab:" + path, i * 1.0f / allStrUIPrefab.Length);
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] allDepend = AssetDatabase.GetDependencies(path);
                List<string> allDependPath = new List<string>();
                allDependPath.Add(path);
                for (int j = 0; j < allDepend.Length; j++)
                {
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                    {
                        m_AllFileAB.Add(allDepend[j]);
                        allDependPath.Add(allDepend[j]);
                    }
                }
                if (m_AllPrefabDir.ContainsKey(obj.name))
                {
                    Debug.LogError("存在相同名字的UI Prefab！ 名字：" + obj.name);
                }
                m_AllPrefabDir.Add("ui/module/" + obj.name, allDependPath);
            }
        }

        //获取 大图片文件夹（打包单个）
        string[] allStrUITexture = AssetDatabase.FindAssets("BigImg l", abConfig.m_UITexturePath.ToArray());
        for (int i = 0; i < allStrUITexture.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allStrUITexture[i]);
            string[] p1 = path.Split(new string[] { "/" }, StringSplitOptions.None);
            string tempName1 = p1[p1.Length - 1];
            string[] p2 = tempName1.Split(new string[] { "." }, StringSplitOptions.None);
            string tempName2 = p2[0];
            EditorUtility.DisplayProgressBar("查找Texture", "Texture:" + path, i * 1.0f / allStrUITexture.Length);
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                List<string> allDependPath = new List<string>();
                allDependPath.Add(path);
                if (m_AllPrefabDir.ContainsKey(tempName2.ToLower()))
                {
                    Debug.LogError("存在相同名字的Texture！ 名字：" + tempName2);
                }
                m_AllPrefabDir.Add("ui/texture/" + tempName2, allDependPath);
            }
        }

        //获取 场景Obj文件夹（有一个common）
        foreach (ABConfig.FileDirABName fileDir in abConfig.m_SceneFloderPath)
        {
            if (m_AllFileDic.ContainsKey(fileDir.ABName))
            {
                Debug.LogError("场景Obj文件夹   AB包配置名字重复，请检查");
            }
            else
            {
                m_AllFileDic.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }

        //获取 场景scene文件夹（打包单个） no edit
        string[] allStrScene = AssetDatabase.FindAssets("t:unity", abConfig.m_SceneUnityPath.ToArray());
        for (int i = 0; i < allStrScene.Length; i++)
        {
            //string path = AssetDatabase.GUIDToAssetPath(allStrScene[i]);
            //string[] p1 = path.Split(new string[] { "/" }, StringSplitOptions.None);
            //string tempName1 = p1[p1.Length - 1];
            //string[] p2 = tempName1.Split(new string[] { "." }, StringSplitOptions.None);
            //string tempName2 = p2[0];
            //EditorUtility.DisplayProgressBar("查找Texture", "Texture:" + path, i * 1.0f / allStrScene.Length);
            //m_ConfigFil.Add(path);
            //if (!ContainAllFileAB(path))
            //{
            //    List<string> allDependPath = new List<string>();
            //    allDependPath.Add(path);
            //    if (m_AllPrefabDir.ContainsKey(tempName2.ToLower()))
            //    {
            //        Debug.LogError("存在相同名字的Texture！ 名字：" + tempName2);
            //    }
            //    m_AllPrefabDir.Add("ui/texture/" + tempName2, allDependPath);
            //}
        }

        //获取 角色Obj文件夹
        foreach (ABConfig.FileDirABName fileDir in abConfig.m_RoleFloderPath)
        {
            if (m_AllFileDic.ContainsKey(fileDir.ABName))
            {
                Debug.LogError("角色Obj文件夹   AB包配置名字重复，请检查");
            }
            else
            {
                m_AllFileDic.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }

        //获取 角色prefab文件夹（打包单个） no edit
        string[] allStrRolePrefab = AssetDatabase.FindAssets("t:Prefab", abConfig.m_RolePrefabPath.ToArray());
        for (int i = 0; i < allStrRolePrefab.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allStrRolePrefab[i]);
            EditorUtility.DisplayProgressBar("查找Role Prefab", "Role Prefab:" + path, i * 1.0f / allStrRolePrefab.Length);
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] allDepend = AssetDatabase.GetDependencies(path);
                List<string> allDependPath = new List<string>();
                allDependPath.Add(path);
                for (int j = 0; j < allDepend.Length; j++)
                {
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                    {
                        m_AllFileAB.Add(allDepend[j]);
                        allDependPath.Add(allDepend[j]);
                    }
                }
                if (m_AllPrefabDir.ContainsKey(obj.name))
                {
                    Debug.LogError("存在相同名字的Role Prefab！ 名字：" + obj.name);
                }
                m_AllPrefabDir.Add("ui/module/" + obj.name, allDependPath);
            }
        }

        foreach (ABConfig.FileDirABName fileDir in abConfig.m_AllFileDirAB)
        {
            if (m_AllFileDic.ContainsKey(fileDir.ABName))
            {
                Debug.LogError("AB包配置名字重复，请检查");
            }
            else
            {
                m_AllFileDic.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }

        string[] allStr = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray());
        for (int i=0;i< allStr.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayProgressBar("查找Prefab", "Prefab:" + path, i * 1.0f / allStr.Length);
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] allDepend = AssetDatabase.GetDependencies(path);
                List<string> allDependPath = new List<string>();
                allDependPath.Add(path);
                for(int j = 0; j < allDepend.Length; j++)
                {
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                    {
                        m_AllFileAB.Add(allDepend[j]);
                        allDependPath.Add(allDepend[j]);
                    }
                }
                if (m_AllPrefabDir.ContainsKey(obj.name))
                {
                    Debug.LogError("存在相同名字的Prefab！ 名字：" + obj.name);
                }
                m_AllPrefabDir.Add(obj.name, allDependPath);
            }
        }
        foreach(string name in m_AllFileDic.Keys)
        {
            SetABName(name, m_AllFileDic[name]);
        }
        foreach(string name in m_AllPrefabDir.Keys)
        {
            SetABName(name, m_AllPrefabDir[name]);
        }

        BuildAssetBundle();

        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < oldABNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
            EditorUtility.DisplayProgressBar("清除AB包名", "名字:" + oldABNames[i], i * 1.0f / oldABNames.Length);
        }

        AssetDatabase.Refresh();

        WriteFilesTxt();

        EditorUtility.ClearProgressBar();
    }

    static void SetABName(string name,string path)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if(assetImporter == null)
        {
            Debug.LogError("不存在此路径文件：" + path);
        }
        else
        {
            assetImporter.assetBundleName = name;
        }
    }
    static void SetABName(string name,List<string>paths)
    {
        for(int i =0;i<paths.Count;i++)
        {
            SetABName(name, paths[i]);
        }
    }

    static void BuildAssetBundle()
    {
        string[] allBundles = AssetDatabase.GetAllAssetBundleNames();
        //key为全路径，value为包名
        Dictionary<string,string> resPathDic = new Dictionary<string, string>();
        for(int i=0; i<allBundles.Length;i++)
        {
            string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundles[i]);
            for (int j = 0; j < allBundlePath.Length; j++)
            {
                if (allBundlePath[j].EndsWith(".cs"))
                    continue;
                Debug.Log("此AB包：" + allBundles[i] + "下面包含的资源文件路径：" + allBundlePath[j]);
                if (!resPathDic.ContainsKey(allBundlePath[j]))
                {
                    resPathDic.Add(allBundlePath[j], allBundles[i]);
                }
            }
        }
        if (!Directory.Exists(m_BundleTargetPath))
        {
            Directory.CreateDirectory(m_BundleTargetPath);
        }

        DeleteAB();
        //生成自己的配置表
        WriteData(resPathDic);

        BuildPipeline.BuildAssetBundles(m_BundleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression,
            EditorUserBuildSettings.activeBuildTarget);
    }

    static void WriteFilesTxt()
    {
        string resPath = m_BundleTargetPath;
        string newFilePath = resPath + "/files.txt";//创建版本文件列表
        tempFiles.Clear();
        if (File.Exists(newFilePath)) File.Delete(newFilePath);
        tempFiles.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        string luaAB = "";//用于处理 lua.ab 与 lua.ab.manifest 序列成同一个md5
        string hashCode = "";
        for (int i = 0; i < tempFiles.Count; i++)
        {
            string file = tempFiles[i];
            string ext = Path.GetExtension(file);
            if (ext.Equals(".meta") || ext.Equals(".svn") || ext.Equals(".txt")
                || ext.Contains(".DS_Store") || ext.Contains(".exe") || ext.Contains(".bat")) continue;
            string md5 = "";
            if (file.IndexOf("lua/") == -1)
            {
                md5 = UtilTool.md5file(file);
            }
            else
            {
                if (luaAB != "" && file.IndexOf(luaAB) != -1)
                {
                    md5 = hashCode;
                }
                else
                {
                    md5 = UtilTool.md5file(file);
                    luaAB = file;
                    hashCode = md5;
                }
            }
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        //格式： v1.v2.v3.v4 其中 vx代表序号
        //版号表示：v1程序更新(全部)，v2（非UI）资源, v3 UI资源， v4 lua脚本
        sw.WriteLine(appVersion + ".0.0.0");
        sw.Close(); fs.Close();
    }

    // 遍历目录及其子目录
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);// 扩展名
            if (ext.Equals(".meta") || ext.Equals(".svn") || ext.Equals(".txt")
                || ext.Contains(".DS_Store") || ext.Contains(".exe") || ext.Contains(".bat")) continue;
            tempFiles.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            Recursive(dir);
        }
    }

    static void WriteData(Dictionary<string,string> resPathDic)
    {
        AssetBundleConfig config = new AssetBundleConfig();
        config.ABList = new List<ABBase>();
        foreach(string path in resPathDic.Keys)
        {
            if (!ValidPath(path))
                continue;
            ABBase abBase = new ABBase();
            abBase.Path = path;
            abBase.Crc = CRC32.GetCRC32(path);
            abBase.ABName = resPathDic[path];
            abBase.AssetName = path.Remove(0, path.LastIndexOf("/") + 1);
            string[] resDependce = AssetDatabase.GetDependencies(path);
            for (int i = 0;i< resDependce.Length; i++)
            {
                string tempPath = resDependce[i];
                if (tempPath == path || path.EndsWith(".cs"))
                    continue;
                string abName = "";
                if(resPathDic.TryGetValue(tempPath,out abName))
                {
                    if (abName == resPathDic[path])
                        continue;
                    if(!abBase.ABDependce.Contains(abName))
                    {
                        abBase.ABDependce.Add(abName);
                    }
                }
            }
            config.ABList.Add(abBase);
        }

        //写入xml
        string xmlPath = Application.dataPath + "/TempABData/AssetBundleConfig.xml";
        if (File.Exists(xmlPath)) File.Delete(xmlPath);
        FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xs = new XmlSerializer(config.GetType());
        xs.Serialize(sw, config);
        sw.Close();
        fileStream.Close();

        //写入二进制
        foreach(ABBase tempABBase in config.ABList)
        {
            tempABBase.Path = "";
        }
        string bytePath = "Assets/TempABData/AssetBundleConfig.bytes";
        FileStream fs = new FileStream(bytePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        fs.Seek(0, SeekOrigin.Begin);
        fs.SetLength(0);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, config);
        fs.Close();
        AssetDatabase.Refresh();
        SetABName("assetbundleconfig", bytePath);
    }

    /// <summary>
    /// 删除无用的AB包
    /// </summary>
    static void DeleteAB()
    {
        string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo direction = new DirectoryInfo(m_BundleTargetPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        for(int i = 0; i < files.Length; i++)
        {
            if(ContainABName(files[i].Name,allBundlesName) || files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith("manifest")
                || files[i].Name.EndsWith("assetbundleconfig"))
            {
                continue;
            }
            else
            {
                Debug.Log("此AB包已经被删除或者改名了:" + files[i].Name);
                if(File.Exists(files[i].FullName))
                {
                    File.Delete(files[i].FullName);
                }
                if (File.Exists(files[i].FullName + ".manifest"))
                {
                    File.Delete(files[i].FullName + ".manifest");
                }
            }
        }
    }

    /// <summary>
    /// 遍历文件夹里的文件名与设置的所有AB包进行检查判断
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    static bool ContainABName(string name,string[] strs)
    {
        for(int i = 0; i < strs.Length; i++)
        {
            if (name==strs[i])
                return true;
        }
        return false;
    }

    /// <summary>
    /// 是否包含在已经有的AB包里，用来做AB包冗余剔除
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ContainAllFileAB(string path)
    {
        for (int i = 0; i < m_AllFileAB.Count; i++)
        {
            if (path == m_AllFileAB[i] || (path.Contains(m_AllFileAB[i]) && (path.Replace(m_AllFileAB[i],"")[0] == '/')))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 是否有效路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ValidPath(string path)
    {
        for(int i=0;i<m_ConfigFil.Count;i++)
        {
            if (path.Contains(m_ConfigFil[i]))
            {
                return true;
            }
        }
        return false;
    }
}


