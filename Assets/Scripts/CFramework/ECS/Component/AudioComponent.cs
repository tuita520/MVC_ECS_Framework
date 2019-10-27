//=====================================================
// - FileName:      AudioComponent.cs
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
using Zero.ZeroEngine.Common;

namespace Zero.ZeroEngine.ECS
{
    public class AudioComponent : BaseComponent
    {
        public EffectAudioPrefab effectAudioObj;

        public void Reset()
        {
            effectAudioObj.m_AudioSou.clip = null;
            effectAudioObj.m_AudioSou.Stop();
        }
    }
}