// using System.Collections;
// using GuldeLib.Builders;
// using GuldeLib.Cities;
// using GuldeLib.Companies;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// namespace GuldePlayTests.Company
// {
//     public class MasterComponentTests
//     {
//         GameBuilder GameBuilder { get; set; }
//         CityBuilder CityBuilder { get; set; }
//         CompanyBuilder CompanyBuilder { get; set; }
//
//         GameObject CompanyObject => CompanyBuilder.CompanyObject;
//         CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
//         MasterComponent Master => Company.GetComponent<MasterComponent>();
//
//         [UnitySetUp]
//         public IEnumerator Setup()
//         {
//             CompanyBuilder = A.Company
//                 .WithCarts(1)
//                 .WithEmployees(1)
//                 .WithEntryCell(3, 3)
//                 .WithMaster()
//                 .WithWagePerHour(10)
//                 .WithSlots(2, 2);
//
//             CityBuilder = A.City
//                 .WithCompany(CompanyBuilder)
//                 .WithSize(20, 20)
//                 .WithNormalTimeSpeed(60)
//                 .WithAutoAdvance(true)
//                 .WithWorkerHomes(1);
//
//             GameBuilder = A.Game
//                 .WithCity(CityBuilder)
//                 .WithTimeScale(10f);
//
//             yield return GameBuilder.Build();
//         }
//
//         [TearDown]
//         public void Teardown()
//         {
//             foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
//             {
//                 Object.DestroyImmediate(gameObject);
//             }
//         }
//
//         [Test]
//         public void ShouldInitializeWithZero()
//         {
//             Assert.AreEqual(0, Master.Autonomy);
//             Assert.AreEqual(0, Master.Investivity);
//             Assert.AreEqual(0, Master.Riskiness);
//         }
//     }
// }