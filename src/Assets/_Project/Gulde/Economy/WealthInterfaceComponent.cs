using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

namespace Gulde.Economy
{
    public class WealthInterfaceComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        WealthComponent WealthComponent { get; set; }

        [OdinSerialize]
        [Required]
        TextMeshProUGUI MoneyLabel { get; set; }

        void SetWealth(float value)
        {
            MoneyLabel.text = $"{value} Gulden";
        }
    }
}