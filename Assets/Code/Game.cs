using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyuzu;

public class Game : MonoBehaviour
{
    public float BPM;
    public HyuzuEnums.Keys Key;
    public AudioSource[] clips;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach (AudioSource source in clips)
        {
            float semitoneShift = (((int)source.GetComponent<Clip>().key + 1) - ((int)Key + 1)) * -1;
            float pitchShiftFactor = Mathf.Pow(2f, semitoneShift / 12f);
            float newSpeed = BPM / source.GetComponent<Clip>().BPM;

            source.pitch = newSpeed;

            source.outputAudioMixerGroup.audioMixer.SetFloat("PitchLead", (1f / newSpeed) * pitchShiftFactor);
            source.outputAudioMixerGroup.audioMixer.SetFloat("PitchBass", (1f / newSpeed) * pitchShiftFactor);
            source.outputAudioMixerGroup.audioMixer.SetFloat("PitchLoop", (1f / newSpeed) * pitchShiftFactor);
        }
    }
}
