using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class FightButtonComponent : MonoBehaviour, IPointerClickHandler
{
    RectTransform _rectTransform;
    Canvas _canvas;
    CanvasGroup _canvasGroup;
    public UnityEvent fightEvent;
    public void OnPointerClick(PointerEventData eventData)
    {
        fightEvent.Invoke();  
    }
}
