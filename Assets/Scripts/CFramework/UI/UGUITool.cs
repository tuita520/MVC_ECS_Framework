//=====================================================
// - FileName:      UGUITool.cs
// - Created:       mahuibao
// - UserName:      2019-01-01
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Zero.Plugins.Base;
using Zero.ZeroEngine.Util;

namespace Zero.ZeroEngine.UI
{
    static public class UGUITool
    {
#if UNITY_IOS
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private extern static void GetSafeAreaImpl(out float x, out float y, out float w, out float h);
#endif

        public static Rect GetSafeArea()
        {
            float x, y, w, h;
#if UNITY_IOS
        GetSafeAreaImpl(out x,out y,out w,out h);
#else
            x = 0;
            y = 0;
            w = ScreenResizeHelper.width;
            h = ScreenResizeHelper.height;
#endif
            return new Rect(x, y, w, h);
        }

        static public int PreloadCacheRes(string path)
        {
            return 1;//edit
        }
        //获取某个组件是否激活
        static public bool GetActive(Behaviour mb)
        {
            return mb && mb.enabled && mb.gameObject.activeInHierarchy;
        }
        /// <summary>
        /// 帮助函数 返回类型字符串
        /// </summary>
        static public string GetTypeName(UnityEngine.Object obj)
        {
            if (obj == null) return "Null";
            string s = obj.GetType().ToString();
            if (s.StartsWith("UI")) s = s.Substring(2);
            else if (s.StartsWith("UnityEngine")) s = s.Substring(12);
            return s;
        }
        //获取某个对象的根节点
        static public GameObject GetRoot(GameObject go)
        {
            Transform t = go.transform;
            for(;;)
            {
                Transform parent = t.parent;
                if (parent == null) break;
                t = parent;
            }
            return t.gameObject;
        }
        /// <summary>
        /// 找出相同名字的对象（找到第一个就返回，目前逻辑为遍历第一级子节点，并先同样的逻辑继续找其子节点）
        /// </summary>
        public static Transform GetChildTransform(string childName,Transform parent)
        {
            int childCount = parent.childCount;
            for (int i = 0;i<childCount;i++)
            {
                Transform curT = parent.GetChild(i);
                if (childName.Equals(curT.name))
                {
                    return curT;
                }
                Transform findT = GetChildTransform(childName, curT);
                if (findT != null)
                    return findT;
            }
            return null;
        }
        //通过名字查找对象
        public static GameObject GetChildByName(string childName,GameObject parent)
        {
            if (parent ==null)
            {
                ZLogger.Error("{0} parent is null", childName);
                return null;
            }
            Transform childT = GetChildTransform(childName, parent.transform);
            if(childT != null)
            {
                return childT.gameObject;
            }
            else
            {
                ZLogger.Error("{0} can't find {1}", parent.name, childName);
                return null;
            }
        }
        //根据组件类型名字，对象名字，对象，找到组件并返回
        public static Component GetComponent(string componentTypeName,string name,GameObject p)
        {
            GameObject obj = GetChildByName(name, p);
            if (obj) return obj.GetComponent(componentTypeName);
            return null;
        }
        //泛型，根据根节点，路径，找到对象上挂载的组件
        public static T GetChildComponent<T>(GameObject parent,string path)where T:Component
        {
            Transform trans = parent.transform.Find(path);
            if (trans!=null)
            {
                return trans.gameObject.GetComponent<T>();
            }
            else
            {
                return null;
            }
        }
        //获取某个方法的名字 ？
        static public string GetFuncName(object obj,string method)
        {
            if (obj == null) return "<null>";
            string type = obj.GetType().ToString();
            int period = type.LastIndexOf('/');
            if (period > 0) type = type.Substring(period + 1);
            return string.IsNullOrEmpty(method) ? type : type + "/" + method;
        }
        //根据根节点，路径，找到路径的对象并返回
        public static GameObject FindChild(GameObject parent, string path)
        {
            if(parent!=null)
            {
                Transform trans = parent.transform.Find(path);
                if(trans)
                {
                    return trans.gameObject;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                ZLogger.Error("UGUITool.FindChild parent is null({0})", path);
                return null;
            }
        } 
        //根据组件类型名字，根节点，路径，找到路径对象上挂载的组件并返回
        public static Component FindInChild(string componentTypeName,GameObject parent,string path)
        {
            Transform trans = parent.transform.Find(path);
            if(trans==null)
            {
                return null;
            }
            else
            {
                return trans.gameObject.GetComponent(componentTypeName);
            }
        }
        //
        public static Component[] FindComponentsInChild(string componentTypeName,GameObject go)
        {
            if (string.IsNullOrEmpty(componentTypeName) || (go == null))
            {
                ZLogger.Error("Error params for FindComponentsInChild, componentTypeName is {0}, go is {1}", componentTypeName, go);
                return new Component[0];
            }
            Type t = GetTypeFromAssembly(componentTypeName);
            if (t != null)
            {
                return go.transform.GetComponentsInChildren(t);
            }
            else
            {
                ZLogger.Error("No such type {0}", componentTypeName);
                return new Component[0];
            }
        }
        //添加子对象
        //
        static public GameObject AddChild(GameObject parent)
        {
            return AddChild(parent, true);
        }

        static public GameObject AddChild(GameObject parent,bool undo)
        {
            GameObject go = new GameObject();
#if !GAME_PUBLISH && UNITY_EDITOR
            if (undo) UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent.transform);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }

