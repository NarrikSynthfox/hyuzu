using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hyuzu;
using ManagedBass;
using ManagedBass.Mix;
using UnityEngine;

public class HyuzuAudioManager : MonoBehaviour
{
    public List<AudioSource> activeSources;
    public bool previewing;

    int[] moggStreamHandles = new int[4];
    int[] channelHandles = new int[4];
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
        {
            Debug.LogError($"Failed to init mixer: {Bass.LastError}");
        }

        StartCoroutine(LoadAndPlaySongCell(song, song.beat, 0));
        StartCoroutine(LoadAndPlaySongCell(song, song.bass, 1));
        StartCoroutine(LoadAndPlaySongCell(song, song.loop, 2));
        StartCoroutine(LoadAndPlaySongCell(song, song.lead, 3));

        previewing = true;
    }

    IEnumerator LoadAndPlaySongCell(HyuzuSong song, ClipInfo info, int index) {
        int moggIndex = BitConverter.ToInt32(song.GetDefaultClip(info), 4);
        moggStreamHandles[index] = Bass.SampleLoad(song.GetDefaultClip(info), moggIndex, song.GetDefaultClip(info).Length - moggIndex, 1, 0);

        yield return new WaitUntil(() => moggStreamHandles[index] != 0);

        if (moggStreamHandles[index] == 0)
            Debug.LogError($"Failed to load mogg file or position: {Bass.LastError}");

        channelHandles[index] = Bass.SampleGetChannel(moggStreamHandles[index]);
        Bass.ChannelFlags(channelHandles[index], BassFlags.Loop, BassFlags.Loop);

        yield return new WaitUntil(() => moggStreamHandles[index] != 0);

        if (!Bass.ChannelPlay(channelHandles[index], false)) {
            Debug.LogError($"Failed to play: {Bass.LastError}");
        }
    }

    public void OnDestroy() {
        Bass.Stop();
		Bass.Free();
    }

    public void StopPreviewSong() {
        for (int i = 0; i < channelHandles.Length; i++)
        {
            Bass.ChannelStop(channelHandles[i]);
            Bass.SampleFree(moggStreamHandles[i]);
        }
        previewing = false;
    }
}