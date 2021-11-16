using System;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Inventories;
using GuldeLib.Maps;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Companies.Carts
{
    [RequireComponent(typeof(InventoryComponent))]
    [RequireComponent(typeof(TravelComponent))]
    public class CartComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Settings")]
        public CartType CartType { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public CompanyComponent Company { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public EntityComponent Entity { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public InventoryComponent Inventory { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ExchangeComponent Exchange { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public TravelComponent Travel { get; private set; }
        public event EventHandler CompanyReached;
        public event EventHandler MarketReached;

        void Awake()
        {
            this.Log("Cart initializing");

            Entity = GetComponent<EntityComponent>();
            Exchange = GetComponent<ExchangeComponent>();
            Inventory = GetComponent<InventoryComponent>();
            Travel = GetComponent<TravelComponent>();

            Travel.DestinationReached += OnDestinationReached;
        }

        void OnDestinationReached(object sender, LocationEventArgs e)
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
    }
}