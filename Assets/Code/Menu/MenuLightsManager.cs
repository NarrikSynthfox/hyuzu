using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuLightsManager : MonoBehaviour
{
    public Vector3[] lightPositions = new Vector3[4];

    void Start()
    {
        for (int i = 0; i < GetComponentsInChildren<Light>().Length; i++)
        {
            lightPositions[i] = GetComponentsInChildren<Light>()[i].transform.position;
        }

        StartCoroutine(LightPosSwitch());
    }

   IEnumerator LightPosSwitch() {
    for (int i = 0; i < GetComponentsInChildren<Light>().Length; i++)
    {
        float time = Random.RandomRange(15, 20);

        GetComponentsInChildren<Light>()[i].transform.DOMoveX(GetComponentsInChildren<Light>()[i].transform.position.x + Random.Range(-2f, 2f), time);
        GetComponentsInChildren<Light>()[i].transform.DOMoveY(GetComponentsInChildren<Light>()[i].transform.position.y + Random.Range(-2f, 2f), time);

        yield return new WaitForSeconds(time);
        StartCoroutine(LightPosSwitch());
    }
   }
}
