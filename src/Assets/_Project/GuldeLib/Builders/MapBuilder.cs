using System.Collections.Generic;
using GuldeLib.Entities;
using GuldeLib.Generators;
using GuldeLib.Maps;
using GuldeLib.Names;
using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class MapBuilder : Builder<Map>
    {
        public MapBuilder WithSize(GeneratableVector2Int size)
        {
            Object.Size = size;
            return this;
        }

        public MapBuilder WithSize(int x, int y)
        {
            Object.Size = new GeneratableVector2Int
            {
                Value = new Vector2Int(x, y)
            };
            return this;
        }

        public MapBuilder WithNaming(GeneratableNaming naming)
        {
            Object.Naming = naming;
            return this;
        }

        public MapBuilder WithNav(GeneratableNav nav)
        {
            Object.Nav = nav;
            return this;
        }

        public MapBuilder WithEntityRegistry(GeneratableEntityRegistry entityRegistry)
        {
            Object.EntityRegistry = entityRegistry;
            return this;
        }

        public MapBuilder WithMarket(GeneratableMarket market)
        {
            Object.Market = market;
            return this;
        }

        public MapBuilder WithWorkerHomes(List<GeneratableWorkerHome> workerHomes)
        {
            Object.WorkerHomes = workerHomes;
            return this;
        }

        public MapBuilder WithCompanies(List<GeneratableCompany> companies)
        {
            Object.Companies = companies;
            return this;
        }
    }
}