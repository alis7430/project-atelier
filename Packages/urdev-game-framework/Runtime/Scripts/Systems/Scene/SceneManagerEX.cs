using UnityEngine;
using UnityEngine.SceneManagement;

namespace URDev.GameFramework
{
    public class SceneManagerEX : IGameSystem
    {
        public BaseScene CurrentScene
        {
            get
            {
                return Object.FindFirstObjectByType<BaseScene>();
            }
        }
        
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name cannot be null or empty.");
                return;
            }

            Managers.Clear();
            SceneManager.LoadScene(sceneName);
        }

        public void LoadScene(int buildIndex)
        {
            Managers.Clear();
            SceneManager.LoadScene(buildIndex);
        }

        public void Clear()
        {
            CurrentScene?.Clear();
        }

        public void Init()
        {
        }
    }
}
