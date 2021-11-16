using System.Collections.Generic;
using System.Linq;
using GuldeLib.Economy;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GuldeEditor.Exchange
{
    public class Exchanger : OdinEditorWindow
    {
        [Button]
        void Refresh()
        {
            if (First) FirstInventory = First.Inventory.Items;
            if (Second) SecondInventory = Second.Inventory.Items;

            FirstAmount = Mathf.Min(FirstAmount, FirstSelectedProductSupply);
            SecondAmount = Mathf.Min(SecondAmount, SecondSelectedProductSupply);
        }

        [MenuItem("Gulde/Exchanger")]
        static void ShowWindow() => GetWindow<Exchanger>();

        [OdinSerialize]
        [HorizontalGroup("Exchange")]
        [BoxGroup("Exchange/First")]
        [HorizontalGroup("Exchange/First/Info")]
        [OnValueChanged("OnFirstChanged")]
        [ValueDropdown("@FindObjectsOfType<ExchangeComponent>()")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        ExchangeComponent First { get; set; }

        bool HasFirstWealthComponent => First && First.Owner;

        [OdinSerialize]
        [TableList]
        [BoxGroup("Exchange/First")]
        [ShowIf("First")]
        [PropertySpace(10)]
        Dictionary<Item, int> FirstInventory { get; set; }

        List<Item> FirstProducts => First ? First.Inventory.Items.Keys.ToList() : null;

        [OdinSerialize]
        [ShowIf("First")]
        [ValueDropdown("FirstProducts")]
        [LabelText("Product")]
        [LabelWidth(60)]
        [HorizontalGroup("Exchange/First/Sell")]
        [PropertySpace(10)]
        Item FirstSelectedItem { get; set; }

        int FirstSelectedProductSupply =>
            FirstSelectedItem && FirstInventory != null ? FirstInventory.ContainsKey(FirstSelectedItem) ?
                FirstInventory[FirstSelectedItem] : 0 : 0;

        [OdinSerialize]
        [HorizontalGroup("Exchange/First/Sell", 150)]
        [HideLabel]
        [PropertyRange(0, "FirstSelectedProductSupply")]
        [ShowIf("First")]
        [PropertySpace(10)]
        int FirstAmount { get; set; }

        [Button]
        [HorizontalGroup("Exchange/First/Sell")]
        [LabelText("Sell")]
        [ShowIf("First")]
        [PropertySpace(10, 10)]
        void Sell()
        {
            Debug.Log("Sell");
            for (var i = 0; i < FirstAmount; i++)
            {
                First.Sell(FirstSelectedItem, Second);
            }

            Refresh();
        }

        [OdinSerialize]
        [HorizontalGroup("Exchange")]
        [BoxGroup("Exchange/Second")]
        [HorizontalGroup("Exchange/Second/Info")]
        [OnValueChanged("OnSecondChanged")]
        [ValueDropdown("@FindObjectsOfType<ExchangeComponent>()")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        ExchangeComponent Second { get; set; }

        bool HasSecondWealthComponent => Second && Second.Owner;

        [ShowInInspector]
        [HorizontalGroup("Exchange/Second/Info")]
        [ShowIf("HasSecondWealthComponent")]
        [LabelText("Money")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        float SecondMoney => HasSecondWealthComponent ? Second.Owner.Money : 0;

        [OdinSerialize]
        [TableList]
        [BoxGroup("Exchange/Second")]
        [ShowIf("Second")]
        [PropertySpace(10)]
        Dictionary<Item, int> SecondInventory { get; set; }

        List<Item> SecondProducts =>
            Second ? Second.Inventory.Items.Keys.ToList() : null;

        [OdinSerialize]
        [ShowIf("Second")]
        [ValueDropdown("SecondProducts")]
        [LabelText("Product")]
        [LabelWidth(60)]
        [HorizontalGroup("Exchange/Second/Buy")]
        [PropertySpace(10)]
        Item SecondSelectedItem { get; set; }

        int SecondSelectedProductSupply =>
            SecondSelectedItem && SecondInventory != null ? SecondInventory.ContainsKey(SecondSelectedItem) ?
                SecondInventory[SecondSelectedItem] : 0 : 0;

        [OdinSerialize]
        [HorizontalGroup("Exchange/Second/Buy", 150)]
        [HideLabel]
        [PropertyRange(0, "SecondSelectedProductSupply")]
        [ShowIf("Second")]
        [PropertySpace(10)]
        int SecondAmount { get; set; }

        [Button]
        [HorizontalGroup("Exchange/Second/Buy")]
        [LabelText("Buy")]
        [ShowIf("Second")]
        [PropertySpace(10, 10)]
        void Buy()
        {
            for (var i = 0; i < SecondAmount; i++)
            {
                First.Purchase(SecondSelectedItem, Second);
            }

            Refresh();
        }

        void OnFirstChanged()
        {
            Refresh();
        }

        void OnSecondChanged()
        {
            Refresh();
        }
    }
}