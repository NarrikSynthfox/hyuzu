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

namespace Hyuzu {
    public class HyuzuPakManager : MonoBehaviour
    {
        public Image image;

            private byte[] GetBytes<T>(T value)
            {
                int size = Marshal.SizeOf(value);
                byte[] bytes = new byte[size];
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
                handle.Free();
                return bytes;
            }

        public struct MainContext {
            public struct CurrentPak {
                public HyuzuPakFile file;
                public HyuzuFuserAsset.AssetRoot assetRoot;
            }
            public CurrentPak pak;
        }
        public MainContext context;

        public void Start () {
            OpenFile(Application.streamingAssetsPath + "/test.pak");
        }

        public void OpenFile(string path) {
            Debug.Log("[Fyuzu] Opening .PAK file: " + path);

            LoadFile(path);
        }

        public void LoadFile(string path) {
           HyuzuPakImage.Texture2DFuser tex = new HyuzuPakImage.Texture2DFuser();

           using (var reader = PakFile.Open(new FileInfo(path)))
            {
                foreach (var (name, entry) in reader.AbsoluteIndex)
                {
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

                        Debug.Log("[Fyuzu] lol: " + context.pak.assetRoot.shortName);
                    }

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

                                    image.sprite = Sprite.Create(tex2d, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f));
                                    image.rectTransform.localScale = new Vector2(1, -1);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}