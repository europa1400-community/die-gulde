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
        public MapFactory(Map map, GameObject gameObject = null, GameObject parentObject = null) : base(map, gameObject, parentObject)
        {
        }

        public override MapComponent Create()
        {
            if (TypeObject.Naming?.Value)
            {
                var namingFactory = new NamingFactory(TypeObject.Naming.Value, GameObject);
                namingFactory.Create();
            }

            var entityRegistryFactory = new EntityRegistryFactory(TypeObject.EntityRegistry.Value, GameObject);
            var entityRegistryComponent = entityRegistryFactory.Create();

            Component.Size = TypeObject.Size.Value;
            Component.MapLayout = TypeObject.MapLayout.Value;

            var navFactory = new NavFactory(TypeObject.Nav.Value, GameObject);
            var navComponent = navFactory.Create();

            entityRegistryComponent.Registered += Component.OnEntityRegistered;
            entityRegistryComponent.Unregistered += Component.OnEntityUnregistered;

            CreateMarket(TypeObject);
            navComponent.CalculateNavMap();

            foreach (var workerHome in TypeObject.WorkerHomes)
            {
                var workerHomeFactory = new WorkerHomeFactory(workerHome.Value, parentObject: GameObject);
                workerHomeFactory.Create(Component);

                navComponent.CalculateNavMap();
            }

            foreach (var company in TypeObject.Companies)
            {
                var companyFactory = new CompanyFactory(company.Value, parentObject: GameObject);
                companyFactory.Create(Component);

                navComponent.CalculateNavMap();
            }

            return Component;
        }

        void CreateMarket(Map map)
        {
            var marketFactory = new MarketFactory(TypeObject.Market.Value, parentObject: GameObject);
            var marketObject = marketFactory.Create();
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