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
    /// <summary>
    /// Provides functions to build a city.
    /// </summary>
    public class CityBuilder : Builder
    {
        /// <summary>
        /// Gets the built city <see cref="GameObject">GameObject</see>.
        /// </summary>
        public GameObject CityObject { get; private set; }

        /// <summary>
        /// Gets the prefab used to build the city.
        /// </summary>
        [LoadAsset("prefab_city")]
        GameObject CityPrefab { get; set; }

        /// <summary>
        /// Gets or Sets the size of the map.
        /// </summary>
        Vector2Int MapSize { get; set; } = new Vector2Int(10, 10);

        /// <summary>
        /// Gets or Sets the <see cref="MarketBuilder">MarketBuilder</see>.
        /// </summary>
        MarketBuilder MarketBuilder { get; set; } = new MarketBuilder();

        /// <summary>
        /// Gets or Sets the amount of worker homes to be built.
        /// </summary>
        int WorkerHomeCount { get; set; }

        /// <summary>
        /// Gets the cell positions of the worker homes.
        /// </summary>
        HashSet<Vector3Int> WorkerHomePositions { get; } = new HashSet<Vector3Int>();

        /// <summary>
        /// Gets the list of <see cref="CompanyBuilder">CompanyBuilders</see>.
        /// </summary>
        List<CompanyBuilder> CompaniesToBuild { get; } = new List<CompanyBuilder>();

        /// <summary>
        /// Gets the list of <see cref="WorkerHomeBuilder">WorkerHomeBuilders</see>.
        /// </summary>
        List<WorkerHomeBuilder> WorkerHomesToBuild { get; } = new List<WorkerHomeBuilder>();

        /// <summary>
        /// Gets or sets the starting hour.
        /// </summary>
        int Hour { get; set; }

        /// <summary>
        /// Gets or sets the starting minute.
        /// </summary>
        int Minute { get; set; }

        /// <summary>
        /// Gets or sets the starting year.
        /// </summary>
        int Year { get; set; }

        /// <inheritdoc cref="TimeComponent.NormalTimeSpeed"/>
        int NormalTimeSpeed { get; set; } = 5;

        /// <inheritdoc cref="TimeComponent.AutoAdvance"/>
        bool AutoAdvance { get; set; }

        /// <summary>
        /// Sets the map's size.
        /// </summary>
        /// <param name="width">The width in cells.</param>
        /// <param name="height">The height in cells.</param>
        public CityBuilder WithSize(int width, int height)
        {
            MapSize = new Vector2Int(width, height);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="MarketBuilder">MarketBuilder</see>.
        /// </summary>
        public CityBuilder WithMarket(MarketBuilder marketBuilder)
        {
            MarketBuilder = marketBuilder;
            return this;
        }

        /// <summary>
        /// Requests a worker home to be built at the given position.
        /// </summary>
        /// <param name="x">The x coordinate in cells.</param>
        /// <param name="y">The y coordinate in cells.</param>
        public CityBuilder WithWorkerHome(int x, int y)
        {
            WorkerHomePositions.Add(new Vector3Int(x, y, 0));
            return this;
        }

        /// <summary>
        /// Requests a given worker home to be built.
        /// </summary>
        public CityBuilder WithWorkerHome(WorkerHomeBuilder workerHomeBuilder)
        {
            WorkerHomesToBuild.Add(workerHomeBuilder);
            return this;
        }

        /// <summary>
        /// Requests a given amount of worker homes to be built at random positions.
        /// </summary>
        public CityBuilder WithWorkerHomes(int count)
        {
            WorkerHomeCount = count;
            return this;
        }

        /// <summary>
        /// Requests a given company to be built.
        /// </summary>
        public CityBuilder WithCompany(CompanyBuilder companyBuilder)
        {
            CompaniesToBuild.Add(companyBuilder);
            return this;
        }

        /// <summary>
        /// Requests no companies to be built.
        /// Cancels all previous requests for companies.
        /// </summary>
        public CityBuilder WithoutCompanies()
        {
            CompaniesToBuild.Clear();
            return this;
        }

        /// <summary>
        /// Sets the game's starting time.
        /// </summary>
        /// <param name="hour">The starting hour.</param>
        /// <param name="minute">The starting minute.</param>
        /// <param name="year">The starting year.</param>
        public CityBuilder WithTime(int hour, int minute, int year)
        {
            Hour = hour;
            Minute = minute;
            Year = year;
            return this;
        }

        /// <summary>
        /// Sets the game's normal time speed.
        /// </summary>
        public CityBuilder WithNormalTimeSpeed(int normalTimeSpeed)
        {
            NormalTimeSpeed = normalTimeSpeed;
            return this;
        }

        /// <summary>
        /// Sets the game's auto advance.
        /// </summary>
        public CityBuilder WithAutoAdvance(bool autoAdvance)
        {
            AutoAdvance = autoAdvance;
            return this;
        }

        /// <inheritdoc cref="Builder.Build"/>
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

            yield return BuildMarket(map);

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

            foreach (var companyBuilder in CompaniesToBuild)
            {
                yield return companyBuilder.WithMap(map).Build();
            }

            foreach (var workerHome in WorkerHomesToBuild)
            {
                yield return workerHome.WithMap(map).Build();
            }
        }

        /// <summary>
        /// Builds the market.
        /// </summary>
        /// <param name="map">The map to build the market on.</param>
        IEnumerator BuildMarket(MapComponent map)
        {
            if (MarketBuilder == null) yield break;

            if (!MarketBuilder.EntryCell.IsInBounds(MapSize))
            {
                this.Log($"Market is out of bounds.", LogType.Warning);

                yield break;
            }

            yield return MarketBuilder.Build();

            var market = MarketBuilder.MarketObject.GetComponent<MarketComponent>();
            map.Register(market.Location);
        }
    }
}