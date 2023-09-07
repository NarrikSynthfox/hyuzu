using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hyuzu;
using ManagedBass;
using ManagedBass.Mix;
using Unity.VisualScripting;
using UnityEngine;

public class HyuzuAudioManager : MonoBehaviour
{
    public bool previewing, isPlaying;

    public List<int> handles = new List<int>();
    int mixerHandle = 0;

    public void Start() {
        Debug.Log("[Hyuzu] Initing BASS...");
        Bass.Configure(Configuration.IncludeDefaultDevice, true);

        Bass.UpdatePeriod = 5;
        Bass.DeviceBufferLength = 10;
        Bass.PlaybackBufferLength = 75;
        Bass.DeviceNonStop = true;

        Bass.Configure(Configuration.UnicodeDeviceInformation, true);
        Bass.Configure(Configuration.TruePlayPosition, 0);
        Bass.Configure(Configuration.UpdateThreads, 2);
        Bass.Configure(Configuration.FloatDSP, true);

        Bass.Configure((Configuration) 68, 1);
        Bass.Configure((Configuration) 70, false);

        int deviceCount = Bass.DeviceCount;
        Debug.Log($"[Hyuzu] Devices found: {deviceCount}");

        if(!Bass.Init(-1, 44100, DeviceInitFlags.Default | DeviceInitFlags.Latency, IntPtr.Zero)) Debug.LogError("[Hyuzu] Error initing BASS: " + Bass.LastError);
    }

    public void PreviewSong(HyuzuSong song) {
        if (!previewing) {
            InitSongCells(song);
        }
    }

    void InitSongCells(HyuzuSong song) {
        mixerHandle = BassMix.CreateMixerStream(44100, 2, BassFlags.Default);

        if (mixerHandle == 0)
            Debug.LogError($"Failed to init mixer: {Bass.LastError}");

        // Mixer processing threads (for some reason this attribute is undocumented in ManagedBass?)
        Bass.ChannelSetAttribute(mixerHandle, (ChannelAttribute) 86017, 2);

        LoadSongCells(song, song.beat);
        LoadSongCells(song, song.bass);
        LoadSongCells(song, song.loop);
        LoadSongCells(song, song.lead);

        if (!Bass.ChannelPlay(mixerHandle, true)) {
            Debug.LogError($"Failed to play: {Bass.LastError}");
        } else {
            previewing = true;
            isPlaying = true;
        }
    }

    public void LoadSongCells(HyuzuSong song, ClipInfo info) {
        StartCoroutine(LoadSongCell(song, info));
        StartCoroutine(LoadSongMiscCell(song, info));
    }

    IEnumerator LoadSongCell(HyuzuSong song, ClipInfo info) {
        yield return new WaitUntil(() => !isPlaying);

        const BassFlags flags = BassFlags.Prescan | BassFlags.Decode | BassFlags.AsyncFile | BassFlags.Loop | (BassFlags) 64;

        if (song.GetDefaultClip(info) == null)
            yield break;

        int moggIndex = BitConverter.ToInt32(song.GetDefaultClip(info), 4);
        int handle = Bass.CreateStream(song.GetDefaultClip(info), moggIndex, song.GetDefaultClip(info).Length - moggIndex, flags);

        if (handle == 0)
            Debug.LogError($"Failed to load mogg file or position: {Bass.LastError}");

        yield return new WaitUntil(() => handle != 0);

        if (!BassMix.MixerAddChannel(mixerHandle, handle, BassFlags.MixerChanMatrix | BassFlags.MixerChanDownMix)) {
            Debug.Log("Couldn't add channel to mixer! Uh-oh stinky! Error: " + Bass.LastError);
            if (!Bass.StreamFree(mixerHandle)) {
                Debug.LogError("Failed to free stream. THIS WILL SUCK FOR MEMORY.");
            }
        } else {
            handles.Add(handle);
        }
    }

    IEnumerator LoadSongMiscCell(HyuzuSong song, ClipInfo info) {
        for (int i = 0; i < song.GetSharedClips(info).Count; i++)
        {
            yield return new WaitUntil(() => !isPlaying);

            const BassFlags flags = BassFlags.Prescan | BassFlags.Decode | BassFlags.AsyncFile | BassFlags.Loop | (BassFlags) 64;

            if (song.GetSharedClips(info)[i] == null)
                yield break;

            int moggIndex = BitConverter.ToInt32(song.GetSharedClips(info)[i], 4);
            int handle = Bass.CreateStream(song.GetSharedClips(info)[i], moggIndex, song.GetSharedClips(info)[i].Length - moggIndex, flags);

            if (handle == 0)
                Debug.LogError($"Failed to load mogg file or position: {Bass.LastError}");

            yield return new WaitUntil(() => handle != 0);

            if (!BassMix.MixerAddChannel(mixerHandle, handle, BassFlags.MixerChanMatrix | BassFlags.MixerChanDownMix)) {
                Debug.Log("Couldn't add channel to mixer! Uh-oh stinky! Error: " + Bass.LastError);
                if (!Bass.StreamFree(mixerHandle)) {
                    Debug.LogError("Failed to free stream. THIS WILL SUCK FOR MEMORY.");
                }
            } else {
                handles.Add(handle);
            }
        }
    }

    public void OnApplicationQuit() {
        Bass.Stop();
		Bass.Free();
    }

    public void StopPreviewSong() {
        if (!Bass.ChannelStop(mixerHandle)) Debug.LogError("Failed to stop stream. Error: " + Bass.LastError);
    
        for (int i = 0; i < handles.Count; i++)
        {
            if (handles[i] != 0)
                if (!Bass.StreamFree(handles[i])) Debug.LogError("Failed to free stream. Error: " + Bass.LastError); 
        }

        handles.Clear();

        if (!Bass.StreamFree(mixerHandle))
            Debug.LogError("Failed to free stream. THIS WILL SUCK FOR MEMORY.");

        mixerHandle = 0;

        isPlaying = false;
        previewing = false;
    }
}