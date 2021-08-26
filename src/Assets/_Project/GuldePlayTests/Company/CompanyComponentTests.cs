using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gulde;
using Gulde.Builders;
using Gulde.Company;
using Gulde.Economy;
using Gulde.Inventory;
using Gulde.Production;
using Gulde.Timing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Company
{
    public class CompanyComponentTests
    {
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }
        PlayerBuilder PlayerBuilder { get; set; }


        GameObject CityObject => CityBuilder.CityObject;
        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        GameObject PlayerObject => PlayerBuilder.PlayerObject;

        float PaidWage { get; set; }
        bool EmployeeHiredFlag { get; set; }
        bool CartHiredFlag { get; set; }
        bool EmployeeArrivedFlag { get; set; }
        bool EmployeeLeftFlag { get; set; }
        bool CartArrivedFlag { get; set; }
        bool CartLeftFlag { get; set; }

        WealthComponent Owner => PlayerObject.GetComponent<WealthComponent>();
        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
        AssignmentComponent Assignment => CompanyObject.GetComponent<AssignmentComponent>();
        ProductionComponent Production => CompanyObject.GetComponent<ProductionComponent>();
        ProductionRegistryComponent ProductionRegistry => CompanyObject.GetComponent<ProductionRegistryComponent>();
        InventoryComponent ResourceInventory => CompanyObject.GetComponents<InventoryComponent>()[0];
        InventoryComponent ProductionInventory => CompanyObject.GetComponents<InventoryComponent>()[1];

        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayerBuilder = A.Player;
            yield return PlayerBuilder.Build();

            CompanyBuilder = A.Company
                .WithOwner(Owner)
                .WithSlots(5, 3)
                .WithEmployees(1)
                .WithCarts(1);
            CityBuilder = A.City
                .WithTime(7, 00, 1400)
                .WithTimeSpeed(60)
                .WithCompany(CompanyBuilder)
                .WithWorkerHome(5, 0)
                .WithAutoAdvance(true);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            PaidWage = 0f;
            EmployeeHiredFlag = false;
            CartHiredFlag = false;
            EmployeeArrivedFlag = false;
            EmployeeLeftFlag = false;
        }

        [UnityTest]
        public IEnumerator ShouldHireEmployee()
        {
            CompanyBuilder = CompanyBuilder
                .WithEmployees(0)
                .WithEntryCell(0, 5);

            yield return CityBuilder
                .WithSize(10, 10)
                .WithTimeSpeed(120)
                .Build();

            var time = CityObject.GetComponent<TimeComponent>();

            Company.EmployeeHired += OnEmployeeHired;
            Company.EmployeeArrived += OnEmployeeArrived;
            Company.EmployeeLeft += OnEmployeeLeft;

            Company.HireEmployee();

            var employee = Company.Employees.ElementAt(0);

            Assert.False(EmployeeArrivedFlag);
            Assert.False(EmployeeLeftFlag);
            Assert.True(EmployeeHiredFlag);
            Assert.True(Company.IsEmployed(employee));
            Assert.True(Company.Employees.Count > 0);
            Assert.IsNotNull(employee);

            yield return time.WaitForYearTicked;

            Assert.True(EmployeeArrivedFlag);
            Assert.True(EmployeeLeftFlag);
        }

        [UnityTest]
        public IEnumerator ShouldHireCart()
        {
            yield return CompanyBuilder.WithEmployees(0).WithCarts(0).Build();

            Company.CartHired += OnCartHired;

            Company.HireCart();

            var cart = Company.Carts.ElementAt(0);

            Assert.True(CartHiredFlag);
            Assert.True(Company.IsEmployed(cart));
            Assert.True(Company.IsAvailable(cart));
            Assert.True(Company.Carts.Count > 0);
            Assert.NotNull(cart);
        }

        [UnityTest]
        public IEnumerator ShouldBillWagesWhenAvailable()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("product").Build();
            var externalRecipe = A.Recipe.WithExternality(true).WithResources(resources).WithProduct(product).Build();

            CompanyBuilder = CompanyBuilder
                .WithEmployees(3)
                .WithWagePerHour(100f)
                .WithRecipe(externalRecipe)
                .WithEntryCell(0, 5);

            yield return CityBuilder
                .WithSize(10, 10)
                .WithTime(10, 55, 1400)
                .Build();

            Company.WagePaid += OnWagePaid;

            var time = CityObject.GetComponent<TimeComponent>();

            var employee0 = Company.Employees.ElementAt(0);
            var employee1 = Company.Employees.ElementAt(1);
            var employee2 = Company.Employees.ElementAt(2);

            yield return employee0.WaitForCompanyReached;
            yield return employee1.WaitForCompanyReached;
            yield return employee2.WaitForCompanyReached;

            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            Assignment.Assign(employee0, recipe);
            Assignment.Assign(employee1, externalRecipe);
            Assignment.Assign(employee2, recipe);

            Assert.AreEqual(3, Company.WorkingEmployees.Count);

            yield return time.WaitForWorkingHourTicked;

            Assert.AreEqual(Company.WagePerHour * 3, PaidWage);
        }

        [UnityTest]
        public IEnumerator ShouldRegisterAndUnregisterCarts()
        {
            CompanyBuilder = CompanyBuilder.WithCarts(0);
            yield return CityBuilder.Build();

            Company.CartHired += OnCartHired;
            Company.CartArrived += OnCartArrived;
            Company.CartLeft += OnCartLeft;

            Company.HireCart();

            var cart = Company.Carts.ElementAt(0);

            Assert.True(Company.IsEmployed(cart));
            Assert.True(Company.IsAvailable(cart));
            Assert.True(CartHiredFlag);
            Assert.False(CartArrivedFlag);
            Assert.False(CartLeftFlag);

            Debug.Log("Cart is in the company now");

            cart.Travel.TravelTo(Locator.Market.Location);

            Assert.True(Company.IsEmployed(cart));
            Assert.False(Company.IsAvailable(cart));
            Assert.True(CartLeftFlag);
            Assert.False(CartArrivedFlag);

            Debug.Log("Cart is leaving");

            CartLeftFlag = false;

            yield return cart.Travel.WaitForLocationReached;

            Debug.Log("Cart is in the market now");

            Assert.False(CartArrivedFlag);
            Assert.False(CartLeftFlag);

            cart.Travel.TravelTo(cart.Company.Location);

            Debug.Log("Cart is coming back");

            yield return cart.Travel.WaitForLocationReached;

            Debug.Log("Cart is in the company again");

            Assert.True(CartArrivedFlag);
            Assert.False(CartLeftFlag);
        }

        void OnWagePaid(object sender, CostEventArgs e)
        {
            PaidWage += e.Cost;
        }

        void OnEmployeeHired(object sender, HiringEventArgs e)
        {
            EmployeeHiredFlag = true;
        }

        void OnCartHired(object sender, HiringEventArgs e)
        {
            CartHiredFlag = true;
        }

        void OnEmployeeArrived(object sender, EmployeeEventArgs e)
        {
            EmployeeArrivedFlag = true;
        }

        void OnEmployeeLeft(object sender, EmployeeEventArgs e)
        {
            EmployeeLeftFlag = true;
        }

        void OnCartArrived(object sender, CartEventArgs e)
        {
            CartArrivedFlag = true;
        }

        void OnCartLeft(object sender, CartEventArgs e)
        {
            CartLeftFlag = true;
        }
    }
}