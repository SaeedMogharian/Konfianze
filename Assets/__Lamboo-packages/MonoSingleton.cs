using UnityEngine;

namespace __Lamboo_packages
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object Lock = new object();
        private static bool _quitting;

        public static T Instance
        {
            get
            {
                if (_quitting) return null;

                lock (Lock)
                {
                    if (_instance != null) return _instance;
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance != null) return _instance;
                    var singleton = new GameObject(typeof(T).Name);
                    _instance = singleton.AddComponent<T>();
                    DontDestroyOnLoad(singleton);

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }
    }
}