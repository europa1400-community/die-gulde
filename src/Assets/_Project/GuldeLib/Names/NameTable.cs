using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Names
{
    [CreateAssetMenu(fileName = "table_names", menuName = "Tables/Names")]
    public class NameTable : SerializedScriptableObject
    {
        [OdinSerialize]
        public List<string> MaleFirstNames { get; set; } = new List<string>();

        [OdinSerialize]
        public List<string> FemaleFirstNames { get; set; } = new List<string>();

        [OdinSerialize]
        public List<string> LastNames { get; set; } = new List<string>();

        [OdinSerialize]
        public List<string> InnNames { get; set; } = new List<string>();
    }
}