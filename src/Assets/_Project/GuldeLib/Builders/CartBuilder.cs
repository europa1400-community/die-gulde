using System;
using System.Collections;
using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using MonoLogger.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GuldeLib.Builders
{
    // TODO: Add documentation
    public class CartBuilder : Builder
    {
        [LoadAsset("prefab_cart")]
        GameObject CartPrefab { get; set; }

        public GameObject CartObject { get; private set; }

        public CartComponent Cart { get; private set; }

        CompanyComponent Company { get; set; }

        CartType CartType { get; set; } = CartType.Small;

        public CartBuilder WithCompany(CompanyComponent company)
        {
            Company = company;
            return this;
        }

        public CartBuilder WithCartType(CartType cartType)
        {
            CartType = cartType;
            return this;
        }

        public override IEnumerator Build()
        {
            if (!Company)
            {
                this.Log($"Cannot create cart without a company.", LogType.Error);
                yield break;
            }

            yield return base.Build();

            CartObject = Object.Instantiate(CartPrefab);

            Cart = CartObject.GetComponent<CartComponent>();

            switch (CartType)
            {
                case CartType.Small:
                    Cart.Inventory.Slots = 1;
                    break;
                case CartType.Medium:
                    Cart.Inventory.Slots = 2;
                    break;
                case CartType.Large:
                    Cart.Inventory.Slots = 3;
                    break;
            }

            Cart.SetCompany(Company);
        }
    }
}