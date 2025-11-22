using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// 如果您的新專案沒有 Zenject，請自行定義 [Inject] 屬性或移除相關依賴
using Zenject;

namespace Core.MVC
{
    #region Event System (Listener)

    /// <summary>
    /// 用於標記在 Mediator 或 Command 中的方法，使其能自動監聽事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ListenerAttribute : Attribute
    {
        public string EventName { get; private set; }
        public ListenerAttribute(string eventName)
        {
            EventName = eventName;
        }
    }

    /// <summary>
    /// 事件中心，負責管理事件的註冊與廣播
    /// </summary>
    public class Listener
    {
        private Dictionary<string, List<Delegate>> eventTable = new Dictionary<string, List<Delegate>>();

        /// <summary>
        /// 廣播事件 (無參數)
        /// </summary>
        public void BroadCast(string eventName)
        {
            if (eventTable.TryGetValue(eventName, out var list))
            {
                // 複製一份列表以避免在事件執行過程中修改列表導致錯誤
                var callbacks = new List<Delegate>(list);
                foreach (var callback in callbacks)
                {
                    if (callback is Action action)
                        action.Invoke();
                    else
                        Debug.LogWarning($"Event {eventName} signature mismatch. Expected Action.");
                }
            }
        }

        /// <summary>
        /// 廣播事件 (1個參數)
        /// </summary>
        public void BroadCast<T>(string eventName, T arg1)
        {
            if (eventTable.TryGetValue(eventName, out var list))
            {
                var callbacks = new List<Delegate>(list);
                foreach (var callback in callbacks)
                {
                    if (callback is Action<T> action)
                        action.Invoke(arg1);
                    else
                        Debug.LogWarning($"Event {eventName} signature mismatch. Expected Action<{typeof(T).Name}>.");
                }
            }
        }

        /// <summary>
        /// 廣播事件 (2個參數)
        /// </summary>
        public void BroadCast<T1, T2>(string eventName, T1 arg1, T2 arg2)
        {
            if (eventTable.TryGetValue(eventName, out var list))
            {
                var callbacks = new List<Delegate>(list);
                foreach (var callback in callbacks)
                {
                    if (callback is Action<T1, T2> action)
                        action.Invoke(arg1, arg2);
                }
            }
        }

        /// <summary>
        /// 廣播事件 (3個參數)
        /// </summary>
        public void BroadCast<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            if (eventTable.TryGetValue(eventName, out var list))
            {
                var callbacks = new List<Delegate>(list);
                foreach (var callback in callbacks)
                {
                    if (callback is Action<T1, T2, T3> action)
                        action.Invoke(arg1, arg2, arg3);
                }
            }
        }

