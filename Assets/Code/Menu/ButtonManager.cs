using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    public Transform buttonBG;

    public void ChangeBGPos(Transform transform) {
        buttonBG.DOMoveY(transform.position.y, 0.1f);
    }
}
