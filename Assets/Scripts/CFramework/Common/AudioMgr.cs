//=====================================================
// - FileName:      AudioMgr.cs
// - Created:       mahuibao
// - UserName:      2019-01-13
// - Email:         1023276156@qq.com
// - Description:   音效管理层
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.SceneFrame;
using Zero.ZeroEngine.Util;

//=====================================================
// - 1.播放UI效果音是只有一个播放器的，目前只有除了UI效果音以外是有多个播放器的
// - 2.
// - 3.
// - 4.
// - 5.
// - 6.
//======================================================

namespace Zero.ZeroEngine.Common
{
    public class AudioObj
    {
        public AudioClip m_AudioClip = null;
        public bool m_ClearScene = true;

        public void Reset()
        {
            m_AudioClip = null;
            m_ClearScene = true;
        }
    }

    public class EffectAudioPrefab
    {
        //唯一ID，也是协程中的唯一ID
        public string m_guid = null;
        public Transform m_AudioTrs = null;
        public AudioSource m_AudioSou = null;

        //以下是为了协程中进行逻辑的属性（针对协程使用Audio预置的方式）
        public bool m_UsingBoo = false;
        public bool m_ResetParentBoo = false;

        public void Reset()
        {
            m_guid = null;
            m_AudioTrs = null;
            m_AudioSou = null;
            m_UsingBoo = false;
            m_ResetParentBoo = false;
        }
    }

    /// <summary>
    /// 音效管理层
    /// </summary>
    public class AudioMgr : SingletonMono<AudioMgr>
    {
        public const string MUSIC_PATH = "Assets/Resources/Music/";

        //Audio节点（用来存放AudioObj）
        public Transform SoundStageTrs;
        //缓存已经加载好的音效文件
        private Dictionary<uint, AudioObj> AudioAssetDic = new Dictionary<uint, AudioObj>();
        //音频文件类对象池
        private ClassObjectPool<AudioObj> AudioObjClassPool = new ClassObjectPool<AudioObj>(20);

        //以下是为了针对以后使用3D音效使用
        //正在使用的音效预置字典
        private Dictionary<string, EffectAudioPrefab> EffectAudioDic = new Dictionary<string, EffectAudioPrefab>();
        //效果音预置回收池
        private Queue<EffectAudioPrefab> EffectAudioRecyclePool = new Queue<EffectAudioPrefab>();
        //效果音预置类对象池
        private ClassObjectPool<EffectAudioPrefab> EffectAudioClassPool = new ClassObjectPool<EffectAudioPrefab>(40);
        //正在异步加载效果音列表（里面包括了全部需要设置Audio的对象）
        //（其实资源加载中，异步加载已经做了回调的累加，加载完资源后会回调所有，所以可以选择不在这里设置的）
        //在两种方式的，第二种时候的是资源加载内部的累加回调方式
        private Dictionary<uint, List<EffectAudioPrefab>> EffectAudioAsyncDic = new Dictionary<uint, List<EffectAudioPrefab>>();

        private const string guid_Pre = "EAudioCor";
        private int guid_Post = 0;


        public AudioSource bgAudio;
        private float bgAudioVolume = 1.0f;

        public AudioSource uiAudio;
        private float EffectAudioVolume = 1.0f;

