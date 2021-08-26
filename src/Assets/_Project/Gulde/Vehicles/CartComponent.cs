using Gulde.Company;
using Gulde.Economy;
using Gulde.Entities;
using Gulde.Inventory;
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
        [OdinSerialize]
        [BoxGroup("Info")]
        public CompanyComponent Company { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public EntityComponent Entity { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public InventoryComponent Inventory { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public ExchangeComponent Exchange { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TravelComponent Travel { get; private set; }

        void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Exchange = GetComponent<ExchangeComponent>();
            Inventory = GetComponent<InventoryComponent>();
            Travel = GetComponent<TravelComponent>();
        }

        public void SetCompany(CompanyComponent company)
        {
            Company = company;

            Exchange.Owner = company.Owner;
            Travel.TravelTo(company.Location);
        }
    }
}