        static public GameObject AddChild(GameObject parent,GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
#if !GAME_PUBLISH && UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if(go !=null &&parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent.transform);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }

        //简单的拷贝游戏预设物体
        static public GameObject SimpleClone(GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            Transform parentTrans = prefab.transform.parent;
            if (go != null && parentTrans != null)
            {
                Transform t = go.transform;
                t.SetParent(parentTrans.transform);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parentTrans.gameObject.layer;
            }
            return go;
        }
        /// <summary>
        /// determines whether the 'parent' contains a 'child' in its hierarchy
        /// 确定“父”在其层次结构中是否包含“子”
        /// </summary>
        static public bool IsChild(Transform parent,Transform child)
        {
            if ((parent == null) || (child == null)) return false;

            while (child != null)
            {
                if (child == parent)
                    return true;
                child = child.parent;
            }
            return false;
        }
        /// <summary>
        /// 重新设置游戏对象的图层
        /// </summary>
        static public void SetLayer(GameObject go,int layer)
        {
            go.layer = layer;

            Transform t = go.transform;
            for(int i=0,imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                SetLayer(child.gameObject, layer);
            }
        }

        static public void SetLayerWithIgnore(GameObject go,int layer,int ignoreLayer)
        {
            if (go.layer != ignoreLayer)
                go.layer = layer;

            Transform t = go.transform;
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                SetLayerWithIgnore(child.gameObject, layer, ignoreLayer);
            }
        }

        static public void SetChildLayer(Transform t,int layer)
        {
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                child.gameObject.layer = layer;
                SetChildLayer(child, layer);
            }
        }
        //更新LayeroutGroup
        public static void UpdateVerticalLayoutGroup(GameObject go)
        {
            VerticalLayoutGroup layOut = go.GetComponent<VerticalLayoutGroup>();
            LayoutElement childLayout;
            float height = 0;
            foreach(Transform tf in go.transform)
            {
                childLayout = tf.gameObject.GetComponent<LayoutElement>();
                if (childLayout != null)
                {
                    height += childLayout.preferredHeight;
                }
            }
            layOut.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        /// <summary>
        /// Access to the clipboard via undocumented apis
        /// 通过未文档化的api访问剪贴板
        /// </summary>
        static public string clipboard
        {
            get
            {
                TextEditor te = new TextEditor();
                te.Paste();
#if UNITY_5 || UNITY_2017
                return te.text;
#else
                return te.content.text;
#endif
            }
            set
            {
                TextEditor te = new TextEditor();
#if UNITY_5 || UNITY_2017
                te.text = (value);
#else
                te.content = new GUIContent(value);
#endif
                te.OnFocus();
                te.Copy();
            }
        }

        /// <summary>
        /// Destroy the specified object, immediately if in edit mode.
        /// </summary>
        static public void Destroy(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                if (Application.isPlaying)
                {
                    if (obj is GameObject)
                    {
                        GameObject go = obj as GameObject;
                        go.transform.parent = null;
                    }
                    UnityEngine.Object.Destroy(obj);
                }
                else UnityEngine.Object.DestroyImmediate(obj);
            }
        }
        //依次获取 Assembly-CSharp, UnityEngine.UI, UnityEngine Assembly 以便后面进行获取 type
        static private Assembly _assemblyCSharp = null;
        static public Type GetTypeFromAssemblyCSharp(string typeName)
        {
            if (_assemblyCSharp == null)
            {
                _assemblyCSharp = Assembly.Load("Assembly-CSharp");
            }
            Type t = _assemblyCSharp.GetType(typeName);
            return t;
        }

