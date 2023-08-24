using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public ButtonManager manager;
    public UnityEvent events;

    void Start()
    {
        manager = FindAnyObjectByType<ButtonManager>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        manager.ChangeBGPos(transform);
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        events.Invoke();
    }
}
