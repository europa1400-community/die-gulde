using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.TypeObjects;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Producing
{
    public class ProductionAgentComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Settings")]
        [SuffixLabel("Items")]
        public int ResourceBuffer { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        Recipe TargetRecipe { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        Recipe NextRecipe { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        Queue<Recipe> RecipeQueue { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        CompanyComponent Company => GetComponent<CompanyComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        ProductionComponent Production => GetComponent<ProductionComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        MasterComponent Master => GetComponent<MasterComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        Dictionary<CartComponent, CartAgentComponent> CartToAgent { get; } =
            new Dictionary<CartComponent, CartAgentComponent>();

        List<Recipe> BestRecipes => ExpectedProfitsPerHour.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).ToList();

        /// <summary>
        /// Profits per recipe calculated with current market prices of resources and products.
        /// </summary>
        Dictionary<Recipe, float> ProfitsPerHour
        {
            get
            {
                var profitsPerHour = new Dictionary<Recipe, float>();

                foreach (var recipe in Production.Registry.Recipes)
                {
                    var resourceCost = GetResourceCost(recipe);
                    var productRevenue = GetProductRevenue(recipe);
                    var recipeTime = GetRecipeTime(recipe);
                    var profitPerHour = (productRevenue - resourceCost) / recipeTime;

                    profitsPerHour.Add(recipe, profitPerHour);
                }

                return profitsPerHour;
            }
        }

        /// <summary>
        /// Profits per recipe calculated with current market prices of resources and mean prices of products.
        /// </summary>
        Dictionary<Recipe, float> SpeculativeProfitsPerHour
        {
            get
            {
                var profitsPerHour = new Dictionary<Recipe, float>();

                foreach (var recipe in Production.Registry.Recipes)
                {
                    var resourceCost = GetResourceCost(recipe);
                    var productRevenue = recipe.Product.MeanPrice;
                    var recipeTime = GetRecipeTime(recipe);
                    var profitPerHour = (productRevenue - resourceCost) / recipeTime;

                    profitsPerHour.Add(recipe, profitPerHour);
                }

                return profitsPerHour;
            }
        }

        /// <summary>
        /// Profits per recipe interpolated between current and mean product prices according to the masters stats.
        /// </summary>
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

                    var expectedProfitPerHour = (profitPerHour + speculativeProfitPerHour) / 2f;
                    expectedProfitPerHour = Mathf.Max(profitPerHour, expectedProfitPerHour);
                    expectedProfitsPerHour.Add(recipe, expectedProfitPerHour);
                }

                return expectedProfitsPerHour;
            }
        }

        public event EventHandler ProductionFinished;

        public event EventHandler<InitializedEventArgs> Initialized;

        void Awake()
        {
            this.Log("Production agent initializing");

            foreach (var cart in Company.Carts)
            {
                RegisterCart(cart);
            }

            ProductionFinished += OnProductionFinished;
        }

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
            Produce();
        }

        void RegisterCart(CartComponent cart)
        {
            this.Log($"ProductionAgent initializing cart {cart}");

            var cartAgent = cart.gameObject.AddComponent<CartAgentComponent>();

            var logtype = this.GetLogLevel();
            cartAgent.SetLogLevel(logtype);

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

            this.Log($"Calculated total resource cost for {recipe}: {resourceCost}");

            return resourceCost;
        }

        float GetProductRevenue(Recipe recipe) =>
            Locator.Market ? Locator.Market.GetPrice(recipe.Product) : recipe.Product.MeanPrice;

        float GetRecipeTime(Recipe recipe)
        {
            var recipeTime = recipe.Time;

            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                var resourceRecipe = Production.Registry.GetRecipe(resource);
                if (!resourceRecipe) continue;

                recipeTime += resourceRecipe.Time * amount;
            }

            this.Log($"Calculated recipe time for {recipe}: {recipeTime}");

            return recipeTime;
        }

        void Produce()
        {
            var recipe = BestRecipes.First();

            this.Log($"ProductionAgent starting production for {recipe}");
            TargetRecipe = recipe;

            var recipes = new List<Recipe>();
            var orders = new List<CartAgentComponent.ItemOrder>();

            CalculateProduction(recipe, recipes, orders);

            recipes.Reverse();
            RecipeQueue = new Queue<Recipe>(recipes);

            orders.Reverse();
            var orderQueue = new Queue<CartAgentComponent.ItemOrder>(orders);

            PlacePurchaseOrders(orderQueue);
            AssignNextRecipe();
        }

        void CalculateProduction(Recipe targetRecipe, List<Recipe> recipes, List<CartAgentComponent.ItemOrder> orders, int amount = 1)
        {
            this.Log($"ProductionAgent calculating production for {targetRecipe}");
            for (var i = 0; i < amount; i++)
            {
                recipes.Add(targetRecipe);
            }

            foreach (var pair in targetRecipe.Resources)
            {
                var resource = pair.Key;
                var resourceAmount = pair.Value;

                var resourceRecipe = Production.Registry.GetRecipe(resource);

                var supply = Production.ResourceInventory.GetSupply(resource);
                var neededAmount = resourceAmount * amount * ResourceBuffer - supply;

                if (neededAmount <= 0)
                {
                    this.Log($"ProductionAgent skipping order for {resource}: Needed amount already satisfied");
                    continue;
                }

                if (!resourceRecipe)
                {
                    //kann gekauft werden

                    var itemOrder = new CartAgentComponent.ItemOrder(resource, neededAmount);
                    orders.Add(itemOrder);
                    this.Log($"ProductionAgent calculated order for {neededAmount} {resource}");
                }
                else
                {
                    //kann produziert werden

                    this.Log($"ProductionAgent encountered producable resource {resource}");
                    CalculateProduction(resourceRecipe, recipes, orders, neededAmount);
                }
            }
        }

        void PlacePurchaseOrders(Queue<CartAgentComponent.ItemOrder> orders)
        {
            this.Log($"ProductionAgent placing purchase orders");

            var agentToOrders = new Dictionary<CartAgentComponent, List<CartAgentComponent.ItemOrder>>();

            foreach (var pair in CartToAgent)
            {
                if (orders.Count <= 0) break;

                var agent = pair.Value;

                this.Log($"ProductionAgent placing purchase order for {agent}");

                agentToOrders.Add(agent, new List<CartAgentComponent.ItemOrder>());

                for (var i = 0; i < pair.Key.Inventory.Slots; i++)
                {
                    if (orders.Count <= 0) break;

                    var order = orders.Dequeue();
                    agentToOrders[agent].Add(order);

                    this.Log($"ProductionAgent placed purchase order for {order.Amount} {order.Item}");
                }
            }

            if (orders.Count > 0)
            {
                foreach (var pair in CartToAgent)
                {
                    if (orders.Count <= 0) break;

                    var agent = pair.Value;
                    var order = orders.Dequeue();

                    this.Log($"ProductionAgent placing purchase order for {agent}");

                    agentToOrders[agent].Add(order);
                    this.Log($"ProductionAgent placed purchase order for {order.Amount} {order.Item}");
                }
            }

            foreach (var pair in CartToAgent)
            {
                var agent = pair.Value;
                if (!agentToOrders.ContainsKey(agent)) continue;
                agent.AddPurchaseOrders(agentToOrders[agent]);
            }
        }

        void AssignNextRecipe()
        {
            if (RecipeQueue.Count == 0)
            {
                ProductionFinished?.Invoke(this, EventArgs.Empty);
                return;
            }

            NextRecipe = RecipeQueue.Dequeue();

            this.Log($"ProductionAgent assigning next recipe {NextRecipe}");

            foreach (var employee in Company.Employees)
            {
                if (Production.Assignment.IsAssignable(employee))
                {
                    Production.Assignment.Assign(employee, NextRecipe);
                }
                else
                {
                    employee.CompanyReached += OnCompanyReached;
                }
            }
        }

        void OnCompanyReached(object sender, EventArgs e)
        {
            var employee = sender as EmployeeComponent;

            this.Log($"ProductionAgent assigning employee {employee} to next recipe {NextRecipe}");

            Production.Assignment.Assign(employee, NextRecipe);
        }

        public void OnRecipeFinished(object sender, ProductionComponent.ProductionEventArgs e)
        {
            this.Log("ProductionAgent finished recipe");
            AssignNextRecipe();
        }

        void OnProductionFinished(object sender, EventArgs e)
        {
            this.Log("ProductionAgent finished production queue");
            Produce();
        }

        public class InitializedEventArgs : EventArgs
        {
        }
    }
}