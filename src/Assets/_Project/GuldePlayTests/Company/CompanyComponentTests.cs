using System.Collections;
using System.Linq;
using Gulde.Builders;
using Gulde.Company;
using Gulde.Economy;
using Gulde.Inventory;
using Gulde.Production;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Company
{
    public class CompanyComponentTests
    {
        CompanyBuilder CompanyBuilder { get; set; }
        PlayerBuilder PlayerBuilder { get; set; }

        GameObject CompanyObject { get; set; }
        GameObject PlayerObject { get; set; }

        bool EmployeeHiredFlag { get; set; }
        bool CartHiredFlag { get; set; }
        bool EmployeeArrivedFlag { get; set; }
        bool EmployeeLeftFlag { get; set; }

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
            PlayerObject = PlayerBuilder.PlayerObject;

            CompanyBuilder = A.Company.WithOwner(Owner).WithSlots(5, 3).WithEmployees(1);
        }

        [UnityTearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(CompanyObject);
            Object.DestroyImmediate(PlayerObject);

            EmployeeHiredFlag = false;
            CartHiredFlag = false;
            EmployeeArrivedFlag = false;
            EmployeeLeftFlag = false;
        }

        [UnityTest]
        public IEnumerator ShouldHireEmployee()
        {
            yield return CompanyBuilder.WithEmployees(0).Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            Company.EmployeeHired += OnEmployeeHired;

            Company.HireEmployee();

            var employee = Company.Employees.ElementAt(0);

            Assert.True(EmployeeHiredFlag);
            Assert.True(Company.IsEmployed(employee));
            Assert.True(Company.Employees.Count > 0);
            Assert.IsNotNull(employee);
        }

        [UnityTest]
        public IEnumerator ShouldHireCart()
        {
            yield return CompanyBuilder.WithEmployees(0).WithCarts(0).Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            Company.CartHired += OnCartHired;

            Company.HireCart();

            var cart = Company.Carts.ElementAt(0);

            Assert.True(CartHiredFlag);
            Assert.True(Company.IsEmployed(cart));
            Assert.True(Company.Carts.Count > 0);
            Assert.NotNull(cart);
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
    }
}