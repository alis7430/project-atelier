using System.Collections.Generic;
using UnityEngine;

namespace URDev.GameFramework
{
    public class Managers : MonoBehaviourSingleton<Managers>
    {
        public static DataManager Data { get { return Instance._data; } }
        public static InputManager Input { get { return Instance._input; } }
        public static PoolManager Pool { get { return Instance._pool; } }
        public static ResourceManager Resource { get { return Instance._resource; } }
        public static SceneManagerEX Scene { get { return Instance._scene; } }
        public static SoundManager Sound { get { return Instance._sound; } }
        public static UIManager UI { get { return Instance._ui; } }

        DataManager _data = new DataManager();
        InputManager _input = new InputManager();
        PoolManager _pool = new PoolManager();
        ResourceManager _resource = new ResourceManager();
        SceneManagerEX _scene = new SceneManagerEX();
        SoundManager _sound = new SoundManager();
        UIManager _ui = new UIManager();
        
        readonly List<IGameSystem> _systems = new List<IGameSystem>();
        readonly List<IUpdatableSystem> _updatableSystems = new List<IUpdatableSystem>();
        bool _initialized;

        protected override void OnInit()
        {
            base.OnInit();

            if (_initialized)
                return;

            _systems.Clear();
            _updatableSystems.Clear();

            AddSystem(_resource, false);
            AddSystem(_sound, false);
            AddSystem(_pool, false);
            AddSystem(_data, false);
            AddSystem(_input, false);
            AddSystem(_scene, false);
            AddSystem(_ui, false);

            foreach (IGameSystem system in _systems)
                system.Init();

            _initialized = true;
        }

        void Update()
        {
            for (int i = 0; i < _updatableSystems.Count; i++)
                _updatableSystems[i].Tick(Time.deltaTime);
        }

        public static void RegisterSystem(IGameSystem system)
        {
            Instance.AddSystem(system, true);
        }

        public static T RegisterSystem<T>(T system) where T : class, IGameSystem
        {
            Instance.AddSystem(system, true);
            return system;
        }

        public static T GetSystem<T>() where T : class, IGameSystem
        {
            foreach (IGameSystem system in Instance._systems)
            {
                if (system is T target)
                    return target;
            }

            return null;
        }

        public static void Clear()
        {
            List<IGameSystem> systems = Instance._systems;
            for (int i = systems.Count - 1; i >= 0; i--)
                systems[i].Clear();
        }

        void AddSystem(IGameSystem system, bool initializeNow)
        {
            if (system == null || _systems.Contains(system))
                return;

            _systems.Add(system);

            if (system is IUpdatableSystem updatableSystem)
                _updatableSystems.Add(updatableSystem);

            if (initializeNow && _initialized)
                system.Init();
        }
    }
}
