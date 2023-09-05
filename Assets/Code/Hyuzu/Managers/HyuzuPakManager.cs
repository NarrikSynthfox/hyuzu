using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UETools.Pak;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace Hyuzu {
    public class HyuzuPakManager : MonoBehaviour
    {
        public AudioSource source;

        public int offset;
        public int offsetUexp;

        byte[] inData;
        byte[] inDataUexp;

        object[][] imports;
        Dictionary<int, Tuple<int, byte[], int>> names;
        
        byte[][] exports;

        //main data
        Dictionary<string, object> songMetadata = new Dictionary<string, object>();

        Dictionary<string, object> beatMetadata = new Dictionary<string, object>();
        Dictionary<string, object> bassMetadata = new Dictionary<string, object>();
        Dictionary<string, object> loopMetadata = new Dictionary<string, object>();
        Dictionary<string, object> leadMetadata = new Dictionary<string, object>();

        public void Start () {
            //OpenFile(Application.streamingAssetsPath + "/test.pak");
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

        T structRead<T>(byte[] data, int offset_, int length)
        {
            byte[] buffer = new byte[length];
            Array.Copy(data, offset_, buffer, 0, length);
            offset_ += length;

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
            inData = null;
            inDataUexp = null;
            
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

                SetupFuserFile(reader, "Fuser/Content/DLC/Songs/" + shortName + "/Meta_" + shortName, false, songMetadata);

                SetupFuserFile(reader, "Fuser/Content/Audio/Songs/" + shortName + "/" + shortName + "bs/Meta_" + shortName + "bs", true, bassMetadata);
                SetupFuserFile(reader, "Fuser/Content/Audio/Songs/" + shortName + "/" + shortName + "bt/Meta_" + shortName + "bt", true, beatMetadata);
                SetupFuserFile(reader, "Fuser/Content/Audio/Songs/" + shortName + "/" + shortName + "ld/Meta_" + shortName + "ld", true, leadMetadata);
                SetupFuserFile(reader, "Fuser/Content/Audio/Songs/" + shortName + "/" + shortName + "lp/Meta_" + shortName + "lp", true, loopMetadata);

                song.songName = bassMetadata["Title"].ToString();
                song.artist = (string)songMetadata["Artist"];

                song.year = (int)songMetadata["Year"];
                song.BPM = (int)bassMetadata["BPM"];

                song.creator = "Unknown";

                var keyString = bassMetadata["Key"].ToString().Replace("EKey::", String.Empty);
                song.key = (HyuzuEnums.Keys)Enum.Parse(typeof(HyuzuEnums.Keys), keyString);

                var modeString = bassMetadata["Mode"].ToString().Replace("EKeyMode::", String.Empty);
                song.mode = (HyuzuEnums.Modes)Enum.Parse(typeof(HyuzuEnums.Modes), modeString);

                SetupInstrumentIcons(song);

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

                GetAudio(ref song.beat, reader, "bt");
                GetAudio(ref song.bass, reader, "bs");
                GetAudio(ref song.loop, reader, "lp");
                GetAudio(ref song.lead, reader, "ld");
                //Debug.Log("MOGG Length: " + clips[0].Length);
            }

            song.fromPak = true;
            return song;
        }

        public void GetAudio(ref ClipInfo info, PakFile reader, string instrument) {
            List<Hyuzu.HyuzuMogg> clipsLoop = GetMoggsFromInstrument(reader, info, instrument);

            info.keyzonesClips = new Keyzone[clipsLoop.Count];
            info.clipsRaw = clipsLoop.ToArray();

            bool noAudio = false;

            for (int i = 0; i < clipsLoop.Count; i++)
            {
                if (clipsLoop[i].samples == 0) { 
                    noAudio = true;
                    break; 
                }
            }

            if (!noAudio) {
                for (int j = 0; j < info.metadata.nodes.GetNode("keymap").children.Count; j++)
                {
                    if (info.metadata.nodes.GetNode("keymap").children[j]?.key == "keyzone") {
                        HyuzuFusion.FusionNode? editAdvanced = info.metadata.nodes.GetChild("edit_advanced");
                        HyuzuFusion.FusionNode? preset = ((HyuzuFusion.FusionNodes)info.metadata.nodes.GetNode("keymap").children[j]?.value).GetChild("keymap_preset");

                        string samplePath = (string)((HyuzuFusion.FusionNodes)info.metadata.nodes.GetNode("keymap").children[j]?.value).GetChild("sample_path")?.value;
                        int samplePathIndex = int.Parse(samplePath.Substring("C:/".Length + ((string)songMetadata["SongShortName"]).Length + 3).Replace(".mogg", String.Empty));

                        float root_note = (float)((HyuzuFusion.FusionNodes)info.metadata.nodes.GetNode("keymap").children[j]?.value).GetChild("root_note")?.value;
                        float unpitched = (float)((HyuzuFusion.FusionNodes)info.metadata.nodes.GetNode("keymap").children[j]?.value).GetChild("unpitched")?.value;
                        
                        float min_note = (float)((HyuzuFusion.FusionNodes)info.metadata.nodes.GetNode("keymap").children[j]?.value).GetChild("min_note")?.value;
                        float max_note = (float)((HyuzuFusion.FusionNodes)info.metadata.nodes.GetNode("keymap").children[j]?.value).GetChild("max_note")?.value;

                        if (editAdvanced != null || preset != null) {
                            info.keyzonesClips = new Keyzone[info.metadata.nodes.GetNode("keymap").children.Count];

                            float presetValue = (float)preset?.value;
                            string presetName = (string)((HyuzuFusion.FusionNodes)info.metadata.nodes.GetNode("keymap").children[j]?.value).GetChild("zone_label")?.value;

                            info.keyzonesClips[samplePathIndex] = new Keyzone(){
                                label = presetName,
                                index = samplePathIndex,
                                preset = (HyuzuEnums.KeymapPreset)presetValue,
                                unpitched = unpitched == 1 ? true : false
                            };
                        } else {
                            if (instrument == "bt" || info.clipsRaw.Length == 1) {
                                info.keyzonesClips[0] = new Keyzone() 
                                {
                                    label = "Shared",
                                    index = 0,
                                    preset = HyuzuEnums.KeymapPreset.Shared,
                                    unpitched = true
                                };
                            } else{
                                info.keyzonesClips[0] = new Keyzone() 
                                {
                                    label = "Major",
                                    index = 0,
                                    preset = HyuzuEnums.KeymapPreset.Major,
                                    unpitched = false
                                };
                                info.keyzonesClips[1] = new Keyzone() 
                                {
                                    label = "Minor",
                                    index = 1,
                                    preset = HyuzuEnums.KeymapPreset.Minor,
                                    unpitched = false
                                };
                            }
                        }
                    }
                }
            }
        }

        List<Hyuzu.HyuzuMogg> GetMoggsFromInstrument(PakFile reader, ClipInfo info, string instrument) {
            // get encrypted moggs
            byte[] fusionData = reader.AbsoluteIndex[("Fuser/Content/Audio/Songs/" + songMetadata["SongShortName"] + "/" + songMetadata["SongShortName"] 
                                                    + instrument + "/patches/" + songMetadata["SongShortName"] + instrument + "_fusion.uexp")].ReadBytes().ToArray();
            List<Hyuzu.HyuzuMogg> clips = new List<Hyuzu.HyuzuMogg>();
            int offsetF = 0;

            MemoryStream stream = new MemoryStream(fusionData);
            BinaryReader readerM = new BinaryReader(stream);

            var val1 = readerM.ReadUInt64();
            var val2 = readerM.ReadUInt64();

            var propName = readerM.ReadUInt32();

            var hash = Encoding.UTF8.GetString(readerM.ReadBytes(8)).Replace("\0", String.Empty);

            var nameLen = readerM.ReadUInt32();
            var name = Encoding.UTF8.GetString(readerM.ReadBytes((int)nameLen)).Replace("\0", String.Empty);

            var unk1 = readerM.ReadUInt32();
            var unk2 = readerM.ReadUInt32();

            stream.Position += 8;

            Hyuzu.HyuzuMogg hMogg = new Hyuzu.HyuzuMogg();
            bool gotMetadata = false;

            while (true) {
                var unk0 = readerM.ReadUInt32();
                if (unk0 == 2653586369) break;

                var fNameLen = readerM.ReadUInt32();
                var fName = Encoding.UTF8.GetString(readerM.ReadBytes((int)fNameLen)).Replace("\0", String.Empty);

                stream.Position += 4;

                var fTypeLen = readerM.ReadUInt32();
                var fType = Encoding.UTF8.GetString(readerM.ReadBytes((int)fTypeLen)).Replace("\0", String.Empty);

                var totalSize = readerM.ReadUInt64();

                if (fType == "FusionPatchResource") { // Assuming Python's [2:-1] slice is equivalent
                    if (!gotMetadata) {
                        info.metadata.nodes = info.metadata.ParseFusionAsset(readerM.ReadBytes((int)totalSize));
                        gotMetadata = true;
                    }
                } else if (fType == "MoggSampleResource") {
                    var ident = readerM.ReadUInt32();
                    hMogg.unk1_ = readerM.ReadUInt32();
                    hMogg.srate = readerM.ReadUInt32();
                    hMogg.chan = readerM.ReadUInt32();
                    hMogg.samples = readerM.ReadUInt32();
                    hMogg.unk2_ = readerM.ReadUInt32();
                    hMogg.chan2 = readerM.ReadUInt32();
                    hMogg.size = readerM.ReadUInt32();

                    hMogg.data = readerM.ReadBytes((int)hMogg.size);
                    Debug.Log("MoggSampleResource: " + instrument + ", " + (string)songMetadata["SongShortName"] + ", " + hMogg.size);
                    
                    if(hMogg.data[0] != 10){
                        #if HYUZU_PAK_AUDIO
                        Hyuzu.HyuzuMoggManager.nativeDecrypt(hMogg.data);
                        #endif
                    }

                    clips.Add(hMogg);
                    
                } else {
                    Debug.Log("Name: " + fName + ", Type: " + fType);
                    offset += (int)totalSize;
                }
            }

            return clips;
        }

        void SetupInstrumentIcons(HyuzuSong song) {
            var bassIcon = bassMetadata["Instrument"].ToString().Replace("EInstrument::", String.Empty);
            song.bass.instrument = (HyuzuEnums.Instruments)Enum.Parse(typeof(HyuzuEnums.Instruments), bassIcon);

            var beatIcon = beatMetadata["Instrument"].ToString().Replace("EInstrument::", String.Empty);
            song.beat.instrument = (HyuzuEnums.Instruments)Enum.Parse(typeof(HyuzuEnums.Instruments), beatIcon);

            var loopIcon = loopMetadata["Instrument"].ToString().Replace("EInstrument::", String.Empty);
            song.loop.instrument = (HyuzuEnums.Instruments)Enum.Parse(typeof(HyuzuEnums.Instruments), loopIcon);

            var leadIcon = leadMetadata["Instrument"].ToString().Replace("EInstrument::", String.Empty);
            song.lead.instrument = (HyuzuEnums.Instruments)Enum.Parse(typeof(HyuzuEnums.Instruments), leadIcon);
        }

        #region .UASSET and .UEXP stuff

        void SetupFuserFile(PakFile reader, string path, bool checkAfterNone, Dictionary<string, object> metadata) {
            inData = null;
            inDataUexp = null;

            inData = reader.AbsoluteIndex[(path + ".uasset")].ReadBytes().ToArray();
            SetupUASset(inData);

            inDataUexp = reader.AbsoluteIndex[(path + ".uexp")].ReadBytes().ToArray();
            SetupUExp(inDataUexp, names, imports, exports, checkAfterNone, metadata);

            offset = 0;
            offsetUexp = 0;

            inData = null;
            inDataUexp = null;
        }

        void SetupUASset(byte[] inData) {
            byte[] usig = structRead<byte[]>(4);
            uint ver = structRead<uint>(4) ^ 0xffffffff;

            offset += 16;

            uint fDirOffset = structRead<uint>(4);
            uint unk1 = structRead<uint>(4);
            uint pn = structRead<uint>(4);

            offset += 4;

            byte unk2 = structRead<byte>(1);
            uint num_names = structRead<uint>(4);
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

                //Debug.Log(Encoding.UTF8.GetString(strdata).Replace("\0", String.Empty));

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

        void SetupUExp(byte[] inData, Dictionary<int, Tuple<int, byte[], int>> names, object[][] imports, byte[][] exports, bool checkAfterNone, Dictionary<string, object> metadata) {
            bool check = checkAfterNone;
            while (offsetUexp <= inDataUexp.Length) {
                byte[] nameIdBytes = names[(int)structReadUexp<sbyte>(8)].Item2;
                string nameId = Encoding.UTF8.GetString(nameIdBytes).Replace("\0", String.Empty);

                if(nameId == "None") {
                    if(offsetUexp + 8 >= inData.Length) {
                        break;
                    } else if (check) {
                        nameIdBytes = names[(int)structReadUexp<sbyte>(8)].Item2;
                        nameId = Encoding.UTF8.GetString(nameIdBytes).Replace("\0", String.Empty);
                        check = false;
                    }
                }
                //Debug.Log(nameId);

                byte[] classIdBytes = names[(int)structReadUexp<sbyte>(8)].Item2;
                string classIdString = Encoding.UTF8.GetString(classIdBytes).Replace("\0", String.Empty);

                ulong lenData = structReadUexp<ulong>(8);

                //Debug.Log(classIdString);

                if (classIdString == "NameProperty") {
                    offsetUexp += 1;

                    byte[] nameBytes_ = names[(int)structReadUexp<uint>(4)].Item2;
                    string name_ = Encoding.UTF8.GetString(nameBytes_).Replace("\0", String.Empty);;
                    uint nameUnk = structReadUexp<uint>(4);

                    if(!metadata.ContainsKey(nameId)) metadata.Add(nameId, name_);
                } else if (classIdString == "SoftObjectProperty") {
                    offsetUexp += 1;

                    byte[] nameBytes_ = names[(int)structReadUexp<uint>(4)].Item2;
                    string name_ = Encoding.UTF8.GetString(nameBytes_).Replace("\0", String.Empty);;
                    ulong value = structReadUexp<ulong>(8);

                    if(!metadata.ContainsKey(nameId)) metadata.Add(nameId, name_);
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

                                string u16Str = Encoding.UTF8.GetString(strData).Replace("\0", String.Empty);;
                                strings.Add(u16Str);
                                
                                if(!metadata.ContainsKey(nameId)) metadata.Add(nameId, u16Str);
                            } else {
                                byte[] strData = new byte[strLen];
                                Array.Copy(inDataUexp, offsetUexp, strData, 0, strLen);

                                string u16Str = Encoding.UTF8.GetString(strData).Replace("\0", String.Empty);;
                                strings.Add(u16Str);

                                if(!metadata.ContainsKey(nameId)) metadata.Add(nameId, u16Str);
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
                    } else if(aClassString == "FloatProperty") {
                        for (int i = 0; i < numValues; i++)
                        {
                            float value = structReadUexp<float>(4);
                            values.Add(value);
                        }
                    } else if(aClassString == "SoftObjectProperty") {
                        for (int i = 0; i < numValues; i++)
                        {
                            offsetUexp += 1;
                            string name_ = Encoding.UTF8.GetString(names[(int)structReadUexp<uint>(4)].Item2).Replace("\0", String.Empty);
                            float value = structReadUexp<uint>(8);
                            values.Add(value);
                        }
                    }

                    if(!metadata.ContainsKey(nameId)) metadata.Add(nameId, values);
                } else if (classIdString == "EnumProperty") {
                    string enumType = Encoding.UTF8.GetString(names[structReadUexp<int>(8)].Item2).Replace("\0", String.Empty);
                    offsetUexp += 1;
                    string enumValue = Encoding.UTF8.GetString(names[structReadUexp<int>(8)].Item2).Replace("\0", String.Empty);;
                    if(!metadata.ContainsKey(nameId)) metadata.Add(nameId, enumValue);
                } else if (classIdString == "IntProperty") {
                    offsetUexp += 1;
                    int value = structReadUexp<int>(4);
                    if(!metadata.ContainsKey(nameId)) metadata.Add(nameId, value);
                } else if (classIdString == "ObjectProperty") {
                    offsetUexp += 1;
                    long value = structReadUexp<int>(4) ^ 0xffffffff;
                    if(!metadata.ContainsKey(nameId)) metadata.Add(nameId, value);
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
                            //Debug.Log("Key: " + key + ", value: " + value);
                            if(!metadata.ContainsKey("Transposes " + key)) metadata.Add("Transposes " + key, value);
                        }
                    }
                } else if (classIdString == "mReferencedChlidAssets") {
                    var refType = Encoding.UTF8.GetString(names[structReadUexp<int>(8)].Item2).Replace("\0", String.Empty);
                    Debug.Log(refType);

                    if (refType == "HmxMidiFileAsset") {
                        structReadUexp<int>(8);
                        offsetUexp += 1;
                        structReadUexp<int>(4);
                        structReadUexp<int>(4);
                        structReadUexp<int>(4);
                        structReadUexp<int>(4);
                        structReadUexp<int>(4);
                        structReadUexp<int>(4);
                        structReadUexp<int>(4);
                        structReadUexp<int>(4);
                    }
                } 
            }
        }

        #endregion
    }
}