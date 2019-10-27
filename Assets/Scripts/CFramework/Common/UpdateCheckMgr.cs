//=====================================================
// - FileName:      UpdateCheckMgr.cs
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
using UnityEngine;
using UnityEngine.Networking;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.Common
{
    public class UpdateCheckMgr : Singleton<UpdateCheckMgr>
    {
        //是否初始化完成boo
        public static bool initialize = false;
        //游戏使用的版本(用于与后端通知更新使用)
        public string gameVersion = "0.0.0.0";
        //v1 本地缓存app版号
        private string cacheAppVersion = "";
        //v1 远程app版号
        private string lastAppVersion = null;
        //存储Key
        private const string appVesionKey = "__app_Version";
        //key为路径，value为md5
        private Dictionary<string, string> localVersionInfo = new Dictionary<string, string>();

        public const string assetExtractCor = "AssetExtractCor";
        public const string assetUpdateCor = "AssetUpdateCor";

        public void Init()
        {
            ZLogger.Info("资源更新管理层初始化");
        }

        public void AfterInit()
        {
            ZLogger.Info("资源更新检查开始");
            cacheAppVersion = PlayerPrefs.GetString(appVesionKey, "");
            CheckExtractResource();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void CheckExtractResource()
        {
            if (AppConst.DebugMode) // 本地调试
                OnResourceInited();//StartCoroutine(OnUpdateResource());
            else
            {
                // 文件已经解压过了
                bool isExists = Directory.Exists(UtilTool.DataPath) && File.Exists(UtilTool.DataPath + "files.txt");
                if (isExists)
                {
                    string dataPath = UtilTool.DataPath;  //数据目录
                    if (!Directory.Exists(dataPath))
                        Directory.CreateDirectory(dataPath);
                    string[] files = UtilTool.GetVersionMap(dataPath + "files.txt");
                    int count = files.Length;
                    string lastLine = files[count - 1];
                    lastAppVersion = UtilTool.GetVersion(lastLine, 0);//获得v1
                    if (lastAppVersion != cacheAppVersion)//本地记录v1与应用文件v1是否相同，不相同表示 1.没解压完成,2.被黑了
                        CoroutineMgr.Instance.StartCoroutine(assetExtractCor, OnExtractResource());
                    else
                        CoroutineMgr.Instance.StartCoroutine(assetUpdateCor, OnUpdateResource());
                }
                else //启动释放
                {
                    PlayerPrefs.DeleteAll();//清除并重置所有记录
                    CoroutineMgr.Instance.StartCoroutine(assetExtractCor, OnExtractResource());
                }
            }
        }

        /// <summary>
        /// 提取游戏包资源到本地目录中
        /// </summary>
        /// <returns></returns>
		IEnumerator OnExtractResource()
        {
            string dataPath = UtilTool.DataPath;  //数据目录
            string pkgPath = UtilTool.AppContentPath(); //游戏包资源目录

            if (Directory.Exists(dataPath))//删除所有本地资源
                Directory.Delete(dataPath, true);
            Directory.CreateDirectory(dataPath);

            string pkgMapFile = pkgPath + "files.txt";
            string localMapFile = dataPath + "files.txt";

            if (File.Exists(localMapFile)) //删除旧的map文件
                File.Delete(localMapFile);

            EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "(此过程不消耗任何流量，请放心等待)首次进入游戏,初始化中...", 100);
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityWebRequest unityWeb = UnityWebRequest.Get(pkgMapFile);
                unityWeb.downloadHandler = new DownloadHandlerFile(pkgMapFile);
                yield return unityWeb;
                while (!unityWeb.isDone)
                {
                    EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "(此过程不消耗任何流量，请放心等待)首次进入游戏,初始化中...|", unityWeb.downloadProgress * 100);
                    yield return null;
                }
            }
            else
            {
                File.Copy(pkgMapFile, localMapFile, true);
            }
            yield return new WaitForEndOfFrame();

            string[] files = File.ReadAllLines(localMapFile);//释放所有文件到数据目录
            int count = files.Length;//总文件
            int step = 0;//第N个文件
            string lastLine = files[count - 1];
            lastAppVersion = UtilTool.GetVersion(lastLine, 0);//包中的版本号

            foreach (var file in files)
            {
                string[] fs = file.Split('|');
                if (fs.Length != 2) break;
                pkgMapFile = pkgPath + fs[0];
                localMapFile = dataPath + fs[0];
                EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "(此过程不消耗任何流量，请放心等待)正在准备进入游戏中...", Mathf.FloorToInt((++step * 100 / count)));

//#if !SyncLocal //进行更新场景		
//                if (fs[0].Contains("scene/"))
//                {//跳过场景资源，进行动态加载
//                    loaderMgr.CacheAssetBundleLoaderData(fs[0], fs[1]);
//                    continue;
//                }
//#endif
                string dir = Path.GetDirectoryName(localMapFile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (Application.platform == RuntimePlatform.Android)
                {
                    UnityWebRequest unityWeb = UnityWebRequest.Get(pkgMapFile);
                    unityWeb.downloadHandler = new DownloadHandlerFile(pkgMapFile);
                    yield return unityWeb;
                }
                else
                {
                    if (File.Exists(localMapFile))
                        File.Delete(localMapFile);
                    File.Copy(pkgMapFile, localMapFile, true);
                }
                yield return null;
            }
            yield return 1;

            PlayerPrefs.SetString(appVesionKey, lastAppVersion);// 本地记录v1
            cacheAppVersion = lastAppVersion;//解压完成当前的版本号

            CoroutineMgr.Instance.StartCoroutine(assetUpdateCor, OnUpdateResource());//释放完成，开始启动更新资源
        }

        /// <summary>
        /// 更新本地文件
        /// </summary>
        /// <returns></returns>
		IEnumerator OnUpdateResource()
        {
            string dataPath = UtilTool.DataPath;  //数据目录
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            string[] lastMapList = UtilTool.GetVersionMap(dataPath + "files.txt");
            int count = lastMapList.Length;//总文件
            int step = 0;//第N个文件
            string lastLine = lastMapList[count - 1];
            string lastVersion = lastLine;//最近版本号
            gameVersion = lastVersion.Trim();

            //不进行更新 no edit

            bool hasUpdate = false;//是否存在必要更新
            #region 本地资源版本
            //收集当前版本文件信息
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(lastMapList[i])) continue;
                string[] keyValue = lastMapList[i].Split('|');
                if (keyValue.Length != 2)
                    break;
                localVersionInfo.Add(keyValue[0].Trim(), keyValue[1].Trim());
            }
            lastAppVersion = UtilTool.GetVersion(lastLine, 0);//最近app v1
            string lv2 = UtilTool.GetVersion(lastVersion, 1);//非UI资源
            string lv3 = UtilTool.GetVersion(lastVersion, 2);//UI资源
            string lv4 = UtilTool.GetVersion(lastVersion, 3);//脚本资源
            #endregion

            #region 服务器资源版本
            EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "正在通讯中... ",0);
            string remoteVersion = lastVersion;//cdn版本号 暂定与本地一样
            string url = AppConst.WebUrl;
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string webMapUrl = url + "files.txt?v=" + random;
            UnityWebRequest unityWeb = new UnityWebRequest(webMapUrl);
            ZLogger.Info("资源位置：" + webMapUrl);
            yield return unityWeb;
            if (unityWeb.error != null)
            {
                ZLogger.Info("可能网络问题，也可能服务器资源没提交!  此处可以考虑直接进游戏用本地资源[不进行更新 #SyncLocal]");

                #region 临时解决方案(没有连接上cdn 使用本地资源)
                EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "连接不到服务器中心，应用最近版本进入游戏，建议稍候重启游戏更新!!",100);
                for (int i = 0; i < count; i++)
                {
                    if (string.IsNullOrEmpty(lastMapList[i])) continue;
                    string[] keyValue = lastMapList[i].Split('|');
                    if (keyValue.Length != 2)
                        break;
                    string f = keyValue[0];

                    //if (keyValue[0].Contains("scene/"))
                    //{//跳过场景资源，进行动态加载
                    //    loaderMgr.CacheAssetBundleLoaderData(keyValue[0], keyValue[1]);
                    //    continue;
                    //}
                }
                yield return new WaitForSeconds(1);
                OnResourceInited();
                yield break;
                #endregion

                //EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "(此过程不消耗任何流量，请放心等待)请求失败,您的网络可能不稳定，请稍后再重新启动游戏！");
                yield break;
            }
            else
            {
                int p = Mathf.FloorToInt(unityWeb.downloadProgress * 100);
                int size = Mathf.CeilToInt(9877);
                EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "加载版本配置中,需要消耗流量约 " + size + "kb, 已经完成", p);
            }
            byte[] webMapData = unityWeb.downloadHandler.data;
            string webMap = unityWeb.downloadHandler.text.Trim();
            string[] webMapList = webMap.Split('\n');
            count = webMapList.Length;
            lastLine = webMapList[count - 1];
            remoteVersion = lastLine;
            string remoteAppVersion = UtilTool.GetVersion(lastLine, 0);
            string rv2 = UtilTool.GetVersion(remoteVersion, 1);//非UI资源
            string rv3 = UtilTool.GetVersion(remoteVersion, 2);//UI资源
            string rv4 = UtilTool.GetVersion(remoteVersion, 3);//脚本资源
            #endregion
            Debug.Log("服务器版本：" + remoteVersion);
            bool updateV1 = !remoteAppVersion.Equals(lastAppVersion);
            bool updateV2 = (!lv2.Equals(rv2)) || updateV1;
            bool updateV3 = (!lv3.Equals(rv3)) || updateV1;
            bool updateV4 = (!lv4.Equals(rv4)) || updateV1;

            int resCount = 0;
            int resStep = 0;
            int uiCount = 0;
            int uiStep = 0;
            int luaCount = 0;
            int luaStep = 0;
            if (updateV2 || updateV3 || updateV4) //需要更新时，计算各部分文件总量
            {
                for (int i = 0; i < count; i++)
                {
                    if (string.IsNullOrEmpty(webMapList[i])) continue;
                    string[] keyValue = webMapList[i].Split('|');
                    if (keyValue.Length != 2)
                        break;
                    if (keyValue[0].Contains("/UI/"))
                        uiCount++;
                    else if (keyValue[0].Contains("/Lua/"))
                        luaCount++;
                    else
                        resCount++;
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(webMapList[i])) continue;
                string[] keyValue = webMapList[i].Split('|');
                if (keyValue.Length != 2)
                    break;
                string f = keyValue[0].Trim();
                
                //if (keyValue[0].Contains("scene/"))
                //{//跳过场景资源，进行动态加载
                //    loaderMgr.CacheAssetBundleLoaderData(keyValue[0], keyValue[1]);
                //    continue;
                //}
                if (lastVersion == remoteVersion)//版本一样，不用更新
                {
                    EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, "(此过程不消耗任何流量，请放心等待)初始化游戏环境中... ", Mathf.FloorToInt((++step * 100 / count)));
                    continue;
                }

                string fileUrl = url + f + "?v=" + random; //接取服务器资源
                string localfile = (dataPath + f).Trim();
                bool canUpdate = false;// 是否需要更新
                string path = "";
                string message = "";
                bool checkUpdate = false;
                checkUpdate = ((f.Contains("/UI/") && updateV3) || (f.Contains("/Lua/") && updateV4) || updateV2);

                if (checkUpdate)
                {
                    canUpdate = !File.Exists(localfile);
                    path = Path.GetDirectoryName(localfile);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    if (!canUpdate) //检查是否更新
                    {
                        string localKey = "*";
                        if (localVersionInfo.ContainsKey(f))
                            localKey = localVersionInfo[f];
                        string remoteKey = keyValue[1].Trim();
                        canUpdate = !remoteKey.Equals(localKey);
                        if (canUpdate)
                            File.Delete(localfile);
                    }
                }

                if (canUpdate) //更新或新增文件
                {
                    //方式1 UnityWebRequest更新
                    hasUpdate = true; //Debug.Log("更新-->" + fileUrl);
                    unityWeb = new UnityWebRequest(fileUrl);
                    yield return unityWeb;
                    if (unityWeb.error != null)
                    {
                        OnUpdateFailed(path);
                        yield break;
                    }
                    int size = 0;
                    if (f.Contains("/UI/"))
                    {
                        size = 311 * uiCount;
                        message = String.Format("正在更新{0}文件, 需要消耗流量约 {1} kb", "UI", size);
                        EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, message , Mathf.FloorToInt((++uiStep) * 100 / uiCount));
                    }
                    else if (f.Contains("/Lua/"))
                    {
                        size = 6 * luaCount;
                        message = String.Format("正在更新{0}文件, 需要消耗流量约 {1} kb", "Lua", size);
                        EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, message , Mathf.FloorToInt((++luaStep) * 100 / luaCount));
                    }
                    else
                    {
                        size = 151 * resCount;
                        message = String.Format("正在更新{0}文件, 需要消耗流量约 {1} kb", "环境", size);
                        EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, message , Mathf.FloorToInt((++resStep) * 100 / resCount));
                    }
                    //byte[] tempDownByte = unityWeb.downloadHandler.data;
                    File.WriteAllBytes(localfile, unityWeb.downloadHandler.data);
                    yield return null;
                }
            }
            if (hasUpdate)
            {
                File.WriteAllBytes(dataPath + "files.txt", webMapData);
                PlayerPrefs.SetString(appVesionKey, remoteAppVersion);// 本地记录v1
                cacheAppVersion = remoteAppVersion;//解压完成当前的版本号
                gameVersion = remoteVersion.Trim();
                ZLogger.Info("写入版本号");
            }

            //Debug.Log("=================版本：===================>>最近:" + lastVersion + "| 远程:" + remoteVersion);
            yield return new WaitForEndOfFrame();

            EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_COMPLETED, " 游戏更新检查完毕!!", 100);
            OnResourceInited();
            yield return 0;
        }

        //加载失败
        void OnUpdateFailed(string file)
        {
            string message = "游戏环境初始失败!>" + file;
            ZLogger.Info("更新失败!>" + file);
            EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_PROGRESS, message, 0);
        }


        /// 资源初始化结束
        public void OnResourceInited()
        {
            EventMgr.Instance.TriggerEvent(UpdataConst.LOADER_ALL_COMPLETED);
        }
    }
}
