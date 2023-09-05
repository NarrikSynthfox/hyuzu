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
    public struct ClipInfo {
        public HyuzuMogg[] clipsRaw;
        public HyuzuFusion metadata;
        public AudioClip[] risers;

        [Space]

        public Keyzone[] keyzonesClips;
        public Keyzone[] keyzonesRisers;

        [Space]

        public float[] pickups;
        public HyuzuEnums.Instruments instrument;
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
        public ClipInfo beat;

        [SerializeField]
        public ClipInfo bass;

        [SerializeField]
        public ClipInfo loop;

        [SerializeField]
        public ClipInfo lead;

        public bool fromPak = false;

        public byte[] GetDefaultClip(ClipInfo songCell) {
            foreach (Keyzone item in songCell.keyzonesClips)
            {
                if ((int)item.preset == (int)mode) {
                    return songCell.clipsRaw[item.index].data;
                }
                else if (item.preset == HyuzuEnums.KeymapPreset.Shared) {
                    return songCell.clipsRaw[0].data;
                }
            }
            return null;
        }

        public HyuzuSong() {
            if (transposes == null) {
                transposes = new Dictionary<HyuzuEnums.Keys, int>();

                for (int i = 0; i < (int)HyuzuEnums.Keys.B; i++)
                {
                    transposes.Add((HyuzuEnums.Keys)i, ((int)key + 1 - i + 1));
                }
            }

            beat.metadata = new HyuzuFusion();
            bass.metadata = new HyuzuFusion();
            loop.metadata = new HyuzuFusion();
            lead.metadata = new HyuzuFusion();
        }
    }
}