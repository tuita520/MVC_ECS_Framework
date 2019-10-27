//=====================================================
// - FileName:      Menu.cs
// - Created:       #AuthorName#
// - UserName:      #CreateTime#
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using UnityEngine;
using UnityEditor;

public class Menu {
    [MenuItem("Tools/Excel导表工具")]
    public static void OpenExcelWindows()
    {
        ExcelWindows win = ExcelWindows.GetWindow<ExcelWindows>();
        win.titleContent = new GUIContent("Excel导表工具");
        win.Show();
    }
    [MenuItem("Tools/地图编辑器")]
    public static void OpenMapEditorWindows()
    {
        MapWindows win = EditorStyleViewer.GetWindow<MapWindows>();
        win.titleContent = new GUIContent("地图编辑器");
        win.Show();
    }
    [MenuItem("Tools/Atlas打包工具")]
    public static void OpenIconEditorWindows()
    {
        AtlasTool.updateAtlas();
    }
    [MenuItem("Tools/文件名检测工具")]
    public static void OpenFileEditorWindows()
    {
        EditorStyleViewer win = EditorStyleViewer.GetWindow<EditorStyleViewer>();
        win.titleContent = new GUIContent("文件名检测工具");
        win.Show();
    }
    [MenuItem("Tools/GUI样式查看器")]
    public static void OpenGUILookWindows()
    {
        EditorStyleViewer win = EditorStyleViewer.GetWindow<EditorStyleViewer>();
        win.titleContent = new GUIContent("GUI样式查看器");
        win.Show();
    }
    [MenuItem("打包出包/AB打包/Android资源打包")]
    public static void packageABAssetBundleAndroid()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
        Caching.ClearCache();
        ABBuildTools.Build();
    }
    [MenuItem("打包出包/AB打包/Iphone资源打包")]
    public static void packageABAssetBundleIphone()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        Caching.ClearCache();
        ABBuildTools.Build();
    }
    [MenuItem("打包出包/AB打包/Windows资源打包")]
    public static void packageABAssetBundleWindows()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        Caching.ClearCache();
        ABBuildTools.Build();
    }
}
