using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyuzu;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

namespace Hyuzu {
    [System.Serializable]
    public struct Keyzone {
        public string label;
        public int index;

        [Space]

        public bool unpitched;
        public HyuzuEnums.KeymapPreset preset;
    }

    [System.Serializable]
    public class ClipInfo {
        public List<HyuzuMogg> clipsRaw = new List<HyuzuMogg>();
        public HyuzuFusion metadata = new HyuzuFusion();

        [Space]

        public Keyzone[] keyzonesClips;
        public Keyzone[] keyzonesRisers;

        [Space]

        public float[] pickups;
        public HyuzuEnums.Instruments instrument = HyuzuEnums.Instruments.None;

        [Space]

        public HyuzuEnums.DiscLength discLength = HyuzuEnums.DiscLength.BARS_32;
    }

    [CreateAssetMenu(fileName = "Song", menuName = "Hyuzu/New Song", order = 1)]
    public class HyuzuSong : ScriptableObject
    {
        public string creator;

        [Space]

        [Header("Song Info")]

        public string songName;
        public string artist;

        [Space]

        public HyuzuEnums.Genres genre;
        public string subgenre;

        [Space]

        public int year;

        [Space]

        public int BPM;

        [Space]

        public HyuzuEnums.Keys key;
        public HyuzuEnums.Modes mode;

        public Dictionary<HyuzuEnums.Keys, int> transposes;

        [Space]

        public Sprite cover;

        [Header("Clips")]

        [Space]

        [SerializeField]
        public ClipInfo beat = new ClipInfo();

        [SerializeField]
        public ClipInfo bass = new ClipInfo();

        [SerializeField]
        public ClipInfo loop = new ClipInfo();

        [SerializeField]
        public ClipInfo lead = new ClipInfo();

        public bool fromPak = false;

        public byte[] GetDefaultClip(ClipInfo songCell) {
            foreach (Keyzone item in songCell.keyzonesClips)
            {
                if ((int)item.preset == (int)mode) {
                    return songCell.clipsRaw[item.index].data;
                }
            }
            return null;
        }

        public List<byte[]> GetSharedClips(ClipInfo songCell) {
            List<byte[]> bytes = new List<byte[]>();
            foreach (Keyzone item in songCell.keyzonesClips)
            {
                if (item.preset == HyuzuEnums.KeymapPreset.Shared) {
                    bytes.Add(songCell.clipsRaw[item.index].data);
                }
            }
            return bytes;
        }

        public HyuzuSong() {
            if (transposes == null) {
                transposes = new Dictionary<HyuzuEnums.Keys, int>();

                for (int i = 0; i < (int)HyuzuEnums.Keys.B; i++)
                {
                    transposes.Add((HyuzuEnums.Keys)i, ((int)key + 1 - i + 1));
                }
            }
        }
    }
}