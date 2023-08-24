using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hyuzu;
using UnityEngine;

public class MenuCrate : MonoBehaviour
{
    public string[] paths;

    [Space]

    public GameObject songContentsObj;
    public GameObject songInChartPrefab;

    void Start()
    {
        StartCoroutine(LoadSong());
    }

    IEnumerator LoadSong() {
        paths = Directory.GetFiles(Application.streamingAssetsPath + "/pak/", "*.pak", SearchOption.AllDirectories);
        
        for (int i = 0; i < paths.Length; i++)
        {
            GameObject prefab = Instantiate(songInChartPrefab);

            prefab.GetComponent<SongInChart>().pakName = paths[i];
            prefab.GetComponent<SongInChart>().loadFromPak = true;

            yield return new WaitUntil(() =>  prefab.GetComponent<SongInChart>().song != null);

            Debug.Log("PAK file loaded: " + paths[i]);

            prefab.transform.parent = songContentsObj.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
