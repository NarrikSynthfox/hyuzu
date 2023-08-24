using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuLightsManager : MonoBehaviour
{
    public Vector3[] lightPositions = new Vector3[4];

    void Start()
    {
        for (int i = 0; i < GetComponentsInChildren<Light>().Length - 1; i++)
        {
            lightPositions[i] = GetComponentsInChildren<Light>()[i].transform.position;
        }

        StartCoroutine(LightPosSwitch());
    }

   IEnumerator LightPosSwitch() {
    for (int i = 0; i < GetComponentsInChildren<Light>().Length - 1; i++)
    {
        float time = Random.RandomRange(15, 20);

        GetComponentsInChildren<Light>()[i].transform.DOMoveX(Random.Range(0.9f, 9.1f), time);
        GetComponentsInChildren<Light>()[i].transform.DOMoveY(Random.Range(2.848807f, 7.678807f), time);

        yield return new WaitForSeconds(time);
        StartCoroutine(LightPosSwitch());
    }
   }
}
