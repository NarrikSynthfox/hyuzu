using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Hyuzu;

public class SongViewer : MonoBehaviour
{
    public Song song;
    [Space]
    public Image img;
    public TMP_Text text;
    [Space]
    public AudioSource[] sources;
    public AudioSource sources2;

    void Start()
    {
        //sources[0].clip = song.GetPreviewClip(song.beat);
        //sources[1].clip = song.GetPreviewClip(song.bass);
        //sources[2].clip = song.GetPreviewClip(song.loop);
        //sources[3].clip = song.GetPreviewClip(song.lead);

        /*foreach (Keyzone zone in song.lead.keyzonesClips)
        {
            if (song.lead.clips[zone.index] != sources[3].clip && zone.preset == Enums.KeymapPreset.Shared) {
                AudioSource newSource = sources[3].gameObject.AddComponent<AudioSource>();
                newSource.clip = song.lead.clips[zone.index];
                sources2 = newSource;
            }
        }*/

        img.sprite = song.cover;
        text.text = "<size=32>" + song.songName + "\n<size=18>by " + song.artist + "\n\n<size=26>Genre: " + song.genre + "\nBPM: " + song.BPM + "\nKey/Mode: " + song.key + " " + song.mode + "\n\nYear: " + song.year + "\n\nHold Z to preview";
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            if (Input.GetKey(KeyCode.Z)) {
                if (!sources[i].isPlaying) {
                    sources[i].Play();
                    sources2.Play();
                }
            }
            else {
                sources[i].Stop();
                sources2.Stop();
            }
        }

        
    }
}
