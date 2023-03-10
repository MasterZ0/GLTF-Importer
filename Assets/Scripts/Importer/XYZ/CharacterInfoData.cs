using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

namespace GLTFImporter.Importer
{
    [CreateAssetMenu(menuName = "ScriptableObjects/XYZ", fileName = "New" + nameof(CharacterInfoData))]
    public class CharacterInfoData : SerializedScriptableObject
    {
        public string mesh;
        [DictionaryDrawerSettings]
        public Dictionary<string, string> animations;
    }
}