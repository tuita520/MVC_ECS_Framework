//=====================================================
// - FileName:      InterfaceView.cs
// - Created:       mahuibao
// - UserName:      2019-01-19
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero.ZeroEngine.Common;
using Zero.ZeroEngine.UI;
using Zero.ZeroEngine.Util;
using static Zero.ZeroEngine.UI.UIEventListener;

namespace Zero.ZeroEngine.UI
{
    public enum ViewState
    {
        Null = 0,
        Loading = 1,
        Loaded = 2,
        Ininting,
        Opening,
        Closed,
    }

    public delegate void UpdateViewStateDelegate(InterfaceView view, UIMgr.ViewStateType type);

    public class InterfaceView
    {
        //----------共有参数
        //界面对应的GameObject
        public GameObject selfGo;
        //界面对应的GameObject的Transform
        public Transform selfTransform;
        //父界面（父界面这个为空，子界面才有）
        public InterfaceView parentView;
        //子界面列表
        public List<InterfaceView> subViewList = new List<InterfaceView>();
        //组件监听事件Dic
        private Dictionary<UIEventEnum, List<GameObject>> compDelegateDic = new Dictionary<UIEventEnum, List<GameObject>>();
        //最顶父界面（父界面这个为自己，子界面这个为最顶父界面）
        public InterfaceView rootBaseView;
        //是否是子界面
        public bool isSubViewBoo;

        //----------父界面参数
        private int _showSortIndex;//界面深度
        private bool _activeBoo;//是否显示
        private bool _instantiateBoo;//是否正在初始化，绑定界面预置
        private bool _openInitBoo;//已经绑定预置，是否正在打开界面初始中
        private bool _closingViewBoo;//是否正在关闭界面中
        
        public GameObject selfParentObj;//界面所在的UI界面层
        public ViewState selfState;//当前界面状态
        public int selfStateChange;//buzhidao

        //此处为可以配置参数，最好在Init中配置
        public bool closeDestroyBoo;//关闭时是否销魂，默认false
        public bool maskBgBoo;//是否有遮罩层，供UIMgr使用
        public bool hideOtherViewBoo;//是否隐藏其他界面
        public int thickness;   //buzhidao
        public bool openResourceHUDBoo;//是否打开资源小界面
        public bool sceneCloseBoo;//是否在切换场景时关闭，默认true

        public long loadAsyncGuid;//ObjectPoolMgr异步加载唯一ID

        public string uiPrefabPath;//预置物路径

        //----------子界面参数
        private bool subInitBoo;//子界面是否已经初始化过（关系到注册事件，防止重复添加或者重复移除事件）
        private bool subOpenBoo;//子界面是否显示中（关系到注册事件，防止重复添加或者重复移除事件）
        private bool subHideForParentBoo;//是否因为父界面隐藏而隐藏

