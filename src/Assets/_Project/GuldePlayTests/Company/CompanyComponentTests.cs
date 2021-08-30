using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gulde;
using Gulde.Builders;
using Gulde.Cities;
using Gulde.Company;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Inventory;
using Gulde.Production;
using Gulde.Timing;
using Gulde.Vehicles;
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
        float HiringCost { get; set; }
        bool EmployeeHiredFlag { get; set; }
        bool CartHiredFlag { get; set; }
        bool EmployeeArrivedFlag { get; set; }
        bool EmployeeLeftFlag { get; set; }
        bool CartArrivedFlag { get; set; }
        bool CartLeftFlag { get; set; }
        EmployeeComponent HiredEmployee { get; set; }
        CartComponent HiredCart { get; set; }
        EmployeeComponent ArrivedEmployee { get; set; }
        CartComponent ArrivedCart { get; set; }
        EmployeeComponent LeftEmployee { get; set; }
        CartComponent LeftCart { get; set; }

        WealthComponent Owner => PlayerObject.GetComponent<WealthComponent>();
        CityComponent City => CityObject.GetComponent<CityComponent>();
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
                .WithTimeSpeed(300)
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

            HiringCost = 0f;
            PaidWage = 0f;
            EmployeeHiredFlag = false;
            CartHiredFlag = false;
            EmployeeArrivedFlag = false;
            EmployeeLeftFlag = false;
            HiredEmployee = null;
            HiredCart = null;
            ArrivedEmployee = null;
            ArrivedCart = null;
            LeftEmployee = null;
            LeftCart = null;
        }

        [UnityTest]
        public IEnumerator ShouldHireEmployee()
        {
            CompanyBuilder = CompanyBuilder
                .WithEmployees(0)
                .WithEntryCell(0, 5);

            yield return CityBuilder
                .WithSize(20, 20)
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
            Assert.AreEqual(employee, HiredEmployee);
            Assert.True(Company.IsEmployed(employee));
            Assert.True(Company.Employees.Count > 0);
            Assert.IsNotNull(employee);
            Assert.AreEqual(Company.HiringCost, HiringCost);

            yield return time.WaitForMorning;

            Assert.False(EmployeeArrivedFlag);
            Assert.False(EmployeeLeftFlag);

            yield return employee.WaitForCompanyReached;

            Assert.False(EmployeeLeftFlag);
            Assert.True(EmployeeArrivedFlag);
            Assert.AreEqual(employee, ArrivedEmployee);

            yield return time.WaitForEvening;

            Assert.True(EmployeeLeftFlag);
            Assert.AreEqual(employee, LeftEmployee);
        }

        [UnityTest]
        public IEnumerator ShouldHireCart()
        {
            CompanyBuilder = CompanyBuilder.WithEmployees(0).WithCarts(0);
            yield return CityBuilder
                .WithSize(20, 20)
                .Build();

            Company.CartHired += OnCartHired;

            Company.HireCart();

            var cart = Company.Carts.ElementAt(0);

            Assert.False(CartArrivedFlag);
            Assert.False(CartLeftFlag);
            Assert.True(CartHiredFlag);
            Assert.AreEqual(Company.CartCost, HiringCost);
            Assert.AreEqual(cart, HiredCart);
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

            yield return time.WaitForWorkingHourTicked;

            yield return employee0.WaitForCompanyReached;
            yield return employee1.WaitForCompanyReached;
            yield return employee2.WaitForCompanyReached;

            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            Assignment.Assign(employee0, recipe);
            Assignment.Assign(employee1, externalRecipe);
            Assignment.Assign(employee2, recipe);

            Assert.AreEqual(3, Company.WorkingEmployees.Count);
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

            cart.Travel.TravelTo(Locator.Market.Location);

            Assert.True(Company.IsEmployed(cart));
            Assert.False(Company.IsAvailable(cart));
            Assert.True(CartLeftFlag);
            Assert.AreEqual(cart, LeftCart);
            Assert.False(CartArrivedFlag);

            CartLeftFlag = false;

            yield return cart.Travel.WaitForLocationReached;

            Assert.False(CartArrivedFlag);
            Assert.False(CartLeftFlag);

            cart.Travel.TravelTo(cart.Company.Location);

            yield return cart.Travel.WaitForLocationReached;

            Assert.True(CartArrivedFlag);
            Assert.AreEqual(cart, ArrivedCart);
            Assert.False(CartLeftFlag);
        }

        void OnWagePaid(object sender, CostEventArgs e)
        {
            PaidWage += e.Cost;
        }

        void OnEmployeeHired(object sender, HiringEventArgs e)
        {
            EmployeeHiredFlag = true;
            HiringCost = e.Cost;
            HiredEmployee = e.Entity.GetComponent<EmployeeComponent>();
        }

        void OnCartHired(object sender, HiringEventArgs e)
        {
            CartHiredFlag = true;
            HiringCost = e.Cost;
            HiredCart = e.Entity.GetComponent<CartComponent>();
        }

        void OnEmployeeArrived(object sender, EmployeeEventArgs e)
        {
            EmployeeArrivedFlag = true;
            ArrivedEmployee = e.Employee;
        }

        void OnEmployeeLeft(object sender, EmployeeEventArgs e)
        {
            EmployeeLeftFlag = true;
            LeftEmployee = e.Employee;
        }

        void OnCartArrived(object sender, CartEventArgs e)
        {
            CartArrivedFlag = true;
            ArrivedCart = e.Cart;
        }

        void OnCartLeft(object sender, CartEventArgs e)
        {
            CartLeftFlag = true;
            LeftCart = e.Cart;
        }
    }
}