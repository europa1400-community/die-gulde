using System.Collections;
using System.Collections.Generic;
using Gulde.Cities;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Maps;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Gulde.Builders
{
    public class CityBuilder : Builder
    {
        public GameObject CityObject { get; private set; }

        [LoadAsset("prefab_city")]
        GameObject CityPrefab { get; set; }

        Vector2Int MapSize { get; set; }
        int WorkerHomeCount { get; set; }
        Vector3Int MarketPosition { get; set; }
        HashSet<Vector3Int> WorkerHomePositions { get; set; } = new HashSet<Vector3Int>();

        public CityBuilder() : base()
        {

        }

        public CityBuilder WithSize(int x, int y)
        {
            MapSize = new Vector2Int(x, y);

            return this;
        }

        public CityBuilder WithWorkerHomes(int count)
        {
            WorkerHomeCount = count;

            return this;
        }

        public CityBuilder WithWorkerHome(int x, int y)
        {
            WorkerHomePositions.Add(new Vector3Int(x, y, 0));

            return this;
        }

        public CityBuilder WithMarket(int x, int y)
        {
            MarketPosition = new Vector3Int(x, y, 0);

            return this;
        }

        public override IEnumerator Build()
        {
            yield return base.Build();

            var cityObject = Object.Instantiate(CityPrefab);

            var city = cityObject.GetComponent<CityComponent>();
            var map = cityObject.GetComponent<MapComponent>();

            var workerHomePrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/prefab_worker_home.prefab");

            for (var i = 0; i < WorkerHomeCount; i++)
            {
                var x = Random.Range(-map.Size.x / 2, map.Size.x / 2);
                var y = Random.Range(-map.Size.y / 2, map.Size.y / 2);
                var workerHomeObject = Object.Instantiate(workerHomePrefab, cityObject.transform);
                var workerHome = workerHomeObject.GetComponent<WorkerHomeComponent>();
                workerHome.Location.EntryCell = new Vector3Int(x, y, 0);
            }

            foreach (var cell in WorkerHomePositions)
            {
                var workerHomeObject = Object.Instantiate(workerHomePrefab, cityObject.transform);
                var workerHome = workerHomeObject.GetComponent<WorkerHomeComponent>();
                workerHome.Location.EntryCell = cell;
            }

            var marketPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project_Prefabs_prefab_market.prefab");
            var marketObject = Object.Instantiate(marketPrefab, cityObject.transform);
            var market = marketObject.GetComponent<MarketComponent>();
            market.Location.EntryCell = MarketPosition;
        }
    }
}