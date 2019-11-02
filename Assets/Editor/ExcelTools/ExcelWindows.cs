//=====================================================
// - FileName:      ExcelWindows.cs
// - Created:           mahuibao
// - UserName:      2018-11-24
// - Email:         1023276206@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Data;
using System.Text;
using System.Reflection;
using Zero.ZeroEngine.Data;


public class ExcelWindows :EditorWindow {
    private const int CONST_FILE_INFO_ROW_INDEX = 0;
    private const int CONST_COLUMN_NAMES_ROW_INDEX = 1;
    private const int CONST_KEYS_ROW_INDEX = 2;
    private const int CONST_CLIENT_MEMBERS_ROW_INDEX = 3;
    private const int CONST_CLIENT_TYPES_ROW_INDEX = 4;
    private const int CONST_SERVER_MEMBERS_ROW_INDEX = 5;
    private const int CONST_SERVER_TYPES_ROW_INDEX = 6;

    private const string CONST_CLIENT_UINT_TYPE = "uint";
    private const string CONST_CLIENT_INT_TYPE = "int";
    private const string CONST_CLIENT_FLOAT_TYPE = "float";
    private const string CONST_CLIENT_SHORT_TYPE = "short";
    private const string CONST_CLIENT_BYTE_TYPE = "byte";
    private const string CONST_CLIENT_STRING_TYPE = "string";
    private const string CONST_CLIENT_BOOL_TYPE = "bool";
    private const string CONST_CLIENT_ARRAY_TYPE = "array";
    private const string CONST_CLIENT_ARRAY1_TYPE = "array1";
    private const string CONST_CLIENT_ARRAY2_TYPE = "array2";
    private const string CONST_CLIENT_ARRAY3_TYPE = "array3";

    private const int EXPORT_TYPE_LUA = 1;
    private const int EXPORT_TYPE_JSON = 2;
    private const int EXPORT_TYPE_SCRIPT = 3;
    private const int EXPORT_TYPE_ASSET = 4;

    private Dictionary<int, string> EXPORT_TYPE_STRING = new Dictionary<int, string>();

    private static List<string> CLIENT_TYPE_LIST = new List<string>()
    {
        CONST_CLIENT_UINT_TYPE,
        CONST_CLIENT_INT_TYPE,
        CONST_CLIENT_FLOAT_TYPE,
        CONST_CLIENT_SHORT_TYPE,
        CONST_CLIENT_BYTE_TYPE,
        CONST_CLIENT_STRING_TYPE,
        CONST_CLIENT_BOOL_TYPE,
        CONST_CLIENT_ARRAY_TYPE,
        CONST_CLIENT_ARRAY1_TYPE,
        CONST_CLIENT_ARRAY2_TYPE,
        CONST_CLIENT_ARRAY3_TYPE,
    };

    private const string CONST_SERVER_MEMBER_NEED = "e";
    private const string CONST_SERVER_INT_TYPE = "int";
    private const string CONST_SERVER_STRING_TYPE = "string";
    private const string CONST_SERVER_TERM_TYPE = "term";
    private const string CONST_SERVER_TERM1_TYPE = "term1";
    private const string CONST_SERVER_TERM2_TYPE = "term2";

    private const string CONST_CLIENT_FILE_PREFIX = "Sys";
    private const string CONST_CS_KEY_NAME = "unikey";
    private const string CONST_ERL_FILE_PREFIX = "sys_";

    #region 界面相关
    //private ExcelInfo excelInfo = new ExcelInfo();
    /// <summary>
    /// Excel表主题
    /// </summary>
    private string _threePackName = "cn";
    /// <summary>
    /// Excel表路径
    /// </summary>
    private string _excelPath = "";
    private const string SAVE_CLIENT_EXCLE_PATH = "ExcelWindows._excelPath";
    /// <summary>
    /// 客户端Editor临时数据路径
    /// </summary>
    private string _clientEditorPath = "";
    private const string SAVE_CLIENT_EDITOR_PATH = "ExcelWindows._clientEditorPath";
    /// <summary>
    /// 客户端导出数据路径
    /// </summary>
    private string _clientPath = "";
    private const string SAVE_CLIENT_DATA_PATH = "ExcelWindows,_clientPath";
    /// <summary>
    /// 脚本转移位置
    /// </summary>
    private string _clientScriptPath = "";
    private const string SAVE_CLIENT_SCRIPT_PATH = "ExcelWindows,_clientScriptPath";
    /// <summary>
    /// 服务器数据路径
    /// </summary>
    private string _serverPath = "";
    /// <summary>
    /// 搜索关键词
    /// </summary>
    private string _search = "";

    void OnEnable()
    {
        EXPORT_TYPE_STRING.Add(EXPORT_TYPE_LUA, ".lua");
        EXPORT_TYPE_STRING.Add(EXPORT_TYPE_JSON, ".json");
        EXPORT_TYPE_STRING.Add(EXPORT_TYPE_SCRIPT, ".cs");
        EXPORT_TYPE_STRING.Add(EXPORT_TYPE_ASSET, ".asset");

        _excelPath = EditorPrefs.GetString(SAVE_CLIENT_EXCLE_PATH, _excelPath);
        _clientEditorPath = EditorPrefs.GetString(SAVE_CLIENT_EDITOR_PATH, _clientEditorPath);
        _clientPath = EditorPrefs.GetString(SAVE_CLIENT_DATA_PATH, _clientPath);
        _clientScriptPath = EditorPrefs.GetString(SAVE_CLIENT_SCRIPT_PATH, _clientScriptPath);
        GetExcelFileInfos();
    }

