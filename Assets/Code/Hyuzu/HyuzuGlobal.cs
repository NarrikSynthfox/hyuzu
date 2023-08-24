using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyuzu {
    public class HyuzuGlobal : MonoBehaviour {
        public const string version = "v0.0.1-alpha";

        public string pathToSongs;
        public string pathToPaks;

        public void Start() {
            DontDestroyOnLoad(this);
        }
    }
}