        /// <summary>
        /// 初始化
        /// </summary>
        public void Ctor()
        {
            //----------共有参数
            selfGo = null;
            selfTransform = null;
            parentView = null;

            subViewList.Clear();
            compDelegateDic.Clear();

            rootBaseView = null;             //最顶父界面
            isSubViewBoo = false;

            //----------主界面参数
            _showSortIndex = 0;
            _activeBoo = false;
            _instantiateBoo = false;
            _openInitBoo = false;
            _closingViewBoo = false;
            
            selfParentObj = null;
            selfState = 0;
            selfStateChange = 0;
            closeDestroyBoo = false;
            maskBgBoo = false;
            hideOtherViewBoo = true;
            thickness = 0;   //buzhidao
            openResourceHUDBoo = false;
            sceneCloseBoo = true;
            loadAsyncGuid = 0;
            uiPrefabPath = string.Empty;

            //----------子界面参数
            subInitBoo = false;
            subOpenBoo = false;
            subHideForParentBoo = false;
        }
        public virtual void Init()
        { }
        public virtual string ViewName()
        {
            return null;
        }
        public virtual void OpenView(int subIndex, int arg1 = -1, int arg2 = -1, int arg3 = -1, string arg4 = null, string arg5 = null, string arg6 = null)
        {}
        /// <summary>
        /// 注册事件，界面隐藏移除的事件
        /// </summary>
        public virtual void RegisterUpdateHandler()
        { }
        /// <summary>
        /// 注册事件，界面销毁移除的事件
        /// </summary>
        public virtual void RegisterUpdateHandlerHold()
        { }
        /// <summary>
        /// 移除事件，界面隐藏移除的事件
        /// </summary>
        public virtual void CancelUpdateHandler()
        { }
        /// <summary>
        /// 移除事件，界面销毁移除的事件
        /// </summary>
        public virtual void CancelUpdateHandlerHold()
        { }
        /// <summary>
        /// 打开UI后的处理，打开 view 的特效，动画，协议请求等处理，子类重写，基类调用
        /// </summary>
        public virtual void HandleAfterOpenView()
        { }
        /// <summary>
        /// 在打开所有的子界面的HandleAfterOpenView后触发
        /// </summary>
        public virtual void HandleAfterOpenSubViews()
        { }
        /// <summary>
        /// 关闭UI前的处理，关闭 view 的特效，动画，等处理，子类重写，基类调用
        /// </summary>
        public virtual void HandleBeforeCloseView()
        { }
        /// <summary>
        /// update 函数
        /// </summary>
        public virtual void Update(double deltatime)
        { }
        /// <summary>
        /// lateUpdate 函数
        /// </summary>
        public virtual void LateUpdate()
        { }
        /// <summary>
        /// 页面显隐状态改变时回调
        /// </summary>
        public virtual void VisibleChange(bool activeBoo)
        { }
        /// <summary>
        /// 销毁相应的处理
        /// </summary>
        public virtual void Destory()
        { }
        ///// <summary>
        ///// 获取界面资源预置物路径
        ///// </summary>
        ///// <returns></returns>
        //public virtual string GetAssetUrl()
        //{
        //    return null;
        //}
        /// <summary>
        /// 获取主界面位于UI层哪层
        /// </summary>
        public virtual int Layer()
        {
            return UIMgr.MIDDLE_LAYER_INT;
        }
        public string GetThemeParkAssetUrl()
        {
            //return MainData.ThemeParkResPath + GetAssetUrl();
            return MainData.ThemeParkResPath + uiPrefabPath + ".prefab";
        }
        //=================== 不需要重写函数 ==========================
        /// <summary>
        /// 打开界面
        /// </summary>
        public void Open(int subIndex, int arg1 = -1, int arg2 = -1, int arg3 = -1, string arg4 = null, string arg5 = null, string arg6 = null)
        {
            OpenView(subIndex, arg1, arg2, arg3, arg4, arg5, arg6);

            if (selfState == ViewState.Null)
            {
                selfState = ViewState.Loading;
                loadAsyncGuid = ObjectPoolMgr.Instance.InstantiateObjectAsync(GetThemeParkAssetUrl(), _OnViewLoadComplete, LoadResPriority.RES_MIDDLE, false);
            }
            else
            {
                _OpenViewInit();
            }

        }
        // 打开界面回调
        void _OnViewLoadComplete(string path, object go, object param1 = null, object param2 = null, object param3 = null)
        {
            GameObject prefabObj = go as GameObject;
            if (prefabObj == null)
            {
                ZLogger.Warning("{0} 加载成功，但是 result.asset = null ，请检查资源！！！！！", ViewName());
            }
            BindGo(prefabObj);
            selfState = ViewState.Loaded;

            Init();
            selfState = ViewState.Ininting;
            updateViewState(this, UIMgr.ViewStateType.LOAD_COMPLETE);

            RegisterUpdateHandlerHold();

            _OpenViewInit();
        }

        // 父界面打开时初始化
        public void _OpenViewInit()
        {
            _openInitBoo = true;
            if (selfState != ViewState.Opening)
            {
                RegisterUpdateHandler();
            }

            //UI特效界面管理也要在这思考如何处理下 no edit

            if (!selfGo.activeInHierarchy)
            {
                SetActive(true);
            }

            selfState = ViewState.Opening;
            updateViewState(this, UIMgr.ViewStateType.OPEN);
            _HandleBeforeOpenView();

            if (openResourceHUDBoo)
            {
                //打开资源小界面 no edit
                //暂时没有编辑
            }

            _openInitBoo = false;
            EventMgr.Instance.TriggerEvent(GlobalEvent.OPEN_VIEW, ViewName());
        }
        
        public UpdateViewStateDelegate updateViewState;

        // 主界面绑定预置GameObject
        private void BindGo(GameObject mainGo)
        {
            _instantiateBoo = true;
            selfGo = mainGo;
            selfTransform = mainGo.transform;
            selfGo.SetActive(false);
            rootBaseView = this;
            SetParent(true);

            selfGo.gameObject.name = ViewName();
            _instantiateBoo = false;
        }

