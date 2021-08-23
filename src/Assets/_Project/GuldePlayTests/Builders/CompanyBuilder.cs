using System.Collections.Generic;
using Gulde.Company;
using Gulde.Economy;
using Gulde.Production;
using UnityEditor;
using UnityEngine;

namespace GuldePlayTests.Builders
{
    public class CompanyBuilder
    {
        GameObject CompanyObject { get; set; }

        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
        ProductionRegistryComponent ProductionRegistry => CompanyObject.GetComponent<ProductionRegistryComponent>();

        int Employees { get; set; }
        int Carts { get; set; }

        public CompanyBuilder()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/prefab_company.prefab");
            CompanyObject = Object.Instantiate(prefab);
        }

        public CompanyBuilder WithOwner(WealthComponent owner)
        {
            Company.Owner = owner;

            return this;
        }

        public CompanyBuilder WithSlots(int resourceSlots, int productSlots)
        {
            Company.Production.ResourceInventory.Slots = resourceSlots;
            Company.Production.ProductInventory.Slots = productSlots;

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
            ProductionRegistry.Register(recipes);

            return this;
        }

        public CompanyBuilder WithRecipe(Recipe recipe)
        {
            ProductionRegistry.Register(recipe);

            return this;
        }

        public GameObject Build()
        {
            for (var i = 0; i < Employees; i++)
            {
                Company.HireEmployee();
            }

            for (var i = 0; i < Carts; i++)
            {
                Company.HireCart();
            }

            return CompanyObject;
        }
    }
}