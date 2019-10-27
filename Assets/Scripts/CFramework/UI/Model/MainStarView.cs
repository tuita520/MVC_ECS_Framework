//=====================================================
// - FileName:      GameBeginMainView.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zero.ZeroEngine.Common;

namespace Zero.ZeroEngine.UI
{
    public class MainStarView : InterfaceView
    {
        Button star_Btn;
        Button setting_Btn;
        Button exit_Btn;
        RawImage star_Bg;

        public override void Init()
        {
            base.Init();
            maskBgBoo = true;

            star_Btn = FindInChild("UnityEngine.UI.Button", "Group/StarBtn") as Button;
            setting_Btn = FindInChild("UnityEngine.UI.Button", "Group/SettingBtn") as Button;
            exit_Btn = FindInChild("UnityEngine.UI.Button", "Group/ExitBtn") as Button;
            star_Bg = FindInChild("UnityEngine.UI.RawImage", "Bg") as RawImage;
        }
        public override string ViewName()
        {
            return "MainStarView";
        }
        public override void OpenView(int subIndex, int arg1 = -1, int arg2 = -1, int arg3 = -1, string arg4 = null, string arg5 = null, string arg6 = null)
        {
        }
        public override void RegisterUpdateHandler()
        {
            AddComponentCallbackListnener(star_Btn.gameObject, UIEventEnum.ON_CLICK, OnClickStarBtn);
            AddComponentCallbackListnener(setting_Btn.gameObject, UIEventEnum.ON_CLICK, OnClickSettingBtn);
            AddComponentCallbackListnener(exit_Btn.gameObject, UIEventEnum.ON_CLICK, OnClickExitBtn);
        }
        public override void RegisterUpdateHandlerHold()
        {
        }
        public override void CancelUpdateHandler()
        {
            RemoveComponentCallbackListnener(star_Btn.gameObject, UIEventEnum.ON_CLICK);
            RemoveComponentCallbackListnener(setting_Btn.gameObject, UIEventEnum.ON_CLICK);
            RemoveComponentCallbackListnener(exit_Btn.gameObject, UIEventEnum.ON_CLICK);
        }
        public override void CancelUpdateHandlerHold()
        {
        }
        void OnClickStarBtn(GameObject canObj)
        {

        }

        void OnClickSettingBtn(GameObject canObj)
        {

        }

        void OnClickExitBtn(GameObject canObj)
        {
            Application.Quit();
        }
        public override void HandleAfterOpenView()
        {
            base.HandleAfterOpenView();
            UTextureMgr.Instance.SetTexture("StarBg_01.png", star_Bg.gameObject, false);
            AudioMgr.Instance.PlayBgAudio("op.mp3", true);
        }
        public override void HandleAfterOpenSubViews()
        {
        }
        public override void HandleBeforeCloseView()
        {
        }
        public override void Update(double deltatime)
        {
        }
        public override void LateUpdate()
        {
        }
        public override void VisibleChange(bool activeBoo)
        {
        }
        public override void Destory()
        {
        }
        public override int Layer()
        {
            return UIMgr.HIGH_LAYER_INT;
        }
    }
}
