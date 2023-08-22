using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyuzu {
    public class HyuzuPakImage {
        public struct Mip {
            public UInt64 entry_identifier;
            public UInt32 flags;
            public UInt32 len_1;
            public UInt32 len_2;
            public UInt64 offset;
            public byte[] mipData;
            public UInt32 width;
            public UInt32 height;
        }

        public class Texture2DFuser {
            public byte[] header;
            public byte[] footer;
            public Mip mip = new Mip();
            public UInt64 badFileSwitch1;
            public UInt64 badFileSwitch2;
            public UInt32 headerSize = 355;
            public UInt32 footerSize = 0x10;
            public byte mip_count = 10;

            public void Serialize(byte[] data) {
                MemoryStream stream = new MemoryStream(data);
                BinaryReader readerM = new BinaryReader(stream);

                badFileSwitch1 = BitConverter.ToUInt64(readerM.ReadBytes(8));
                badFileSwitch2 = BitConverter.ToUInt64(readerM.ReadBytes(8));

                if (badFileSwitch2 == 5) {
                    headerSize = 329;
                    mip_count = 9;
                }

                header = readerM.ReadBytes((int)headerSize);

                mip.entry_identifier = BitConverter.ToUInt64(readerM.ReadBytes(8));
                mip.flags = BitConverter.ToUInt32(readerM.ReadBytes(4));

                mip.len_1 = BitConverter.ToUInt32(readerM.ReadBytes(4));
                mip.len_2 = BitConverter.ToUInt32(readerM.ReadBytes(4));

                mip.offset = BitConverter.ToUInt64(readerM.ReadBytes(8));

                mip.mipData = readerM.ReadBytes((int)mip.len_1);
                mip.width = BitConverter.ToUInt32(readerM.ReadBytes(4));
                mip.height = BitConverter.ToUInt32(readerM.ReadBytes(4));

                footer = readerM.ReadBytes((int)footerSize);
            }
        }
    }
}