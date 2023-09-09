using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hyuzu;
using DG.Tweening;
using TMPro;

public class SongInfo : MonoBehaviour
{
    public Song song;

    [Space]

    public Image songCover;

    [Space]

    public Image beatIcon;
    public Image bassIcon;
    public Image loopIcon;
    public Image leadIcon;

    [Space]

    public TMP_Text songInfo;
    public bool infoEnabled;

    float offset = 21.5f;
    float posY = 0;

    void Start() {
        GetComponent<CanvasGroup>().alpha = 0;
        posY = transform.position.y;
    }

    void Update() {
        if (infoEnabled) {
            if(Input.GetKeyDown(KeyCode.T)) {
                Hide();
            }
        }
    }

    public void SwitchSongAndToggle(Song song) {
        Hide();
        this.song = song;
        Show();
    }

    public void Show()
    {
        infoEnabled = true;

        songCover.sprite = song.cover;
        
        if (song.fromPak) songCover.transform.DOScaleY(-0.7828592f, 0f);
        if(song.cover == null) { 
            songCover.sprite = Hyuzu.AssetManager.GetMissingAlbumArtIcon();
            songCover.transform.DOScaleY(0.7828592f, 0f);
        }

        beatIcon.sprite = Hyuzu.AssetManager.GetInstrumentIcon(song.beat.instrument);
        bassIcon.sprite = Hyuzu.AssetManager.GetInstrumentIcon(song.bass.instrument);
        loopIcon.sprite = Hyuzu.AssetManager.GetInstrumentIcon(song.loop.instrument);
        leadIcon.sprite = Hyuzu.AssetManager.GetInstrumentIcon(song.lead.instrument);

        string songTitle = (song.songName.Length > 35) ? song.songName.Substring(0, 20) + "..." : song.songName;
        songInfo.text = song.artist + "\n" + songTitle + "\n" + song.key + " " + song.mode + "\n" + song.BPM + "\n\n" + ((song.creator == "") ? "Unknown" : song.creator);

        transform.DOMoveY(102.8996f, 0.15f);
        GetComponent<CanvasGroup>().DOFade(1f, 0.15f);
    }

    public void Hide() {
        infoEnabled = false;
        GetComponent<CanvasGroup>().DOFade(0f, 0.15f);
        transform.DOMoveY(78.7f, 0.15f);
    }
}
