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
using Unity.Mathematics;

namespace Hyuzu {
    public class HyuzuPakManager : MonoBehaviour
    {
        int offset;
        int offsetUexp;

        byte[] inData;
        byte[] inDataUexp;

        object[][] imports;
        Dictionary<int, Tuple<int, byte[], int>> names;
        
        byte[][] exports;

        //main data
        Dictionary<string, object> songMetadata = new Dictionary<string, object>();

        public void Start () {
            //OpenFile(Application.streamingAssetsPath + "/test.pak");
        }

        public void OpenFile(string path) {
            Debug.Log("[Hyuzu] Opening .PAK file: " + path);

            TurnPAKFileIntoSong(path);
        }

        T structRead<T>(int length)
        {
            byte[] buffer = new byte[length];
            Array.Copy(inData, offset, buffer, 0, length);
            offset += length;

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T result = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();

            return result;
        }

        T structReadUexp<T>(int length)
        {
            byte[] buffer = new byte[length];
            Array.Copy(inDataUexp, offsetUexp, buffer, 0, length);
            offsetUexp += length;

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T result = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();

            return result;
        }

        public HyuzuSong TurnPAKFileIntoSong(string path) {
            HyuzuPakImage.Texture2DFuser tex = new HyuzuPakImage.Texture2DFuser();
            HyuzuSong song = new HyuzuSong();

            FileInfo info = new FileInfo(path);
            string shortName = "";

            using (var reader = PakFile.Open(new FileInfo(path)))
            {
                foreach (var (name, entry) in reader.AbsoluteIndex)
                {
                    // get short name
                    int pos = name.IndexOf("DLC/Songs/");
                    if (pos != -1) {
                        shortName = "";
                        for (int i = pos + 10; i < name.Length; ++i) {
                            if(name[i] == '/') {
                                break;
                            }

                            shortName += name[i];
                        }
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
                                }
                            }
                        }
                    }
                }

                inData = reader.AbsoluteIndex[("Fuser/Content/DLC/Songs/" + shortName + "/Meta_" + shortName + ".uasset")].ReadBytes().ToArray();
                SetupUASset(inData);

                inDataUexp = reader.AbsoluteIndex[("Fuser/Content/DLC/Songs/" + shortName + "/Meta_" + shortName + ".uexp")].ReadBytes().ToArray();
                SetupUExp(inDataUexp, names, imports, exports);

                offset = 0;
                offsetUexp = 0;

                inData = reader.AbsoluteIndex[("Fuser/Content/Audio/Songs/" + shortName + "/" + shortName + "bs/Meta_" + shortName + "bs.uasset")].ReadBytes().ToArray();
                SetupUASset(inData);

                inDataUexp = reader.AbsoluteIndex[("Fuser/Content/Audio/Songs/" + shortName + "/" + shortName + "bs/Meta_" + shortName + "bs.uexp")].ReadBytes().ToArray();
                SetupUExp(inDataUexp, names, imports, exports);


                song.songName = (string)songMetadata["Title"];
                song.artist = (string)songMetadata["Artist"];
                song.year = (int)songMetadata["Year"];
                
                var genreString = songMetadata["Genre"].ToString().Replace("EGenre::", String.Empty);

