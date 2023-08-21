using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyuzu;

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
        public AudioClip[] clips;
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

        public AudioClip GetPreviewClip(ClipInfo songCell) {
            foreach (Keyzone item in songCell.keyzonesClips)
            {
                if ((int)item.preset == (int)mode) {
                    return songCell.clips[item.index];
                }
                else if (item.preset == HyuzuEnums.KeymapPreset.Shared) {
                    return songCell.clips[0];
                }
            }
            return null;
        }
    }
}