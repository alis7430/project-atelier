using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace URDev.GameFramework
{
    public abstract class UI_Base : MonoBehaviour
    {
        Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
        bool _initialized;

        protected bool IsInitialized => _initialized;

        public virtual void Init()
        {
            if (_initialized)
                return;

            _initialized = true;
        }

        protected void Bind<T>(Type t) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(t);
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];

            _objects[typeof(T)] = objects;
            for (int i = 0; i < objects.Length; i++)
            {
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Utils.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

                if (objects[i] == null)
                    Debug.LogError($"Failed to bind UI object. Root={gameObject.name}, Name={names[i]}, Type={typeof(T).Name}");
            }
        }

        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = null;
            if (_objects.TryGetValue(typeof(T), out objects) == false)
                return null;

            if (idx < 0 || idx >= objects.Length)
                return null;

            return objects[idx] as T;
        }


        protected GameObject GetObject(int idx) => Get<GameObject>(idx);
        protected Text GetText(int idx) => Get<Text>(idx);
        protected Button GetButton(int idx) => Get<Button>(idx);
        protected Image GetImage(int idx) => Get<Image>(idx);
    }
}
