using System.Collections;
using System.Linq;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.Economy;
using GuldeLib.Inventories;
using GuldeLib.Producing;
using GuldeLib.Timing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Company
{
    public class CompanyComponentTests
    {
        GameBuilder GameBuilder { get; set; }
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
                .WithSize(20, 20)
                .WithTime(7, 00, 1400)
                .WithNormalTimeSpeed(300)
                .WithCompany(CompanyBuilder)
                .WithWorkerHome(5, 0)
                .WithAutoAdvance(true);

            GameBuilder = A.Game
                .WithTimeScale(10f)
                .WithCity(CityBuilder);
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

            yield return GameBuilder.Build();

            var time = CityObject.GetComponent<TimeComponent>();

            Company.EmployeeHired += OnEmployeeHired;
            Company.EmployeeArrived += OnEmployeeArrived;
            Company.EmployeeLeft += OnEmployeeLeft;

            yield return Company.HireEmployee();

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
            Assert.True(Company.IsAvailable(employee));
            Assert.AreEqual(employee, ArrivedEmployee);

            yield return time.WaitForEvening;

            Assert.True(EmployeeLeftFlag);
            Assert.AreEqual(employee, LeftEmployee);
        }

        [UnityTest]
        public IEnumerator ShouldHireCart()
        {
            CompanyBuilder = CompanyBuilder.WithEmployees(0).WithCarts(0);
            yield return GameBuilder.Build();

            Company.CartHired += OnCartHired;

            yield return Company.HireCart();

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
        public IEnumerator ShouldBillWagesDuringWorktime()
        {
            CompanyBuilder = CompanyBuilder
                .WithEmployees(3)
                .WithWagePerHour(100f)
                .WithEntryCell(0, 5);

            CityBuilder = CityBuilder.WithTime(10, 55, 1400);

            yield return GameBuilder.Build();

            Company.WagePaid += OnWagePaid;

            var time = CityObject.GetComponent<TimeComponent>();

            PaidWage = 0;
            yield return time.WaitForWorkingHourTicked;

            Assert.AreEqual(3, Company.Employees.Count);
            Assert.AreEqual(Company.WagePerHour * 3, PaidWage);
        }

        [UnityTest]
        public IEnumerator ShouldRegisterAndUnregisterCarts()
        {
            CompanyBuilder = CompanyBuilder.WithCarts(0);
            yield return GameBuilder.Build();

            Company.CartHired += OnCartHired;
            Company.CartArrived += OnCartArrived;
            Company.CartLeft += OnCartLeft;

            yield return Company.HireCart();

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

            yield return cart.Travel.WaitForDestinationReached;

            Assert.False(CartArrivedFlag);
            Assert.False(CartLeftFlag);

            cart.Travel.TravelTo(cart.Company.Location);

            yield return cart.Travel.WaitForDestinationReached;

            Assert.True(CartArrivedFlag);
            Assert.AreEqual(cart, ArrivedCart);
            Assert.False(CartLeftFlag);
        }

        void OnWagePaid(object sender, WagePaidEventArgs e)
        {
            PaidWage += e.Cost;
        }

        void OnEmployeeHired(object sender, EmployeeHiredEventArgs e)
        {
            EmployeeHiredFlag = true;
            HiringCost = e.Cost;
            HiredEmployee = e.Employee;
        }

        void OnCartHired(object sender, CartHiredEventArgs e)
        {
            CartHiredFlag = true;
            HiringCost = e.Cost;
            HiredCart = e.Cart;
        }

        void OnEmployeeArrived(object sender, EmployeeArrivedEventArgs e)
        {
            EmployeeArrivedFlag = true;
            ArrivedEmployee = e.Employee;
        }

        void OnEmployeeLeft(object sender, EmployeeLeftEventArgs e)
        {
            EmployeeLeftFlag = true;
            LeftEmployee = e.Employee;
        }

        void OnCartArrived(object sender, CartArrivedEventArgs e)
        {
            CartArrivedFlag = true;
            ArrivedCart = e.Cart;
        }

        void OnCartLeft(object sender, CartLeftEventArgs e)
        {
            CartLeftFlag = true;
            LeftCart = e.Cart;
        }
    }
}