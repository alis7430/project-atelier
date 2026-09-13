using System.Collections.Generic;
using UnityEngine;

namespace URDev.GameFramework
{
    public interface IDictable<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager : IGameSystem
    {
        readonly Dictionary<string, object> _tablesByKey = new Dictionary<string, object>();
        readonly Dictionary<System.Type, object> _tablesByType = new Dictionary<System.Type, object>();

        public void Init()
        {
        }

        public void Clear()
        {
        }

        public Loader LoadJson<Loader>(string path)
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{AssetKeys.DataRoot}/{path}");
            if (textAsset == null)
            {
                Debug.LogError($"Failed to load data table: {path}");
                return default;
            }

            return JsonUtility.FromJson<Loader>(textAsset.text);
        }

        public Dictionary<Key, Value> LoadTable<Loader, Key, Value>(string path) where Loader : IDictable<Key, Value>
        {
            Loader loader = LoadJson<Loader>(path);
            if (loader == null)
                return new Dictionary<Key, Value>();

            Dictionary<Key, Value> table = loader.MakeDict();
            RegisterTable(path, table);
            return table;
        }

        public void RegisterTable<T>(string key, T table)
        {
            if (string.IsNullOrEmpty(key) || table == null)
                return;

            _tablesByKey[key] = table;
            _tablesByType[typeof(T)] = table;
        }

        public T GetTable<T>(string key) where T : class
        {
            if (_tablesByKey.TryGetValue(key, out object table))
                return table as T;

            return null;
        }

        public T GetTable<T>() where T : class
        {
            if (_tablesByType.TryGetValue(typeof(T), out object table))
                return table as T;

            return null;
        }

        public bool TryGetTable<T>(string key, out T table) where T : class
        {
            table = GetTable<T>(key);
            return table != null;
        }
    }
}
