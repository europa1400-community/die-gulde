using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using Gulde.Production;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gulde.Company
{
    [RequireComponent(typeof(MasterComponent))]
    [RequireComponent(typeof(CompanyComponent))]
    [RequireComponent(typeof(ProductionComponent))]
    public class ProductionAgentComponent : SerializedMonoBehaviour
    {
        MasterComponent Master { get; set; }

        CompanyComponent Company { get; set; }

        ProductionComponent Production { get; set; }

        [ShowInInspector]
        Dictionary<Recipe, float> PPH => GetProfitsPerHour();

        void Awake()
        {
            Master = GetComponent<MasterComponent>();
            Company = GetComponent<CompanyComponent>();
            Production = GetComponent<ProductionComponent>();

            Production.RecipeFinished += OnRecipeFinished;

            Locator.Time.Morning += OnMorning;
            Locator.Time.Evening += OnEvening;
        }

        void OnMorning(object sender, EventArgs e)
        {
            AssignEmployees();
        }

        void OnEvening(object sender, EventArgs e)
        {
            UnassignEmployees();
        }

        Dictionary<Recipe, float> GetProfitsPerHour()
        {
            if (!Production) return null;

            var profitsPerHour = new Dictionary<Recipe, float>();

            foreach (var recipe in Production.Recipes)
            {
                var resourceCost = GetResourceCost(recipe);
                var productRevenue = GetProductRevenue(recipe);
                var profitPerHour = (productRevenue - resourceCost) / recipe.Time;

                profitsPerHour.Add(recipe, profitPerHour);
            }

            return profitsPerHour;
        }

        float GetResourceCost(Recipe recipe)
        {
            var resourceCost = 0f;

            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;
                var currentPrice = Locator.Market.GetPrice(resource);

                var resourceRecipe = Production.GetRecipe(resource);
                resourceCost += resourceRecipe ? GetResourceCost(resourceRecipe) * amount : currentPrice * amount;
            }

            return resourceCost;
        }

        float GetProductRevenue(Recipe recipe) =>
            Locator.Market ? Locator.Market.GetPrice(recipe.Product) : recipe.Product.MeanPrice;

        void AssignEmployees()
        {
            var profitsPerHour = GetProfitsPerHour();
            var bestRecipe = profitsPerHour.OrderByDescending(pair => pair.Value).First().Key;

            foreach (var employee in Company.Employees)
            {
                Production.Assign(employee, bestRecipe);
            }
        }

        void UnassignEmployees()
        {
            foreach (var employee in Company.Employees)
            {
                Production.Unassign(employee);
            }
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            UnassignEmployees();
            AssignEmployees();
        }
    }
}