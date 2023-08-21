using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UETools.Core.Enums;
using UETools.Pak;
using UETools.Assets;
using System.Security.Permissions;
using UETools.Core;
using System.Drawing;
using System.CodeDom.Compiler;

namespace Hyuzu {
    public class HyuzuPakManager : MonoBehaviour
    {
        public Image image;
        byte[] datas;
        [TextArea(5, 10)]
        public string text;

        public void Start () {
            OpenFile(Application.streamingAssetsPath + "/test.pak");
        }

        public void OpenFile(string path) {
            Debug.Log("[Fyuzu] Opening .PAK file: " + path);
            var fs = new FileInfo(path);
            using (var reader = PakFile.Open(fs)) {

                /*foreach (var (name, entry) in reader.AbsoluteIndex)
                {
                     Debug.Log($"[Fyuzu] PAK file: {name}, {entry.Size}");
                }*/

                UAssetAsset? asset = default;
                var data = reader.AbsoluteIndex["Fuser/Content/DLC/Songs/RockMeAmadeus_Falco_hatoving/Meta_RockMeAmadeus_Falco_hatoving.uexp"].Read();
                string str = "";
                Debug.Log($"[Fyuzu] PAK file: { str.ToString()}");
                
                FileStream stream = new FileStream("C:/pak/t.txt", FileMode.OpenOrCreate);
                stream.Write(datas, 0, datas.Length);
            }
        }
    }

    public class SongPakEntry {

    }

    public class AssetRoot {
        public string shortName;
        public string artistName;
        public string songName;
        public string songKey;

        public HyuzuEnums.Modes mode;
        public HyuzuEnums.Genres genre;

        public int year, bpm;
    }
}
