using System.Collections;
using System.Collections.Generic;
using GuldeLib.Company;
using GuldeLib.Economy;
using GuldeLib.Maps;
using GuldeLib.Production;
using GuldeLib.Vehicles;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GuldeLib.Builders
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
        PlayerBuilder PlayerBuilder { get; set; }
        float WagePerHour { get; set; }
        int Employees { get; set; }
        int SmallCarts { get; set; }
        int LargeCarts { get; set; }
        int ResourceSlots { get; set; } = int.MaxValue;
        int ProductSlots { get; set; } = int.MaxValue;
        HashSet<Recipe> Recipes { get; } = new HashSet<Recipe>();
        bool HasMaster { get; set; }

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
        
        public CompanyBuilder WithOwner(PlayerBuilder playerBuilder)
        {
            PlayerBuilder = playerBuilder;
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

        public CompanyBuilder WithCarts(int count, CartType type = CartType.Small)
        {
            if (type == CartType.Small) SmallCarts = count;
            else if (type == CartType.Large) LargeCarts = count;
            return this;
        }

        public CompanyBuilder WithRecipe(Recipe recipe)
        {
            Recipes.Add(recipe);
            return this;
        }

        public CompanyBuilder WithRecipes(HashSet<Recipe> recipes)
        {
            Recipes.AddRange(recipes);
            return this;
        }

        public CompanyBuilder WithMaster()
        {
            HasMaster = true;
            return this;
        }

        public CompanyBuilder WithoutMaster()
        {
            HasMaster = false;
            return this;
        }

        public override IEnumerator Build()
        {
            if (!Map)
            {
                this.Log("Company cannot be created without a map", LogType.Error);
                yield break;
            }

            if (!EntryCell.IsInBounds(Map.Size))
            {
                this.Log($"Company cannot be created out of bounds at {EntryCell}", LogType.Error);
                yield break;
            }

            yield return base.Build();

            var parent = Parent ? Parent.transform : Map.transform;
            CompanyObject = Object.Instantiate(CompanyPrefab, parent);

            var company = CompanyObject.GetComponent<CompanyComponent>();
            var productionRegistry = CompanyObject.GetComponent<ProductionRegistryComponent>();
            var location = CompanyObject.GetComponent<LocationComponent>();

            Map.Register(location);
            location.EntryCell = EntryCell;

            company.Production.ResourceInventory.Slots = ResourceSlots;
            company.Production.ProductInventory.Slots = ProductSlots;

            if (PlayerBuilder != null && !PlayerBuilder.PlayerObject)
            {
                yield return PlayerBuilder.Build();
            }
            
            var owner = 
                PlayerBuilder != null
                ? PlayerBuilder.PlayerObject.GetComponent<WealthComponent>()
                : Owner;
            
            company.Owner = owner;
            company.Exchange.Owner = owner;
            company.WagePerHour = WagePerHour;

            productionRegistry.Register(Recipes);

            for (var i = 0; i < Employees; i++)
            {
                company.HireEmployee();
            }

            for (var i = 0; i < SmallCarts; i++)
            {
                company.HireCart();
            }

            for (var i = 0; i < LargeCarts; i++)
            {
                company.HireCart(CartType.Large);
            }

            if (owner) owner.RegisterCompany(company);

            if (HasMaster) CompanyObject.AddComponent<ProductionAgentComponent>();
        }
    }
}