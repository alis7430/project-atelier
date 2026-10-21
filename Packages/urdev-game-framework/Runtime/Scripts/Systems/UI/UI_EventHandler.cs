using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Click, Drag등의 UI이벤트를 캐치하여 콜백으로 날려줌
// 일반 UI는 Button을 사용하고 특수 상호작용이 필요할 때 UI_EventHandler를 사용하면 됨
namespace URDev.GameFramework
{
    public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        public event Action<PointerEventData> OnClickHandler = null;
        public event Action<PointerEventData> OnDragHandler = null;

        public void OnDrag(PointerEventData eventData)
        {
            OnDragHandler?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickHandler?.Invoke(eventData);
        }
    }
}