    void OnGUI()
    {
        TopGUI();
        MiddleGUI();
        SearchGUI();
        BottomGUI();
    }
    void TopGUI()
    {
        EditorGUILayout.BeginHorizontal();
        _threePackName = EditorGUILayout.TextField("数据类型:", _threePackName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _excelPath = EditorGUILayout.TextField("excel目录:", _excelPath);
        if (GUILayout.Button( "Browse",GUILayout.ExpandWidth(false)))
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            if (path != null)
            {
                _excelPath = path;

                //_excelPath = path.Substring(path.IndexOf("Assets")).Replace("\\", "/");

                Debug.LogWarning("Select _excelPath folder =  "+_excelPath);
                EditorPrefs.SetString(SAVE_CLIENT_EXCLE_PATH, _excelPath);
                GetExcelFileInfos();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _clientEditorPath = EditorGUILayout.TextField("客户端Editor临时目录:", _clientEditorPath);
        if (GUILayout.Button( "Browse",GUILayout.ExpandWidth(false)))
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            if (path != null)
            {
                _clientEditorPath = path.Substring(path.IndexOf("Assets")+6).Replace("\\", "/")+"/";
                Debug.LogWarning("Select _clientEditorPath folder =  " + _clientEditorPath);
                EditorPrefs.SetString(SAVE_CLIENT_EDITOR_PATH, _clientEditorPath);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _clientPath = EditorGUILayout.TextField("客户端导出数据目录:", _clientPath);
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            if (path != null)
            {
                _clientPath = path.Substring(path.IndexOf("Assets") + 6).Replace("\\", "/") + "/";
                Debug.LogWarning("Select _clientPath folder =  " + _clientPath);
                EditorPrefs.SetString(SAVE_CLIENT_DATA_PATH, _clientPath);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _clientScriptPath = EditorGUILayout.TextField("客户端脚本转移目录:", _clientScriptPath);
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            if (path != null)
            {
                _clientScriptPath = path.Substring(path.IndexOf("Assets") + 6).Replace("\\", "/") + "/";
                Debug.LogWarning("Select _clientScriptPath folder =  " + _clientScriptPath);
                EditorPrefs.SetString(SAVE_CLIENT_SCRIPT_PATH, _clientScriptPath);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _serverPath = EditorGUILayout.TextField("服务器目录:", _serverPath);
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            if (path != null)
            {
                _serverPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    void MiddleGUI()
    {
        GUI.color = Color.white;
        if (GUILayout.Button( "一键前端lua" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button( "一键前端json" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button("一键前端二进制" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button( "一键前端其他" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button("一键服务器java" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button( "一键服务器其他" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button( "一键服务器其他" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button( "点击提交客户端数据" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button("点击提交服务端数据" ))
        {

        }
        GUI.color = Color.white;
        if (GUILayout.Button( "点击提交。。。。。。。" ))
        {

        }

    }
    void SearchGUI()
    {
        GUI.color = Color.white;
        GUILayout.Label("以下两个功能专门为：ScriptableObject以及asset服务");
        GUI.color = Color.white;
        if (GUILayout.Button("点击转移ScriptableObject脚本文件（暂时废弃，请手动转移）"))
        {
            //string path = "Assets" + _clientEditorPath;
            //DirectoryInfo direction = new DirectoryInfo(path);
            //FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            //string clien_script_path = "Assets/Scripts/CFramework/Data/CN/";
            //for (int i = 0; i < files.Length; i++)
            //{
            //    //if (files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".asset"))
            //    //{
            //    if (files[i].Name.EndsWith(".asset.meta") || files[i].Name.EndsWith(".asset"))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        string tempOldPath = path + files[i].Name;
            //        string tempPath = clien_script_path + files[i].Name;
            //        if (File.Exists(tempPath))
            //        {
            //            File.Delete(tempPath);
            //            AssetDatabase.Refresh();
            //            AssetDatabase.SaveAssets();
            //            try
            //            {
            //                AssetDatabase.MoveAsset(tempOldPath, tempPath);
            //            }
            //            catch(Exception ex)
            //            {
            //                throw ex;
            //            }

            //        }
            //        else
            //        {
            //            try
            //            {
            //                AssetDatabase.MoveAsset(tempOldPath, tempPath);
            //            }
            //            catch (Exception ex)
            //            {
            //                throw ex;
            //            }
            //        }
            //    }
            //}
            //Debug.Log("转移ScriptableObject脚本文件 完成");
        }
        GUI.color = Color.white;
        if (GUILayout.Button("点击转移asset资源文件（暂时废弃）"))
        {
            //string path = "Assets" + _clientPath;
            //DirectoryInfo direction = new DirectoryInfo(path);
            //FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            //string clien_assets_path = "Assets/Resources/Data/";
            //for (int i = 0; i < files.Length; i++)
            //{
            //    if (files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".cs"))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        string tempOldPath = path + files[i].Name;
            //        string tempPath = clien_assets_path + files[i].Name;
            //        if (File.Exists(tempPath))
            //        {
            //            File.Delete(tempPath);
            //            AssetDatabase.MoveAsset(tempOldPath, tempPath);
            //        }
            //        else
            //        {
            //            AssetDatabase.MoveAsset(tempOldPath, tempPath);
            //        }
            //    }
            //}
        }

        GUI.color = Color.white;
        GUILayout.Label( "提示-------------------------------------------------------------------------:");
        _search = EditorGUILayout.TextField("搜索关键词:", _search);
    }

    private Vector2 _scroll_pos;
    void BottomGUI()
    {
        _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
        {
            foreach (FileInfoData fi in fileInfos)
            {
                if (!string.IsNullOrEmpty(_search))
                {
                    if (!fi.fileName.Contains(_search))
                    {
                        continue;
                    }
                }

                EditorGUILayout.BeginHorizontal();
                if (fi.toLua) GUI.color = Color.red;
                else GUI.color = Color.green;
                if (GUILayout.Button("导出 " + fi.fileName + " 的lua文件", GUILayout.Height(40), GUILayout.Width(226)))
                {
                    Debug.Log("开始导出 " + fi.filePath + "  的lua文件");
                }

                if (fi.toJson) GUI.color = Color.red;
                else GUI.color = Color.green;
                if (GUILayout.Button("导出 " + fi.fileName + " 的json文件", GUILayout.Height(40), GUILayout.Width(226)))
                {
                    Debug.Log("开始导出 " + fi.filePath + "  的json文件");
                    UpdateClientData(fi,EXPORT_TYPE_JSON);
                }

                if (fi.toScript) GUI.color = Color.red;
                else GUI.color = Color.green;
                if (GUILayout.Button("导出 " + fi.fileName + " 的脚本文件", GUILayout.Height(40), GUILayout.Width(226)))
                {
                    Debug.Log("开始导出 " + fi.filePath + "  的脚本文件");
                    UpdateClientData(fi, EXPORT_TYPE_SCRIPT);
                }

                if (fi.toAsset) GUI.color = Color.red;
                else GUI.color = Color.green;
                if (GUILayout.Button("导出 " + fi.fileName + " 的Asset文件", GUILayout.Height(40), GUILayout.Width(226)))
                {
                    Debug.Log("开始导出 " + fi.filePath + "  的Asset文件");
                    UpdateClientData(fi, EXPORT_TYPE_ASSET);
                }

                if (fi.toJava) GUI.color = Color.red;
                else GUI.color = Color.green;
                if (GUILayout.Button("导出 " + fi.fileName + " 的java文件", GUILayout.Height(40), GUILayout.Width(226)))
                {
                    Debug.Log("开始导出 " + fi.filePath + "  的java文件");
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }
    #endregion


    #region 客户端读取各个字段Tool
    private bool ReadColumnNames(DataTable dataTable)
    {
        for (int i=0;i<dataTable.Columns.Count;i++)
        {
            dicColumnNames[i] = dataTable.Rows[CONST_COLUMN_NAMES_ROW_INDEX][dataTable.Columns[0]].ToString();
        }
        if (dicColumnNames.Count <=0)
        {
            //WriteLogToLogFile("LogError: " + curFilePath + "未找到该字段的说明文字，请把表的单元格式设为文字");
            Debug.LogError(curFilePath + "未找到该字段的说明文字，请把表的单元格式设为文字");
            return false;
        }
        return true;
    }

    private bool ReadKeys(DataTable dataTable)
    {
        bool success = true;
        string str = dataTable.Rows[CONST_KEYS_ROW_INDEX][dataTable.Columns[0]].ToString().ToLower();
        keyNum = 1;
        needError = true;
        if (str.Equals("1*1"))
        {
            keyNum = 1;
            split = false;
        }
        else if(str.Equals("1*1_split"))
        {
            keyNum = 1;
            split = true;
        }
        else if(str.Equals("2*2"))
        {
            keyNum = 2;
            split = false;
        }
        else if(str.Equals("2*2_split"))
        {
            keyNum = 2;
            split = true;
        }
        else if(str.Equals("3*3"))
        {
            keyNum = 3;
            split = false;
        }
        else if(str.Equals("3*3_split"))
        {
            keyNum = 3;
            split = true;
        }
        if (dataTable.Columns.Count > 1)
        {
            str = dataTable.Rows[CONST_KEYS_ROW_INDEX][dataTable.Columns[1]].ToString().ToLower();
            if (str.Equals("n_error"))
            {
                needError = false;
            }
        }
        return success;
    }

    private bool ReadClientMembers(DataTable dataTable,bool show,out bool success)
    {
        string member;
        success = true;
        List<string> memberList = new List<string>();
        for (int i= 0;i<dataTable.Columns.Count;i++)
        {
            member = dataTable.Rows[CONST_CLIENT_MEMBERS_ROW_INDEX][dataTable.Columns[i]].ToString();
            if (string.IsNullOrEmpty(member)||member.Equals(""))
            {
                continue;//第几列，空，就跳转，不加入到index中，这样循环的时候，index会找到对应的有数据的列
            }
            if (memberList.Contains(member))
            {
                //WriteLogToLogFile("LogError: " + curFilePath + "存在表字段重名: " + member);
                Debug.LogError(curFilePath + "存在表字段重名: " + member);
                success = false;
                return false;
            }
            memberList.Add(member);
            dicClientMembers[i] = member;
            index.Add(i);
        }
        memberList.Clear();
        if (dicClientMembers.Count<=0)
        {
            //WriteLogToLogFile("LogWarning:" + curFilePath + "没有表字段配置，若此表要导出客户端数据，请检查字段设置，并把表的单元格式设置为文字");
            if(show)
            {
                Debug.LogError(curFilePath + "没有表字段配置，若此表要导出客户端数据，请检查字段设置，并把表的单元格式设置为文字");
                success = false;
            }
            return false;
        }
        return true;
    }

    private bool ReadClientTypes(DataTable dataTable, bool show, out bool success)
    {
        success = true;
        string type;
        for (int i=0;i<dataTable.Columns.Count;i++)
        {
            type = dataTable.Rows[CONST_CLIENT_TYPES_ROW_INDEX][dataTable.Columns[i]].ToString();
            if(!CLIENT_TYPE_LIST.Contains(type))
            {
                if(dicClientMembers.ContainsKey(i))
                {
                    Debug.LogError(curFilePath + "第" + (i + 1) + "列配置了错误的客户端数据类型" + type + "，目前仅支持的类型有" + string.Join(",", CLIENT_TYPE_LIST.ToArray()));
                    success = false;
                    return false;
                }
                continue;
            }
            else
            {
                if(!dicClientMembers.ContainsKey(i))
                {
                    Debug.LogError(curFilePath + "第" + (i + 1) + "列没有客户端类型名");
                    success = false;
                    return false;
                }
            }
            dicClientTypes[i] = type;
        }
        if (dicClientTypes.Count<=0)
        {
            //WriteLogToLogFile("LogWarning:" + curFilePath + "没有客户端字段类型配置，若此表要导出客户端数据，请检查字段类型配置，并把表的单元格式设置为“文字”");
            if (show)
            {
                Debug.LogError(curFilePath + "没有客户端字段类型配置，若此表要导出客户端数据，请检查字段类型配置，并把表的单元格式设置为“文字”");
                success = false;
            }
            return false;
        }
        return true;
    }

    #endregion

    /// <summary>
    /// 选中文件路径
    /// </summary>
    private string curFilePath;
    /// <summary>
    /// 选中文件名字
    /// </summary>
    private string curFileName;
    /// <summary>
    /// 选中文件大写名字
    /// </summary>
    private string curNewName;

    private string fileInfo;                                 //excel里面的中文名字
    private int keyNum = 0;                                  //key数量
    private bool split = false;
    private bool needError = true;
    private Dictionary<string, string> dicFileNames;         //excel名、excel里面的中文名字
    private Dictionary<int, string> dicColumnNames;          //excel表内每行对应的中文名字
    private Dictionary<int, int> dicKeyMultiples;
    private Dictionary<int, string> dicClientMembers;        //客户端类型对应的属性名
    private Dictionary<int, string> dicClientTypes;          //客户端类型
    private Dictionary<int, string> dicServerMembers;        //服务端类型对应的属性名
    private Dictionary<int, string> dicServerTypes;          //服务端类型
    //private List<string> serverMembersList;
    //private Dictionary<int, string> serverTypes;
    private List<int> index;                                 //列数
    //private List<int> erlindex;
    private Dictionary<int, Dictionary<int, string>> dicData;//数据字典


    private bool UpdateClientData(FileInfoData fi,int exportType, bool show = true)
    {
        //要取回当前的导出文件名
        curFilePath = fi.filePath;
        curFileName = fi.fileName;
        curNewName = fi.fileNewName;

        dicColumnNames = new Dictionary<int, string>();
        dicClientMembers = new Dictionary<int, string>();
        dicClientTypes = new Dictionary<int, string>();
        //servermenberlist
        //servertypes
        dicData = new Dictionary<int, Dictionary<int, string>>();

        dicFileNames = new Dictionary<string, string>();
        index = new List<int>();
        //erindex
        dicServerMembers = new Dictionary<int, string>();
        dicServerTypes = new Dictionary<int, string>();

        bool success = ExportExcelToClient(fi, show, exportType);

        //ResetDictionary();

        if(show)
        {
            if(!success)
            {
                Debug.LogError("导出" + curFileName + EXPORT_TYPE_STRING[exportType] + " 失败");
                if(show)
                {
                    EditorUtility.DisplayDialog("导出" + EXPORT_TYPE_STRING[exportType] + "文件", "导出" + curFileName + EXPORT_TYPE_STRING[exportType] + " 失败\n     失败\n     失败\n", "哦,不");
                }
                WriteMD5();
                AssetDatabase.Refresh();
            }
        }
        return success;
    }
    private bool ExportExcelToClient(FileInfoData fi, bool show, int exportType)
    {
        bool success = true;
        bool suc;
        DataTable dataTable = ExcelUtil.readExcel(curFilePath, curFileName);
        if (null == dataTable)
        {
            success = false;
            Debug.LogError(curFilePath + "读取" + curFileName + "表错误，请检查excel的表名字是否与表文件名一直");
            return success;
        }
        if (dataTable.Rows.Count < (CONST_SERVER_TYPES_ROW_INDEX + 1))
        {
            success = false;
            Debug.LogError(curFilePath + "配置信息不够" + (CONST_SERVER_TYPES_ROW_INDEX + 1) + " 行");
            return success;
        }
        try
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                switch (i)
                {
                    case CONST_FILE_INFO_ROW_INDEX://读取文件内excel名字
                        fileInfo = dataTable.Rows[CONST_FILE_INFO_ROW_INDEX][dataTable.Columns[0]].ToString();
                        dicFileNames.Add(curNewName, fileInfo);
                        continue;
                    case CONST_COLUMN_NAMES_ROW_INDEX://读取文件内每行中文名字
                        success = ReadColumnNames(dataTable);
                        if (!success)
                            return success;
                        continue;
                    case CONST_KEYS_ROW_INDEX://读取key个数
                        success = ReadKeys(dataTable);
                        if (!success)
                            return success;
                        continue;
                    case CONST_CLIENT_MEMBERS_ROW_INDEX://读取客户端属性名字
                        success = ReadClientMembers(dataTable, show, out suc);
                        if (!success)
                            return suc;
                        continue;
                    case CONST_CLIENT_TYPES_ROW_INDEX://读取客户端属性类型
                        success = ReadClientTypes(dataTable, show, out suc);
                        if (!success)
                            return suc;
                        continue;
                    case CONST_SERVER_MEMBERS_ROW_INDEX://读取服务端属性名字

                        continue;
                    case CONST_SERVER_TYPES_ROW_INDEX://读取服务端属性类型

                        continue;
                    default:
                        break;
                }
                //因为前六行已经在上面进行了判断，并且continue之后，跳过了下面的步骤
                if (!show && fi.toJson) return true;
                Dictionary<int, string> dicRowData = new Dictionary<int, string>();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    dicRowData[j] = dataTable.Rows[i][dataTable.Columns[j]].ToString();
                }
                dicData[i] = dicRowData;//哪行，对应那一行的所有列数的数据
            }
            switch(exportType)
            {
                case EXPORT_TYPE_LUA:
                    if (_clientPath.Length <= 4)
                    {
                        Debug.LogError("没有选择客户端导出数据文件路径，无法导出lua内容");
                        return false;
                    }
                    break;
                case EXPORT_TYPE_JSON:
                    if (_clientPath.Length <= 4)
                    {
                        Debug.LogError("没有选择客户端导出数据文件路径，无法导出json内容");
                        return false;
                    }
                    break;
                case EXPORT_TYPE_SCRIPT:
                    if (_clientPath.Length <= 4)
                    {
                        Debug.LogError("没有选择客户端Editor文件路径，无法导出Scriptable内容");
                        return false;
                    }
                    break;
                case EXPORT_TYPE_ASSET:
                    if (_clientPath.Length <= 4)
                    {
                        Debug.LogError("没有选择客户端导出数据文件路径，无法导出asset内容");
                        return false;
                    }
                    break;
            }
            var u8WithoutBom = new System.Text.UTF8Encoding(false);

            string csUpName = curFileName.ToUpper().Substring(0, 1) + curFileName.Substring(1);

            if (exportType == EXPORT_TYPE_LUA)
            {

            }
            if (exportType == EXPORT_TYPE_JSON)
            {
                string path;
                StreamWriter sw;
                path = Application.dataPath + _clientPath + "DataTable_" + curFileName + ".json";
                if (System.IO.File.Exists(path))
                {
                    File.Delete(path);
                }
                // 这里统一换了格式为\n ，因为在自动化流程中，有一个工具也会生成这个表，所以要针对这个表导出的json格式要统一
                ///////////////////////////////////////////////////////////////////////////////////////////////此处卡住，开始输出json的时候。这里就是遍历玩excel数据后的编写位置
                sw = new StreamWriter(path, false, u8WithoutBom);

                sw.Write("[");
                sw.Write("\n");
                switch (keyNum)
                {
                    case 1:
                        int lines = 0;
                        int files = 0;
                        foreach (var pair in dicData)
                        {
                            if (lines > 0)
                                sw.Write(",");
                            lines++;
                            sw.Write("{");
                            sw.Write("\n");
                            for (int i = 0; i < index.Count; i++)//循环所有的列数
                            {
                                if (i > 0)
                                    sw.Write(",");
                                sw.Write("\"" + dicClientMembers[index[i]] + "\":");
                                if (string.IsNullOrEmpty(pair.Value[index[i]]))//某一行的某一列
                                {
                                    if (dicClientTypes[index[i]] == "string")
                                    {
                                        sw.Write("\"\"");
                                    }
                                    else if (dicClientTypes[index[i]] == "int")
                                    {
                                        sw.Write("0");
                                    }
                                    else if (dicClientTypes[index[i]] == "array")
                                    {
                                        sw.Write("[]");
                                    }
                                }
                                else
                                {
                                    if (dicClientTypes[index[i]] == "string")
                                    {
                                        sw.Write("\"" + pair.Value[index[i]] + "\"");
                                    }
                                    else if (dicClientTypes[index[i]] == "array")
                                    {
                                        sw.Write(RoundAndSquareToBraceJson(pair.Value[index[i]]));
                                    }
                                    else if (dicClientTypes[index[i]] == "int" || dicClientTypes[index[i]] == "uint" || dicClientTypes[index[i]] == "float")
                                    {
                                        double o;
                                        if (!double.TryParse(pair.Value[index[i]], out o))
                                            throw new Exception(string.Format("{0}行{1}列,配置了客户端无法解析的内容，类型：{2}，值：{3}", lines + CONST_SERVER_MEMBERS_ROW_INDEX + 2, index[i] + 1, dicClientTypes[index[i]], pair.Value[index[i]]));
                                        sw.Write(pair.Value[index[i]]);
                                    }
                                    else if (dicClientTypes[index[i]] == "bool")
                                    {
                                        bool o;
                                        if (!bool.TryParse(pair.Value[index[i]], out o) && pair.Value[index[i]] != "0" && pair.Value[index[i]] != "1")
                                            throw new Exception(string.Format("{0}行{1}列,配置了客户端无法解析的内容，类型：{2}，值：{3}", lines + CONST_SERVER_MEMBERS_ROW_INDEX + 2, index[i] + 1, dicClientTypes[index[i]], pair.Value[index[i]]));
                                        sw.Write(pair.Value[index[i]]);
                                    }
                                    else
                                        throw new Exception("配置了客户端不支持的类型");
                                }
                            }
                            sw.Write("\n");
                            sw.Write("}");
                        }
                        break;
                    case 2:

                        break;
                    case 3:

                        break;
                }
                sw.Write("\n");
                sw.Write("]");

                sw.Flush();
                sw.Close();
                sw.Dispose();
                Debug.Log(curFileName + "  json  file  export complete !");
                ///////////////////////////////////////////////////////////////////////////////////////////////到此处
            }
            if (exportType == EXPORT_TYPE_SCRIPT)
            {
                ////以下为使用excel数据时候使用的C# scriptable脚本
                string cPath;
                StreamWriter cSw;
                cPath = Application.dataPath + _clientEditorPath + "DataTable" + csUpName + ".cs";
                string cPath2 = Application.dataPath + _clientScriptPath + "DataTable" + csUpName + ".cs";
                if (System.IO.File.Exists(cPath))
                {
                    File.Delete(cPath);
                }
                if (System.IO.File.Exists(cPath2))
                {
                    //File.Delete(cPath2);
                }
                AssetDatabase.Refresh();
                cSw = new StreamWriter(cPath, false, u8WithoutBom);
                cSw.WriteLine("/*=====================================================");
                cSw.WriteLine("* - Editor by tool");
                cSw.WriteLine("* - Don't editor by handself");
                cSw.WriteLine("=====================================================*/");
                cSw.WriteLine("using System;");
                cSw.WriteLine("using UnityEngine;");
                cSw.WriteLine("using System.Collections.Generic;");
                cSw.WriteLine("using Zero.ZeroEngine.Data;");
                cSw.Write("\n");
                cSw.Write("\n");
                cSw.WriteLine("[System.Serializable]");
                cSw.WriteLine("public class " + csUpName + "Excel {");
                for (int i = 0; i < index.Count; i++)//循环所有的列数
                {
                    cSw.Write("\tpublic ");
                    if (dicClientTypes[index[i]] == "string")
                    {
                        cSw.Write("string ");
                    }
                    else if (dicClientTypes[index[i]] == "int")
                    {
                        cSw.Write("int ");
                    }
                    else if (dicClientTypes[index[i]] == "array")
                    {
                        cSw.Write("int[] ");
                    }
                    else if (dicClientTypes[index[i]] == "array1")
                    {
                        cSw.Write("List<List<int>> ");
                    }
                    cSw.Write(dicClientMembers[index[i]] + ";\n");
                }
                cSw.Write("}\n");
                cSw.WriteLine("public class DataTable" + csUpName + ": DataBase {");
                cSw.WriteLine("\t[SerializeField]");
                cSw.WriteLine("\tpublic List<" + csUpName + "Excel> DataList = new List<" + csUpName + "Excel>();\r");

                if (keyNum == 1)
                {
                    cSw.WriteLine("\tprivate Dictionary<" + dicClientTypes[index[0]] + ", " + csUpName +
                    "Excel> _dataList = new Dictionary<" + dicClientTypes[index[0]] + ", " + csUpName +
                    "Excel>();");
                }
                else if (keyNum == 2)
                {
                    cSw.WriteLine("\tprivate Dictionary<" + dicClientTypes[index[0]] + ", Dictionary<" + dicClientTypes[index[1]] + ", " + csUpName +
                    "Excel>> _dataList = new Dictionary<" + dicClientTypes[index[0]] + ", Dictionary<" + dicClientTypes[index[1]] + ", " + csUpName +
                    "Excel>>();");
                }
                else if(keyNum == 3)
                {

                }

                cSw.WriteLine("\tpublic void AddInfo(" + csUpName + "Excel _info)");
                cSw.WriteLine("\t{");
                cSw.WriteLine("\t\tDataList.Add(_info);");
                cSw.WriteLine("\t}");

                cSw.WriteLine("\tpublic override void Clear()");
                cSw.WriteLine("\t{");
                //cSw.WriteLine("\t\tDataList.Clear();");
                cSw.WriteLine("\t\t_dataList.Clear();");
                cSw.WriteLine("\t}");

                if (keyNum == 1)
                {
                    cSw.WriteLine("\tpublic override void Init()");
                    cSw.WriteLine("\t{");

                    cSw.WriteLine("\t\tif(_dataList.Count>0) return;");

                    cSw.WriteLine("\t\tforeach (var info in DataList)");
                    cSw.WriteLine("\t\t{");
                    cSw.WriteLine("\t\t\t_dataList.Add(info." + dicClientMembers[0] + ", info);");
                    cSw.WriteLine("\t\t}");
                    cSw.WriteLine("\t}");//区分

                    cSw.WriteLine("\tpublic Dictionary<" + dicClientTypes[index[0]] + ", " + csUpName + "Excel> GetInfo()");
                    cSw.WriteLine("\t{");
                    cSw.WriteLine("\t\treturn _dataList;");
                    cSw.WriteLine("\t}");//区分

                    cSw.WriteLine("\tpublic " + csUpName + "Excel GetInfoById(" + dicClientTypes[index[0]] + " _id)");
                    cSw.WriteLine("\t{");
                    cSw.WriteLine("\t\t" + csUpName + "Excel info;");
                    cSw.WriteLine("\t\tif (_dataList.TryGetValue(_id, out info))");
                    cSw.WriteLine("\t\t{");
                    cSw.WriteLine("\t\t\treturn info;");
                    cSw.WriteLine("\t\t}");
                    cSw.WriteLine("\t\treturn null;");
                    cSw.WriteLine("\t}");//区分
                }
                else if (keyNum == 2)
                {
                    cSw.WriteLine("\tpublic override void Init()");
                    cSw.WriteLine("\t{");

                    cSw.WriteLine("\t\tif(_dataList.Count>0) return;");

                    if (dicClientTypes[index[0]] == "string")
                    {
                        cSw.WriteLine("\t\t" + dicClientTypes[index[0]] + " tempKey = \"\";");
                    }
                    else if (dicClientTypes[index[0]] == "int")
                    {
                        cSw.WriteLine("\t\t" + dicClientTypes[index[0]] + " tempKey = 0;");
                    }
                    cSw.WriteLine("\t\tDictionary<" + dicClientTypes[index[1]] + ", " + csUpName +
                    "Excel> tempList = new Dictionary<" + dicClientTypes[index[1]] + ", " + csUpName +
                    "Excel>();");
                    cSw.WriteLine("\t\t"+dicClientTypes[index[0]]+" tempSave;");

                    cSw.WriteLine("\t\tforeach (var info in DataList)");
                    cSw.WriteLine("\t\t{");
                    if (dicClientTypes[index[0]] == "string")
                    {
                        cSw.WriteLine("\t\t\tif(!(string.IsNullOrEmpty(tempKey)))");
                        cSw.WriteLine("\t\t\t{");
                        cSw.WriteLine("\t\t\t\tif(tempKey.Equals(info." + dicClientMembers[0] + "))");
                        cSw.WriteLine("\t\t\t\t{");
                    }
                    else if (dicClientTypes[index[0]] == "int")
                    {
                        cSw.WriteLine("\t\t\tif(!(tempKey==0))");
                        cSw.WriteLine("\t\t\t{");
                        cSw.WriteLine("\t\t\t\tif(tempKey==info." + dicClientMembers[0] + ")");
                        cSw.WriteLine("\t\t\t\t{");
                    }
                    cSw.WriteLine("\t\t\t\t\ttempList.Add(info." + dicClientMembers[1] + ", info);");
                    cSw.WriteLine("\t\t\t\t}");
                    cSw.WriteLine("\t\t\t\telse");
                    cSw.WriteLine("\t\t\t\t{");
                    cSw.WriteLine("\t\t\t\t\t_dataList.Add(info." + dicClientMembers[0] + ", tempList);");
                    cSw.WriteLine("\t\t\t\t\ttempList.Clear();");
                    cSw.WriteLine("\t\t\t\t\ttempKey=info." + dicClientMembers[0] +";");
                    cSw.WriteLine("\t\t\t\t}");
                    cSw.WriteLine("\t\t\t}");
                    cSw.WriteLine("\t\t\telse");
                    cSw.WriteLine("\t\t\t{");
                    cSw.WriteLine("\t\t\ttempKey=info." + dicClientMembers[0] + ";");
                    cSw.WriteLine("\t\t\ttempList.Add(info." + dicClientMembers[1] + ", info);");
                    cSw.WriteLine("\t\t\t}");
                    cSw.WriteLine("\t\t}");
                    cSw.WriteLine("\t\t_dataList.Add(tempKey" + ", tempList);");
                    cSw.WriteLine("\t\ttempList.Clear();");
                    cSw.WriteLine("\t}");
                    cSw.WriteLine("\tpublic Dictionary<" + dicClientTypes[index[0]] + ", Dictionary<" + dicClientTypes[index[1]] + ", " + csUpName + "Excel>> GetInfo()");
                    cSw.WriteLine("\t{");
                    cSw.WriteLine("\t\treturn _dataList;");
                    cSw.WriteLine("\t}");
                    cSw.WriteLine("\tpublic " + csUpName + "Excel GetInfoById(" + dicClientTypes[index[0]] + " _id ," + dicClientTypes[index[1]] + " _key )");
                    cSw.WriteLine("\t{");
                    cSw.WriteLine("\t\tDictionary<" + dicClientTypes[index[1]] + ", " + csUpName + "Excel> tempList;");
                    cSw.WriteLine("\t\tif (_dataList.TryGetValue(_id, out tempList))");
                    cSw.WriteLine("\t\t{");
                    cSw.WriteLine("\t\t\t" + csUpName + "Excel info;");
                    cSw.WriteLine("\t\t\tif (tempList.TryGetValue(_key, out info))");
                    cSw.WriteLine("\t\t\t{");
                    cSw.WriteLine("\t\t\t\treturn info;");
                    cSw.WriteLine("\t\t\t}");
                    cSw.WriteLine("\t\t}");
                    cSw.WriteLine("\t\treturn null;");
                    cSw.WriteLine("\t}");
                }
                else if (keyNum == 3)
                {

                }

                cSw.WriteLine("\tpublic List<" + csUpName + "Excel> GetInfoByNameAndValue(string name,int value)");
                cSw.WriteLine("\t{");
                cSw.WriteLine("\t\tList<"+csUpName+"Excel> tempList = new List<"+csUpName+"Excel>();");
                for (int i = 0; i < index.Count; ++i)
                {
                    if (dicClientTypes[index[i]] == "int")
                    {
                        cSw.WriteLine("\t\tif (name.Equals(\"" + dicClientMembers[index[i]] + "\"))");
                        cSw.WriteLine("\t\t{");
                        cSw.WriteLine("\t\t\tforeach (var item in DataList)");
                        cSw.WriteLine("\t\t\t{");
                        cSw.WriteLine("\t\t\t\tif (item."+ dicClientMembers[index[i]]+"== value)");
                        cSw.WriteLine("\t\t\t\t\ttempList.Add(item);");
                        cSw.WriteLine("\t\t\t}");
                        cSw.WriteLine("\t\t}");
                    }
                }
                cSw.WriteLine("\t\treturn tempList;");
                cSw.WriteLine("\t}");

                cSw.WriteLine("\tpublic List<" + csUpName + "Excel> GetInfoByNameAndValue(string name,string value)");
                cSw.WriteLine("\t{");
                cSw.WriteLine("\t\tList<" + csUpName + "Excel> tempList = new List<" + csUpName + "Excel>();");
                for (int i = 0; i < index.Count; ++i)
                {
                    if (dicClientTypes[index[i]] == "string")
                    {
                        cSw.WriteLine("\t\tif (name.Equals(\"" + dicClientMembers[index[i]] + "\"))");
                        cSw.WriteLine("\t\t{");
                        cSw.WriteLine("\t\t\tforeach (var item in DataList)");
                        cSw.WriteLine("\t\t\t{");
                        cSw.WriteLine("\t\t\t\tif (item." + dicClientMembers[index[i]] + ".Equals(value))");
                        cSw.WriteLine("\t\t\t\t\ttempList.Add(item);");
                        cSw.WriteLine("\t\t\t}");
                        cSw.WriteLine("\t\t}");
                    }
                }
                cSw.WriteLine("\t\treturn tempList;");
                cSw.WriteLine("\t}");
                cSw.WriteLine("}");

                cSw.Flush();
                cSw.Close();
                cSw.Dispose();

                Debug.Log(curFileName + "  scriptableObject  file  export complete !");
                AssetDatabase.Refresh();
            }
            if (exportType == EXPORT_TYPE_ASSET)
            {
                string className = string.Format("DataTable{0}", csUpName);
                Type type = Type.GetType(className);
                string eleName = string.Format("{0}Excel", csUpName);
                Type eleType = Type.GetType(eleName);

                var asset = ScriptableObject.CreateInstance(type);

                MethodInfo addMethod = type.GetMethod("AddInfo");

                foreach (var pair in dicData)
                {
                    var item = Activator.CreateInstance(eleType);
                    FieldInfo[] fields = eleType.GetFields();

                    for (int i = 0; i < index.Count; i++)//循环所有的列数
                    {
                        FieldInfo singleField = fields[i];
                        if (string.IsNullOrEmpty(pair.Value[index[i]]))//某一行的某一列
                        {
                            if (dicClientTypes[index[i]] == "string")
                            {
                                singleField.SetValue(item, Convert.ChangeType("", typeof(string)));
                            }
                            else if (dicClientTypes[index[i]] == "int")
                            {
                                singleField.SetValue(item, Convert.ChangeType(0, typeof(int)));
                            }
                            else if (dicClientTypes[index[i]] == "array")
                            {
                                singleField.SetValue(item, Convert.ChangeType(new int[0], typeof(int[])));
                            }
                            else if (dicClientTypes[index[i]] == "array1")
                            {
                                singleField.SetValue(item, Convert.ChangeType(0, typeof(List<List<int>>)));
                            }
                        }
                        else
                        {
                            if (dicClientTypes[index[i]] == "string")
                            {
                                singleField.SetValue(item, Convert.ChangeType(pair.Value[index[i]], typeof(string)));
                            }
                            else if (dicClientTypes[index[i]] == "array")
                            {
                                int[] test = GetStringToInt(pair.Value[index[i]]);
                                singleField.SetValue(item, test);
                            }
                            else if (dicClientTypes[index[i]] == "array1")
                            {
                                List<List<int>> test = GetStringToList2DInt(pair.Value[index[i]]);
                                singleField.SetValue(item, test);
                            }
                            else if (dicClientTypes[index[i]] == "int" || dicClientTypes[index[i]] == "uint" || dicClientTypes[index[i]] == "float")
                            {
                                singleField.SetValue(item, Convert.ChangeType(pair.Value[index[i]], typeof(int)));
                            }
                            else
                                throw new Exception("配置了客户端不支持的类型");
                        }
                    }
                    addMethod.Invoke(asset, new object[] { item });
                }

                string assetPath = "Assets" + _clientPath + "DataTable" + csUpName + ".asset";
                if (File.Exists(assetPath)) File.Delete(assetPath);
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.Refresh();
                Debug.Log(curFileName + "  assest  file  export complete !");
            }

        }
        catch (Exception e)
        {
            Debug.LogError("导出 " + curFileName + ".lua 文件失败，原因为：" + e);
            return false;
        }
        if(!success)
        {
            Debug.Log("导出 " + curFileName + ".lua 文件成功");
            if (show)
            {
                EditorUtility.DisplayDialog("导出json文件", "导出" + curFileName + ".lua 成功", "干得漂亮");
            }
            fi.toJson = true;
        }
        return success;
    }

    private string RoundAndSquareToBraceJson(string str)
    {
        StringBuilder sb = new StringBuilder();
        int yinhao = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '"') yinhao ^= 1;
            if (str[i] == '\'') yinhao ^= 2;
            if (yinhao == 0)
            {
                if (str[i] == '[' || str[i] == '(')
                    sb.Append('[');
                else if (str[i] == ']' || str[i] == ')')
                    sb.Append(']');
                else
                    sb.Append(str[i]);
            }
            else
                sb.Append(str[i]);
        }
        return sb.ToString();
    }
    /// <summary>
    /// 解析{[1,2,3],[4,5,6]}这类的字符返回的int[][]数组
    /// </summary>
    public static int[][] Get2DArrayStringToInt4(string str)
    {
        try
        {
            if (str.Length >= 4)
            {
                string pStr = str.Substring(2, str.Length - 4);
                if (pStr.Length > 0)
                {
                    string pStr2;
                    string[] proS1 = pStr.Replace("],[", "|").Split('|');
                    string[] proS2;
                    if (proS1.Length > 0)
                    {
                        int[][] result = new int[proS1.Length][];
                        for (int i = 0, length1 = proS1.Length; i < length1; i++)
                        {
                            pStr2 = proS1[i];
                            proS2 = pStr2.Split(',');
                            result[i] = new int[proS2.Length];
                            for (int j = 0, length2 = proS2.Length; j < length2; j++)
                            {
                                result[i][j] = int.Parse(proS2[j]);
                            }
                        }
                        return result;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Get2DArrayStringToInt4 转换失败 " + str);
            throw (e);
        }
        return null;
    }
    /// <summary>
    /// 解析[{1,2,3},{4,5,6}]这类的字符返回的int[][]数组
    /// </summary>
    public static int[][] Get2DArrayStringToInt2(string str)
    {
        try
        {
            if(str.Length>=4)
            {
                string pStr = str.Substring(2, str.Length - 4);
                if (pStr.Length>0)
                {
                    string pStr2;
                    string[] proS1 = pStr.Replace("},{", "|").Split('|');
                    string[] proS2;
                    if (proS1.Length>0)
                    {
                        int[][] result = new int[proS1.Length][];
                        for (int i =0,length1=proS1.Length;i<length1 ;i++)
                        {
                            pStr2 = proS1[i];
                            proS2 = pStr2.Split(',');
                            result[i] = new int[proS2.Length];
                            for (int j = 0,length2 = proS2.Length;j<length2;j++)
                            {
                                result[i][j] = int.Parse(proS2[j]);
                            }
                        }
                        return result;
                    }
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError("Get2DArrayStringToInt2 转换失败 " + str);
            throw (e);
        }
        return null;
    }
    /// <summary>
    /// 解析[[1,2,3],[4,5,6]]这类的字符返回的int[][]数组
    /// </summary>
    public static int[][] Get2DArrayStringToInt3(string str)
    {
        try
        {
            if (str.Length >= 4)
            {
                string pStr = str.Substring(2, str.Length - 4);
                if (pStr.Length > 0)
                {
                    string pStr2;
                    string[] proS1 = pStr.Replace("],[", "|").Split('|');
                    string[] proS2;
                    if (proS1.Length > 0)
                    {
                        int[][] result = new int[proS1.Length][];
                        for (int i = 0, length1 = proS1.Length; i < length1; i++)
                        {
                            pStr2 = proS1[i];
                            proS2 = pStr2.Split(',');
                            result[i] = new int[proS2.Length];
                            for (int j = 0, length2 = proS2.Length; j < length2; j++)
                            {
                                result[i][j] = int.Parse(proS2[j]);
                            }
                        }
                        return result;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Get2DArrayStringToInt3 转换失败 " + str);
            throw (e);
        }
        return null;
    }
    /// <summary>
    /// 解析[1,2,3]这类的字符返回的int[]数组
    /// </summary>
    public static int[] GetStringToInt(string str)
    {
        if (str == null||str.Length<=2)
        {
            return new int[0];
        }

        string pStr = str.Substring(1, str.Length - 2);
        string[] proS = pStr.Split(',');
        int lens = proS.Length;
        int[] r = new int[lens];
        for (int i =0;i<lens;i++)
        {
            try
            {
                r[i] = int.Parse(proS[i].Trim());
            }
            catch (System.Exception e)
            {
                Debug.LogError("GetStringToInt 转换失败 " + str);
                throw (e);
            }
        }
        return r;
    }

    public static List<List<int>> GetStringToList2DInt(string str)
    {
        try
        {
            if (str.Length >= 4)
            {
                string pStr = str.Substring(2, str.Length - 4);
                if (pStr.Length > 0)
                {
                    string pStr2;
                    string[] proS1 = pStr.Replace("],[", "|").Split('|');
                    string[] proS2;
                    if (proS1.Length > 0)
                    {
                        List<List<int>> tempReturn = new List<List<int>>();
                        for (int i = 0, length1 = proS1.Length; i < length1; i++)
                        {

                            pStr2 = proS1[i];
                            proS2 = pStr2.Split(',');
                            List<int> tempInt = new List<int>();
                            for (int j = 0, length2 = proS2.Length; j < length2; j++)
                            {
                                tempInt.Add(int.Parse(proS2[j]));
                            }
                            tempReturn.Add(tempInt);
                        }
                        return tempReturn;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetStringToList2DInt 转换失败 " + str);
            throw (e);
        }
        return null;
    }















    /// <summary>
    /// excel数据类
    /// </summary>
    public class FileInfoData
    {
        /// <summary>
        /// Excel表的路径
        /// </summary>
        public string filePath;
        public string fileNameWithPostfix;
        /// <summary>
        /// Excel表的名字
        /// </summary>
        public string fileName;
        public string fileNewName;
        /// <summary>
        /// Excel表的MD5存储
        /// </summary>
        public string excelMD5Save;
        /// <summary>
        /// Excel表的新MD5
        /// </summary>
        public string excelMD5New;
        public bool toLua;
        public bool toJson;
        public bool toScript;
        public bool toAsset;
        public bool toJava;
    }

    public class MD5Item
    {
        public MD5Item(string md5, bool tolua, bool tojson, bool toscript, bool toasset, bool tojava)
        {
            MD5 = md5;
            toLua = tolua;
            toJson = tojson;
            toScript = toscript;
            toAsset = toasset;
            toJava = tojava;
        }
        public string MD5;
        public bool toLua;
        public bool toJson;
        public bool toScript;
        public bool toAsset;
        public bool toJava;
    }

    private const int MD5Version = 0;
    private Dictionary<string, MD5Item> MD5List = new Dictionary<string, MD5Item>();

    private void ReadMD5()
    { }
    private void WriteMD5()
    { }

    private List<FileInfoData> fileInfos = new List<FileInfoData>();
    /// <summary>
    /// 获取所有的Excel文件
    /// </summary>
    private void GetExcelFileInfos()
    {
        //string curExcelFolderLocation = Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets")) + "数据表/" + _threePackName + "/";
        string curExcelFolderLocation = _excelPath + "/" + _threePackName + "/";
        fileInfos.Clear();

        List<string> fileList = new List<string>();

        fileList.AddRange(Directory.GetFiles(curExcelFolderLocation, "*", SearchOption.AllDirectories));
        fileList.Sort();

        for (int i = 0; i < fileList.Count; i++)
        {
            string filePath = fileList[i].Replace("\\", "/");
            string fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
            if (!(fileName.EndsWith(".xlsx") || fileName.EndsWith(".xlsm") || fileName.EndsWith(".xls")) || fileName.StartsWith("~$") 
                || fileName.IndexOf("name_filter") >= 0 || fileName.IndexOf("chat_filter") >= 0 )
            {
                continue;
            }
            string fileNameWithPostfix = fileName;
            fileName = fileName.Substring(0, fileName.IndexOf("."));

            FileInfoData curFileInfo = new FileInfoData();
            curFileInfo.filePath = filePath;
            curFileInfo.fileNameWithPostfix = fileNameWithPostfix;
            curFileInfo.fileName = fileName;
            curFileInfo.fileNewName = fileName.ToUpper().Substring(0, 1) + fileName.Substring(1);

            MD5Item item = null;
            if (MD5List.TryGetValue(fileName, out item))
            {
                curFileInfo.excelMD5Save = item.MD5;
                curFileInfo.toLua = item.toLua;
                curFileInfo.toJson = item.toJson;
                curFileInfo.toScript = item.toScript;
                curFileInfo.toAsset = item.toAsset;
                curFileInfo.toJava = item.toJava;
            }
            else
            {
                curFileInfo.excelMD5Save = "";
                curFileInfo.toLua = false;
                curFileInfo.toJson = false;
                curFileInfo.toScript = false;
                curFileInfo.toAsset = false;
                curFileInfo.toJava = false;
            }
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            curFileInfo.excelMD5New = MD5Hash.Get(fs);
            if (curFileInfo.excelMD5New != curFileInfo.excelMD5Save)
            {
                curFileInfo.toLua = false;
                curFileInfo.toJson = false;
                curFileInfo.toScript = false;
                curFileInfo.toAsset = false;
                curFileInfo.toJava = false;
            }
            fs.Close();

            fileInfos.Add(curFileInfo);
        }
        fileInfos.Sort(compareFileInfos);
    }

    static int compareFileInfos(FileInfoData a,FileInfoData b)
    {
        return string.Compare(a.fileName, b.fileName);
    }
}