        // 子界面绑定预置GameObject
        public void BindGo(GameObject subGo, InterfaceView parentViewCan = null)
        {
            selfGo = subGo;
            selfTransform = subGo.transform;
            if (parentViewCan != null)
            {
                parentView = parentViewCan;
                rootBaseView = parentView.rootBaseView;
                isSubViewBoo = true;
            }
        }
        public void SetActive(bool activeBooCan)
        {
            if (isSubViewBoo)
            {
                _ShowSubView(activeBooCan);
            }
            else
            {
                _SetActive(activeBooCan);
            }
        }
        private void _SetActive(bool activeBoo)
        {
            if (selfGo != null)
            {
                if (activeBoo != selfGo.activeSelf)
                {
                    selfGo.SetActive(activeBoo);
                }
            }
        }
        /// <summary>
        /// 主界面设置父节点
        /// </summary>
        public bool SetParent(bool activeBooCan)
        {
            if (_activeBoo == activeBooCan) return false;

            int layer = Layer();
            GameObject parent = null;
            if (activeBooCan)
            {
                switch (layer)
                {
                    case UIMgr.NICK_LAYER_INT:
                        parent = UIMgr.Instance.nickLayer;
                        break;
                    case UIMgr.LOW_LAYER_INT:
                        parent = UIMgr.Instance.lowLayer;
                        break;
                    case UIMgr.MIDDLE_LAYER_INT:
                        parent = UIMgr.Instance.middleLayer;
                        break;
                    case UIMgr.DIALOG_LAYER_INT:
                        parent = UIMgr.Instance.dialogLayer;
                        break;
                    case UIMgr.HIGH_LAYER_INT:
                        parent = UIMgr.Instance.highLayer;
                        break;
                    case UIMgr.TOP_LAYER_INT:
                        parent = UIMgr.Instance.topLayer;
                        break;
                    case UIMgr.USER_LAYER_INT:
                        parent = selfParentObj;
                        break;
                }
            }
            else
            {
                parent = UIMgr.Instance.closeViewLayer;
            }
            if (parent != selfTransform.parent)
            {
                selfTransform.SetParent(parent.transform,false);
            }
            _activeBoo = activeBooCan;

            if(!_openInitBoo && !_closingViewBoo && !_instantiateBoo)
            {
                _VisibleChange(activeBooCan);
            }
            return true; 
        }
        /// <summary>
        /// 设置主界面的父节点obj
        /// </summary>
        public void SetParentObj(GameObject obj)
        {
            selfParentObj = obj;
        }

        // 页面显隐时调用
        protected void _VisibleChange(bool visible)
        {
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                if (subViewList[i].subIsOpen())
                {
                    subViewList[i]._VisibleChange(visible);
                }
            }
            VisibleChange(visible);
        }

