using UnityEngine;
using UnityEngine.EventSystems;

namespace URDev.GameFramework
{
    public abstract class BaseScene : MonoBehaviour
    {
        public string SceneName { get; protected set; }

        void Awake()
        {
            Init();
        }

        // call by Start
        protected virtual void Init()
        {
            // check EventSystem
            Object obj = Object.FindFirstObjectByType<EventSystem>();
            if (obj == null)
            {
                GameObject eventSystem = Managers.Resource.Instantiate(AssetKeys.Prefabs.EventSystem);
                if (eventSystem != null)
                    eventSystem.name = "@EventSystem";
            }
        }

        public abstract void Clear();
    }
}
