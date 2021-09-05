using GuldeLib;
using GuldeLib.Economy;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GuldeEditor.Economy
{
    public class BillingReporter : OdinEditorWindow
    {
        [MenuItem("Gulde/Billing Reporter")]
        static void ShowWindow() => GetWindow<BillingReporter>();

        [OdinSerialize]
        [OnValueChanged("OnWealthChanged")]
        [ValueDropdown("@FindObjectsOfType<WealthComponent>()")]
        WealthComponent Wealth { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Sales { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string TotalRevenue { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Purchases { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Hirings { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Carts { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Repairs { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Wages { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string TotalExpense { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Total { get; set; }

        void Awake()
        {
            if (!Wealth) return;

            Wealth.Billed -= OnBilled;
            Wealth.Billed += OnBilled;
        }

        void OnWealthChanged()
        {
            if (!Wealth) return;

            Wealth.Billed -= OnBilled;
            Wealth.Billed += OnBilled;
        }

        void OnBilled(object sender, BillingEventArgs e)
        {
            Debug.Log("Bill updated");

            var total = 0f;
            var totalExpense = 0f;
            var totalRevenue = 0f;

            if (e.Expenses.ContainsKey(TurnoverType.Purchase))
            {
                var value = e.Expenses[TurnoverType.Purchase];
                total -= value;
                totalExpense -= value;
                Purchases = $"Wareneinkäufe:\t-{value} Gulden";
            }

            if (e.Expenses.ContainsKey(TurnoverType.Hiring))
            {
                var value = e.Expenses[TurnoverType.Hiring];
                total -= value;
                totalExpense -= value;
                Hirings = $"Handgelder:\t-{value} Gulden";
            }

            if (e.Expenses.ContainsKey(TurnoverType.Cart))
            {
                var value = e.Expenses[TurnoverType.Cart];
                total -= value;
                totalExpense -= value;
                Carts = $"Fußgelder:\t-{value} Gulden";
            }

            if (e.Expenses.ContainsKey(TurnoverType.Repair))
            {
                var value = e.Expenses[TurnoverType.Repair];
                total -= value;
                totalExpense -= value;
                Repairs = $"Reparaturen & Sanierungen:\t-{value} Gulden";
            }

            if (e.Expenses.ContainsKey(TurnoverType.Wage))
            {
                var value = e.Expenses[TurnoverType.Wage];
                total -= value;
                totalExpense -= value;
                Wages = $"Personalkosten:\t-{value} Gulden";
            }

            if (e.Revenues.ContainsKey(TurnoverType.Sale))
            {
                var value = e.Revenues[TurnoverType.Sale];
                total += value;
                totalRevenue += value;
                Sales = $"Warenverkäufe:\t+{value} Gulden";
            }

            Total = $"Gesamt:\t{total} Gulden";
            TotalExpense = $"Ausgaben Gesamt:\t{totalExpense} Gulden";
            TotalRevenue = $"Einnahmen Gesamt:\t{totalRevenue} Gulden";
        }

        bool IsDisabled => !Locator.Time || Locator.Time.IsRunning || !Application.isPlaying;

        [Button]
        [DisableIf("IsDisabled")]
        void Advance()
        {
            Locator.Time.StartTime();
        }
    }
}