        /// <summary>
        /// 自動掃描目標物件中帶有 [Listener] 的方法並註冊
        /// </summary>
        public void RegisterListeners(object target)
        {
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(ListenerAttribute), true);
                foreach (ListenerAttribute attr in attributes)
                {
                    AddListener(attr.EventName, CreateDelegate(method, target));
                }
            }
        }

        /// <summary>
        /// 移除目標物件的所有監聽
        /// </summary>
        public void UnregisterListeners(object target)
        {
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(ListenerAttribute), true);
                foreach (ListenerAttribute attr in attributes)
                {
                    RemoveListener(attr.EventName, target, method);
                }
            }
        }

        private void AddListener(string eventName, Delegate callback)
        {
            if (!eventTable.ContainsKey(eventName))
            {
                eventTable[eventName] = new List<Delegate>();
            }
            eventTable[eventName].Add(callback);
        }

        private void RemoveListener(string eventName, object target, MethodInfo method)
        {
            if (eventTable.TryGetValue(eventName, out var list))
            {
                // 找到對應 Target 和 Method 的 Delegate 並移除
                list.RemoveAll(d => d.Target == target && d.Method == method);
            }
        }

        private Delegate CreateDelegate(MethodInfo method, object target)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
                return Delegate.CreateDelegate(typeof(Action), target, method);
            else if (parameters.Length == 1)
                return Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(parameters[0].ParameterType), target, method);
            else if (parameters.Length == 2)
                return Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType), target, method);
            else if (parameters.Length == 3)
                return Delegate.CreateDelegate(typeof(Action<,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType), target, method);

            Debug.LogError($"Listener does not support methods with {parameters.Length} parameters.");
            return null;
        }
    }

    #endregion

    #region MVC Base Classes

    /// <summary>
    /// View 介面，通常由 MonoBehaviour 實作
    /// </summary>
    public interface IView
    {
        // 標記介面，可擴充通用 View 方法
    }

    /// <summary>
    /// 泛型 View 基底類別，自動處理 Mediator 的注入與註冊
    /// </summary>
    /// <typeparam name="TMediator">對應的 Mediator 類型</typeparam>
    public abstract class BaseView<TMediator> : MonoBehaviour, IView where TMediator : IMediator
    {
        [Inject] protected TMediator mediator;

        protected virtual void OnEnable()
        {
            mediator.Register(this);
        }

        protected virtual void OnDisable()
        {
            mediator.DeRegister(this);
        }
    }

    /// <summary>
    /// Proxy 基底介面，負責保存資料與業務邏輯
    /// </summary>
    public class IProxy
    {
        [Inject] protected Listener listener;
    }

    /// <summary>
    /// Mediator 基底類別，負責 View 與 Proxy 的溝通，以及監聽事件
    /// </summary>
    public abstract class IMediator
    {
        [Inject] protected Listener listener;

        /// <summary>
        /// 當 View 啟用 (OnEnable) 時呼叫此方法進行註冊
        /// </summary>
        public virtual void Register(IView view)
        {
            // 自動註冊此 Mediator 內所有帶有 [Listener] 的方法
            listener.RegisterListeners(this);
        }

        /// <summary>
        /// 當 View 停用 (OnDisable) 時呼叫此方法進行解註冊
        /// </summary>
        public virtual void DeRegister(IView view)
        {
            // 自動移除此 Mediator 的所有監聽
            listener.UnregisterListeners(this);
        }
    }

    /// <summary>
    /// 泛型 Mediator 基底類別，自動處理 View 的轉型與保存
    /// </summary>
    /// <typeparam name="TView">對應的 View 類型</typeparam>
    public abstract class BaseMediator<TView> : IMediator where TView : class, IView
    {
        protected TView view;

        public override void Register(IView view)
        {
            this.view = view as TView;
            if (this.view == null)
            {
                Debug.LogWarning($"Mediator {GetType().Name} expected view of type {typeof(TView).Name} but got {view.GetType().Name}");
            }
            base.Register(view);
        }

        public override void DeRegister(IView view)
        {
            base.DeRegister(view);
            this.view = null;
        }
    }

    /// <summary>
    /// Command 基底類別，用於執行特定邏輯或初始化流程
    /// 改為 ScriptableObject 以便在 Inspector 中配置
    /// </summary>
    public abstract class ICommand : ScriptableObject
    {
        [Inject] protected Listener listener;

        // 標記此 Command 是否為 Lazy 加載 (可選)
        // Lazy = true 代表 Controller 不會等待此 Command 完成
        public bool isLazy = false;

        // 標記工作是否完成
        public bool IsJobComplete { get; protected set; } = false;

        public virtual void Initialize(MonoBehaviour context, Listener listener, DiContainer container)
        {
            this.listener = listener;
            // 重置狀態，因為 ScriptableObject 會保留數值
            IsJobComplete = isLazy;
            // 如果需要手動注入其他依賴可在這裡處理
        }

        public abstract void Execute(MonoBehaviour mono);

        public virtual void SetComplete()
        {
            IsJobComplete = true;
        }
    }

#if UNITY_EDITOR
    public class ScriptableObjectCreator
    {
        [UnityEditor.MenuItem("Assets/Create/Create Command SO from Script", false, 0)]
        public static void CreateScriptableObject()
        {
            var selected = UnityEditor.Selection.activeObject;
            if (selected is UnityEditor.MonoScript script)
            {
                var type = script.GetClass();
                if (type != null && type.IsSubclassOf(typeof(ScriptableObject)))
                {
                    var asset = ScriptableObject.CreateInstance(type);
                    string path = UnityEditor.AssetDatabase.GetAssetPath(script);
                    path = System.IO.Path.GetDirectoryName(path);
                    string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/" + type.Name + ".asset");

                    UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    UnityEditor.EditorUtility.FocusProjectWindow();
                    UnityEditor.Selection.activeObject = asset;
                }
                else
                {
                    Debug.LogWarning("Selected script does not inherit from ScriptableObject.");
                }
            }
        }

        [UnityEditor.MenuItem("Assets/Create/SnekRun/Create Command SO from Script", true)]
        public static bool ValidateCreateScriptableObject()
        {
            var selected = UnityEditor.Selection.activeObject;
            return selected is UnityEditor.MonoScript script && script.GetClass() != null && script.GetClass().IsSubclassOf(typeof(ScriptableObject));
        }
    }
#endif

    #endregion

    #region Helper Services

    /// <summary>
    /// 簡單的注入服務單例，用於在 View 的 Awake 中手動注入依賴
    /// </summary>
    public class InjectService
    {
        private static InjectService _instance;
        public static InjectService Instance => _instance ??= new InjectService();

        private DiContainer _container;

        public void SetContainer(DiContainer container)
        {
            _container = container;
        }

        public void Inject(object obj)
        {
            if (_container != null)
            {
                _container.Inject(obj);
            }
            else
            {
                Debug.LogWarning("InjectService: Container is not set.");
            }
        }
    }

    #endregion
}