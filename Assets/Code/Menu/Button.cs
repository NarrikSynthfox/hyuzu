using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public ButtonManager manager;
    public UnityEvent events;
    
    public void OnPointerEnter(PointerEventData pointerEventData) {
        if (manager != null) manager.ChangeBGPos(transform);
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        events.Invoke();
    }
}
