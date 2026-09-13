using System.Collections.Generic;
using UnityEngine;

namespace URDev.GameFramework
{
    public class UIManager : IGameSystem
    {
        int _order = 0;

        Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
        UI_Scene _sceneUI = null;

        public void Init()
        {
        }

        public GameObject Root
        {
            get
            {
                var root = GameObject.Find("@UI_Root");
                if (root == null)
                    root = new GameObject { name = "@UI_Root" };

                return root;
            }
        }
        public void SetCanvas(GameObject go, bool sort = true)
        {
            Canvas canvas = go.AddMissingComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            if (sort)
            {
                canvas.sortingOrder = _order;
                _order++;
            }
            else
            {
                canvas.sortingOrder = 0;
            }
        }

        public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate($"{AssetKeys.UI.SubItemRoot}/{name}");
            if (go == null)
                return null;

            if (parent != null)
                go.transform.SetParent(parent, false);

            return go.AddMissingComponent<T>();
        }

        public T ShowSceneUI<T>(string name = null) where T : UI_Scene
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate($"{AssetKeys.UI.SceneRoot}/{name}");
            if (go == null)
                return null;

            T sceneUI = go.AddMissingComponent<T>();
            _sceneUI = sceneUI;

            go.transform.SetParent(Root.transform, false);

            return sceneUI;
        }

        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate($"{AssetKeys.UI.PopupRoot}/{name}");
            if (go == null)
                return null;

            T popup = go.AddMissingComponent<T>();
            _popupStack.Push(popup);

            go.transform.SetParent(Root.transform, false);

            return popup;
        }

        public void ClosePopupUI(UI_Popup popup)
        {
            if (_popupStack.Count == 0)
                return;

            if (_popupStack.Peek() != popup)
            {
                Debug.Log("Close Popup Failed!");
                return;
            }

            ClosePopupUI();
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0)
                return;

            UI_Popup popup = _popupStack.Pop();
            Managers.Resource.Destroy(popup.gameObject);
            popup = null;

            _order = Mathf.Max(0, _order - 1);
        }

        public void CloseAllPopupUI()
        {
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }

        public void Clear()
        {
            CloseAllPopupUI();
            _sceneUI = null;
        }
    }
}
