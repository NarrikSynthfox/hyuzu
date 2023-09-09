using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyuzu {
    public class AssetManager {
        public static Sprite GetInstrumentIcon(Enums.Instruments instrument) {
            return Resources.Load<Sprite>("InstrumentIcons/" + instrument.ToString().ToLower());
        }
        public static Sprite GetMissingAlbumArtIcon() {
            return Resources.Load<Sprite>("AlbumArtUnknown");
        }
    }
}