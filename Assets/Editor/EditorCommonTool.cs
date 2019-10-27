//=====================================================
// - FileName:      EditorCommonTool.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EditorCommonTool : Editor{
    
    static void CreateImage()
    {
        GameObject go = new GameObject("Imgae", typeof(Image));
        var img = go.GetComponent<Image>();
        img.raycastTarget = false;
        //img.gameObject.layer = 
    }
}