                switch (genreString) {
                    case "Classical":
                        song.genre = HyuzuEnums.Genres.Classical;
                    break;

                    case "Country":
                        song.genre = HyuzuEnums.Genres.Country;
                    break;

                    case "Rock":
                        song.genre = HyuzuEnums.Genres.Rock;
                    break;

                    case "RnB":
                        song.genre = HyuzuEnums.Genres.RnB;
                    break;

                    case "HipHop":
                        song.genre = HyuzuEnums.Genres.HipHop;
                    break;

                    case "LatinAndCarribean":
                        song.genre = HyuzuEnums.Genres.International;
                    break;

                    case "Dance":
                        song.genre = HyuzuEnums.Genres.Dance;
                    break;

                    case "Pop":
                        song.genre = HyuzuEnums.Genres.Pop;
                    break;

                    default:
                        song.genre = HyuzuEnums.Genres.Misc;
                    break;
                }
            }

            return song;
        }

        public void SetupUASset (byte[] inData) {
            byte[] usig = structRead<byte[]>(4);
            uint ver = structRead<uint>(4) ^ 0xffffffff;

            offset += 16;

            uint fDirOffset = structRead<uint>(4);
            uint unk1 = structRead<uint>(4);
            uint pn = structRead<uint>(4);

            offset += 4;

            byte unk2 = structRead<byte>(1);
            uint num_names = structRead<uint>(4);
            Debug.Log(num_names);
            uint off_names = structRead<uint>(4);

            offset += 8;

            uint num_exp = structRead<uint>(4);
            uint off_exp = structRead<uint>(4);
            uint num_imp = structRead<uint>(4);
            uint off_imp = structRead<uint>(4);

            offset += 20;

            offset += 16;
            
            uint unk3 = structRead<uint>(4);
            uint unk4 = structRead<uint>(4);
            uint unk5 = structRead<uint>(4);

            offset += 36;

            uint unk6 = structRead<uint>(4);

            offset += 4;

            uint padding_offset = structRead<uint>(4);
            uint fLen = structRead<uint>(4);

            offset += 12;

            uint unk7 = structRead<uint>(4);
            uint fDataOffset = structRead<uint>(4);

            names = new Dictionary<int, Tuple<int, byte[], int>>();
            for (int name = 0; name < num_names; name++)
            {
                uint strlen = structRead<uint>(4);
                byte[] strdata = new byte[strlen];
                Array.Copy(inData, offset, strdata, 0, strlen);
                offset += (int)strlen;
                uint flags = structRead<uint>(4);

                names[name] = Tuple.Create((int)strlen, strdata, (int)flags);
            }

            imports = new object[num_imp][];
            for (int imp = 0; imp < num_imp; imp++)
            {
                ulong parentNameId = structRead<ulong>(8);
                byte[] parentNameBytes = names[(int)parentNameId].Item2;
                int parentNameLength = Array.IndexOf(parentNameBytes, (byte)0);
                byte[] parentNameArrayWithoutNull = new byte[parentNameLength];
                Array.Copy(parentNameBytes, parentNameArrayWithoutNull, parentNameLength);
                string parentName = Encoding.UTF8.GetString(parentNameArrayWithoutNull);

                ulong classId = structRead<ulong>(8);
                byte[] classIdBytes = names[(int)classId].Item2;
                int classIdLength = Array.IndexOf(classIdBytes, (byte)0);
                byte[] classIdArrayWithoutNull = new byte[classIdLength];
                Array.Copy(classIdBytes, classIdArrayWithoutNull, classIdLength);
                string classIdString = Encoding.UTF8.GetString(classIdArrayWithoutNull);

                uint parentImportId = structRead<uint>(4) ^ 0xFFFFFFFF;

                uint nameId = structRead<uint>(4);
                byte[] nameBytes = names[(int)nameId].Item2;
                int nameLength = Array.IndexOf(nameBytes, (byte)0);
                byte[] nameArrayWithoutNull = new byte[nameLength];
                Array.Copy(nameBytes, nameArrayWithoutNull, nameLength);
                string name = Encoding.UTF8.GetString(nameArrayWithoutNull);

                uint unkid = structRead<uint>(4);

                imports[imp] = new object[] { parentName, classIdString, parentImportId, name, unkid };
            }


            exports = new byte[num_exp][];
            for (int exp = 0; exp < num_exp; exp++)
            {
                exports[exp] = new byte[100];
                Array.Copy(inData, offset, exports[exp], 0, 100);
                offset += 100;
            }
            offset += 4;
        }

        public void SetupUExp(byte[] inData, Dictionary<int, Tuple<int, byte[], int>> names, object[][] imports, byte[][] exports) {
            while (offsetUexp <= inDataUexp.Length) {
                byte[] nameIdBytes = names[(int)structReadUexp<sbyte>(8)].Item2;
                string nameId = Encoding.UTF8.GetString(nameIdBytes).Replace("\0", String.Empty);

                Debug.Log(nameId);

                if(nameId == "None")
                    break;

                byte[] classIdBytes = names[(int)structReadUexp<sbyte>(8)].Item2;
                string classIdString = Encoding.UTF8.GetString(classIdBytes).Replace("\0", String.Empty);

                ulong lenData = structReadUexp<ulong>(8);

                Debug.Log(classIdString);

                if (classIdString == "NameProperty") {
                    offsetUexp += 1;

                    byte[] nameBytes_ = names[(int)structReadUexp<uint>(4)].Item2;
                    string name_ = Encoding.UTF8.GetString(nameBytes_);
                    uint nameUnk = structReadUexp<uint>(4);

                    songMetadata.Add(nameId, name_);
                } else if (classIdString == "SoftObjectProperty") {
                    offsetUexp += 1;

                    byte[] nameBytes_ = names[(int)structReadUexp<uint>(4)].Item2;
                    string name_ = Encoding.UTF8.GetString(nameBytes_);
                    ulong value = structReadUexp<ulong>(8);

                    songMetadata.Add(nameId, name_);
                } else if (classIdString == "TextProperty") {
                    offsetUexp += 1;

                    var flag = structReadUexp<uint>(4);
                    sbyte historyType = structReadUexp<sbyte>(1);

                    List<string> strings = new List<string>();

                    if (historyType == -1) {
                        var numStr = structReadUexp<sbyte>(4);

                        for (int i = 0; i < numStr; i++)
                        {
                            int strLen = structReadUexp<sbyte>(4);
                            if (strLen < 0) {
                                strLen = strLen * -2;

                                byte[] strData = new byte[strLen];
                                Array.Copy(inDataUexp, offsetUexp, strData, 0, strLen);

                                string u16Str = Encoding.UTF8.GetString(strData);
                                strings.Add(u16Str);
                                songMetadata.Add(nameId, u16Str);
                            } else {
                                byte[] strData = new byte[strLen];
                                Array.Copy(inDataUexp, offsetUexp, strData, 0, strLen);

                                string u16Str = Encoding.UTF8.GetString(strData);
                                strings.Add(u16Str);
                                songMetadata.Add(nameId, u16Str);
                            }
                            offsetUexp += (int)strLen;
                        }
                    }
                } else if (classIdString == "ArrayProperty") {
                    byte[] aClass = names[(int)structReadUexp<sbyte>(8)].Item2;
                    string aClassString = Encoding.UTF8.GetString(aClass).Replace("\0", String.Empty);;

                    offsetUexp += 1;
                    var numValues = structReadUexp<uint>(4);

                    List<object> values = new List<object>();

                    if(aClassString == "ObjectProperty") {
                        for (int i = 0; i < numValues; i++)
                        {
                            uint valueIndex = structReadUexp<uint>(4) ^ 0xFFFFFFFF;
                            object[] value = imports[(int)valueIndex];
                            values.Add(value);
                        }
                    }
                } else if (classIdString == "EnumProperty") {
                    string enumType = Encoding.UTF8.GetString(names[structReadUexp<int>(8)].Item2).Replace("\0", String.Empty);
                    offsetUexp += 1;
                    string enumValue = Encoding.UTF8.GetString(names[structReadUexp<int>(8)].Item2).Replace("\0", String.Empty);;
                    songMetadata.Add(nameId, enumValue);
                } else if (classIdString == "IntProperty") {
                    offsetUexp += 1;
                    int value = structReadUexp<int>(4);
                    songMetadata.Add(nameId, value);
                } else if (classIdString == "StructProperty") {
                    var curOffset = offsetUexp;

                    string structClass = Encoding.UTF8.GetString(names[structReadUexp<int>(8)].Item2).Replace("\0", String.Empty);

                    if (structClass == "Transposes") {
                        offsetUexp += 1;
                        offsetUexp += 16;
                        while (offsetUexp <= curOffset + (int)lenData) {
                            string key = Encoding.UTF8.GetString(names[structReadUexp<int>(8)].Item2).Replace("\0", String.Empty);
                            offsetUexp += 8;
                            int value = structReadUexp<sbyte>(4);
                            offsetUexp += 9;
                            Debug.Log("Key: " + key + ", value: " + value);
                            songMetadata.Add("Transposes " + key, value);
                        }
                    }
                }
            }
        }
    }
}