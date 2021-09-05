using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Production;
using GuldeLib.Vehicles;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Company
{
    [RequireComponent(typeof(MasterComponent))]
    [RequireComponent(typeof(CompanyComponent))]
    [RequireComponent(typeof(ProductionComponent))]
    public class ProductionAgentComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        [SuffixLabel("Items")]
        int ResourceBuffer { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        MasterComponent Master { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        CompanyComponent Company { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        ProductionComponent Production { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        Dictionary<CartComponent, CartAgentComponent> CartToAgent { get; } =
            new Dictionary<CartComponent, CartAgentComponent>();

        void Awake()
        {
            this.Log("Production agent initializing");

            Master = GetComponent<MasterComponent>();
            Company = GetComponent<CompanyComponent>();
            Production = GetComponent<ProductionComponent>();

            Production.Registry.RecipeFinished += OnRecipeFinished;

            if (Locator.Time)
            {
                Locator.Time.Morning += OnMorning;
                Locator.Time.Evening += OnEvening;
            }

            foreach (var cart in Company.Carts)
            {
                if (!cart) continue;
                InitializeCart(cart);
            }
        }

        void InitializeCart(CartComponent cart)
        {
            this.Log($"Agent initializing cart {cart}");

            var cartAgent = cart.gameObject.AddComponent<CartAgentComponent>();
            CartToAgent.Add(cart, cartAgent);
            cartAgent.Company = Company;
        }

        void OnMorning(object sender, EventArgs e)
        {
            Produce(BestRecipes.First());
        }

        void OnEvening(object sender, EventArgs e)
        {
            UnassignEmployees();
        }

        Dictionary<Recipe, float> GetProfitsPerHour()
        {
            if (!Production) return null;

            var profitsPerHour = new Dictionary<Recipe, float>();

            foreach (var recipe in Production.Registry.Recipes)
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

                var resourceRecipe = Production.Registry.GetRecipe(resource);
                resourceCost += resourceRecipe ? GetResourceCost(resourceRecipe) * amount : currentPrice * amount;
            }

            return resourceCost;
        }

        float GetProductRevenue(Recipe recipe) =>
            Locator.Market ? Locator.Market.GetPrice(recipe.Product) : recipe.Product.MeanPrice;

        List<Recipe> BestRecipes =>
            GetProfitsPerHour().OrderByDescending(pair => pair.Value).Select(pair => pair.Key).ToList();

        void Produce(Recipe recipe)
        {
            Production.Assignment.AssignAll(recipe);

            if (Production.HasResources(recipe, ResourceBuffer)) return;

            var orders = new Queue<ItemOrder>();

            foreach (var pair in recipe.Resources)
            {
                orders.Enqueue(new ItemOrder(pair.Key, pair.Value));
            }

            foreach (var pair in CartToAgent)
            {
                for (var i = 0; i < pair.Key.Inventory.Slots; i++)
                {
                    if (orders.Count == 0) break;
                    var order = orders.Dequeue();
                    pair.Value.AddOrder(order);
                }

                if (orders.Count == 0) break;
            }
        }

        void UnassignEmployees()
        {
            foreach (var employee in Company.Employees)
            {
                Production.Assignment.Unassign(employee);
            }
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            UnassignEmployees();
            Produce(BestRecipes.First());
        }
    }
}