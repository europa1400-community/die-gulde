using System;
using System.Collections.Generic;
using GuldeLib.Companies;
using GuldeLib.Timing;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Economy
{
    /// <summary>
    /// Provides functionality for money management, property management and transaction billing.
    /// </summary>
    public class WealthComponent : SerializedMonoBehaviour
    {
        /// <summary>
        /// Gets the current amount of owned money.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public float Money { get; set; }

        /// <summary>
        /// Gets or sets the amount of spend money that is not yet billed.
        /// </summary>
        /// <remarks>
        /// This is used for wages, which are billed only at the end of year, but registered hourly.
        /// </remarks>
        [ShowInInspector]
        [BoxGroup("Info")]
        float VirtualExpenses { get; set; }

        /// <summary>
        /// Gets the dictionary mapping <see cref = "TurnoverType">TurnOvertype</see> to the amount of spent money.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        Dictionary<TurnoverType, float> Expenses { get; } = new Dictionary<TurnoverType, float>();

        /// <summary>
        /// Gets the dictionary mapping <see cref = "TurnoverType">TurnOvertype</see> to the amount of earned money.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        Dictionary<TurnoverType, float> Revenues { get; } = new Dictionary<TurnoverType, float>();

        /// <summary>
        /// Gets the list of <see cref = "CompanyComponent">CompanyComponents</see> owned by this <see cref = "WealthComponent"/>.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public List<CompanyComponent> Companies { get; } = new List<CompanyComponent>();

        /// <summary>
        /// Gets the <see cref = "ExchangeComponent">ExchangeComponent</see> associated to this <see cref = "WealthComponent"/>.
        /// </summary>
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ExchangeComponent Exchange { get; private set; }

        /// <summary>
        /// Returns a new <see cref = "WaitForBilled">WaitForBilled</see> <see cref = "CustomYieldInstruction">CustomYieldInstruction</see>
        /// that waits for the <see cref = "Billed"/> event to be invoked.
        /// </summary>
        public WaitForBilled WaitForBilled => new WaitForBilled(this);

        /// <summary>
        /// Adds a given amount of money to this <see cref = "WealthComponent">WealthComponent's</see> <see cref = "Money"/>.
        /// </summary>
        /// <param name="value">The amount of money to add.</param>
        public void AddMoney(float value) => Money += value;

        /// <summary>
        /// Removes a given amount of money from this <see cref = "WealthComponent">WealthComponent's</see> <see cref = "Money"/>.
        /// </summary>
        /// <param name="value">The amount of money to remove.</param>
        public void RemoveMoney(float value) => Money -= value;

        /// <summary>
        /// Invoked after a year's transactions were billed.
        /// </summary>
        public event EventHandler<BillingEventArgs> Billed;

        /// <remarks>
        /// Initializes references to <see cref = "Exchange"/>, sets up event callbacks.
        /// </remarks>
        void Awake()
        {
            this.Log("Wealth initialized");

            Exchange = GetComponent<ExchangeComponent>();

            if (Exchange) Exchange.ItemSold += OnItemSold;
            if (Exchange) Exchange.ItemBought += OnItemBought;

            if (Locator.Time) Locator.Time.YearTicked += OnYearTicked;
        }

        /// <summary>
        /// Registers a new <see cref = "CompanyComponent"/> to be owned by this <see cref = "WealthComponent"/>.
        /// </summary>
        /// <param name="company">The CompanyComponent to register.</param>
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

        /// <summary>
        /// Registers a new expense transaction to be billed at the end of the year.
        /// </summary>
        /// <param name="type">The <see cref = "TurnoverType"/> of the transaction.</param>
        /// <param name="amount">The amount of money transacted.</param>
        void RegisterExpense(TurnoverType type, float amount)
        {
            this.Log($"Wealth registered expense of {type} and {amount}");

            if (!Expenses.ContainsKey(type)) Expenses.Add(type, 0f);

            Expenses[type] += amount;
        }

        /// <summary>
        /// Registers a new revenue transaction to be billed at the end of the year.
        /// </summary>
        /// <param name="type">The <see cref = "TurnoverType"/> of the transaction.</param>
        /// <param name="amount">The amount of money transacted.</param>
        void RegisterRevenue(TurnoverType type, float amount)
        {
            this.Log($"Wealth registered revenue of {type} and {amount}");

            if (!Revenues.ContainsKey(type)) Revenues.Add(type, 0f);

            Revenues[type] += amount;
        }

        /// <summary>
        /// Callback for the <see cref = "ExchangeComponent.ItemBought">ItemBought</see> event
        /// of any <see cref = "ExchangeComponent"/> associated to or owned by this <see cref = "WealthComponent"/>.
        /// <see cref = "RegisterExpense">Registers</see> the exchange as an expense.
        /// </summary>
        void OnItemBought(object sender, ItemBoughtEventArgs e)
        {
            this.Log($"Wealth registered purchase of {e.Item} for {e.Price}");

            Money -= e.Price * e.Amount;
            RegisterExpense(TurnoverType.Purchase, e.Price * e.Amount);
        }

        /// <summary>
        /// Callback for the <see cref = "ExchangeComponent.ItemSold">ItemSold</see> event
        /// of any <see cref = "ExchangeComponent"/> associated to or owned by this <see cref = "WealthComponent"/>.
        /// <see cref = "RegisterExpense">Registers</see> the exchange as a revenue.
        /// </summary>
        void OnItemSold(object sender, ItemSoldEventArgs e)
        {
            this.Log($"Wealth registered sale of {e.Item} for {e.Price}");

            Money += e.Price * e.Amount;
            RegisterRevenue(TurnoverType.Sale, e.Price * e.Amount);
        }

        /// <summary>
        /// Callback for the <see cref = "CompanyComponent.EmployeeHired"/> event
        /// of any <see cref = "CompanyComponent"/> owned by this <see cref = "WealthComponent"/>.
        /// <see cref = "RegisterExpense">Registers</see> the exchange as an expense.
        /// </summary>
        void OnEmployeeHired(object sender, EmployeeHiredEventArgs e)
        {
            this.Log($"Wealth registered hiring of employee {e.Employee} for {e.Cost}");

            Money -= e.Cost;
            RegisterExpense(TurnoverType.Hiring, e.Cost);
        }

        /// <summary>
        /// Callback for the <see cref = "CompanyComponent.CartHired"/> event
        /// of any <see cref = "CompanyComponent"/> owned by this <see cref = "WealthComponent"/>.
        /// <see cref = "RegisterExpense">Registers</see> the exchange as a revenue and sets up event callbacks for the cart.
        /// </summary>
        void OnCartHired(object sender, CartHiredEventArgs e)
        {
            this.Log($"Wealth registered hiring of cart {e.Cart} for {e.Cost}");

            var exchange = e.Cart.GetComponent<ExchangeComponent>();
            if (exchange)
            {
                exchange.ItemBought += OnItemBought;
                exchange.ItemSold += OnItemSold;
            }

            Money -= e.Cost;
            RegisterExpense(TurnoverType.Cart, e.Cost);
        }

        /// <summary>
        /// Callback for the <see cref = "CompanyComponent.WagePaid"/> event
        /// of any <see cref = "CompanyComponent"/> owned by this <see cref = "WealthComponent"/>.
        /// <see cref = "RegisterExpense">Registers</see> the wage as a <see cref = "VirtualExpenses">virtual</see> expense.
        /// </summary>
        void OnWagePaid(object sender, WagePaidEventArgs e)
        {
            this.Log($"Wealth registered wage bill of {e.Cost}");

            VirtualExpenses += e.Cost;
            RegisterExpense(TurnoverType.Wage, e.Cost);
        }

        /// <summary>
        /// Callback for the <see cref = "TimeComponent.YearTicked"/> event.
        /// Bills any outstanding revenues and expenses.
        /// </summary>
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
}