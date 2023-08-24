using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Hyuzu;
using TMPro;

public class SongInChart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public HyuzuSong song;

    [Space]

    public Transform textContainer;
    public CanvasGroup instrumentContainer;

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

    [Space]

    public bool touching;

    [Space]

    public bool loadFromPak;
    public string pakName = "test.pak";

    [Space]

    public CanvasGroup preview;

    public void Start () {
        if (loadFromPak) {
            song = new HyuzuPakManager().TurnPAKFileIntoSong(pakName);
            songCover.transform.localScale = new Vector2(1f, -1f);
        }

        instrumentContainer.alpha = 0;

        songCover.sprite = song.cover;
        if(song.cover == null) { 
            songCover.sprite = HyuzuAssetManager.GetMissingAlbumArtIcon();
            songCover.transform.DOScaleY(1f, 0f);
        }

        beatIcon.sprite = HyuzuAssetManager.GetInstrumentIcon(song.beat.instrument);
        bassIcon.sprite = HyuzuAssetManager.GetInstrumentIcon(song.bass.instrument);
        loopIcon.sprite = HyuzuAssetManager.GetInstrumentIcon(song.loop.instrument);
        leadIcon.sprite = HyuzuAssetManager.GetInstrumentIcon(song.lead.instrument);

        songTitle.text = song.songName;
        songArtist.text = song.artist;
    }

    public void Update() {
        transform.DOScale(new Vector2(0.414141f, 0.414141f), 0.5f);
        if(Input.GetKey(KeyCode.Z) && touching) {
            FindObjectOfType<HyuzuAudioManager>().PreviewSong(song);
            preview.DOFade(1f, 0.15f);
        }
        else if(!Input.GetKey(KeyCode.Z)){
            FindObjectOfType<HyuzuAudioManager>().StopPreviewSong();
            preview.DOFade(0f, 0.15f);
        }

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

        textContainer.DOLocalMoveY(-57f, 0.15f);
        instrumentContainer.DOFade(1f, 0.15f);

        SongInfo info = FindObjectOfType<SongInfo>();
        if(info != null && info.infoEnabled) {
            info.song = song;
            info.SwitchSongAndToggle(song);
        }

        touching = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        Debug.Log("exit");

        textContainer.DOLocalMoveY(-97f, 0.15f);
        
        instrumentContainer.DOFade(0f, 0.15f);
        preview.DOFade(0f, 0.15f);

        touching = false;
        FindObjectOfType<HyuzuAudioManager>().StopPreviewSong();
    }
}
