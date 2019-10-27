//=====================================================
// - FileName:      LoadingView.cs
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
    public class LoadingView : InterfaceView
    {
        RawImage loadingBg;
        Image loadingSlider;
        Text loadingText;

        public override void Init()
        {
            base.Init();
            maskBgBoo = true;
            hideOtherViewBoo = true;

            loadingBg = FindInChild("UnityEngine.UI.RawImage", "RawImage") as RawImage;
            loadingSlider = FindInChild("UnityEngine.UI.Image", "loadingSlider") as Image;
            loadingText = FindInChild("UnityEngine.UI.Text", "loadingText") as Text;
        }
        public override string ViewName()
        {
            return "LoadingView";
        }
        public override void OpenView(int subIndex, int arg1 = -1, int arg2 = -1, int arg3 = -1, string arg4 = null, string arg5 = null, string arg6 = null)
        {
        }
        public override void RegisterUpdateHandler()
        {

        }
        public override void RegisterUpdateHandlerHold()
        {
        }
        public override void CancelUpdateHandler()
        {

        }
        public override void CancelUpdateHandlerHold()
        {
        }
        public void UpdateSlider(string arg1,int arg2)
        {
            loadingText.text = arg1;
            loadingSlider.fillAmount = (float)arg2 / 100f;
        }
        public override void HandleAfterOpenView()
        {
            base.HandleAfterOpenView();
            switch (Random.Range(1, 3))
            {
                case 1:
                    UTextureMgr.Instance.SetTexture("LoadingBg_01", loadingBg.gameObject, false);
                    break;
                case 2:
                    UTextureMgr.Instance.SetTexture("LoadingBg_02", loadingBg.gameObject, false);
                    break;
                case 3:
                    UTextureMgr.Instance.SetTexture("LoadingBg_03", loadingBg.gameObject, false);
                    break;
            }
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
