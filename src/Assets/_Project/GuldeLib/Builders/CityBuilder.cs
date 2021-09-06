using System.Collections;
using System.Collections.Generic;
using GuldeLib.Cities;
using GuldeLib.Economy;
using GuldeLib.Maps;
using GuldeLib.Timing;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GuldeLib.Builders
{
    public class CityBuilder : Builder
    {
        public GameObject CityObject { get; private set; }

        [LoadAsset("prefab_city")]
        GameObject CityPrefab { get; set; }

        [LoadAsset("prefab_market")]
        GameObject MarketPrefab { get; set; }

        Vector2Int MapSize { get; set; } = new Vector2Int(10, 10);
        int WorkerHomeCount { get; set; }
        Vector3Int MarketPosition { get; set; }
        HashSet<Vector3Int> WorkerHomePositions { get; } = new HashSet<Vector3Int>();
        List<CompanyBuilder> CompaniesToBuild { get; } = new List<CompanyBuilder>();
        List<WorkerHomeBuilder> WorkerHomesToBuild { get; } = new List<WorkerHomeBuilder>();
        int Hour { get; set; }
        int Minute { get; set; }
        int Year { get; set; }
        int NormalTimeSpeed { get; set; } = 5;
        bool AutoAdvance { get; set; }

        public CityBuilder() : base()
        {
        }

        public CityBuilder WithSize(int x, int y)
        {
            MapSize = new Vector2Int(x, y);
            return this;
        }

        public CityBuilder WithWorkerHome(int x, int y)
        {
            WorkerHomePositions.Add(new Vector3Int(x, y, 0));
            return this;
        }

        public CityBuilder WithWorkerHome(WorkerHomeBuilder workerHomeBuilder)
        {
            WorkerHomesToBuild.Add(workerHomeBuilder);
            return this;
        }

        public CityBuilder WithWorkerHomes(int count)
        {
            WorkerHomeCount = count;
            return this;
        }

        public CityBuilder WithMarket(int x, int y)
        {
            MarketPosition = new Vector3Int(x, y, 0);
            return this;
        }

        public CityBuilder WithCompany(CompanyBuilder companyBuilder)
        {
            CompaniesToBuild.Add(companyBuilder);
            return this;
        }

        public CityBuilder WithoutCompanies()
        {
            CompaniesToBuild.Clear();
            return this;
        }

        public CityBuilder WithTime(int hour, int minute, int year)
        {
            Hour = hour;
            Minute = minute;
            Year = year;
            return this;
        }

        public CityBuilder WithNormalTimeSpeed(int normalTimeSpeed)
        {
            NormalTimeSpeed = normalTimeSpeed;
            return this;
        }

        public CityBuilder WithAutoAdvance(bool autoAdvance)
        {
            AutoAdvance = autoAdvance;
            return this;
        }

        public override IEnumerator Build()
        {
            if (MapSize.x <= 0 || MapSize.y <= 0)
            {
                this.Log($"City cannot be created with invalid size {MapSize}", LogType.Error);
                yield break;
            }

            yield return base.Build();

            CityObject = Object.Instantiate(CityPrefab);

            var city = CityObject.GetComponent<CityComponent>();
            var map = CityObject.GetComponent<MapComponent>();
            var time = CityObject.GetComponent<TimeComponent>();

            time.AutoAdvance = AutoAdvance;
            time.NormalTimeSpeed = NormalTimeSpeed;
            time.TimeSpeed = NormalTimeSpeed;
            time.SetTime(Minute, Hour, Year);

            map.SetSize(MapSize.x, MapSize.y);

            var workerHomeBuilder = new WorkerHomeBuilder().WithMap(map);

            for (var i = 0; i < WorkerHomeCount; i++)
            {
                var x = Random.Range(-map.Size.x / 2, map.Size.x / 2);
                var y = Random.Range(-map.Size.y / 2, map.Size.y / 2);

                this.Log($"Generating worker home at {x}, {y}.");

                yield return workerHomeBuilder
                    .WithEntryCell(x, y)
                    .Build();
            }

            foreach (var cell in WorkerHomePositions)
            {
                if (!cell.IsInBounds(map.Size))
                {
                    this.Log($"Worker Home at {cell.x}, {cell.y} is out of bounds", LogType.Error);
                    continue;
                }

                yield return workerHomeBuilder
                    .WithEntryCell(cell)
                    .Build();
            }

            if (MarketPosition.IsInBounds(map.Size))
            {
                var marketObject = Object.Instantiate(MarketPrefab, CityObject.transform);
                var market = marketObject.GetComponent<MarketComponent>();

                map.Register(market.Location);
                market.Location.EntryCell = MarketPosition;
            }

            foreach (var companyBuilder in CompaniesToBuild)
            {
                yield return companyBuilder.WithMap(map).Build();
            }

            foreach (var workerHome in WorkerHomesToBuild)
            {
                yield return workerHome.WithMap(map).Build();
            }
        }
    }
}