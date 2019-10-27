//=====================================================
// - FileName:      StarUIMainView.cs
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
    public class StarUIMainView
    {
        private Image loadingSlider;
        private Text loadingText;
        private GameObject selfGo;

        private bool AudioLoadComplete = false;
        private bool USpriteLoadComplete = false;
        private bool UTextureLoadComplete = false;
        private bool DataLoadComplete = false;

        public void Init(GameObject canGo)
        {
            selfGo = canGo;
            loadingSlider = UGUITool.FindInChild("UnityEngine.UI.Image", selfGo, "loadingSlider") as Image;
            loadingText = UGUITool.FindInChild("UnityEngine.UI.Text", selfGo, "loadingText") as Text;
            RegisterUpdateHandler();
        }
        /// <summary>
        /// 注册事件，界面隐藏移除的事件
        /// </summary>
        public void RegisterUpdateHandler()
        {
            EventMgr.Instance.AddEventListener<string, int>(UpdataConst.LOADER_PROGRESS, UpdateSlider);
            EventMgr.Instance.AddEventListener<string, int>(UpdataConst.LOADER_COMPLETED, UpdateSlider);

            EventMgr.Instance.AddEventListener<string, int, int>(GlobalEvent.USPRITE_LOAD_PROGRESS, UpdateInitSlider);
            EventMgr.Instance.AddEventListener<string, int, int>(GlobalEvent.UTEXTURE_LOAD_PROGRESS, UpdateInitSlider);
            EventMgr.Instance.AddEventListener<string, int, int>(GlobalEvent.DATA_LOAD_PROGRESS, UpdateInitSlider);
            EventMgr.Instance.AddEventListener<string, int, int>(GlobalEvent.AUDIO_LOAD_PROGRESS, UpdateInitSlider);

        }

        void UpdateInitSlider(string arg1, int arg2, int arg3)
        {
            float tempDou = (float)arg3 / 100.0f;
            switch (arg2)
            {
                case 1:
                    if (tempDou >= 1)
                        USpriteLoadComplete = true;
                    break;
                case 2:
                    if (tempDou >= 1)
                        UTextureLoadComplete = true;
                    break;
                case 3:
                    if (tempDou >= 1)
                        DataLoadComplete = true;
                    break;
                case 4:
                    if (tempDou >= 1)
                        AudioLoadComplete = true;
                    break;
            }
            loadingText.text = arg1;
            loadingSlider.fillAmount = tempDou;
            if(USpriteLoadComplete && UTextureLoadComplete && DataLoadComplete && AudioLoadComplete)
            {
                EventMgr.Instance.TriggerEvent(GlobalEvent.INIT_COMPLETE);
            }
        }

        void UpdateSlider(string arg1, int arg2)
        {
            loadingText.text = arg1;
            loadingSlider.fillAmount = (float)arg2 / 100.0f;
        }

        /// <summary>
        /// 移除事件，界面隐藏移除的事件
        /// </summary>
        public void CancelUpdateHandler()
        {
            EventMgr.Instance.RemoveEventListener<string, int>(UpdataConst.LOADER_PROGRESS, UpdateSlider);
            EventMgr.Instance.RemoveEventListener<string, int>(UpdataConst.LOADER_COMPLETED, UpdateSlider);

            EventMgr.Instance.RemoveEventListener<string, int, int>(GlobalEvent.USPRITE_LOAD_PROGRESS, UpdateInitSlider);
            EventMgr.Instance.RemoveEventListener<string, int, int>(GlobalEvent.UTEXTURE_LOAD_PROGRESS, UpdateInitSlider);
            EventMgr.Instance.RemoveEventListener<string, int, int>(GlobalEvent.DATA_LOAD_PROGRESS, UpdateInitSlider);
            EventMgr.Instance.RemoveEventListener<string, int, int>(GlobalEvent.AUDIO_LOAD_PROGRESS, UpdateInitSlider);
        }
        public void CloseView()
        {
            CancelUpdateHandler();
            Destory();
        }
        public void Destory()
        {

        }
    }
}
