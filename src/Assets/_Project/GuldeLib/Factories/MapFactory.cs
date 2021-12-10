using GuldeLib.Cities;
using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Maps.Buildings;
using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class MapFactory : Factory<Map, MapComponent>
    {
        public MapFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override MapComponent Create(Map map)
        {
            if (map.Naming?.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(map.Naming.Value);
            }

            var entityRegistryFactory = new EntityRegistryFactory(GameObject);
            var entityRegistryComponent = entityRegistryFactory.Create(map.EntityRegistry.Value);

            Component.Size = map.Size.Value;
            Component.MapLayout = map.MapLayout.Value;

            var navFactory = new NavFactory(GameObject);
            var navComponent = navFactory.Create(map.Nav.Value);

            entityRegistryComponent.Registered += Component.OnEntityRegistered;
            entityRegistryComponent.Unregistered += Component.OnEntityUnregistered;

            CreateMarket(map);
            navComponent.CalculateNavMap();

            foreach (var workerHome in map.WorkerHomes)
            {
                var workerHomeFactory = new WorkerHomeFactory(parentObject: GameObject);
                workerHomeFactory.Create(workerHome.Value, Component);

                navComponent.CalculateNavMap();
            }

            foreach (var company in map.Companies)
            {
                var companyFactory = new CompanyFactory(parentObject: GameObject);
                companyFactory.Create(company.Value, Component);

                navComponent.CalculateNavMap();
            }

            return Component;
        }

        void CreateMarket(Map map)
        {
            var marketFactory = new MarketFactory(parentObject: GameObject);
            var marketObject = marketFactory.Create(map.Market.Value);
            var marketLocationComponent = marketObject.GetComponent<LocationComponent>();
            var marketBuildingComponent = marketObject.GetComponent<BuildingComponent>();

            var marketEntryCell = Component.MapLayout.PlaceBuilding(marketBuildingComponent.Building);

            if (!marketEntryCell.HasValue)
            {
                this.Log($"Could not create market: No build space available.", LogType.Warning);
                return;
            }

            Component.Register(marketLocationComponent);
            marketLocationComponent.EntryCell = marketEntryCell.Value;
        }
    }
}