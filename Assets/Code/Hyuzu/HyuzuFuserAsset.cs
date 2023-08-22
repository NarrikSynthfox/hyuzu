using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyuzu {
    public class HyuzuFuserAsset {
        public struct CelType {
            public enum Type {
                Beat,
                Bass,
                Loop,
                Lead
            }
            Type value;
        }

        public struct KeyMode {
            public enum Value {
                Major,
                Minor,
                Num
		    };
        }

        public struct Genre {
            public enum Value {
                Classical,
                Country,
                Rock,
                LatinAndCaribbean,
                Pop,
                RnB,
                HipHop,
                Dance
            };
        }
        
        public enum MidiType {
            Major,
            Minor
        }

        public struct SongSerializationCtx {
            public static bool loading = true;

            public string shortName, songName, songKey, songGenre;
            public int bpm;

            public CelType curType;
            public MidiType curMidiType;

            public KeyMode.Value curKeyMode;
            public Genre.Value curGenre;

            public int year;
            
            public static bool isTransition = false;
            public static bool smallArt = false;
        }

        public struct AssetRoot {
            public string shortName, artistName, songName, songKey;

            HyuzuEnums.Keys key;
            HyuzuEnums.Genres genre;

            public int year, bpm;

            public void Serialize(SongSerializationCtx ctx) {

            }
        }
    }
}
