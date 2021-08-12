using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using Gulde.Inventory;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GuldeEditor.Exchange
{
    public class ExchangerWindow : OdinEditorWindow
    {
        [Button]
        void Refresh()
        {
            if (First) FirstInventory = First.Inventory.Inventory;
            if (Second) SecondInventory = Second.Inventory.Inventory;

            FirstAmount = Mathf.Min(FirstAmount, FirstSelectedProductSupply);
            SecondAmount = Mathf.Min(SecondAmount, SecondSelectedProductSupply);
        }

        [MenuItem("Gulde/Exchanger")]
        static void ShowWindow() => GetWindow<ExchangerWindow>();

        [OdinSerialize]
        [HorizontalGroup("Exchange")]
        [BoxGroup("Exchange/First")]
        [HorizontalGroup("Exchange/First/Info")]
        [OnValueChanged("OnFirstChanged")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        ExchangeComponent First { get; set; }

        bool HasFirstWealthComponent => First && First.Wealth;

        [ShowInInspector]
        [HorizontalGroup("Exchange/First/Info")]
        [ShowIf("HasFirstWealthComponent")]
        [LabelText("Money")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        float FirstMoney => HasFirstWealthComponent ? First.Wealth.Money : 0;

        [OdinSerialize]
        [TableList]
        [BoxGroup("Exchange/First")]
        [ShowIf("First")]
        [PropertySpace(10)]
        List<InventoryItem> FirstInventory { get; set; }

        List<Product> FirstProducts => First ? First.Inventory.Inventory.Select(e => e.Product).ToList() : null;

        [OdinSerialize]
        [ShowIf("First")]
        [ValueDropdown("FirstProducts")]
        [LabelText("Product")]
        [LabelWidth(60)]
        [HorizontalGroup("Exchange/First/Sell")]
        [PropertySpace(10)]
        Product FirstSelectedProduct { get; set; }

        int FirstSelectedProductSupply =>
            FirstSelectedProduct ? FirstInventory.Find(e => e.Product == FirstSelectedProduct).Supply : 0;

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
        void FirstBuy()
        {
            for (var i = 0; i < FirstAmount; i++)
            {
                First.SellProduct(FirstSelectedProduct, Second);
            }

            Refresh();
        }

        [OdinSerialize]
        [HorizontalGroup("Exchange")]
        [BoxGroup("Exchange/Second")]
        [HorizontalGroup("Exchange/Second/Info")]
        [OnValueChanged("OnSecondChanged")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        ExchangeComponent Second { get; set; }

        bool HasSecondWealthComponent => Second && Second.Wealth;

        [ShowInInspector]
        [HorizontalGroup("Exchange/Second/Info")]
        [ShowIf("HasSecondWealthComponent")]
        [LabelText("Money")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        float SecondMoney => HasSecondWealthComponent ? Second.Wealth.Money : 0;

        [OdinSerialize]
        [TableList]
        [BoxGroup("Exchange/Second")]
        [ShowIf("Second")]
        [PropertySpace(10)]
        List<InventoryItem> SecondInventory { get; set; }

        List<Product> SecondProducts =>
            Second ? Second.Inventory.Inventory.Select(e => e.Product).ToList() : null;

        [OdinSerialize]
        [ShowIf("Second")]
        [ValueDropdown("SecondProducts")]
        [LabelText("Product")]
        [LabelWidth(60)]
        [HorizontalGroup("Exchange/Second/Buy")]
        [PropertySpace(10)]
        Product SecondSelectedProduct { get; set; }

        int SecondSelectedProductSupply =>
            SecondSelectedProduct ? SecondInventory.Find(e => e.Product == SecondSelectedProduct).Supply : 0;

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
        void SecondBuy()
        {
            for (var i = 0; i < SecondAmount; i++)
            {
                First.BuyProduct(SecondSelectedProduct, Second);
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