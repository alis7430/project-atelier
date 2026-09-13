using UnityEngine;

// Interface for convert addressables after..
namespace URDev.GameFramework
{
    public interface IAssetProvider
    {
        T Load<T>(string path) where T : Object;
        GameObject Instantiate(string path, Transform parent = null);
        void Destroy(GameObject go);
    }

    public class ResourceManager : IGameSystem, IAssetProvider
    {
        public void Init()
        {
        }

        public void Clear()
        {
        }

        public T Load<T>(string path) where T : Object
        {
            // check pooling
            if (typeof(T) == typeof(GameObject))
            {
                GameObject go = FindPoolObject(path);
                if (go != null)
                    return go as T;
            }

            return Resources.Load<T>(path);
        }

        public GameObject Instantiate(string path, Transform parent = null)
        {
            var prefab = Load<GameObject>($"{AssetKeys.PrefabRoot}/{path}");
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab: {path}");
                return null;
            }

            // check pooling
            if (prefab.GetComponent<PoolableObject>() != null)
            {
                PoolableObject poolable = Managers.Pool.Pop(prefab, parent);
                return poolable != null ? poolable.gameObject : null;
            }

            var go = Object.Instantiate(prefab, parent);
            go.name = prefab.name;  //remove "(Clone)" string

            return go;
        }

        public void Destroy(GameObject go)
        {
            if (go == null) return;

            // check pooling
            PoolableObject poolable = go.GetComponent<PoolableObject>();
            if (poolable != null)
            {
                Managers.Pool.Push(poolable);
                return;
            }

            Object.Destroy(go);
        }

        private GameObject FindPoolObject(string path)
        {
            string name = path;

            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            return go;
        }
    }
}
