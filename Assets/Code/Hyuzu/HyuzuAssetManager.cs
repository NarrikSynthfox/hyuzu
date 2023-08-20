using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyuzu {
    public class HyuzuAssetManager {
        public static Sprite GetInstrumentIcon(HyuzuEnums.Instruments instrument) {
            return Resources.Load<Sprite>("InstrumentIcons/" + instrument.ToString().ToLower());
        }
    }
}