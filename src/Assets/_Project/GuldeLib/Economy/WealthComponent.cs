using System;
using System.Collections.Generic;
using GuldeLib.Company;
using GuldeLib.Timing;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Economy
{
    public class WealthComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Info")]
        public float Money { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        float VirtualExpenses { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        Dictionary<TurnoverType, float> Expenses { get; } = new Dictionary<TurnoverType, float>();

        [ShowInInspector]
        [BoxGroup("Info")]
        Dictionary<TurnoverType, float> Revenues { get; } = new Dictionary<TurnoverType, float>();

        [ShowInInspector]
        [BoxGroup("Info")]
        public List<CompanyComponent> Companies { get; } = new List<CompanyComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        ExchangeComponent Exchange { get; set; }

        public WaitForBilled WaitForBilled => new WaitForBilled(this);

        public void AddMoney(float value) => Money += value;

        public void RemoveMoney(float value) => Money -= value;

        public event EventHandler<BillingEventArgs> Billed;

        void Awake()
        {
            this.Log("Wealth initialized");

            Exchange = GetComponent<ExchangeComponent>();

            if (Exchange) Exchange.ItemSold += OnItemSold;
            if (Exchange) Exchange.ItemBought += OnItemBought;

            if (Locator.Time) Locator.Time.YearTicked += OnYearTicked;
        }

        public void RegisterCompany(CompanyComponent company)
        {
            if (!company) return;

            Companies.Add(company);

            company.EmployeeHired += OnEmployeeHired;
            company.CartHired += OnCartHired;
            company.WagePaid += OnWagePaid;

            foreach (var cart in company.Carts)
            {
                cart.Exchange.ItemBought += OnItemBought;
                cart.Exchange.ItemSold += OnItemSold;
            }
        }

        void RegisterExpense(TurnoverType type, float amount)
        {
            this.Log($"Wealth registered expense of {type} and {amount}");

            if (!Expenses.ContainsKey(type)) Expenses.Add(type, 0f);

            Expenses[type] += amount;
        }

        void RegisterRevenue(TurnoverType type, float amount)
        {
            this.Log($"Wealth registered revenue of {type} and {amount}");

            if (!Revenues.ContainsKey(type)) Revenues.Add(type, 0f);

            Revenues[type] += amount;
        }

        void OnItemBought(object sender, ExchangeEventArgs e)
        {
            this.Log($"Wealth registered purchase of {e.Item} for {e.Price}");

            Money -= e.Price * e.Amount;
            RegisterExpense(TurnoverType.Purchase, e.Price * e.Amount);
        }

        void OnItemSold(object sender, ExchangeEventArgs e)
        {
            this.Log($"Wealth registered sale of {e.Item} for {e.Price}");

            Money += e.Price * e.Amount;
            RegisterRevenue(TurnoverType.Sale, e.Price * e.Amount);
        }

        void OnEmployeeHired(object sender, HiringEventArgs e)
        {
            this.Log($"Wealth registered hiring of employee {e.Entity} for {e.Cost}");

            Money -= e.Cost;
            RegisterExpense(TurnoverType.Hiring, e.Cost);
        }

        void OnCartHired(object sender, HiringEventArgs e)
        {
            this.Log($"Wealth registered hiring of cart {e.Entity} for {e.Cost}");

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
            this.Log($"Wealth registered wage bill of {e.Cost}");

            VirtualExpenses += e.Cost;
            RegisterExpense(TurnoverType.Wage, e.Cost);
        }

        void OnYearTicked(object sender, TimeEventArgs e)
        {
            this.Log($"Wealth billing virtual expenses of total {VirtualExpenses}");

            Money -= VirtualExpenses;
            VirtualExpenses = 0f;

            foreach (var type in Expenses.Keys)
            {
                this.Log($"Wealth billing expenses of {type} for {Expenses[type]}");
            }

            foreach (var type in Revenues.Keys)
            {
                this.Log($"Wealth billing revenues of {type} for {Revenues[type]}");
            }

            Billed?.Invoke(this, new BillingEventArgs(new Dictionary<TurnoverType, float>(Expenses), new Dictionary<TurnoverType, float>(Revenues)));

            Expenses.Clear();
            Revenues.Clear();
        }
    }

    public class WaitForBilled : CustomYieldInstruction
    {
        bool IsBilled { get; set; }

        public override bool keepWaiting => !IsBilled;

        public WaitForBilled(WealthComponent wealthComponent)
        {
            wealthComponent.Billed += OnBilled;
        }

        void OnBilled(object sender, BillingEventArgs e)
        {
            IsBilled = true;
        }
    }
}