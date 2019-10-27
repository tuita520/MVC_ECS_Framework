//=====================================================
// - FileName:      FileTool.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FileTool {

    public static List<string> getAllFilesInFolder(string path)
    {
        DirectoryInfo direction = new DirectoryInfo(path);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        List<string> temp = new List<string>();
        for(int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            temp.Add(files[i].Name);
        }
        return temp;
    }

    public static string getParentFolder(string path)
    {
        string[] tempList = path.Split('/');
        string tempStr = "";
        for(int i = 0, n = tempList.Length - 1,m= tempList.Length - 2; i < n; i++)
        {
            tempStr = string.Concat(tempStr, tempList[i]);
            if (i < m)
            {
                tempStr = string.Concat(tempStr, "/");
            }
        }
        return tempStr;
    }

    public static string getGuidFromMetaFile(string path)
    {
        string tempStr = readFile(path);
        int startIndex = tempStr.IndexOf("guid:");
        startIndex = startIndex + 6;
        int endIndex = tempStr.IndexOf("\n", startIndex);
        string retunStr = tempStr.Substring(startIndex, endIndex - startIndex);
        return retunStr;
    }

    public static string readFile(string path)
    {
        //FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamReader sr = new StreamReader(path);
        string line;
        string tempStr = "";
        while ((line = sr.ReadLine()) != null)
        {
            tempStr = string.Concat(tempStr, line, "\n");
        }
        sr.Close();
        return tempStr;
    }

    public static void writeFile(string path,string contentStr)
    {
        StreamWriter sw = new StreamWriter(path);
        string[] tempStr = contentStr.Split('\n');
        foreach(var each in tempStr)
        {
            sw.WriteLine(each);
        }
        sw.Close();
    }

    public static void deletaFileOrFolder(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            return;
        }
        if (Directory.Exists(path))
        {
            Directory.Delete(path);
            return;
        }
    }

    //无用
    public static string getProjectPath(string name)
    {
        string path = Path.GetFullPath(name);
        Debug.Log(path);
        return null;
    }

	
}
