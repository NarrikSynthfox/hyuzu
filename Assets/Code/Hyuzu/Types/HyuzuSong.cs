using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
        public Enums.KeymapPreset preset;
    }

    [System.Serializable]
    public class ClipInfo {
        public List<Mogg> clipsRaw = new List<Mogg>();
        public HyuzuFusion metadata = new HyuzuFusion();

        [Space]

        public Keyzone[] keyzonesClips;
        public Keyzone[] keyzonesRisers;

        [Space]

        public float[] pickups;
        public Enums.Instruments instrument = Enums.Instruments.None;

        [Space]

        public Enums.DiscLength discLength = Enums.DiscLength.BARS_32;
    }

    [CreateAssetMenu(fileName = "Song", menuName = "Hyuzu/New Song", order = 1)]
    public class Song : ScriptableObject
    {
        public string creator;

        [Space]

        [Header("Song Info")]

        public string songName;
        public string artist;

        [Space]

        public Enums.Genres genre;
        public string subgenre;

        [Space]

        public int year;

        [Space]

        public int BPM;

        [Space]

        public Enums.Keys key;
        public Enums.Modes mode;

        public List<int> transposes;

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

        [Space]

        public bool fromPak = false;
        public string jsonPath = "";

        [Space]

        int lol;

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
                if (item.preset == Enums.KeymapPreset.Shared) {
                    bytes.Add(songCell.clipsRaw[item.index].data);
                }
            }
            return bytes;
        }

        public Song() {
            if (transposes == null) {
                transposes = new List<int>();
                TransposeKeys();
            }
        }

        public void TransposeKeys() {
            transposes.Clear();

            Func<int, int> FindIdx = key => {
                for (int i = 0; i < 11; i++)
                {
                    if (i == key) {
                        return i;
                    }
                }
                return -1;
            };

            int songKey = FindIdx((int)key);

            for (int i = 0; i < 12; i++)
            {
                int index = FindIdx(i);
                int offset = index - songKey;

                if(offset < -6) offset += 12;
                else if (offset > 6) offset -= 6;

                transposes.Add(offset);
            }
        }
    }
}