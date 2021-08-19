using System;
using System.Collections.Generic;
using Gulde.Buildings;
using Gulde.Company;
using Gulde.Timing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Economy
{
    public class WealthComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public float Money { get; private set; }

        [OdinSerialize]
        public List<CompanyComponent> Companies { get; set; } = new List<CompanyComponent>();

        [OdinSerialize]
        [ReadOnly]
        ExchangeComponent Exchange { get; set; }

        [OdinSerialize]
        [ReadOnly]
        float VirtualExpenses { get; set; }

        [OdinSerialize]
        Dictionary<TurnoverType, float> Expenses { get; set; } = new Dictionary<TurnoverType, float>();

        [OdinSerialize]
        Dictionary<TurnoverType, float> Revenues { get; set; } = new Dictionary<TurnoverType, float>();

        public void AddMoney(float value) => Money += value;

        public void RemoveMoney(float value) => Money -= value;

        public event EventHandler<BillingEventArgs> Billed;

        void Awake()
        {
            Exchange = GetComponent<ExchangeComponent>();

            if (Exchange) Exchange.ItemSold += OnItemSold;
            if (Exchange) Exchange.ItemBought += OnItemBought;

            Locator.Time.YearTicked += OnYearTicked;

            foreach (var company in Companies)
            {
                company.EmployeeHired += OnEmployeeHired;
                company.CartHired += OnCartHired;
                company.WagePaid += OnWagePaid;

                foreach (var cartExchange in company.CartExchanges)
                {
                    cartExchange.ItemBought += OnItemBought;
                    cartExchange.ItemSold += OnItemSold;
                }
            }
        }

        void RegisterExpense(TurnoverType type, float amount)
        {
            if (!Expenses.ContainsKey(type)) Expenses.Add(type, 0f);

            Expenses[type] += amount;
        }

        void RegisterRevenue(TurnoverType type, float amount)
        {
            if (!Revenues.ContainsKey(type)) Revenues.Add(type, 0f);

            Revenues[type] += amount;
        }

        void OnItemBought(object sender, ExchangeEventArgs e)
        {
            Money -= e.Price;
            RegisterExpense(TurnoverType.Purchase, e.Price);
        }

        void OnItemSold(object sender, ExchangeEventArgs e)
        {
            Debug.Log($"Registered sale on {name} of {e.Item} for {e.Price}");

            Money += e.Price;
            RegisterRevenue(TurnoverType.Sale, e.Price);
        }

        void OnEmployeeHired(object sender, HiringEventArgs e)
        {
            Money -= e.Cost;
            RegisterExpense(TurnoverType.Hiring, e.Cost);
        }

        void OnCartHired(object sender, HiringEventArgs e)
        {
            var exchange = e.Entity.GetComponent<ExchangeComponent>();
            if (exchange)
            {
                exchange.ItemBought += OnItemBought;
                exchange.ItemSold += OnItemSold;
            }

            Money -= e.Cost;
            RegisterExpense(TurnoverType.Cart, e.Cost);
        }

        void OnWagePaid(object sender, CostEventArgs e)
        {
            VirtualExpenses += e.Cost;
            RegisterExpense(TurnoverType.Wage, e.Cost);
        }

        void OnYearTicked(object sender, TimeEventArgs e)
        {
            Money -= VirtualExpenses;
            VirtualExpenses = 0f;

            Billed?.Invoke(this, new BillingEventArgs(Expenses, Revenues));

            Expenses = new Dictionary<TurnoverType, float>();
            Revenues = new Dictionary<TurnoverType, float>();
        }
    }
}