using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyuzu {
    public struct Guid {
        public static char[] guid = new char[16];
    }

    public struct SHAHash {
        public static byte[] data = new byte[20];
    }

    public enum PakVersion
    {
        INITIAL = 1,
        NO_TIMESTAMPS = 2,
        COMPRESSION_ENCRYPTION = 3,         // UE4.13+
        INDEX_ENCRYPTION = 4,               // UE4.17+ - encrypts only pak file index data leaving file content as is
        RELATIVE_CHUNK_OFFSETS = 5,         // UE4.20+
        DELETE_RECORDS = 6,                 // UE4.21+ - this constant is not used in UE4 code
        ENCRYPTION_KEY_GUID = 7,            // ... allows to use multiple encryption keys over the single project
        FNAME_BASED_COMPRESSION_METHOD = 8, // UE4.22+ - use string instead of enum for compression method
        FROZEN_INDEX = 9,
        PATH_HASH_INDEX = 10,
        FNV64BUGFIX = 11,
        LAST,
        INVALID,
        LATEST = LAST - 1
    };

    public struct HyuzuPakFile {
        public static UInt32 OFFSET = 221;
        public bool isEncrypted;
        public UInt32 magic;
        public PakVersion version;
        public int indexOffset;
        public int indexSize;
        public SHAHash hash;
        public static bool isFrozen = false;
        public static char[] compressionName = new char[32];
    }
}