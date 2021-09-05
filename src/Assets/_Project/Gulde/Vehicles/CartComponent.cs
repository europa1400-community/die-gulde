using Gulde.Company;
using Gulde.Economy;
using Gulde.Entities;
using Gulde.Inventory;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Vehicles
{
    [RequireComponent(typeof(ExchangeComponent))]
    [RequireComponent(typeof(InventoryComponent))]
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(TravelComponent))]
    public class CartComponent : SerializedMonoBehaviour
    {
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

        void Awake()
        {
            this.Log("Cart initializing");

            Entity = GetComponent<EntityComponent>();
            Exchange = GetComponent<ExchangeComponent>();
            Inventory = GetComponent<InventoryComponent>();
            Travel = GetComponent<TravelComponent>();
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