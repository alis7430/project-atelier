using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace URDev.GameFramework
{
    public static class MonoBehaviourExtensions
    {
        public static T AddMissingComponent<T>(this GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        public static void AddUIEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
        {
            UI_EventHandler evt = go.AddMissingComponent<UI_EventHandler>();
            switch (type)
            {
                case Define.UIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                case Define.UIEvent.Drag:
                    evt.OnDragHandler -= action;
                    evt.OnDragHandler += action;
                    break;
            }
        }
    }
}
