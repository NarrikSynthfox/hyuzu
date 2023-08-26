using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Animations;

public class HyuzuFusion : MonoBehaviour
{
    public struct FusionNode {
        public string key;
        public object value;
    }

    public struct FusionNodes {
        List<FusionNode> nodes;
    }

    public void ParseFusionAsset(byte[] data) {
    }
}
