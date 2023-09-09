using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Hyuzu;
using TMPro;

public class SongInGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public TMP_Text songTitle;
    public TMP_Text songArtist;
    public TMP_Text songGenre;

    [Space]

    public bool touching;

    public void Start () {
        songArtist.alpha = 0;
        songCover.sprite = song.cover;

        beatIcon.sprite = Hyuzu.AssetManager.GetInstrumentIcon(song.beat.instrument);
        bassIcon.sprite = Hyuzu.AssetManager.GetInstrumentIcon(song.bass.instrument);
        loopIcon.sprite = Hyuzu.AssetManager.GetInstrumentIcon(song.loop.instrument);
        leadIcon.sprite = Hyuzu.AssetManager.GetInstrumentIcon(song.lead.instrument);

        songTitle.text = song.songName;
        songArtist.text = song.artist;
        songGenre.text = song.genre + " * " + song.year;
    }

    public void Update() {
        if(Input.GetKeyDown(KeyCode.T) && touching) {
            SongInfo info = FindObjectOfType<SongInfo>();
            if(info != null) {
                info.song = song;
                info.SwitchSongAndToggle(song);
            }
        }
    }
 
    public void OnPointerEnter(PointerEventData pointerEventData) {
        Debug.Log("enter");

        songTitle.transform.DOLocalMoveY(43.5f, 0.15f);
        songArtist.DOFade(1f, 0.15f);

        SongInfo info = FindObjectOfType<SongInfo>();
        if(info != null && info.infoEnabled) {
            info.song = song;
            info.SwitchSongAndToggle(song);
        }

        touching = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        Debug.Log("exit");
        songTitle.transform.DOLocalMoveY(17.9f, 0.15f);
        songArtist.DOFade(0f, 0.15f);
        touching = false;
    }
}
