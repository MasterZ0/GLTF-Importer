using System.Collections.Generic;
using System;

namespace CharacterXYZ.GLTF
{
    [Serializable]
    public class CharacterData
    {
        public int status;
        public string message;
        public CharacterInfo response;
    }

    [Serializable]
    public class CharacterInfo
    {
        public string uid;
        public string slug;
        public string name;
        public MeshData mesh;
        public Dictionary<string, string> animations;
    }

    [Serializable]
    public class MeshData
    {
        public string glb;
    }
}