        public void Init(Transform canTrs)
        {
            ZLogger.Info("音效管理层初始化");
            SoundStageTrs = canTrs;
            bgAudio = GameObject.Find("BGStage").GetComponent<AudioSource>();
            bgAudio.loop = true;
            uiAudio = GameObject.Find("UIStage").GetComponent<AudioSource>();

            EventMgr.Instance.AddEventListener(ABMgrConst.AB_CONFIG_LOAD_COMPLETE, LoadHoldAudio);
            EventMgr.Instance.AddEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, ClearCache);
        }
        public void Clear()
        {
            EventMgr.Instance.RemoveEventListener(ABMgrConst.AB_CONFIG_LOAD_COMPLETE, LoadHoldAudio);
            EventMgr.Instance.RemoveEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, ClearCache);
        }
        public void AfterInit()
        {

        }

        public void LoadHoldAudio()
        {
            EventMgr.Instance.TriggerEvent(GlobalEvent.AUDIO_LOAD_PROGRESS, "音乐资源初始化...开始...", 4, 0);

            EventMgr.Instance.TriggerEvent(GlobalEvent.AUDIO_LOAD_PROGRESS, "音乐资源初始化中...", 4, 60);

            EventMgr.Instance.TriggerEvent(GlobalEvent.AUDIO_LOAD_PROGRESS, "音乐资源初始化完成!!!", 4, 100);
        }

        public void ClearCache()
        {
            List<uint> tempClearKeyList = new List<uint>();
            foreach (uint tempKey in AudioAssetDic.Keys)
            {
                AudioObj tempAudioObj = AudioAssetDic[tempKey];
                if (tempAudioObj.m_ClearScene)
                {
                    tempClearKeyList.Add(tempKey);
                }
            }
            foreach (uint tempKey in tempClearKeyList)
            {
                AudioObj tempAudioObj;
                if (AudioAssetDic.TryGetValue(tempKey, out tempAudioObj) && tempAudioObj != null)
                {
                    ResourcesMgr.Instance.ReleaseResource(tempAudioObj.m_AudioClip, true);
                    tempAudioObj.Reset();
                    AudioObjClassPool.Recycle(tempAudioObj);
                    AudioAssetDic.Remove(tempKey);
                }
            }
            tempClearKeyList.Clear();
        }

        public void SetBgVolume(float canVolume)
        {
            bgAudioVolume = canVolume;
            bgAudio.volume = canVolume;
            if (bgAudioVolume == 0f)
            {
                bgAudio.Stop();
            }
            else
            {
                bgAudio.Play();
            }
        }
        public void SetEffectVolume(float canVolume)
        {
            EffectAudioVolume = canVolume;
            uiAudio.volume = canVolume;
            if (EffectAudioVolume == 0f)
            {
                uiAudio.Stop();
            }
            else
            {
                uiAudio.Play();
            }
            foreach (EffectAudioPrefab tempEffectAudio in EffectAudioDic.Values)
            {
                tempEffectAudio.m_AudioSou.volume = canVolume;
                if (EffectAudioVolume == 0f)
                {
                    tempEffectAudio.m_AudioSou.Stop();
                }
                else
                {
                    tempEffectAudio.m_AudioSou.Play();
                }
            }
        }
        private string GetEffectAudioGuid()
        {
            guid_Post += 1;
            string tempStr = string.Concat(guid_Pre, "_", guid_Post.ToString());
            return tempStr;
        }

        public void PlayBgAudio(string musicName, bool isLoopBoo = true, bool clearScene = true)
        {
            string tempPath = MUSIC_PATH + musicName;
            uint tempCrc = CRC32.GetCRC32(tempPath);
            AudioObj tempAudioObj;
            if(AudioAssetDic.TryGetValue(tempCrc,out tempAudioObj) && tempAudioObj != null)
            {
                bgAudio.clip = tempAudioObj.m_AudioClip;
                bgAudio.volume = bgAudioVolume;
                bgAudio.loop = isLoopBoo;
                if (bgAudioVolume > 0f) bgAudio.Play();
                else bgAudio.Stop();
            }
            else
            {
                ResourcesMgr.Instance.AsyncLoadResource(tempPath, LoadBgAssetFinish, LoadResPriority.RES_SLOW, false, clearScene, isLoopBoo);
            }
        }

        void LoadBgAssetFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        {
            uint tempCrc = CRC32.GetCRC32(path);
            AudioObj tempObj = AudioObjClassPool.Spawn(true);
            tempObj.m_AudioClip = obj as AudioClip;
            tempObj.m_ClearScene = (bool)param1;
            AudioAssetDic.Add(tempCrc, tempObj);
            bgAudio.clip = tempObj.m_AudioClip;
            bgAudio.volume = bgAudioVolume;
            bgAudio.loop = (bool)param2;
            if (bgAudioVolume > 0f) bgAudio.Play();
            else bgAudio.Stop();
        }

        public void StopBgAudio()
        {
            bgAudio.Stop();
        }

        public void PlayUiAudio(string musicName, bool isLoopBoo = false, bool clearScene = false)
        {
            string tempPath = MUSIC_PATH + musicName;
            uint tempCrc = CRC32.GetCRC32(tempPath);
            AudioObj tempAudioObj;
            if (AudioAssetDic.TryGetValue(tempCrc, out tempAudioObj) && tempAudioObj != null)
            {
                uiAudio.clip = tempAudioObj.m_AudioClip;
                uiAudio.volume = EffectAudioVolume;
                uiAudio.loop = isLoopBoo;
                if (EffectAudioVolume > 0f) bgAudio.Play();
                else bgAudio.Stop();
            }
            else
            {
                ResourcesMgr.Instance.AsyncLoadResource(tempPath, LoadUiAssetFinish, LoadResPriority.RES_MIDDLE,false, clearScene, isLoopBoo);
            }
        }

        void LoadUiAssetFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        {
            uint tempCrc = CRC32.GetCRC32(path);
            AudioObj tempObj = AudioObjClassPool.Spawn(true);
            tempObj.m_AudioClip = obj as AudioClip;
            tempObj.m_ClearScene = (bool)param1;
            AudioAssetDic.Add(tempCrc, tempObj);
            uiAudio.clip = tempObj.m_AudioClip;
            uiAudio.volume = EffectAudioVolume;
            uiAudio.loop = (bool)param2;
            if (EffectAudioVolume > 0f) bgAudio.Play();
            else bgAudio.Stop();
        }

        public void PreLoadAudio(string musicName, bool clearScene = false)
        {
            string tempPath = MUSIC_PATH + musicName;
            uint tempCrc = CRC32.GetCRC32(tempPath);
            AudioObj tempAudioObj;
            if (AudioAssetDic.TryGetValue(tempCrc, out tempAudioObj) && tempAudioObj != null)
            {
                return;
            }
            else
            {
                ResourcesMgr.Instance.AsyncLoadResource(tempPath, PreLoadFinish, LoadResPriority.RES_SLOW, false, clearScene);
            }
        }

        void PreLoadFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        {
            uint tempCrc = CRC32.GetCRC32(path);
            AudioObj tempObj = AudioObjClassPool.Spawn(true);
            tempObj.m_AudioClip = obj as AudioClip;
            tempObj.m_ClearScene = (bool)param1;
            AudioAssetDic.Add(tempCrc, tempObj);
        }

        #region -------------以下是使用协程来对播放完效果音的预置进行处理的方式---------------
        public void PlayEffectAudio(string musicName, bool is3DBoo, Vector3 canVector3, Transform canTrs = null, bool isLoopBoo = false, bool clearScene = false)
        {
            string tempPath = MUSIC_PATH + musicName;
            uint tempCrc = CRC32.GetCRC32(tempPath);
            AudioObj tempAudioObj;
            if (AudioAssetDic.TryGetValue(tempCrc, out tempAudioObj) && tempAudioObj != null)
            {
                EffectAudioPrefab tempEffectPrefabObj;
                if (EffectAudioRecyclePool.Count > 0)
                {
                    tempEffectPrefabObj = EffectAudioRecyclePool.Dequeue();
                    if (canTrs)
                    {
                        tempEffectPrefabObj.m_AudioTrs.SetParent(canTrs, false);
                        tempEffectPrefabObj.m_ResetParentBoo = true;
                    }
                    tempEffectPrefabObj.m_AudioTrs.transform.localPosition = canVector3;
                    tempEffectPrefabObj.m_AudioSou.spatialBlend = is3DBoo ? 1 : 0;
                    tempEffectPrefabObj.m_AudioSou.clip = tempAudioObj.m_AudioClip;
                    tempEffectPrefabObj.m_AudioSou.volume = EffectAudioVolume;
                    tempEffectPrefabObj.m_AudioSou.loop = isLoopBoo;
                    if (EffectAudioVolume > 0f)
                        tempEffectPrefabObj.m_AudioSou.Play();
                    else
                        tempEffectPrefabObj.m_AudioSou.Stop();
                    tempEffectPrefabObj.m_UsingBoo = true;
                    EffectAudioDic.Add(tempEffectPrefabObj.m_guid, tempEffectPrefabObj);
                }
                else
                {
                    tempEffectPrefabObj = EffectAudioClassPool.Spawn(true);
                    tempEffectPrefabObj.m_guid = GetEffectAudioGuid();
                    GameObject tempObj = new GameObject(tempEffectPrefabObj.m_guid + "_Stage");
                    tempEffectPrefabObj.m_AudioTrs = tempObj.transform;
                    if (canTrs)
                    {
                        tempEffectPrefabObj.m_AudioTrs.SetParent(canTrs, false);
                        tempEffectPrefabObj.m_ResetParentBoo = true;
                    }
                    else
                        tempEffectPrefabObj.m_AudioTrs.SetParent(SoundStageTrs, false);
                    tempEffectPrefabObj.m_AudioTrs.localPosition = canVector3;
                    tempEffectPrefabObj.m_AudioSou = tempObj.AddComponent<AudioSource>();
                    tempEffectPrefabObj.m_AudioSou.spatialBlend = is3DBoo ? 1 : 0;
                    tempEffectPrefabObj.m_AudioSou.volume = EffectAudioVolume;
                    tempEffectPrefabObj.m_AudioSou.clip = tempAudioObj.m_AudioClip;
                    tempEffectPrefabObj.m_AudioSou.loop = isLoopBoo;
                    if (EffectAudioVolume > 0f)
                        tempEffectPrefabObj.m_AudioSou.Play();
                    else
                        tempEffectPrefabObj.m_AudioSou.Stop();
                    tempEffectPrefabObj.m_UsingBoo = true;
                    EffectAudioDic.Add(tempEffectPrefabObj.m_guid, tempEffectPrefabObj);
                    CoroutineMgr.Instance.StartCoroutine(tempEffectPrefabObj.m_guid, EffectAudioTimerCor(tempEffectPrefabObj.m_guid));
                }
            }
            else
            {
                EffectAudioPrefab tempEffectPrefabObj;
                if (EffectAudioRecyclePool.Count > 0)
                {
                    tempEffectPrefabObj = EffectAudioRecyclePool.Dequeue();
                    if (canTrs)
                    {
                        tempEffectPrefabObj.m_AudioTrs.SetParent(canTrs, false);
                        tempEffectPrefabObj.m_ResetParentBoo = true;
                    }
                    tempEffectPrefabObj.m_AudioTrs.transform.localPosition = canVector3;
                    tempEffectPrefabObj.m_AudioSou.spatialBlend = is3DBoo ? 1 : 0;
                }
                else
                {
                    tempEffectPrefabObj = EffectAudioClassPool.Spawn(true);
                    tempEffectPrefabObj.m_guid = GetEffectAudioGuid();
                    GameObject tempObj = new GameObject(tempEffectPrefabObj.m_guid + "_Stage");
                    tempEffectPrefabObj.m_AudioTrs = tempObj.transform;
                    if (canTrs)
                    {
                        tempEffectPrefabObj.m_AudioTrs.SetParent(canTrs, false);
                        tempEffectPrefabObj.m_ResetParentBoo = true;
                    }
                    else
                        tempEffectPrefabObj.m_AudioTrs.SetParent(SoundStageTrs, false);
                    tempEffectPrefabObj.m_AudioTrs.localPosition = canVector3;
                    tempEffectPrefabObj.m_AudioSou = tempObj.AddComponent<AudioSource>();
                    tempEffectPrefabObj.m_AudioSou.spatialBlend = is3DBoo ? 1 : 0;
                }
                ResourcesMgr.Instance.AsyncLoadResource(tempPath, LoadEffectAssetFinish, LoadResPriority.RES_HIGHT, false, clearScene, isLoopBoo, tempEffectPrefabObj);
            }
        }

        void LoadEffectAssetFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        {
            uint tempCrc = CRC32.GetCRC32(path);
            AudioObj tempObj = AudioObjClassPool.Spawn(true);
            tempObj.m_AudioClip = obj as AudioClip;
            tempObj.m_ClearScene = (bool)param1;
            AudioAssetDic.Add(tempCrc, tempObj);
            EffectAudioPrefab tempEff = param3 as EffectAudioPrefab;
            tempEff.m_AudioSou.clip = tempObj.m_AudioClip;
            tempEff.m_AudioSou.volume = EffectAudioVolume;
            tempEff.m_AudioSou.loop = (bool)param2;
            if (EffectAudioVolume > 0f)
                tempEff.m_AudioSou.Play();
            else
                tempEff.m_AudioSou.Stop();
            tempEff.m_UsingBoo = true;
            EffectAudioDic.Add(tempEff.m_guid, tempEff);
            CoroutineMgr.Instance.StartCoroutine(tempEff.m_guid, EffectAudioTimerCor(tempEff.m_guid));
            
        }

        IEnumerator EffectAudioTimerCor(string canGuid)
        {
            while (true)
            {
                EffectAudioPrefab tempEffectAudioPrefab;
                if(EffectAudioDic.TryGetValue(canGuid,out tempEffectAudioPrefab) && tempEffectAudioPrefab != null)
                {
                    if (tempEffectAudioPrefab.m_UsingBoo)
                    {
                        float tempTimer = tempEffectAudioPrefab.m_AudioSou.clip.length + 0.5f;
                        yield return new WaitForSeconds(tempTimer);
                        if (tempEffectAudioPrefab.m_ResetParentBoo)
                        {
                            tempEffectAudioPrefab.m_AudioTrs.SetParent(SoundStageTrs, false);
                            tempEffectAudioPrefab.m_AudioTrs.localPosition = new Vector3(0, 0, 0);
                            tempEffectAudioPrefab.m_ResetParentBoo = false;
                        }
                        tempEffectAudioPrefab.m_UsingBoo = false;
                        tempEffectAudioPrefab.m_AudioSou.Stop();
                        EffectAudioDic.Remove(canGuid);
                        EffectAudioRecyclePool.Enqueue(tempEffectAudioPrefab);
                        yield return null;
                    }
                    else
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }
        #endregion

        #region-------------------------以下是将效果音预置，放入对象中，成为组件一般的方式，不适用协程-----------------------
        /// <summary>
        /// 设置效果音组件
        /// </summary>
        public EffectAudioPrefab SetAndGetEffectAudioPrefab(bool is3DBoo, Vector3 canVector3, Transform canTrs = null)
        {
            EffectAudioPrefab tempEffectPrefabObj;
            if (EffectAudioRecyclePool.Count > 0)
            {
                tempEffectPrefabObj = EffectAudioRecyclePool.Dequeue();
                if (canTrs)
                {
                    tempEffectPrefabObj.m_AudioTrs.SetParent(canTrs, false);
                    tempEffectPrefabObj.m_ResetParentBoo = true;
                }
                tempEffectPrefabObj.m_AudioTrs.transform.localPosition = canVector3;
                tempEffectPrefabObj.m_AudioSou.spatialBlend = is3DBoo ? 1 : 0;
                tempEffectPrefabObj.m_AudioSou.volume = EffectAudioVolume;
                tempEffectPrefabObj.m_UsingBoo = true;
                EffectAudioDic.Add(tempEffectPrefabObj.m_guid, tempEffectPrefabObj);
            }
            else
            {
                tempEffectPrefabObj = EffectAudioClassPool.Spawn(true);
                tempEffectPrefabObj.m_guid = GetEffectAudioGuid();
                GameObject tempObj = new GameObject(tempEffectPrefabObj.m_guid + "_Stage");
                tempEffectPrefabObj.m_AudioTrs = tempObj.transform;
                if (canTrs)
                {
                    tempEffectPrefabObj.m_AudioTrs.SetParent(canTrs, false);
                    tempEffectPrefabObj.m_ResetParentBoo = true;
                }
                else
                    tempEffectPrefabObj.m_AudioTrs.SetParent(SoundStageTrs, false);
                tempEffectPrefabObj.m_AudioTrs.localPosition = canVector3;
                tempEffectPrefabObj.m_AudioSou = tempObj.AddComponent<AudioSource>();
                tempEffectPrefabObj.m_AudioSou.spatialBlend = is3DBoo ? 1 : 0;
                tempEffectPrefabObj.m_AudioSou.volume = EffectAudioVolume;
                tempEffectPrefabObj.m_UsingBoo = true;
                EffectAudioDic.Add(tempEffectPrefabObj.m_guid, tempEffectPrefabObj);
            }
            return tempEffectPrefabObj;
        }

        /// <summary>
        /// 播放效果音
        /// </summary>
        public void PlayEffectAudioPrefab(EffectAudioPrefab canEffectAudioPrefab, string musicName, bool isLoopBoo = false, bool clearScene = false)
        {
            string tempPath = MUSIC_PATH + musicName;
            uint tempCrc = CRC32.GetCRC32(tempPath);
            AudioObj tempAudioObj;
            if (AudioAssetDic.TryGetValue(tempCrc, out tempAudioObj) && tempAudioObj != null)
            {
                canEffectAudioPrefab.m_AudioSou.clip = tempAudioObj.m_AudioClip;
                canEffectAudioPrefab.m_AudioSou.volume = EffectAudioVolume;
                canEffectAudioPrefab.m_AudioSou.loop = isLoopBoo;
                if (EffectAudioVolume > 0f)
                    canEffectAudioPrefab.m_AudioSou.Play();
                else
                    canEffectAudioPrefab.m_AudioSou.Stop();

                canEffectAudioPrefab.m_UsingBoo = true;
            }
            else
            {
                ResourcesMgr.Instance.AsyncLoadResource(tempPath, OnLoadEffectAssetFinish, LoadResPriority.RES_HIGHT, false, clearScene, isLoopBoo, canEffectAudioPrefab);
            }
        }

        void OnLoadEffectAssetFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        {
            uint tempCrc = CRC32.GetCRC32(path);
            AudioObj tempObj = AudioObjClassPool.Spawn(true);
            tempObj.m_AudioClip = obj as AudioClip;
            tempObj.m_ClearScene = (bool)param1;
            AudioAssetDic.Add(tempCrc, tempObj);

            EffectAudioPrefab tempEff = param3 as EffectAudioPrefab;
            tempEff.m_AudioSou.clip = tempObj.m_AudioClip;
            tempEff.m_AudioSou.volume = EffectAudioVolume;
            if (EffectAudioVolume > 0f)
                tempEff.m_AudioSou.Play();
            else
                tempEff.m_AudioSou.Stop();
            tempEff.m_UsingBoo = true;
        }

        /// <summary>
        /// 回收音效预置物
        /// </summary>
        public void RecycleEffectAudioPrefab(EffectAudioPrefab canEffectAudioPrefab)
        {
            canEffectAudioPrefab.m_AudioTrs.SetParent(SoundStageTrs, false);
            canEffectAudioPrefab.m_AudioTrs.localPosition = new Vector3(0, 0, 0);
            canEffectAudioPrefab.m_ResetParentBoo = false;
            canEffectAudioPrefab.m_UsingBoo = false;
            canEffectAudioPrefab.m_AudioSou.Stop();
            EffectAudioDic.Remove(canEffectAudioPrefab.m_guid);
            EffectAudioRecyclePool.Enqueue(canEffectAudioPrefab);
        }
        #endregion
    }
}

