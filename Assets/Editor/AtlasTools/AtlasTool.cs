//=====================================================
// - FileName:      AtlasTool.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AtlasTool {
    //图集存放目录
    public static readonly string ATLAS_FOLDER = "Assets/Resources/UI/Atlas";
    public static readonly string THEMEPARK_ATLAS_FOLDER = "Assets/Resources/UI/ThemePark";

    public static readonly string PREFAB_FOLDER = "Assets/Resources/UI/AtlasPrefabs";
    public static readonly string THEMEPARK_PREFAB_FOLDER = "Assets/Resources/UI/ThemePark";

    public static void updateAtlas()
    {
        Debug.Log("开始更新图集");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        List<string> atlas_folder = new List<string>();
        List<string> prefab_folder = new List<string>();
        List<string> pre_str = new List<string>();

        DirectoryInfo atlasDI = new DirectoryInfo(Environment.CurrentDirectory + "/" + ATLAS_FOLDER);
        if (atlasDI.Exists)
        {
            atlas_folder.Add(ATLAS_FOLDER);
            prefab_folder.Add(PREFAB_FOLDER);
            pre_str.Add("");
        }

        //检查出异常的列表
        List<string> exceptionList = new List<string>();

        // packingTag对应的图片文件列表
        Dictionary<string, List<string>> packingTagFileList = new Dictionary<string, List<string>>();

        for (int index = 0, n = atlas_folder.Count; index < n; index++)
        {
            packingTagFileList.Clear();

            DirectoryInfo direction = new DirectoryInfo(atlas_folder[index]);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            List<string> tempDI = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                tempDI.Add(files[i].Name);
            }
            foreach (string pngFile in tempDI)
            {
                if (pngFile.EndsWith(".png") == false)
                {
                    exceptionList.Add("文件扩展名异常，atlas目录下目前只允许放.png文件 " + pngFile);
                }

                //Debug.Log(pngFile);

                string pngRelativeFile = null;
                string packingTag = null;
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.Equals(pngFile))
                    {

                        pngRelativeFile = files[i].DirectoryName.Substring(files[i].DirectoryName.IndexOf("Assets")).Replace("\\", "/") + "/" + pngFile;
                        try
                        {
                            //以图集存放目录为根目录来定义packingTag,文件夹用_代替，避免有重名的情况
                            packingTag = string.Concat(pre_str[index], FileTool.getParentFolder(pngRelativeFile).Substring(atlas_folder[index].Length + 1).Replace("/", "_"));
                        }
                        catch
                        {
                            //出现异常有可能图标直接放在了Atlas目录，而不是子目录
                            exceptionList.Add("图片存放目录异常 " + pngFile);
                        }
                    }
                }

                if (packingTagFileList.ContainsKey(packingTag) == false)
                {
                    packingTagFileList[packingTag] = new List<string>();
                }
                packingTagFileList[packingTag].Add(pngFile);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (exceptionList.Count > 0)
            {
                foreach (string msg in exceptionList)
                {
                    Debug.LogError(msg);
                }
                throw new System.Exception("刷新图集种植，具体原因看日志输出");
            }

            //开始对Atlas中的图片生成prefab
            string listPropertyName = "list";//写进prefab中的列表名称，这个取决于现在用的是SpriteList中的属性名称
            List<string> prefabFileList = new List<string>();
            foreach (string tempPackingTag in packingTagFileList.Keys)
            {
                if (tempPackingTag.Contains("Atlas_"))
                {
                    List<string> pngList = packingTagFileList[tempPackingTag];
                    string prefabRelativeFile = prefab_folder[index] + "/" + tempPackingTag.Substring(tempPackingTag.IndexOf("|") + 1) + ".prefab";

                    Debug.Log("更新图标Prefab " + prefabRelativeFile);

                    //判断并创建prefab
                    if (AssetDatabase.LoadAssetAtPath(prefabRelativeFile, typeof(GameObject)) == false)
                    {
                        GameObject prefabGo = new GameObject(tempPackingTag);
                        prefabGo.AddComponent<AssetList>();
                        PrefabUtility.ReplacePrefab(prefabGo, PrefabUtility.CreateEmptyPrefab(prefabRelativeFile));
                        UnityEngine.Object.DestroyImmediate(prefabGo);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    //文本形式修改prefab （因为暂时没有找到更新prefab的方法，另外这样做可以保证meta的GUID没有变化）
                    List<string> prefabContentList = new List<string>();
                    prefabContentList.Add("  " + listPropertyName + ":");
                    pngList.Sort();
                    foreach(string pngFile in pngList)
                    {
                        string tempPathStr = null;
                        for (int i = 0; i < files.Length; i++)
                        {
                            if (files[i].Name.Equals(pngFile))
                            {

                                tempPathStr = files[i].DirectoryName.Substring(files[i].DirectoryName.IndexOf("Assets")).Replace("\\", "/") + "/" + pngFile + ".meta";
                            }
                        }
                        prefabContentList.Add("  - {fileID: 2800000, guid: " + FileTool.getGuidFromMetaFile(tempPathStr) + ", type: 3}");
                    }

                    string prefabContent = FileTool.readFile(prefabRelativeFile);
                    string prefabOldContent = prefabContent;
                    int startIndex = prefabContent.IndexOf("m_EditorClassIdentifier");
                    startIndex = prefabContent.IndexOf("\n", startIndex);
                    if(prefabContentList.Count == 1)
                    {
                        prefabContentList[0] = "  " + listPropertyName + ": []";
                    }
                    else
                    {
                        //将prefab中的list的第一个保持与第二个元素一样，打包前会将第一个元素换成对应的材质球
                        prefabContentList.Insert(1, prefabContentList[1]);
                    }

                    prefabContent = prefabContent.Substring(0, startIndex) + "\n" + string.Join("\n", prefabContentList.ToArray()) + "\n";

                    if (prefabContent.CompareTo(prefabOldContent) != 0 && File.Exists(prefabRelativeFile))
                    {
                        AssetDatabase.DeleteAsset(prefabRelativeFile);
                    }

                    FileTool.writeFile(prefabRelativeFile,prefabContent);
                    prefabFileList.Add(prefabRelativeFile);
                }
            }

            //删除多余的图集prefab文件（有可能是以前有，现在删了对应的小图）
            DirectoryInfo directionPrefab = new DirectoryInfo(prefab_folder[index]);
            FileInfo[] filesPrefab = directionPrefab.GetFiles("*", SearchOption.AllDirectories);

            //List<string> tempDIPrefab = new List<string>();
            //for (int i = 0; i < files.Length; i++)
            //{
            //    if (files[i].Name.EndsWith(".meta"))
            //    {
            //        continue;
            //    }
            //    tempDI.Add(files[i].Name);
            //}
            foreach (FileInfo prefabFile in filesPrefab)
            {
                if (prefabFile.Name.EndsWith(".meta"))
                {
                    continue;
                }
                string tempPathDelete = prefabFile.DirectoryName.Substring(prefabFile.DirectoryName.IndexOf("Assets")).Replace("\\", "/") + "/" + prefabFile.Name;
                if (prefabFileList.Contains(tempPathDelete) == false)
                {
                    Debug.Log("删除不需要的图集prefab文件 " + tempPathDelete);
                    
                    FileTool.deletaFileOrFolder(tempPathDelete);
                    FileTool.deletaFileOrFolder(tempPathDelete + ".meta");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
