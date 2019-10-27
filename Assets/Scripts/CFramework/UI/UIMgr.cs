//=====================================================
// - FileName:      UIMgr.cs
// - Created:       mahuibao
// - UserName:      2019-01-13
// - Email:         1023276156@qq.com
// - Description:   UI管理层
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zero.Plugins.Base;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.Core;
using Zero.ZeroEngine.Data;
using Zero.ZeroEngine.SceneFrame;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.UI
{
    public class UIMgr : SingletonMono<UIMgr>
    {
        /// <summary>
        /// UI状态枚举
        /// </summary>
        public enum ViewStateType
        {
            NULL = 0,
            CLOSE = 1,
            LOAD_COMPLETE = 2,
            LOAD_FAIL = 3,
            LOAD_SUCESS = 4,
            UNLOAD = 5,
            GIVE_UP_LOAD = 6,
            OPEN = 7,
        }
        public const string CLOSE_LAYER = "UIRoot/Canvas/CloseViewLayer";
        public const string NICK_LAYER = "UIRoot/Canvas/NickLayer";
        public const string LOW_LAYER = "UIRoot/Canvas/LowLayer";
        public const string MIDDLE_LAYER = "UIRoot/Canvas/MiddleLayer";
        public const string DIALOG_LAYER = "UIRoot/Canvas/DialogLayer";
        public const string HIGH_LAYER = "UIRoot/Canvas/HighLayer";
        public const string TOP_LAYER = "UIRoot/Canvas/TopLayer";
        public const string CANVAS_LAYER = "UIRoot/Canvas";

        public const int CLOSE_LAYER_INT = 1;
        public const int NICK_LAYER_INT = 2;
        public const int LOW_LAYER_INT = 3;
        public const int MIDDLE_LAYER_INT = 4;
        public const int DIALOG_LAYER_INT = 5;
        public const int HIGH_LAYER_INT = 6;
        public const int TOP_LAYER_INT = 7;
        public const int USER_LAYER_INT = 8;

        public GameObject uiRoot;                     //UI根节点
        public GameObject closeViewLayer;             //缓存界面层
        public GameObject nickLayer;                  //人物顶端信息层
        public GameObject lowLayer;                   //低界面层
        public GameObject middleLayer;                //中界面层
        public GameObject dialogLayer;                //对话或任务界面层
        public GameObject highLayer;                  //高界面层
        public GameObject topLayer;                   //系统信息层，最顶层界面层
        public GameObject canvas;

        private GameObject _maskBg;                    //遮罩背景
        private RectTransform _maskBgRect;             //遮罩背景Rect

        //=========================各个存储列表
        //缓存中或者已经打开中的
        private Dictionary<int, InterfaceView> OpeningViewsDic;
        //等待开启列表
        private List<InterfaceView> WaitOpenViewList;
        //界面对象池中缓存的界面
        private List<InterfaceView> CacheViewDic;
        //永久持有已关闭的页面缓存列表 数组处理
        private List<InterfaceView> ForeverCacheViewDic;
        //UI层级界面字典int-gameobject
        private Dictionary<int, GameObject> _layerTypeDic;
        //用于记录每个界面层层级深度数值
        public Dictionary<int, int> openViewCountDic;

        //各个模块对应UI模块的根节点
        //private const string NameSpace = "";

        //缓存ui数量
        private int MaxCacheCount = 3;
        //maskBg 背景资源必须使用 common 图集下资源（需要预加载）
        private const string maskBgSpriteName = "comm_mask";
        //遮罩层背景偏移大小
        private const int maskBgSizeOffset = 20;

        //是否显示全部界面层
        private bool showAllBoo = true;
        //是否显示nick界面层
        private bool showNickBoo = true;
        
        //middle界面层中已经排序过的界面
        private List<InterfaceView> SortOpenViewsList;
        //在UIMgr，运行update出错的界面字典
        private Dictionary<string, bool> UpdateErrorViewDic;

        private Dictionary<string, int> _PopViewNameDic;
        private int _FrameCounter;
        private bool _NeedCheckPopViewStateBoo = false;

        //排序之后的最顶界面
        public InterfaceView topView;

        //是否正在关闭传入列表以外的界面
        private bool isClosingAllByViewBoo = false;
        //是否正在关闭所有界面中
        private bool isClosingAllBoo = false;

        private bool InitBoo = false;

        public void Init()
        {
            ZLogger.Info("UI管理层初始化");

            uiRoot = GameObject.Find("UIRoot");

            _layerTypeDic = new Dictionary<int, GameObject>();

            closeViewLayer = GameObject.Find(CLOSE_LAYER);
            _layerTypeDic.Add(CLOSE_LAYER_INT, closeViewLayer);

            nickLayer = GameObject.Find(NICK_LAYER);
            _layerTypeDic.Add(NICK_LAYER_INT, nickLayer);

            lowLayer = GameObject.Find(LOW_LAYER);
            _layerTypeDic.Add(LOW_LAYER_INT, lowLayer);

            middleLayer = GameObject.Find(MIDDLE_LAYER);
            _layerTypeDic.Add(MIDDLE_LAYER_INT, middleLayer);

            dialogLayer = GameObject.Find(DIALOG_LAYER);
            _layerTypeDic.Add(DIALOG_LAYER_INT, dialogLayer);

            highLayer = GameObject.Find(HIGH_LAYER);
            _layerTypeDic.Add(HIGH_LAYER_INT, highLayer);

            topLayer = GameObject.Find(TOP_LAYER);
            _layerTypeDic.Add(TOP_LAYER_INT, topLayer);

            canvas = GameObject.Find(CANVAS_LAYER);
            
            WaitOpenViewList = new List<InterfaceView>();
            OpeningViewsDic = new Dictionary<int, InterfaceView>();
            ForeverCacheViewDic = new List<InterfaceView>();
            CacheViewDic = new List<InterfaceView>();
            SortOpenViewsList = new List<InterfaceView>();
            UpdateErrorViewDic = new Dictionary<string, bool>();
            _PopViewNameDic = new Dictionary<string, int>();
            openViewCountDic = new Dictionary<int, int>();
            topView = null;

            //打开界面数计数器
            ResetOpenViewIndex();

            InitHideLayer();

            RegisterHandler();
            InitBoo = true;
        }
        public void Clear()
        {
            CloseAll(true);
        }
        public void AfterInit()
        {
            
        }
        public void Start()
        {
            
        }
        //初始化不可见层
        public void InitHideLayer()
        {
            if (closeViewLayer != null)
            {
                GameObject layer = closeViewLayer;
                layer.AddComponent<Canvas>();
                RectTransform layerRect = layer.GetComponent<RectTransform>();
                layerRect.SetParent(canvas.transform, false);
                layerRect.SetAsLastSibling();
                layerRect.anchorMin = new Vector2(0, 0);
                layerRect.anchorMin = new Vector2(1, 1);
                layerRect.sizeDelta = new Vector2(0, 0);
                layerRect.anchoredPosition = new Vector2(10000, 10000);
            }
        }
        //注册事件 no edit
        public void RegisterHandler()
        {
            ScreenResizeHelper.AddCallback(ResizeScreenSize);

            EventMgr.Instance.AddEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, SceneCloseView);
        }
        //移除事件 no edit
        public void RemoveHandler()
        {
            ScreenResizeHelper.RemoveCallback(ResizeScreenSize);

            EventMgr.Instance.RemoveEventListener(SceneConst.SWITCH_SCENE_STAR_LOAD, SceneCloseView);
        }
        //触屏事件 no edit
        public void OnTouchEnd()
        {

        }
        //屏幕缩放时，遮罩背景进行大小适配回调函数
        public void ResizeScreenSize()
        {
            if (_maskBg != null)
            {
                _maskBg.GetComponent<RectTransform>().localPosition = Vector3.zero;
                _maskBg.GetComponent<RectTransform>().localScale = Vector3.one;
                var screenSize = ScreenResizeHelper.ScreenSize();
                _maskBg.GetComponent<RectTransform>().sizeDelta = new Vector2(screenSize.x + maskBgSizeOffset, screenSize.y + maskBgSizeOffset);
            }
        }
        /// <summary>
        /// 设置界面缓存数量
        /// </summary>
        public void SetCacheCount(int count)
        {
            MaxCacheCount = count;
        }
        /// <summary>
        /// 重置界面计数器
        /// </summary>
        public void ResetOpenViewIndex()
        {
            foreach (int tempKey in openViewCountDic.Keys)
            {
                openViewCountDic[tempKey] = 0;
            }
        }
        /// <summary>
        /// 获得UI界面层根节点
        /// </summary>
        public GameObject GetRootCanvas()
        {
            return canvas;
        }
        /// <summary>
        /// 获取最上层view名
        /// </summary>
        public string GetTopView()
        {
            if (topView != null)
            {
                return topView.ViewName();
            }
            return null;
        }
        /// <summary>
        /// 打开悬浮界面（点击其他地方关闭）
        /// </summary>
        public void OpenPopView<T>(string viewName, int subIndex = 1, int arg1 = -1, int arg2 = -1, int arg3 = -1, string arg4 = null, string arg5 = null, string arg6 = null) where T : new()
        {
            int tempKey;
            if (_PopViewNameDic.TryGetValue(viewName, out tempKey))
            {
                _PopViewNameDic.Add(viewName, 2);
                _FrameCounter = 0;
                _NeedCheckPopViewStateBoo = true;
                OpenView<T>(viewName, subIndex, arg1, arg2, arg3, arg4, arg5, arg6);
            }
            else
            {
                ZLogger.Error("重复打开popview viewname {0}", viewName);
            }
        }
        /// <summary>
        /// 打开界面，根据界面名
        /// </summary>
        public void OpenView<T>(string viewName, int subIndex = 1, int arg1 = -1, int arg2 = -1, int arg3 = -1, string arg4 = null, string arg5 = null, string arg6 = null) where T : new()
        {
            List<UiExcel> tempList = DataMgr.Instance.tableUI.GetInfoByNameAndValue("name", viewName);
            if (tempList.Count > 0)
            {
                if (tempList[0] != null)
                {
                    _OpenViewByVo<T>(tempList[0], subIndex, arg1, arg2, arg3, arg4, arg5, arg6);
                }
                else
                {
                    ZLogger.Error("ui表数据不存在 {0}", viewName);
                }
            }
            else
            {
                ZLogger.Error("ui表数据不存在 {0}", viewName);
            }
        }
        /// <summary>
        /// 打开界面，根据界面ID
        /// </summary>
        public void OpenView<T>(int viewId, int subIndex = 1, int arg1 = -1, int arg2 = -1, int arg3 = -1, string arg4 = null, string arg5 = null, string arg6 = null) where T : new()
        {
            UiExcel tempData = DataMgr.Instance.tableUI.GetInfoById(viewId);
            if (tempData != null)
            {
                _OpenViewByVo<T>(tempData, subIndex, arg1, arg2, arg3, arg4, arg5, arg6);
            }
            else
            {
                ZLogger.Error("ui表数据不存在 {0}", viewId);
            }
        }
        // 打开界面，供内部调用
        private void _OpenViewByVo<T>(UiExcel uiData, int subIndex, int arg1 = -1, int arg2 = -1, int arg3 = -1, string arg4 = null, string arg5 = null, string arg6 = null) where T : new()
        {
            if (uiData != null)
            {
                InterfaceView tempView = _TryToGetViewAndType(uiData);
                if (tempView == null) tempView = _CreateView<T>(uiData);
                if (tempView == null)
                {
                    ZLogger.Error("页面没有创建过，并且无法重新创建");
                    return;
                }

                tempView.SetCanvasDepth(GetOpenViewIndex(tempView.Layer()));
                tempView.Open(subIndex, arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }
        /// <summary>
        /// 获取某个界面层中已存在的排序最大值，每个界面的间隔为10
        /// </summary>
        public int GetOpenViewIndex(int layerIndex)
        {
            int tempCount;
            if(openViewCountDic.TryGetValue(layerIndex,out tempCount))
            {
                openViewCountDic[layerIndex] = openViewCountDic[layerIndex] + 10;
                tempCount = openViewCountDic[layerIndex];
            }
            else
            {
                openViewCountDic.Add(layerIndex, 10);
                tempCount = openViewCountDic[layerIndex];
            }
            return tempCount;
        }

        // 尝试从等待界面、已打开界面，缓存中永久持有界面，已关闭缓存界面中获取界面
        private InterfaceView _TryToGetViewAndType(UiExcel uiData)
        {
            InterfaceView tempView = GetWaitByName(uiData.name);
            if (tempView == null)
            {
                tempView = GetOpeningView(uiData.id);
                if(tempView == null)
                {
                    tempView = _GetCacheView(uiData.name, uiData.holdBoo);
                }
            }
            return tempView;
        }

        private InterfaceView _CreateView<T>(UiExcel uiData) where T : new()
        {
            InterfaceView tempView = _NewViewByName<T>(uiData);
            if(tempView == null)
            {
                ZLogger.Error("UIManager:_CreateView 创建UI失败：{0}", uiData.name);
                return null;
            }

            tempView.updateViewState = _ViewStateChange;
            WaitOpenViewList.Add(tempView);
            return tempView;
        }

        private InterfaceView _NewViewByName<T>(UiExcel uiData) where T : new()
        {
            InterfaceView tempView;
            if (UIClassHelper.Instance.UIClassList.Contains(uiData.name))
            {
                tempView = new T() as InterfaceView;
                tempView.Ctor();
                tempView.uiPrefabPath = uiData.path;
                return tempView;
            }
            else
            {
                ZLogger.Error("未在UIClass中定义页面类：{0}", name);
            }
            return null;
        }

        //委托，在每个界面发生状态变化时候需要做的操作
        private void _ViewStateChange(InterfaceView viewCan, UIMgr.ViewStateType type)
        {
            List<UiExcel> tempUiExcelList = DataMgr.Instance.tableUI.GetInfoByNameAndValue("name", viewCan.ViewName());
            if (tempUiExcelList.Count < 1)
            {
                ZLogger.Error("找不到界面的信息，请查看UIExcel表配置是否正确!");
                return;
            }
            UiExcel uiData = tempUiExcelList[0];

            switch (type)
            {
                case ViewStateType.CLOSE:
                    UpdateErrorViewDic.Remove(viewCan.ViewName());
                    _UpdateCacheView(uiData.id, viewCan, false, uiData.holdBoo);
                    _AdjustMaskAndActive();
                    break;
                case ViewStateType.LOAD_COMPLETE:
                    break;
                case ViewStateType.UNLOAD:
                    _UpdateDel(uiData.id, viewCan);
                    break;
                case ViewStateType.LOAD_FAIL:
                    _RemoveWaitOpenValue(viewCan);
                    break;
                case ViewStateType.GIVE_UP_LOAD:
                    ZLogger.Info("请注意：{0}放弃加载！！！，原因是：没加载完成之前，有人调用了closeview()", viewCan.ViewName());
                    _RemoveWaitOpenValue(viewCan);
                    break;
                case ViewStateType.OPEN:
                    _RemoveWaitOpenValue(viewCan);
                    _UpdateCacheView(uiData.id, viewCan, true, uiData.holdBoo);
                    _CheckExclusion(uiData, viewCan);
                    _SortOpeningView();
                    _AdjustMaskAndActive();
                    break;
                case ViewStateType.LOAD_SUCESS:
                    break;
            }
        }

        // 适配mask显示，只显示最上层的mask
        private void _AdjustMaskAndActive()
        {
            //此处排序的是所有界面层，所有界面
            List<InterfaceView> tempList = new List<InterfaceView>();
            var iter = OpeningViewsDic.GetEnumerator();
            while (iter.MoveNext())
            {
                tempList.Add(iter.Current.Value);
            }
            tempList.Sort((a, b) =>
            {
                if (a.Layer() == b.Layer())
                {
                    return a.GetCanvasDepth() < b.GetCanvasDepth() ? 1 : -1;
                }
                else
                {
                    return a.Layer() < b.Layer() ? 1 : -1;
                }
            });
            //缓存排序过的打开中界面
            SortOpenViewsList = tempList;
            //调整隐藏显示状态
            _AdjustActiveAndz(tempList);
            //调整maskBg
            _AdjustMaskBg(tempList);

            //设置最上层view
            if (tempList.Count > 0)
            {
                topView = tempList[tempList.Count - 1];
            }
        }

        // 将middle层打开的界面进行排序
        private void _SortOpeningView()
        {
            List<InterfaceView> tempList = new List<InterfaceView>();
            var iter = OpeningViewsDic.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value.Layer() == MIDDLE_LAYER_INT)
                {
                    tempList.Add(iter.Current.Value);
                }
            }
            tempList.Sort((a, b) => a.GetCanvasDepth() < b.GetCanvasDepth() ? 1 : -1);
            for (int i = 0; i < tempList.Count - 1; i++)
            {
                tempList[i].selfTransform.SetSiblingIndex(i);
            }
        }

        // 适配所有层级里面的隐藏显示以及Z轴
        private void _AdjustActiveAndz(List<InterfaceView> tempList)
        {
            int layerZ = 0;
            bool viewActive = false;
            Dictionary<int, int> canvasDepth = new Dictionary<int, int>();
            //从上往下，存在hideOtherView的页面显示时，往下的页面全部关闭（这里是已经排序过后传过来的界面列表）
            if (tempList.Count > 0)
            {
                bool hideOther = false;
                int tempCount = tempList.Count - 1;
                for (int i = tempCount; i >= 0; i--)
                {
                    if (!hideOther && tempList[i].hideOtherViewBoo)
                    {
                        hideOther = true;
                        tempList[i].SetParent(true);
                        viewActive = true;
                    }
                    else if (hideOther)
                    {
                        tempList[i].SetParent(false);
                        viewActive = false;
                    }
                    else
                    {
                        tempList[i].SetParent(true);
                        viewActive = true;
                    }
                    if (viewActive)
                    {
                        layerZ = layerZ + tempList[i].thickness;
                        Vector3 pos = tempList[i].selfTransform.localPosition;
                        if (pos.z != layerZ)
                        {
                            pos.z = layerZ;
                            tempList[i].selfTransform.localPosition = pos;
                        }
                    }
                    //记录当前最大CanvasDepth
                    int tempDepth = 0;
                    if (!canvasDepth.TryGetValue(tempList[i].Layer(), out tempDepth))
                    {
                        canvasDepth.Add(tempList[i].Layer(), tempList[i].GetCanvasDepth());
                    }
                }
            }
            openViewCountDic = canvasDepth;
            //隐藏nickLayer
            HideNickLayer(viewActive);
        }

        // 适配mask遮罩背景界面
        private void _AdjustMaskBg(List<InterfaceView> tempList)
        {
            bool maskBgBoo = false;
            if (tempList.Count > 0)
            {
                int tempCount = tempList.Count - 1;
                for (int i = tempCount; i >= 0; i--)
                {
                    if (tempList[i].maskBgBoo)
                    {
                        if (_maskBg == null)
                        {
                            _CreateMask();
                        }
                        _maskBg.transform.SetParent(tempList[i].selfTransform);
                        _maskBg.transform.SetSiblingIndex(0);
                        ResizeScreenSize();
                        maskBgBoo = true;
                        break;
                    }
                }
            }
            if (!maskBgBoo)
            {
                if (_maskBg != null)
                {
                    _maskBg.transform.SetParent(closeViewLayer.transform);
                }
            }
        }

        // 创建mask遮罩
        private void _CreateMask()
        {
            _maskBg = new GameObject("MaskBg");
            //Image maskBgImg = UGUITool.AddComponent(_maskBg, "UnityEngine.UI.Image") as Image;
            Image maskBgImg = _maskBg.AddComponent<Image>();
            maskBgImg.type = Image.Type.Sliced;
            _maskBgRect = _maskBg.GetComponent<RectTransform>();
            Sprite maskSprite = USpriteMgr.Instance.GetSprite(SpriteName.CommonSprite, maskBgSpriteName);
            if (maskSprite != null)
            {
                maskBgImg.sprite = maskSprite;
            }
            else
            {
                ZLogger.Error("图集{0}中不存在资源{1}，请价差UIManager中的maskBgSpriteName", SpriteName.CommonSprite, maskBgSpriteName);
            }
            maskBgImg.color = new Color(0, 0, 0, 1);
        }

        // 检查当前这个面板互斥的所有面板关闭 no edit
        private void _CheckExclusion(UiExcel uiData, InterfaceView tgView)
        {

        }

        //更新缓存（打开中界面，永久持有界面，已关闭缓存列表）
        private void _UpdateCacheView(int uiId,InterfaceView viewCan, bool openBoo, int holdBoo)
        {
            if (openBoo)
            {
                if (holdBoo == 1 && ForeverCacheViewDic.Contains(viewCan))
                {
                    ForeverCacheViewDic.Remove(viewCan);
                }
                else if (holdBoo == 0 && CacheViewDic.Contains(viewCan))
                {
                    CacheViewDic.Remove(viewCan);
                }
                if (OpeningViewsDic.ContainsKey(uiId))
                {
                    OpeningViewsDic[uiId] = viewCan;
                }
                else
                {
                    OpeningViewsDic.Add(uiId, viewCan);
                }
            }
            else
            {
                if (holdBoo == 1 && !ForeverCacheViewDic.Contains(viewCan))
                {
                    ForeverCacheViewDic.Add(viewCan);
                }
                else if (holdBoo == 0 && !CacheViewDic.Contains(viewCan))
                {
                    CacheViewDic.Add(viewCan);
                }
                if (OpeningViewsDic.ContainsKey(uiId))
                {
                    OpeningViewsDic.Remove(uiId);
                }
            }
        }

        //移除界面，包括打开中界面，永久持有界面，已关闭缓存列表
        private void _UpdateDel(int viewId,InterfaceView viewCan)
        {
            if(OpeningViewsDic.ContainsKey(viewId))
            {
                OpeningViewsDic.Remove(viewId);
            }
            if (CacheViewDic.Contains(viewCan))
            {
                CacheViewDic.Remove(viewCan);
            }
            if (ForeverCacheViewDic.Contains(viewCan))
            {
                ForeverCacheViewDic.Remove(viewCan);
            }
        }

        //尝试从永久缓存界面，已关闭缓存界面中获取界面
        private InterfaceView _GetCacheView(string viewNameCan, int isHoldBoo)
        {
            if(isHoldBoo == 1)
            {
                var iter = ForeverCacheViewDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (iter.Current.ViewName().Equals(viewNameCan))
                    {
                        return iter.Current;
                    }
                }
                //foreach (InterfaceView tempView in ForeverCacheViewDic)
                //{
                //    if (tempView.ViewName().Equals(viewNameCan))
                //    {
                //        return tempView;
                //    }
                //}
            }
            else if (isHoldBoo == 0)
            {
                var iter = CacheViewDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (iter.Current.ViewName().Equals(viewNameCan))
                    {
                        return iter.Current;
                    }
                }
            }
            return null;
        }

        //从已打开界面中获取界面
        public InterfaceView GetOpeningView(int viewIdCan)
        {
            InterfaceView tempView = null;
            if(OpeningViewsDic.TryGetValue(viewIdCan,out tempView) && tempView != null)
            {
                return tempView;
            }
            return null;
        }

        //从等待界面中获取界面
        public InterfaceView GetWaitByName(string viewNameCan)
        {
            var iter = WaitOpenViewList.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.ViewName().Equals(viewNameCan))
                {
                    return iter.Current;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取某个界面是否已经打开
        /// </summary>
        public bool GetViewIsOpenById(int viewIdCan)
        {
            return GetOpeningView(viewIdCan) != null;
        }

        /// <summary>
        /// 获取某个界面是否已经打开
        /// </summary>
        public bool GetViewIsOpenByName(string viewNameCan)
        {
            List<UiExcel> tempList = DataMgr.Instance.tableUI.GetInfoByNameAndValue("name", viewNameCan);
            if (tempList.Count > 0)
            {
                if (tempList[0] != null)
                {
                    return (GetOpeningView(tempList[0].id) != null || GetWaitByName(viewNameCan) != null);
                }
                else
                {
                    ZLogger.Error("ui表数据不存在 {0}", viewNameCan);
                }
            }
            else
            {
                ZLogger.Error("ui表数据不存在 {0}", viewNameCan);
            }
            return false;
        }

        //从等待打开界面列表中移除界面
        private void _RemoveWaitOpenValue(InterfaceView viewCan)
        {
            if (WaitOpenViewList.Contains(viewCan))
            {
                WaitOpenViewList.Remove(viewCan);
            }
        }

        //场景切换时操作
        public void SceneCloseView()
        {
            List<InterfaceView> tempCloseViewList = new List<InterfaceView>();
            foreach (InterfaceView tempView in WaitOpenViewList)
            {
                if (tempView.sceneCloseBoo)
                {
                    tempCloseViewList.Add(tempView);
                }
            }
            foreach (InterfaceView tempView in tempCloseViewList)
            {
                tempView.Close(false);
            }
            tempCloseViewList.Clear();

            foreach (InterfaceView tempView in OpeningViewsDic.Values)
            {
                if (tempView.sceneCloseBoo)
                {
                    tempCloseViewList.Add(tempView);
                }
            }
            foreach (InterfaceView tempView in tempCloseViewList)
            {
                tempView.Close(false);
            }
            tempCloseViewList.Clear();
        }

        //场景切换时操作
        public void LeaveSceneClear(bool closeAllBoo = false)
        {
            if (closeAllBoo)
            {
                CloseAll(true);
            }
            else
            {
                List<InterfaceView> tempCloseViewList = new List<InterfaceView>();
                foreach(InterfaceView tempView in WaitOpenViewList)
                {
                    if (tempView.sceneCloseBoo)
                    {
                        tempCloseViewList.Add(tempView);
                    }
                }
                foreach (InterfaceView tempView in tempCloseViewList)
                {
                    tempView.Close(false);
                }
                tempCloseViewList.Clear();
                
                foreach (InterfaceView tempView in OpeningViewsDic.Values)
                {
                    if (tempView.sceneCloseBoo)
                    {
                        tempCloseViewList.Add(tempView);
                    }
                }
                foreach (InterfaceView tempView in tempCloseViewList)
                {
                    tempView.Close(false);
                }
                tempCloseViewList.Clear();

                //缓存还是交由缓存数量进行释放删除等操作，不在这里处理
                //foreach (InterfaceView tempView in CacheViewDic)
                //{
                //    tempCloseViewList.Add(tempView);
                //}
                //foreach (InterfaceView tempView in tempCloseViewList)
                //{
                //    tempView.DisposeView();
                //}
                //tempCloseViewList.Clear();
            }
        }

        /// <summary>
        /// 关闭除了传入的界面名字列表以外的界面
        /// </summary>
        public void CloseAllByView(bool isDestroyBooCan,List<string> viewNameList = null)
        {
            isClosingAllByViewBoo = true;
            List<InterfaceView> tempCloseViewList = new List<InterfaceView>();
            List<int> noCloseViewIdList = new List<int>();
            foreach(string tempName in viewNameList)
            {
                List<UiExcel> tempList = DataMgr.Instance.tableUI.GetInfoByNameAndValue("name", tempName);
                if (tempList.Count > 0)
                {
                    if (tempList[0] != null)
                    {
                        noCloseViewIdList.Add(tempList[0].id);
                    }
                }
            }
            foreach(int tempViewId in OpeningViewsDic.Keys)
            {
                if (!noCloseViewIdList.Contains(tempViewId))
                {
                    tempCloseViewList.Add(OpeningViewsDic[tempViewId]);
                }
            }
            foreach(InterfaceView tempView in WaitOpenViewList)
            {
                if (!viewNameList.Contains(tempView.ViewName()))
                {
                    tempCloseViewList.Add(tempView);
                }
            }
            foreach(InterfaceView tempView in tempCloseViewList)
            {
                tempView.Close(isDestroyBooCan);
            }
            tempCloseViewList.Clear();
            isClosingAllByViewBoo = false;
        }

        private void CloseAll(bool isDestroyBooCan)
        {
            if (GetIsCloseingAll())
            {
                return;
            }
            isClosingAllBoo = true;
            List<InterfaceView> tempCloseViewList = new List<InterfaceView>();
            foreach (InterfaceView tempView in WaitOpenViewList)
            {
                tempCloseViewList.Add(tempView);
            }
            foreach (InterfaceView tempView in tempCloseViewList)
            {
                tempView.Close(false);
            }
            tempCloseViewList.Clear();

            foreach (InterfaceView tempView in OpeningViewsDic.Values)
            {
                tempCloseViewList.Add(tempView);
            }
            foreach (InterfaceView tempView in tempCloseViewList)
            {
                tempView.Close(false);
            }
            tempCloseViewList.Clear();

            WaitOpenViewList.Clear();
            OpeningViewsDic.Clear();

            if (isDestroyBooCan)
            {
                foreach (InterfaceView tempView in CacheViewDic)
                {
                    tempCloseViewList.Add(tempView);
                }
                foreach (InterfaceView tempView in tempCloseViewList)
                {
                    tempView.DisposeView();
                }
                tempCloseViewList.Clear();
            }

            ResetOpenViewIndex();
            isClosingAllBoo = false;
        }

        //获取是否在关闭所有界面中
        private bool GetIsCloseingAll()
        {
            return isClosingAllBoo || isClosingAllByViewBoo;
        }

        /// <summary>
        /// 通过界面ID关闭界面
        /// </summary>
        public void CloseViewById(int viewIdCan)
        {
            UiExcel uiData = DataMgr.Instance.tableUI.GetInfoById(viewIdCan);
            _CloseViewByVo(uiData);
        }

        /// <summary>
        /// 通过界面名字关闭界面
        /// </summary>
        public void CloseViewByName(string viewNameCan)
        {
            List<UiExcel> tempList = DataMgr.Instance.tableUI.GetInfoByNameAndValue("name", viewNameCan);
            if (tempList.Count > 0)
            {
                if (tempList[0] != null)
                {
                    _CloseViewByVo(tempList[0]);
                }
            }
        }

        private void _CloseViewByVo(UiExcel uiDataCan)
        {
            if (uiDataCan == null)
            {
                ZLogger.Error("找不到界面信息，请检查UIExcel表是否配置正确！");
                return;
            }

            InterfaceView tempView = _TryToGetOpenViewAndType(uiDataCan);
            if (tempView != null)
            {
                tempView.Close(false);
            }
        }

        //从等待打开界面，已经打开界面中获取界面
        private InterfaceView _TryToGetOpenViewAndType(UiExcel uiDataCan)
        {
            return GetWaitByName(uiDataCan.name) != null ? GetWaitByName(uiDataCan.name) : GetOpeningView(uiDataCan.id);
        }

        /// <summary>
        /// 从已经打开的界面中获取界面
        /// </summary>
        public InterfaceView GetView(string viewNameCan)
        {
            List<UiExcel> tempList = DataMgr.Instance.tableUI.GetInfoByNameAndValue("name", viewNameCan);
            if (tempList.Count > 0)
            {
                if (tempList[0] != null)
                {
                    return GetOpeningView(tempList[0].id);
                }
                else
                {
                    ZLogger.Error("ui表数据不存在 {0}", viewNameCan);
                    return null;
                }
            }
            else
            {
                ZLogger.Error("ui表数据不存在 {0}", viewNameCan);
                return null;
            }
        }

        //从已关闭界面列表或永久持有界面列表中，移除指定名字的界面
        private void _ClearCacheView(InterfaceView viewCan)
        {
            InterfaceView tempView = _GetCacheView(viewCan.ViewName(), 0);
            if (tempView != null)
            {
                tempView.DisposeView();
            }
        }

        public void Update()
        {
            if (!InitBoo)
            {
                return;
            }
            if (OpeningViewsDic.Count > 0)
            {
                foreach (InterfaceView tempView in OpeningViewsDic.Values)
                {
                    bool tempErrorBoo = false;
                    if (UpdateErrorViewDic.TryGetValue(tempView.ViewName(), out tempErrorBoo))
                    {
                        if (tempErrorBoo) continue;
                    }
                    UpdateErrorViewDic[tempView.ViewName()] = true;
                    tempView.ViewUpdate(Time.deltaTime);
                    UpdateErrorViewDic[tempView.ViewName()] = false;
                }
            }
        }

        public void LateUpdate()
        {
            if (!InitBoo)
            {
                return;
            }
            for (int i= CacheViewDic.Count; i > MaxCacheCount; i--)
            {
                _ClearCacheView(CacheViewDic[i - 1]);
            }

            if (openViewCountDic.Count > 0)
            {
                foreach (InterfaceView tempView in OpeningViewsDic.Values)
                {
                    tempView.LateUpdate();
                }
            }

            if(_PopViewNameDic.Count >= 1 && _NeedCheckPopViewStateBoo)
            {
                if (_FrameCounter < 5)
                {
                    _FrameCounter = _FrameCounter + 1;
                    return;
                }
                _FrameCounter = 0;
                foreach(string tempKey in _PopViewNameDic.Keys)
                {
                    if(_PopViewNameDic[tempKey] == 2)
                    {
                        _PopViewNameDic[tempKey] = 1;
                    }
                }
                _NeedCheckPopViewStateBoo = false;
            }
        }
        public void Destroy()
        {
            RemoveHandler();
            CloseAll(true);

            //当UIMgr销毁时，才清除永久持有缓存列表
            List<InterfaceView> tempCloseViewList = new List<InterfaceView>();
            foreach (InterfaceView tempView in ForeverCacheViewDic)
            {
                tempCloseViewList.Add(tempView);
            }
            foreach (InterfaceView tempView in tempCloseViewList)
            {
                tempView.DisposeView();
            }
            tempCloseViewList.Clear();
            ForeverCacheViewDic.Clear();
        }

        /// <summary>
        /// 是否隐藏所有界面层
        /// </summary>
        public void HideLayer(bool activeBooCan)
        {
            showAllBoo = activeBooCan;
            nickLayer.SetActive(showAllBoo && showNickBoo);
            lowLayer.SetActive(activeBooCan);
            middleLayer.SetActive(activeBooCan);
            dialogLayer.SetActive(activeBooCan);
            highLayer.SetActive(activeBooCan);
        }

        /// <summary>
        /// 是否隐藏NickLayer层
        /// </summary>
        public void HideNickLayer(bool activeBooCan)
        {
            showNickBoo = activeBooCan;
            nickLayer.SetActive(showAllBoo && showNickBoo);
        }
        //no edit
        public void CheckTopOpenView(string viewName)
        {

        }


    }
}