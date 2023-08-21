using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UETools.Pak;
using UETools;
using UETools.Assets;
using UETools.Core;
using UETools.Core.Enums;
using UETools.Objects.Classes;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.CodeDom.Compiler;

namespace Hyuzu {
    public class HyuzuPakManager : MonoBehaviour
    {
        public Image image;
        [JsonPropertyName("SongShortName")]
        public string shortName;

        public class DataBuffer {
            public bool loading = true;
            public long pos = 0;
            public int size = 0;
            public byte[] buffer;

            public object ctx_;
            public Action<int> resize;

            public bool watch = false;
            public struct WatchedValue {
                public byte[] data;
                public int size;
                public long buffer_pos;
            }
            public List<WatchedValue> watchedValues = new List<WatchedValue>();
            public List<Action<DataBuffer>> finalizeFunctions = new List<Action<DataBuffer>>();

            public class DerivedBuffer {
                public DataBuffer base_ = null;
                public long offset = 0;
            }
            public DerivedBuffer? derivedBuffer;

            public DataBuffer()
            {
                resize = (size) =>
                {
                    throw new ArgumentOutOfRangeException("Cannot resize the buffer!");
                };
            }

            public void Watch(Action fn)
            {
                watch = true;
                fn.Invoke();
                watch = false;
            }

            public void Finalize_()
            {
                foreach (var w in watchedValues)
                {
                    Array.Copy(w.data, 0, buffer, w.buffer_pos, w.size);
                }

                foreach (var f in finalizeFunctions)
                {
                    f(this);
                }

                watchedValues.Clear();
            }

            public DataBuffer SetupFromHere() {
                DerivedBuffer dB = new DerivedBuffer();
                dB.base_ = this;
                dB.offset = pos;

                DataBuffer newBuffer = new DataBuffer();
                newBuffer.buffer = null;
                newBuffer.pos = 0;
                newBuffer.loading = loading;

                if(loading) {
                    newBuffer.size = size - (int)pos;
                } else newBuffer.size = 0;

                newBuffer.derivedBuffer = dB;

                return newBuffer;
            }

            public void SetupVector(byte[] data) {
                buffer = data;
                size = data.Length;

                resize = (int sz) =>
                {
                    Array.Resize(ref data, sz);
                    buffer = data;
                    size = sz;
                };
            }

            public T ctx<T>() where T : class
            {
                return ctx_ as T;
            }

            public void Serialize(byte[] data, int data_size) {
                if(derivedBuffer != null) {
                    long prevPos = derivedBuffer.base_.pos;
                    bool prevWatch = derivedBuffer.base_.watch;

                    derivedBuffer.base_.pos = pos + derivedBuffer.offset;
                    derivedBuffer.base_.watch = watch;

                    if(data_size < 0) {
                        data_size = data_size * -2;
                    }
                    derivedBuffer.base_.Serialize(data, data_size);

                    derivedBuffer.base_.watch = prevWatch;
                    derivedBuffer.base_.pos = prevPos;

                    pos += data_size;

                    if(!loading && pos > size) {
                        size = (int)pos;
                    }

                    return;
                }

                if (loading) {
                    Array.Copy(buffer, pos, data, 0, data_size);
                } else {
                    if (pos > size) {
                        UInt32 diff = (uint)(pos - size);
                        resize(size + (int)diff);
                        Array.Clear(buffer, (int)(pos - (long)diff), (int)diff);
                    }

                    if(pos + data_size > size) {
                        resize((int)(pos + data_size));
                    }

                    Array.Copy(buffer, pos, data, 0, data_size);

                    if (watch) {
                        WatchedValue v = new WatchedValue();
                        v.buffer_pos = pos;
                        v.data = data;
                        v.size = data_size;
                        watchedValues.Add(v);
                    }

                }

                pos += data_size;
            }
        }

        public void Start () {
            OpenFile(Application.streamingAssetsPath + "/test.pak");
        }

        public void OpenFile(string path) {
            Debug.Log("[Fyuzu] Opening .PAK file: " + path);

            byte[] data = File.ReadAllBytes(path);
            DataBuffer dataBuffer = new DataBuffer();

            dataBuffer.SetupVector(data);
            LoadFile(dataBuffer);
        }

        public void LoadFile(DataBuffer buffer) {
           
        }
    }

    public class SongPakEntry {

    }

    public class AssetRoot {
        [JsonPropertyName("SongShortName")]
        public string shortName;
        public string artistName;
        public string songName;
        public string songKey;

        public HyuzuEnums.Modes mode;
        public HyuzuEnums.Genres genre;

        public int year, bpm;
    }
}
