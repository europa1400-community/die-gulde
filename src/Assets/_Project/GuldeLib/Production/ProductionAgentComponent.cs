using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using GuldeLib.Economy;
using GuldeLib.Vehicles;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Production
{
    [RequireComponent(typeof(MasterComponent))]
    [RequireComponent(typeof(CompanyComponent))]
    [RequireComponent(typeof(ProductionComponent))]
    public class ProductionAgentComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        [SuffixLabel("Items")]
        int ResourceBuffer { get; set; } = 1;

        [ShowInInspector]
        [BoxGroup("Info")]
        Recipe TargetRecipe { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        Queue<Recipe> RecipeQueue { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        CompanyComponent Company { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        ProductionComponent Production { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        MasterComponent Master { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        Dictionary<CartComponent, CartAgentComponent> CartToAgent { get; } =
            new Dictionary<CartComponent, CartAgentComponent>();

        List<Recipe> BestRecipes => ExpectedProfitsPerHour.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).ToList();

        Dictionary<Recipe, float> ProfitsPerHour
        {
            get
            {
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
        }

        Dictionary<Recipe, float> SpeculativeProfitsPerHour
        {
            get
            {
                var profitsPerHour = new Dictionary<Recipe, float>();

                foreach (var recipe in Production.Registry.Recipes)
                {
                    var resourceCost = GetResourceCost(recipe);
                    var productRevenue = recipe.Product.MeanPrice;
                    var profitPerHour = (productRevenue - resourceCost) / recipe.Time;

                    profitsPerHour.Add(recipe, profitPerHour);
                }

                return profitsPerHour;
            }
        }

        Dictionary<Recipe, float> ExpectedProfitsPerHour
        {
            get
            {
                var profitsPerHour = ProfitsPerHour;
                var speculativeProfitsPerHour = SpeculativeProfitsPerHour;

                var expectedProfitsPerHour = new Dictionary<Recipe, float>();

                foreach (var pair in profitsPerHour)
                {
                    var recipe = pair.Key;

                    var profitPerHour = pair.Value;
                    var speculativeProfitPerHour = speculativeProfitsPerHour[recipe];

                    var expectedProfitPerHour = Mathf.Lerp(profitPerHour, speculativeProfitPerHour, Master.Riskiness);
                    expectedProfitPerHour = Mathf.Max(profitPerHour, expectedProfitPerHour);
                    expectedProfitsPerHour.Add(recipe, expectedProfitPerHour);
                }

                return expectedProfitsPerHour;
            }
        }

        public event EventHandler ProductionFinished;

        void Awake()
        {
            this.Log("Production agent initializing");

            Company = GetComponent<CompanyComponent>();
            Production = GetComponent<ProductionComponent>();
            Master = GetComponent<MasterComponent>();

            ProductionFinished += OnProductionFinished;
            Production.Registry.RecipeFinished += OnRecipeFinished;

            foreach (var cart in Company.Carts)
            {
                RegisterCart(cart);
            }
        }

        void RegisterCart(CartComponent cart)
        {
            this.Log($"Agent initializing cart {cart}");

            var cartAgent = cart.gameObject.AddComponent<CartAgentComponent>();
            CartToAgent.Add(cart, cartAgent);
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

        void Produce()
        {
            var recipe = BestRecipes.First();

            this.Log($"ProductionAgent starting production for {recipe}");
            TargetRecipe = recipe;

            var recipes = new List<Recipe>();
            var orders = new List<ItemOrder>();

            CalculateProduction(recipe, recipes, orders);

            recipes.Reverse();
            RecipeQueue = new Queue<Recipe>(recipes);

            orders.Reverse();
            var orderQueue = new Queue<ItemOrder>(orders);

            PlaceOrders(orderQueue);
            AssignNextRecipe();
        }

        void CalculateProduction(Recipe targetRecipe, List<Recipe> recipes, List<ItemOrder> orders)
        {
            recipes.Add(targetRecipe);

            foreach (var pair in targetRecipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                var resourceRecipe = Production.Registry.GetRecipe(resource);

                if (!resourceRecipe)
                {
                    //kann gekauft werden

                    var supply = Production.ResourceInventory.GetSupply(resource);
                    var neededAmount = amount * ResourceBuffer - supply;

                    if (neededAmount <= 0) continue;

                    var itemOrder = new ItemOrder(resource, neededAmount);
                    orders.Add(itemOrder);
                }
                else
                {
                    //kann produziert werden

                    recipes.Add(resourceRecipe);
                    CalculateProduction(resourceRecipe, recipes, orders);
                }
            }
        }

        void PlaceOrders(Queue<ItemOrder> orders)
        {
            this.Log($"ProductionAgent placing orders");

            var agentToOrders = new Dictionary<CartAgentComponent, Queue<ItemOrder>>();

            foreach (var pair in CartToAgent)
            {
                if (orders.Count <= 0) break;

                var agent = pair.Value;

                this.Log($"ProductionAgent placing order for {agent}");

                agentToOrders.Add(agent, new Queue<ItemOrder>());

                for (var i = 0; i < pair.Key.Inventory.Slots; i++)
                {
                    if (orders.Count <= 0) break;

                    var order = orders.Dequeue();
                    agentToOrders[agent].Enqueue(order);

                    this.Log($"ProductionAgent placed order for {order.Amount} {order.Item}");
                }
            }

            if (orders.Count > 0)
            {
                foreach (var pair in CartToAgent)
                {
                    if (orders.Count <= 0) break;

                    var agent = pair.Value;
                    var order = orders.Dequeue();

                    this.Log($"ProductionAgent placing order for {agent}");

                    agentToOrders[agent].Enqueue(order);
                    this.Log($"ProductionAgent placed order for {order.Amount} {order.Item}");
                }
            }

            foreach (var pair in CartToAgent)
            {
                var agent = pair.Value;
                agent.AddOrders(agentToOrders[agent]);
            }
        }

        void AssignNextRecipe()
        {
            if (RecipeQueue.Count == 0)
            {
                ProductionFinished?.Invoke(this, EventArgs.Empty);
            }

            var nextRecipe = RecipeQueue.Dequeue();
            Production.Assignment.AssignAll(nextRecipe);
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            AssignNextRecipe();
        }

        void OnProductionFinished(object sender, EventArgs e)
        {
            Produce();
        }
    }
}