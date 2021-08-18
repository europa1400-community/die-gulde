using System.Collections.Generic;
using Gulde.Buildings;
using Gulde.Company;
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

        public void AddMoney(float value) => Money += value;

        public void RemoveMoney(float value) => Money -= value;

        void Awake()
        {
            Exchange = GetComponent<ExchangeComponent>();
            
            if (Exchange) Exchange.ItemSold += OnItemSold;
            if (Exchange) Exchange.ItemBought += OnItemBought;

            foreach (var company in Companies)
            {
                company.EmployeeHired += OnEmployeeHired;
                company.CartHired += OnCartHired;
            }
        }

        void OnItemBought(object sender, ExchangeEventArgs e)
        {
            Money -= e.Price;
        }

        void OnItemSold(object sender, ExchangeEventArgs e)
        {
            Money += e.Price;
        }

        void OnEmployeeHired(object sender, HiringEventArgs e)
        {
            Money -= e.Cost;
        }

        void OnCartHired(object sender, HiringEventArgs e)
        {
            Money -= e.Cost;
        }
    }
}