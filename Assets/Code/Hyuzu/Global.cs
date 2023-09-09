using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hyuzu {
    public class Global : MonoBehaviour {
        public static string version = "v0.0.1 pre-alpha";

        public static string pathToSongs;
        public static string pathToPaks;

        public static string[] randomTexts = {
            "ma FUCKING tilda??",
            "this game was taken down by harmonix, inc. via a dmca takedown /j",
            "rest in peace, steve harwell <3",
            "BOOOOOING",
            "let this man cook",
            "remember, do NOT vape the rb fog juice"
        };

        public void Start() {
            int seed = PlayerPrefs.GetInt("HYUZU:RAND_SEED", 0);
            Random.InitState(seed);
            
            seed = Random.Range(sizeof(int) * -1, sizeof(int));
            PlayerPrefs.SetInt("HYUZU:RAND_SEED", seed);
            
            Debug.Log("[Hyuzu] Random Seed: " + seed);
            DontDestroyOnLoad(this);
        }

        public static string GetRandomText() {
            return randomTexts[Random.Range(0, randomTexts.Length)];
        }
    }
}
