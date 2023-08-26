using System.Collections;
using System.Collections.Generic;
using Hyuzu;
using UnityEngine;

public class HyuzuAudioManager : MonoBehaviour
{
    public List<AudioSource> activeSources;
    public bool previewing;

    public void PreviewSong(HyuzuSong song) {
        if (!previewing) {
            InitSongCells(song);
        }
    }

    void InitSongCells(HyuzuSong song) {
        InitBeatClip(song);
        InitBassClip(song);
        InitLoopClip(song);
        InitLeadClip(song);
    }

    void InitBeatClip(HyuzuSong song) {
        if(song.beat.clips != null) {
            GameObject beatObj = new GameObject("Beat");
            beatObj.transform.parent = transform;

            foreach (Keyzone zone in song.beat.keyzonesClips)
            {
                if ((int)zone.preset == (int)song.mode || zone.preset == HyuzuEnums.KeymapPreset.Shared) {
                    GameObject zoneObj = new GameObject(zone.preset.ToString());
                    zoneObj.transform.parent = beatObj.transform;

                    AudioSource source = zoneObj.AddComponent<AudioSource>();

                    source.loop = true;
                    source.clip = song.beat.clips[zone.index];

                    if(!source.isPlaying) { source.Play(); previewing = true; } 
                    activeSources.Add(source);
                }
            }
        }  else if (song.beat.clips == null) previewing = false;
    }

    void InitBassClip(HyuzuSong song) {
        if(song.bass.clips != null) {
            GameObject bassObj = new GameObject("Bass");
            bassObj.transform.parent = transform;

            foreach (Keyzone zone in song.bass.keyzonesClips)
            {
                if ((int)zone.preset == (int)song.mode || zone.preset == HyuzuEnums.KeymapPreset.Shared) {
                    GameObject zoneObj = new GameObject(zone.preset.ToString());
                    zoneObj.transform.parent = bassObj.transform;

                    AudioSource source = zoneObj.AddComponent<AudioSource>();

                    source.loop = true;
                    source.clip = song.bass.clips[zone.index];

                    if(!source.isPlaying) { source.Play(); previewing = true; } 
                    activeSources.Add(source);
                }
            }
        } else if (song.bass.clips == null) previewing = false;
    }

    void InitLoopClip(HyuzuSong song) {
        if (song.loop.clips != null) {
            GameObject loopObj = new GameObject("Loop");
            loopObj.transform.parent = transform;

            foreach (Keyzone zone in song.loop.keyzonesClips)
            {
                if ((int)zone.preset == (int)song.mode || zone.preset == HyuzuEnums.KeymapPreset.Shared) {
                    GameObject zoneObj = new GameObject(zone.preset.ToString());
                    zoneObj.transform.parent = loopObj.transform;

                    AudioSource source = zoneObj.AddComponent<AudioSource>();

                    source.loop = true;
                    source.clip = song.loop.clips[zone.index];

                    if(!source.isPlaying) { source.Play(); previewing = true; } 
                    activeSources.Add(source);
                }
            }
        } else if (song.loop.clips == null) previewing = false;
    }

      void InitLeadClip(HyuzuSong song) {
        if (song.lead.clips != null) {
            GameObject leadObj = new GameObject("Lead");
            leadObj.transform.parent = transform;

            foreach (Keyzone zone in song.lead.keyzonesClips)
            {
                if ((int)zone.preset == (int)song.mode || zone.preset == HyuzuEnums.KeymapPreset.Shared) {
                    GameObject zoneObj = new GameObject(zone.preset.ToString());
                    zoneObj.transform.parent = leadObj.transform;

                    AudioSource source = zoneObj.AddComponent<AudioSource>();

                    source.loop = true;
                    source.clip = song.lead.clips[zone.index];

                    if(!source.isPlaying) { source.Play(); previewing = true; } 
                    activeSources.Add(source);
                }
            }
        } else if (song.lead.clips == null) previewing = false;
    }

    public void StopPreviewSong() {
        if(activeSources.Count != 0) {
            foreach (AudioSource source in activeSources)
            {
                source.Stop();
                Destroy(source.transform.parent.gameObject);
            }
            activeSources.Clear();
            previewing = false;
        }
    }
}