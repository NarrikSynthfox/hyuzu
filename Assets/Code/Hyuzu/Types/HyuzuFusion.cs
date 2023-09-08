using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class HyuzuFusion
{
    public struct FusionNode {
        public string key;
        public object value;
    }

    public class FusionNodes {
        public List<FusionNode?> children = new List<FusionNode?>();

        public FusionNode? GetChild(string key) {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i]?.key == key) {
                    return children[i];
                }
            }
            return null;
        }

        public int GetInteger(string key) {
            return int.Parse(GetChild(key)?.value.ToString());
        }

        public string GetString(string key) {
            return GetChild(key)?.value.ToString();
        }

        public FusionNodes GetNode(string key) {
            return (FusionNodes)(GetChild(key)?.value);
        }
    }

    public FusionNodes nodes;

    public FusionNodes ParseFusionAsset(byte[] data) {
        int index = 0;
        FusionNodes nodes = new FusionNodes();

        Action<char> Consume = (c) => {
            if (data[index] == c) {
                index++;
            }
        };

        Func<string> GetName = () =>
        {
            string name = "";
    
            while (!char.IsWhiteSpace((char)data[index]))
            {
                name += (char)data[index];
                index++;
            }

            return name;
        };

        Func<string> GetString = () =>
        {
            string name = "";
            Consume.Invoke('"');
            while (data[index] != '"')
            {
                name += (char)data[index];
                index++;
            }
            Consume.Invoke('"');
            
            return name;
        };

        Func<float> GetNumber = () =>
        {
            string number = "";
            float value = 0;

            bool has_dot = false;
            
            while (data[index] != ')')
            {
                number += (char)data[index];

                if(data[index] == '.') {
                    has_dot = true;
                }
                if(char.IsWhiteSpace((char)data[index])) break;

                index++;
            }

            if (has_dot) { 
                value = (float)Convert.ToDouble(number);
            }
            else value = Convert.ToInt32(number);
            
            return value;
        };

        Action SkipWhitespace = () =>
        {
            if ((index + 1) < data.Length) {
                while(char.IsWhiteSpace((char)data[index])) {
                    index++;
                }
            }
        };

        FusionNode ParseNode() {
            SkipWhitespace();
            Consume.Invoke('(');

            FusionNode node;

            node.key = GetName();
            SkipWhitespace();

            if (data[index] == '(') {
                FusionNodes nodes = new FusionNodes();

                while(data[index] == '(') {
                    nodes.children.Add(ParseNode());
                    SkipWhitespace();
                }

                node.value = nodes;
            } else if (data[index] == '"') {
                node.value = GetString();
            } else {
                float num = GetNumber();
                SkipWhitespace();

                if(data[index] != ')') {
                    float num2 = GetNumber();
                    SkipWhitespace();

                    Vector2 vec = Vector2.zero;

                    vec.x = num;
                    vec.y = num2;

                    node.value = vec;
                } else {
                    node.value = num;
                }
            }

            SkipWhitespace();
            Consume(')');

            return node;
        };

        while(index < data.Length - 1) {
            nodes.children.Add(ParseNode());
        }

        return nodes;
    }
}
