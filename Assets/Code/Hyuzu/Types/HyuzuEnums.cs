using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyuzu {
    public class HyuzuEnums
    {
        public enum Keys {
            C = 0,
            Db = 1,
            D = 2,
            Eb = 3,
            E = 4,
            F = 5,
            Gb = 6,
            G = 7,
            Ab = 8,
            A = 9,
            Bb = 10,
            B = 11,
        }

        public enum Modes {
            Major,
            Minor
        }

        public enum Genres {
            Classical,
            Country,
            Rock,
            Pop,
            RnB,
            HipHop,
            Dance,
            International,
            AlternativeIndie,
            Soundtrack, //used for games, movies, etc.
            Misc
        }

        public enum Instruments {
            None,
            Guitar,
            Drums,
            Vocal,
            Synth,
            Sampler,
            Horns,
            Strings
        }

        public enum CelType {
            Beat,
            Bass,
            Loop,
            Lead
        }

        public enum KeymapPreset {
            Major,
            Minor,
            Shared,
            Custom
        }
    }
}