using System.Collections;
using System.Collections.Generic;
using Gulde.Company;
using Gulde.Economy;
using Gulde.Maps;
using Gulde.Production;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gulde.Builders
{
    public class CompanyBuilder : Builder
    {
        public GameObject CompanyObject { get; private set; }

        [LoadAsset("prefab_company")]
        GameObject CompanyPrefab { get; set; }

        GameObject Parent { get; set; }
        MapComponent Map { get; set; }
        Vector3Int EntryCell { get; set; }
        WealthComponent Owner { get; set; }
        float WagePerHour { get; set; }
        int Employees { get; set; }
        int Carts { get; set; }
        int ResourceSlots { get; set; }
        int ProductSlots { get; set; }
        HashSet<Recipe> Recipes { get; set; } = new HashSet<Recipe>();

        public CompanyBuilder() : base()
        {
        }

        public CompanyBuilder WithMap(MapComponent map)
        {
            Map = map;
            return this;
        }

        public CompanyBuilder WithParent(GameObject parent)
        {
            Parent = parent;
            return this;
        }

        public CompanyBuilder WithEntryCell(int x, int y)
        {
            EntryCell = new Vector3Int(x, y, 0);
            return this;
        }

        public CompanyBuilder WithOwner(WealthComponent owner)
        {
            Owner = owner;
            return this;
        }

        public CompanyBuilder WithSlots(int resourceSlots, int productSlots)
        {
            ResourceSlots = resourceSlots;
            ProductSlots = productSlots;
            return this;
        }

        public CompanyBuilder WithWagePerHour(float wagePerHour)
        {
            WagePerHour = wagePerHour;
            return this;
        }

        public CompanyBuilder WithEmployees(int count)
        {
            Employees = count;
            return this;
        }

        public CompanyBuilder WithCarts(int count)
        {
            Carts = count;
            return this;
        }

        public CompanyBuilder WithRecipes(HashSet<Recipe> recipes)
        {
            Recipes.AddRange(recipes);
            return this;
        }

        public CompanyBuilder WithRecipe(Recipe recipe)
        {
            Recipes.Add(recipe);
            return this;
        }

        public override IEnumerator Build()
        {
            yield return base.Build();

            var parent = Parent ? Parent.transform : Map ? Map.transform : null;
            CompanyObject = Object.Instantiate(CompanyPrefab, parent);

            var company = CompanyObject.GetComponent<CompanyComponent>();
            var productionRegistry = CompanyObject.GetComponent<ProductionRegistryComponent>();
            var location = CompanyObject.GetComponent<LocationComponent>();

            for (var i = 0; i < Employees; i++)
            {
                company.HireEmployee();
            }

            for (var i = 0; i < Carts; i++)
            {
                company.HireCart();
            }

            company.Production.ResourceInventory.Slots = ResourceSlots;
            company.Production.ProductInventory.Slots = ProductSlots;

            company.Owner = Owner;
            company.WagePerHour = WagePerHour;

            productionRegistry.Register(Recipes);

            location.EntryCell = EntryCell;
        }
    }
}