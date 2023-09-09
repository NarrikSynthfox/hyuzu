using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuLightsManager : MonoBehaviour
{
    public Vector3[] lightPositions = new Vector3[4];

    void Start()
    {
        Light[] lights = GetComponentsInChildren<Light>();
        for (int i = 0; i < lights.Length; i++)
        {
            lightPositions[i] = lights[i].transform.position;
        }

        StartCoroutine(LightPosSwitch());
    }

   IEnumerator LightPosSwitch() {
        Light[] lights = GetComponentsInChildren<Light>();
        for (int i = 0; i < lights.Length; i++)
        {
            float time = Random.Range(15, 20);

            lights[i].transform.DOMoveX(lights[i].transform.position.x + Random.Range(-2f, 2f), time);
            lights[i].transform.DOMoveY(lights[i].transform.position.y + Random.Range(-2f, 2f), time);

            yield return new WaitForSeconds(time);
            StartCoroutine(LightPosSwitch());
        }
   }
}
