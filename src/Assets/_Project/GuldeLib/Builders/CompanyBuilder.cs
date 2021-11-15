using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    /// <summary>
    /// Provides functionality to build a company.
    /// </summary>
    public class CompanyBuilder : Builder
    {
        /// <summary>
        /// Gets the built company's <see cref = "GameObject">GameObject</see>.
        /// </summary>
        public GameObject CompanyObject { get; private set; }

        /// <summary>
        /// Gets the prefab used to create the company.
        /// </summary>
        [LoadAsset("prefab_company")]
        GameObject CompanyPrefab { get; set; }

        /// <summary>
        /// Gets or sets the parent of the built company.
        /// </summary>
        GameObject Parent { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "MapComponent">Map</see> of the built company.
        /// </summary>
        MapComponent Map { get; set; }

        /// <inheritdoc cref="LocationComponent.EntryCell"/>
        Vector3Int EntryCell { get; set; }

        /// <inheritdoc cref="CompanyComponent.Owner"/>
        WealthComponent Owner { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "PlayerBuilder">PlayerBuilder</see> used to build the player.
        /// </summary>
        PlayerBuilder PlayerBuilder { get; set; }

        /// <inheritdoc cref="CompanyComponent.WagePerHour"/>
        float WagePerHour { get; set; }

        /// <summary>
        /// Gets or sets the amount of employees employed by the company.
        /// </summary>
        int Employees { get; set; }

        /// <summary>
        /// Gets or sets the amount of small carts to be hired by the built company.
        /// </summary>
        int SmallCarts { get; set; }

        /// <summary>
        /// Gets or sets the amount of large carts to be hired by the built company.
        /// </summary>
        int LargeCarts { get; set; }

        /// <summary>
        /// Gets or sets the amount of slots in the resource inventory of the built company.
        /// </summary>
        int ResourceSlots { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the amount of slots in the product inventory of the built company.
        /// </summary>
        int ProductSlots { get; set; } = int.MaxValue;

        /// <inheritdoc cref="ProductionRegistryComponent.Recipes"/>
        HashSet<Recipe> Recipes { get; } = new HashSet<Recipe>();

        /// <summary>
        /// Gets or sets whether the built company has a <see cref = "MasterComponent">Master</see>.
        /// </summary>
        bool HasMaster { get; set; }

        /// <summary>
        /// Gets or sets whether the built company has a <see cref = "ProductionAgentComponent">ProductionAgent</see>.
        /// </summary>
        bool HasProductionAgent { get; set; }

        /// <inheritdoc cref="MasterComponent.Riskiness"/>
        float Riskiness { get; set; }

        /// <inheritdoc cref="MasterComponent.Investivity"/>
        float Investivity { get; set; }

        /// <inheritdoc cref="MasterComponent.Autonomy"/>
        float Autonomy { get; set; }

        /// <summary>
        /// Sets the <see cref = "MapComponent">Map</see> the company will be built on.
        /// </summary>
        public CompanyBuilder WithMap(MapComponent map)
        {
            Map = map;
            return this;
        }

        /// <summary>
        /// Sets the parent of the built company.
        /// </summary>
        public CompanyBuilder WithParent(GameObject parent)
        {
            Parent = parent;
            return this;
        }

        /// <summary>
        /// Sets the entry cell position of the built company.
        /// </summary>
        /// <param name="x">The x coordinate in cells.</param>
        /// <param name="y">The y coordinate in cells.</param>
        public CompanyBuilder WithEntryCell(int x, int y)
        {
            EntryCell = new Vector3Int(x, y, 0);
            return this;
        }

        /// <summary>
        /// Sets the owner of the built company.
        /// </summary>
        /// <param name="owner">The <see cref = "WealthComponent">WealthComponent</see> that owns the company.</param>
        public CompanyBuilder WithOwner(WealthComponent owner)
        {
            Owner = owner;
            return this;
        }

        /// <summary>
        /// Sets the owner of the built company.
        /// </summary>
        /// <param name="playerBuilder">The <see cref = "PlayerBuilder">PlayerBuilder</see> of the player that owns the company.</param>
        public CompanyBuilder WithOwner(PlayerBuilder playerBuilder)
        {
            PlayerBuilder = playerBuilder;
            return this;
        }

        /// <summary>
        /// Sets the amount of slots in the resource and production inventories.
        /// </summary>
        /// <param name="resourceSlots">The amount of slots in the resource inventory.</param>
        /// <param name="productSlots">The amount of slots in the product inventory.</param>
        public CompanyBuilder WithSlots(int resourceSlots, int productSlots)
        {
            ResourceSlots = resourceSlots;
            ProductSlots = productSlots;
            return this;
        }

        /// <summary>
        /// Sets the <see cref = "CompanyComponent.WagePerHour">WagePerHour</see> of the company.
        /// </summary>
        public CompanyBuilder WithWagePerHour(float wagePerHour)
        {
            WagePerHour = wagePerHour;
            return this;
        }

        /// <summary>
        /// Sets the amount of employees employed by the built company.
        /// </summary>
        public CompanyBuilder WithEmployees(int count)
        {
            Employees = count;
            return this;
        }

        /// <summary>
        /// Sets the amount of a given type of cart owned by the built company.
        /// </summary>
        /// <param name="count">The amount of carts.</param>
        /// <param name="type">The <see cref = "CartType">CartType</see> of the carts.</param>
        public CompanyBuilder WithCarts(int count, CartType type = CartType.Small)
        {
            if (type == CartType.Small) SmallCarts = count;
            else if (type == CartType.Large) LargeCarts = count;
            return this;
        }

        /// <summary>
        /// Sets the amount of carts owned by the built company to 0.
        /// </summary>
        public CompanyBuilder WithoutCarts()
        {
            SmallCarts = 0;
            LargeCarts = 0;

            return this;
        }

        /// <summary>
        /// Adds the given <see cref = "Recipe">Recipe</see> to the built company's <see cref = "ProductionRegistryComponent">ProductionRegistry</see>.
        /// </summary>
        public CompanyBuilder WithRecipe(Recipe recipe)
        {
            Recipes.Add(recipe);
            return this;
        }

        /// <summary>
        /// Adds the given <see cref = "Recipe">Recipes</see> to the built company's <see cref = "ProductionRegistryComponent">ProductionRegistry</see>.
        /// </summary>
        /// <param name="recipes">A HashSet of <see cref = "Recipe">Recipes</see>.</param>
        /// <returns></returns>
        public CompanyBuilder WithRecipes(HashSet<Recipe> recipes)
        {
            Recipes.AddRange(recipes);
            return this;
        }

        /// <summary>
        /// Removes all <see cref = "Recipe">Recipes</see> from the built company's <see cref = "ProductionRegistryComponent">ProductionRegistry</see>.
        /// </summary>
        public CompanyBuilder WithoutRecipes()
        {
            Recipes.Clear();

            return this;
        }

        /// <summary>
        /// Sets a defined <see cref = "MasterComponent">Master</see> to be hired by the built company.
        /// </summary>
        /// <param name="riskiness">The Value of [0, 1] defining the <see cref = "MasterComponent.Riskiness">Riskiness</see> of the <see cref = "MasterComponent">Master</see>.</param>
        /// <param name="investivity">The Value of [0, 1] defining the <see cref = "MasterComponent.Riskiness">Riskiness</see> of the <see cref = "MasterComponent">Master</see>.</param>
        /// <param name="autonomy">The Value of [0, 1] defining the <see cref = "MasterComponent.Riskiness">Riskiness</see> of the <see cref = "MasterComponent">Master</see>.</param>
        public CompanyBuilder WithMaster(float riskiness = 0f, float investivity = 0f, float autonomy = 0f)
        {
            Riskiness = Mathf.Clamp01(riskiness);
            Investivity = Mathf.Clamp01(investivity);
            Autonomy = Mathf.Clamp01(autonomy);
            HasMaster = true;
            return this;
        }

        /// <summary>
        /// Removes a previously set <see cref = "MasterComponent">Master</see> from the built company.
        /// </summary>
        public CompanyBuilder WithoutMaster()
        {
            HasMaster = false;
            return this;
        }

        /// <summary>
        /// Sets the company to be built with a <see cref = "ProductionAgentComponent">ProductionAgent</see>.
        /// </summary>
        /// <returns></returns>
        public CompanyBuilder WithProductionAgent()
        {
            HasProductionAgent = true;
            return this;
        }

        /// <inheritdoc cref="Builder.Build"/>
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
                yield return company.HireEmployeeAsync();
            }

            for (var i = 0; i < SmallCarts; i++)
            {
                yield return company.HireCartAsync();
            }

            for (var i = 0; i < LargeCarts; i++)
            {
                yield return company.HireCartAsync(CartType.Large);
            }

            if (owner) owner.RegisterCompany(company);

            if (HasMaster)
            {
                var master = CompanyObject.AddComponent<MasterComponent>();

                var riskinessProperty = master
                    .GetType()
                    .GetProperty(
                        "Riskiness",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                riskinessProperty?.SetValue(master, Riskiness);

                var investivityProperty = master
                    .GetType()
                    .GetProperty(
                        "Investivity",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                investivityProperty?.SetValue(master, Investivity);

                var autonomyProperty = master
                    .GetType()
                    .GetProperty(
                        "Autonomy",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                autonomyProperty?.SetValue(master, Autonomy);

                var masterProperty = company
                    .GetType()
                    .GetProperty(
                        "Master",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                masterProperty?.SetValue(company, master);

                if (HasProductionAgent) CompanyObject.AddComponent<ProductionAgentComponent>();
            }
        }
    }
}