        static private Assembly _assembltUnityEngineUI = null;
        static public Type GetTypeFromAssemblyUnityEngineUI(string typeName)
        {
            if (_assembltUnityEngineUI == null)
            {
                _assembltUnityEngineUI = Assembly.Load("UnityEngine.UI");
            }
            if (!typeName.StartsWith("UnityEngine.UI"))
            {
                typeName = string.Concat("UnityEngine.UI.", typeName);
            }
            Type t = _assembltUnityEngineUI.GetType(typeName);
            return t;
        }

        static private Assembly _assembltUnityEngine = null;
        static public Type GetTypeFromAssemblyUnityEngine(string typeName)
        {
            if (_assembltUnityEngine == null)
            {
                _assembltUnityEngine = Assembly.Load("UnityEngine");
            }
            if (!typeName.StartsWith("UnityEngine"))
            {
                typeName = string.Concat("UnityEngine.", typeName);
            }
            Type t = _assembltUnityEngine.GetType(typeName);
            return t;
        }

        static public Type GetTypeFromAssembly(string typeName)
        {
            Type t = GetTypeFromAssemblyCSharp(typeName);
            if (t != null)
            {

            }
            else
            {
                t = GetTypeFromAssemblyUnityEngineUI(typeName);
                if(t!=null)
                {

                }
                else
                {
                    t = GetTypeFromAssemblyUnityEngine(typeName);
                }
            }
            return t;
        }

        //添加Component
        static public Component AddComponent(GameObject parent,string type)
        {
            return parent.AddComponent(GetTypeFromAssembly(type));
        }

        static public Component AddMissingComponent(GameObject go,string typeName)
        {
#if UNITY_5 || UNITY_2017
            Component comp = go.GetComponent(typeName);
            if (comp == null)
            {
                Type t = GetTypeFromAssembly(typeName);
                if (t != null)
                {
                    comp = go.AddComponent(t);
                }
                else
                {
                    ZLogger.Error("AddMissingComponent  {0}  can't find type", typeName);
                    return null;
                }
            }
            return comp;
#else
            Component comp = go.GetComponent(typeName);
            if (comp == null)
            {
                comp = go.AddComponent(typeName);
            }
            return comp;
#endif
        }

