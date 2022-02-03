using System;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Inventories;
using GuldeLib.Maps;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Companies.Carts
{
    public class CartComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Settings")]
        public CartType Type { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public CompanyComponent Company { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public EntityComponent Entity => GetComponent<EntityComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public InventoryComponent Inventory => GetComponent<InventoryComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ExchangeComponent Exchange => GetComponent<ExchangeComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public TravelComponent Travel => GetComponent<TravelComponent>();

        public event EventHandler CompanyReached;
        public event EventHandler MarketReached;
        public event EventHandler<InitializedEventArgs> Initialized;

        void Awake()
        {
            this.Log("Cart initializing");
        }

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        public void OnDestinationReached(object sender, MapComponent.LocationEventArgs e)
        {
            if (e.Location == Company.Location) CompanyReached?.Invoke(this, EventArgs.Empty);
            if (e.Location == Locator.Market.Location) MarketReached?.Invoke(this, EventArgs.Empty);
        }

        public void SetCompany(CompanyComponent company)
        {
            this.Log($"Cart setting company to {company}");

            Company = company;

            Exchange.Owner = company.Owner;
            Travel.TravelTo(company.Location);
        }

        public enum CartType
        {
            Small,
            Medium,
            Large,
        }

        public class InitializedEventArgs : EventArgs
        {
        }
    }
}