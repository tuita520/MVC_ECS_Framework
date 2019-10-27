//=====================================================
// - FileName:      EditorStyleViewer.cs
// - Created:       #AuthorName#
// - UserName:      #CreateTime#
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using UnityEngine;
using UnityEditor;

public class EditorStyleViewer : EditorWindow {

    private Vector2 scrollVector2 = Vector2.zero;
    private string search = "";

    void OnGUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        GUILayout.Space(30);
        search = EditorGUILayout.TextField("", search, "SearchTextField", GUILayout.MaxWidth(position.x / 3));
        GUILayout.Label("", "SearchCancelButtonEmpty");
        GUILayout.EndHorizontal();
        scrollVector2 = GUILayout.BeginScrollView(scrollVector2);
        foreach (GUIStyle style in GUI.skin.customStyles)
        {
            if (style.name.ToLower().Contains(search.ToLower()))
            {
                DrawStyleItem(style);
            }
        }
        GUILayout.EndScrollView();
    }

    void DrawStyleItem(GUIStyle style)
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Space(40);
        EditorGUILayout.SelectableLabel(style.name);
        GUILayout.FlexibleSpace();
        EditorGUILayout.SelectableLabel(style.name, style);
        GUILayout.Space(40);
        EditorGUILayout.SelectableLabel("", style, GUILayout.Height(40), GUILayout.Width(40));
        GUILayout.Space(50);
        if (GUILayout.Button("复制到剪贴板"))
        {
            TextEditor textEditor = new TextEditor();
            textEditor.text = style.name;
            textEditor.OnFocus();
            textEditor.Copy();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
}
