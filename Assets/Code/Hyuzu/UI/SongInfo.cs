using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hyuzu;
using DG.Tweening;
using TMPro;

public class SongInfo : MonoBehaviour
{
    public HyuzuSong song;

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

    void Start() {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    void Update() {
        if (infoEnabled) {
            if(Input.GetKeyDown(KeyCode.T)) {
                Hide();
            }
        }
    }

    public void SwitchSongAndToggle(HyuzuSong song) {
        StartCoroutine(ToggleProcess(song));
    }

    IEnumerator ToggleProcess(HyuzuSong song) {
        Hide();
        yield return new WaitUntil(() => GetComponent<CanvasGroup>().alpha == 0);
        this.song = song;
        Show();
    }

    public void Show()
    {
        infoEnabled = true;
        songCover.sprite = song.cover;

        beatIcon.sprite = HyuzuAssetManager.GetInstrumentIcon(song.beat.instrument);
        bassIcon.sprite = HyuzuAssetManager.GetInstrumentIcon(song.bass.instrument);
        loopIcon.sprite = HyuzuAssetManager.GetInstrumentIcon(song.loop.instrument);
        leadIcon.sprite = HyuzuAssetManager.GetInstrumentIcon(song.lead.instrument);

        songInfo.text = song.artist + "\n" + song.songName + "\n" + song.key + " " + song.mode + "\n" + song.BPM + "\n\n" + ((song.creator == "") ? "Unknown" : song.creator);

        transform.DOLocalMoveY(-141f, 0.15f);
        GetComponent<CanvasGroup>().DOFade(1f, 0.15f);
    }

    public void Hide() {
        infoEnabled = false;
        GetComponent<CanvasGroup>().DOFade(0f, 0.15f);
        transform.DOLocalMoveY(-199f, 0.15f);
    }
}