        //设置recttrans的宽高
        static public void SetRectTransSize(GameObject go,int width,int height)
        {
            RectTransform rectTrans = go.GetComponent<RectTransform>();
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        static IEnumerator GetCaptrue(Rect rect, Action<Texture2D> callBack)
        {
            yield return new WaitForEndOfFrame();
            Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            texture.ReadPixels(rect, 0, 0);
            texture.Apply();
            callBack(texture);
        }

        static public Coroutine ScreenShot(Rect rect, Action<Texture2D> callBack)
        {
            return CoroutineMgr.Instance.StartCoroutine(GetCaptrue(rect, callBack));
        }

        static public void StopScreenShot(Coroutine coroutine)
        {
            CoroutineMgr.Instance.StopCoroutine(coroutine);
        }
        //将本地坐标转换成全局坐标
        static public Rect ConvertLocalRectToCanvas(RectTransform rectTransform)
        {
            //未找到啥有效的方法算出rect的全局转换，目前是寻找到一个全屏对象来当做参照，然后计算出相应的rect位置
            var layer = GameObject.Find("MiddleLayer");
            var layerRect = layer.GetComponent<RectTransform>();
            Vector3[] tmpvecLayer = new Vector3[4];
            layerRect.GetWorldCorners(tmpvecLayer);
            //ZLogger.Info(tmpvecLayer[0]);//x 0 y 0
            //ZLogger.Info(tmpvecLayer[1]);//y height
            //ZLogger.Info(tmpvecLayer[2]);//x width
            //ZLogger.Info(tmpvecLayer[3]);
            float tmpheight = tmpvecLayer[2].y;
            float tmpwidth = tmpvecLayer[2].x;

            Vector3[] tmpvec = new Vector3[4];
            rectTransform.GetWorldCorners(tmpvec);

            var x1 = (tmpvec[0].x + tmpwidth) * Screen.width / tmpwidth / 2;
            var y1 = (tmpvec[0].y + tmpheight) * Screen.height / tmpheight / 2;
            var x2 = (tmpvec[2].x + tmpwidth) * Screen.width / tmpwidth / 2;
            var y2 = (tmpvec[2].y + tmpheight) * Screen.height / tmpheight / 2;
            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }
        
        //震动
        static public void VibrateToPlayer()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            Handheld.Vibrate();
#else
#endif
        }
        //获得rectTran的四个角对应的屏幕坐标
        static public Vector3[] GetWorldCorners(RectTransform rectTran)
        {
            Vector3[] tmpVec = new Vector3[4];
            rectTran.GetWorldCorners(tmpVec);
            return tmpVec;
        }

        //为lua语言准备的判空
        static public bool IsNil(object obj)
        {
            if (obj != null)
            {
                if (obj.Equals(null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        static public Vector2 MoveToMousePoint(GameObject childGo,GameObject parentGo,Camera uiCam)
        {
            Vector2 localPos;
            RectTransform parentRect = parentGo.GetComponent<RectTransform>();
            RectTransform childRect = childGo.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, uiCam, out localPos))
            {
                childRect.localPosition = localPos;
                return localPos;
            }
            else
                return Vector2.zero;
        }

        static public bool ContainMousePoint(GameObject go,Camera uiCam)
        {
            RectTransform rect = go.GetComponent<RectTransform>();
            return rect && RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, uiCam);
        }

        static public bool IsContainUI(RectTransform biggerOne,RectTransform smallerOne)
        {
            if (biggerOne == null || smallerOne == null || smallerOne.gameObject == null) return false;
            Vector3[] biggerOne_coners = new Vector3[4];
            Vector3[] smallerOne_coners = new Vector3[4];
            biggerOne.GetWorldCorners(biggerOne_coners);
            smallerOne.GetWorldCorners(smallerOne_coners);
            Vector3 worldPos = smallerOne.gameObject.transform.position;
            float world_halfWidth = (smallerOne_coners[2].x - smallerOne_coners[0].x) / 2f;
            float world_halfHeight = (smallerOne_coners[2].y - smallerOne_coners[0].y) / 2;
            float min_x = worldPos.x - world_halfWidth;
            float max_x = worldPos.x + world_halfWidth;
            float min_y = worldPos.y - world_halfHeight;
            float max_y = worldPos.y + world_halfHeight;
            return (biggerOne_coners[0].x <= min_x) && (max_x <= biggerOne_coners[2].x)
                && (biggerOne_coners[0].y <= min_y) && (max_y <= biggerOne_coners[2].y);  
        }
        /// <summary>
        /// 触发指定节点下的所有粒子系统播放
        /// </summary>
        static public void PlayParticleSystem(GameObject root)
        {
            ParticleSystem[] particleSystemList = root.GetComponentsInChildren<ParticleSystem>();
            for(int idx = 0; idx < particleSystemList.Length; ++idx)
            {
                particleSystemList[idx].Stop();
                particleSystemList[idx].Play();
            }
        }

        static public void RefreshLayerAndOrder(GameObject targetObj,string sortingLayerName,int orderInLayer)
        {
            Renderer[] renders = targetObj.GetComponentsInChildren<Renderer>();

            foreach(Renderer render in renders)
            {
                render.sortingOrder = orderInLayer;
                render.sortingLayerName = sortingLayerName;
            }
        }

    }
}
