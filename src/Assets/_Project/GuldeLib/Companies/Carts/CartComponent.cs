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
        public EntityComponent Entity => this.GetCachedComponent<EntityComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public InventoryComponent Inventory => this.GetCachedComponent<InventoryComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ExchangeComponent Exchange => this.GetCachedComponent<ExchangeComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public TravelComponent Travel => this.GetCachedComponent<TravelComponent>();
        public event EventHandler CompanyReached;
        public event EventHandler MarketReached;

        void Awake()
        {
            this.Log("Cart initializing");
        }

        void Start()
        {
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