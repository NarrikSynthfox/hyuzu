using System.Collections;
using System.Collections.Generic;
using Hyuzu;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public TMP_Text[] versionTexts;
    public TMP_Text randomText;

    void Start()
    {
        for (int i = 0; i < versionTexts.Length; i++)
            versionTexts[i].text = Global.version;

        randomText.text = Global.GetRandomText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