        /// <summary>
        /// 设置主界面深度
        /// </summary>
        public void SetCanvasDepth(int depth)
        {
            _showSortIndex = depth;
        }
        /// <summary>
        /// 获取主界面深度
        /// </summary>
        public int GetCanvasDepth()
        {
            return _showSortIndex;
        }
        /// <summary>
        /// 添加子页面，并返回子页面实例
        /// </summary>
        public void AddSubView<T>(String viewName, GameObject subGo) where T : new()
        {

            InterfaceView tempView = null;
            if(UIClassHelper.Instance.UIClassList.Contains(viewName))
            {
                tempView = new T() as InterfaceView;
                tempView.Ctor();
                _AddSubView(tempView);
                tempView.BindGo(subGo, this);
            }
            else
            {
                ZLogger.Error("该界面类不存在于UIClassHelper中，请检查，类名：{0}", viewName);
            }
        }
        private void _AddSubView(InterfaceView viewClsCan)
        {
            var iter = subViewList.GetEnumerator();
            while (iter.MoveNext())
            {
                if(iter.Current == viewClsCan)
                {
                    ZLogger.Error("子界面类已经添加过，子界面名字：{0}", viewClsCan.selfGo.name);
                    return;
                }
            }
            subViewList.Add(viewClsCan);
        }
        /// <summary>
        /// 获取子页面实例列表
        /// </summary>
        public List<InterfaceView> GetSubViews()
        {
            return subViewList;
        }
        /// <summary>
        /// 获取子页面实例列
        /// </summary>
        public InterfaceView GetSubView(int index)
        {
            if(index > subViewList.Count - 1)
            {
                ZLogger.Error("获取的子界面index：{0} 不存在", index);
            }
            return subViewList[index];
        }
        /// <summary>
        /// 显示下标为Index的子页面，关闭其他子页面
        /// </summary>
        public void ShowSubView(int index)
        {
            if (index > subViewList.Count - 1)
            {
                ZLogger.Error("显示的子界面index：{0} 不存在", index);
            }
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                subViewList[i]._ShowSubView(index == i);
            }
        }
        /// <summary>
        /// 设置下标为index的子界面开启或关闭
        /// </summary>
        public void ActiveSubView(int index,bool activeBoo)
        {
            if (index > subViewList.Count - 1)
            {
                ZLogger.Error("获取的子界面index：{0} 不存在", index);
            }
            if (subViewList[index]!=null)
            {
                subViewList[index]._ShowSubView(activeBoo);
            }
        }

        // 子界面供父界面调用显示下标为index的子界面相关操作
        protected void _ShowSubView(bool activeBoo)
        {
            _SetActive(activeBoo);
            if (activeBoo)
            {
                if (!subInitBoo)
                {
                    Init();
                    RegisterUpdateHandlerHold();
                    subInitBoo = true;
                    if (!subOpenBoo)
                    {
                        RegisterUpdateHandler();
                    }
                    subOpenBoo = true;
                    _HandleBeforeOpenView();
                }
                else
                {
                    if (subOpenBoo)
                    {
                        _CancelUpdateHandlerHold();
                    }
                    _HandleBeforeCloseView();
                }
            }
            subHideForParentBoo = false;
        }

        /// <summary>
        /// 子界面是否打开
        /// </summary>
        public bool subIsOpen()
        {
            return subOpenBoo;
        }
        
        public void _HandleBeforeOpenView()
        {
            if (isSubViewBoo)
            {
                if (subOpenBoo)
                {
                    HandleAfterOpenView();
                    _SubViewHandlerAfterOpenView();
                }
            }
            else
            {
                HandleAfterOpenView();
                _SubViewHandlerAfterOpenView();
                HandleAfterOpenSubViews();
            }
        }
        protected void _SubViewHandlerAfterOpenView()
        {
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                subViewList[i]._HandleBeforeOpenView();
            }
        }
        public void _HandleBeforeCloseView()
        {
            if (isSubViewBoo)
            {
                if (subOpenBoo)
                {
                    subOpenBoo = false;
                    _SubViewHandlerAfterCloseView();
                    HandleBeforeCloseView();
                }
            }
            else
            {
                _SubViewHandlerAfterCloseView();
                HandleBeforeCloseView();
            }
        }
        protected void _SubViewHandlerAfterCloseView()
        {
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                subViewList[i]._HandleBeforeCloseView();
            }
        }
        protected void _CancelUpdateHandler()
        {
            if (isSubViewBoo)
            {
                if (subOpenBoo)
                {
                    CancelUpdateHandler();
                    _SubViewCancelUpdateHandler();
                }
            }
            else
            {
                CancelUpdateHandler();
                _SubViewCancelUpdateHandler();
            }
        }
        protected void _SubViewCancelUpdateHandler()
        {
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                subViewList[i]._CancelUpdateHandler();
            }
        }
        protected void _CancelUpdateHandlerHold()
        {
            if (isSubViewBoo)
            {
                if (subInitBoo)
                {
                    CancelUpdateHandlerHold();
                    _SubViewCancelUpdateHandlerHold();
                }
            }
            else
            {
                CancelUpdateHandlerHold();
                _SubViewCancelUpdateHandlerHold();
            }
        }
        protected void _SubViewCancelUpdateHandlerHold()
        {
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                subViewList[i]._CancelUpdateHandlerHold();
            }
        }
        public void ViewUpdate(double deltatime)
        {
            Update(deltatime);
            _SubViewUpdate(deltatime);
        }
        protected void _SubViewUpdate(double deltatime)
        {
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                subViewList[i].ViewUpdate(deltatime);
            }
        }
        public void ViewLateUpdate()
        {
            LateUpdate();
            _SubViewLateUpdate();
        }
        protected void _SubViewLateUpdate()
        {
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                subViewList[i].ViewLateUpdate();
            }
        }
        protected void _Destory()
        {
            if (isSubViewBoo)
            {
                if (subInitBoo)
                {
                    subInitBoo = false;
                    _RemoveAllComponentCallbackListneners();
                    _SubViewDestory();
                    Destory();
                }
            }
            else
            {
                _RemoveAllComponentCallbackListneners();
                _SubViewDestory();
                Destory();
            }
        }
        protected void _SubViewDestory()
        {
            for (int i = 0; i < subViewList.Count - 1; i++)
            {
                subViewList[i]._Destory();
            }
        }

        public void Close(bool destroyBooCan)
        {
            if (isSubViewBoo)
            {

            }
            else
            {
                CloseView(destroyBooCan);
            }
        }
        public void CloseView(bool destroyBooCan)
        {
            if (isSubViewBoo) return;

            _closingViewBoo = true;
            selfState = ViewState.Closed;
            
            if(selfGo == null)
            {
                if (ObjectPoolMgr.Instance.IsIngAsyncLoad(loadAsyncGuid))
                {
                    ObjectPoolMgr.Instance.CancleLoad(loadAsyncGuid);
                }
                else
                {
                    ObjectPoolMgr.Instance.ReleaseObject(selfGo, 0);
                }
                updateViewState(this, UIMgr.ViewStateType.GIVE_UP_LOAD);
                EventMgr.Instance.TriggerEvent(GlobalEvent.CLOSE_VIEW, ViewName());
                GameObject.DestroyImmediate(selfGo);
                selfGo = null;
                selfState = ViewState.Null;
                return;
            }

            SetParent(false);

            updateViewState(this, UIMgr.ViewStateType.CLOSE);
            _CancelUpdateHandler();
            _HandleBeforeCloseView();
            if (openResourceHUDBoo)
            {
                //打开资源小界面 no edit
                //暂时没有编辑
            }

            _closingViewBoo = false;
            EventMgr.Instance.TriggerEvent(GlobalEvent.CLOSE_VIEW, ViewName());

            if (destroyBooCan)
            {
                DisposeView();
            }
        }
        public void DisposeView()
        {
            if (isSubViewBoo) return;

            selfState = ViewState.Null;
            updateViewState(this, UIMgr.ViewStateType.UNLOAD);
            _CancelUpdateHandlerHold();
            _Destory();
            subViewList.Clear();
            ObjectPoolMgr.Instance.ReleaseObject(selfGo, 0);
            selfGo = null;
        }

        //=================== end ==========================

        //获取路径为path的gameObject
        protected GameObject FindChild(string path)
        {
            GameObject childGO = selfGo.transform.Find(path).gameObject;
            return childGO;
        }
        //获取路径为path的gameObject的componType类型的控件
        protected Component FindInChild(string comType, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return UGUITool.FindInChild(comType, selfGo, path);
            }
            else
            {
                return selfGo.GetComponent(comType);
            }
        }
        //=================== 事件 ==========================
        /// <summary>
        /// 添加组件监听事件
        /// </summary>
        /// <param name="objCan"></param>
        /// <param name="eventTypeCan"></param>
        /// <param name="callbackCan"></param>
        public void AddComponentCallbackListnener(GameObject objCan ,UIEventEnum eventEnumCan, VoidDelegate callbackCan)
        {
            UIEventListener.AddEventListener(objCan, eventEnumCan, callbackCan);
            List<GameObject> tempObjList = null;
            if (compDelegateDic.TryGetValue(eventEnumCan, out tempObjList))
            {
                compDelegateDic[eventEnumCan].Add(objCan);
            }
            else
            {
                tempObjList = new List<GameObject>();
                tempObjList.Add(objCan);
                compDelegateDic.Add(eventEnumCan, tempObjList);
            }
        }
        /// <summary>
        /// 移除组件监听事件
        /// </summary>
        /// <param name="objCan"></param>
        /// <param name="eventEnumCan"></param>
        public void RemoveComponentCallbackListnener(GameObject objCan, UIEventEnum eventEnumCan)
        {
            List<GameObject> tempObjList = null;
            if (compDelegateDic.TryGetValue(eventEnumCan, out tempObjList))
            {
                int tempCount = tempObjList.Count - 1;
                for(int i = 0; i < tempCount; i++)
                {
                    if(tempObjList[i] == objCan)
                    {
                        UIEventListener.RemoveEventListener(compDelegateDic[eventEnumCan][i], eventEnumCan);
                        compDelegateDic[eventEnumCan].RemoveAt(i);
                    }
                }
            }
        }
        /// <summary>
        /// 移除所有的组件监听事件
        /// </summary>
        protected void _RemoveAllComponentCallbackListneners()
        {
            var iter = compDelegateDic.GetEnumerator();
            while (iter.MoveNext())
            {
                List<GameObject> tempObjList = iter.Current.Value;
                int tempCount = tempObjList.Count - 1;
                for (int i = 0; i < tempCount; i++)
                {
                    UIEventListener.RemoveAllEventListener(tempObjList[i]);
                }
            }
            compDelegateDic.Clear();
        }

    }
}