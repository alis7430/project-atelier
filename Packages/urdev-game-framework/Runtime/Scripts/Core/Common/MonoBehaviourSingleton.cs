using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ur-dev commented
/// A generic singleton class for unity monobehaviour
/// Inherit from this base class to create a singleton.
/// 
/// - Automatically creates object in scene if not exist.
/// - Find existing instance first, so it can't create object when scene was changed
/// - It has thread lock
/// 
/// e.g. public class MyClassName : MonoBehaviourSingleton<MyClassName>
/// </summary>
namespace URDev.GameFramework
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region[Fields]
        private static T _instance;
        private static bool _shuttingDown = false;
        private static readonly object _instanceLock = new object();

        [SerializeField]
        [FormerlySerializedAs("_DontDestoryOnLoad")]
        private bool _dontDestroyOnLoad = true;
        #endregion

        #region [Properties]
        public static T Instance
        {
            get
            {
                if (_shuttingDown)
                {
                    Debug.LogWarning("A singleton instance '" + typeof(T) + "' already destroyed. It returns null.");
                    return null;
                }

                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = (T)Object.FindFirstObjectByType(typeof(T));

                    if (_instance == null)
                    {
                        var go = new GameObject();
                        _instance = go.AddComponent<T>();
                        go.name = "[Singleton]" + typeof(T).Name;
                    }

                    return _instance;
                }
            }
        }
        #endregion

        #region [Methods]
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;

            // make persistent intance
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this.gameObject);

            OnInit();
        }

        private void OnApplicationQuit() => _shuttingDown = true;
        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
        protected virtual void OnInit() { }
        #endregion
    }
}
