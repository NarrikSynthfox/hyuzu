using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UETools.Pak;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Text;

namespace Hyuzu {
    public class HyuzuPakManager : MonoBehaviour
    {
        public Image image;

        const int SONG_NAME_OFFSET = 0x70;
        const int ARTIST_NAME_OFFSET = 0x91;

        public struct MainContext {
            public struct CurrentPak {
                public HyuzuPakFile file;
                public HyuzuFuserAsset.AssetRoot assetRoot;
            }
            public CurrentPak pak;
        }
        public MainContext context;

        public void Start () {
            //OpenFile(Application.streamingAssetsPath + "/test.pak");
        }

        public void OpenFile(string path) {
            Debug.Log("[Hyuzu] Opening .PAK file: " + path);

            TurnPAKFileIntoSong(path);
        }

        public HyuzuSong TurnPAKFileIntoSong(string path) {
           HyuzuPakImage.Texture2DFuser tex = new HyuzuPakImage.Texture2DFuser();
           HyuzuSong song = new HyuzuSong();

           using (var reader = PakFile.Open(new FileInfo(path)))
            {
                foreach (var (name, entry) in reader.AbsoluteIndex)
                {
                    // get short name
                    int pos = name.IndexOf("DLC/Songs/");
                    if (pos != -1) {
                        string shortName = "";
                        for (int i = pos + 10; i < name.Length; ++i) {
                            if(name[i] == '/') {
                                break;
                            }

                            shortName += name[i];
                        }

                        if (name == ("Fuser/Content/DLC/Songs/" + shortName + "/Meta_" + shortName + ".uexp")) {
                            context.pak.assetRoot.shortName = shortName;
                        }

                        Debug.Log("[Hyuzu] Short Name: " + context.pak.assetRoot.shortName);
                    }

                    // get album cover
                    pos = name.IndexOf("UI/AlbumArt");
                    if(pos != -1) {
                        if(name.Substring(name.Length - 5) == ".uexp") {
                            pos = name.IndexOf("_small"); {
                                if (pos != -1) {
                                    byte[] data = entry.ReadBytes().ToArray();
                                    tex.Serialize(data);

                                    UnityEngine.Texture2D tex2d = new UnityEngine.Texture2D(512, 512, TextureFormat.DXT1, false);

                                    tex2d.LoadRawTextureData(tex.mip.mipData);
                                    tex2d.Apply();

                                    song.cover = Sprite.Create(tex2d, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f));

                                    image.sprite = Sprite.Create(tex2d, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f));
                                    image.rectTransform.localScale = new Vector2(1, -1);
                                }
                            }
                        }
                    }
                }

                //getting song name
                GetSongName(reader, song);

                // getting artist name
                GetArtistName(reader, song);
            }

            return song;
        }

        public void GetSongName(PakFile reader, HyuzuSong song) {
            string s_shortName = context.pak.assetRoot.shortName;
            string s_shortNameWExp = context.pak.assetRoot.shortName + "bs";

            byte[] sdata = reader.AbsoluteIndex[("Fuser/Content/Audio/Songs/" + s_shortName + "/" + s_shortName + "bs/Meta_" + s_shortNameWExp + ".uexp")].ReadBytes().ToArray();

            MemoryStream stream = new MemoryStream(sdata);
            BinaryReader readerM = new BinaryReader(stream);

            readerM.BaseStream.Position = SONG_NAME_OFFSET;
            string songName = ReadNullTerminatedString(readerM);

            song.songName = songName;

            readerM.Close();
            stream.Close();
        }

        public void GetArtistName(PakFile reader, HyuzuSong song) {
            string s_shortName = context.pak.assetRoot.shortName;

            byte[] sdata = reader.AbsoluteIndex[("Fuser/Content/DLC/Songs/" + s_shortName + "/Meta_" + s_shortName + ".uexp")].ReadBytes().ToArray();

            MemoryStream stream = new MemoryStream(sdata);
            BinaryReader readerM = new BinaryReader(stream);

            readerM.BaseStream.Position = ARTIST_NAME_OFFSET;
            string artistName = ReadNullTerminatedString(readerM);

            song.artist = artistName;

            readerM.Close();
            stream.Close();
        }

        public string ReadNullTerminatedString(BinaryReader stream)
        {
            string str = "";
            char ch;
            while ((int)(ch = stream.ReadChar()) != 0)
                str = str + ch;
            return str;
        }
    }
}