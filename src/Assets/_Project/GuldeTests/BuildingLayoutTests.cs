using System.Collections.Generic;
using Gulde;
using Gulde.Buildings;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuldeTests
{
    public class BuildingLayoutTests
    {
        [Test]
        public void ShouldOrientateLayoutUpwards()
        {
            var dummyTile = ScriptableObject.CreateInstance<Tile>();

            var layout = ScriptableObject.CreateInstance<BuildingLayout>();

            layout._tiles = new Dictionary<Vector3Int, TileBase>
            {
                { new Vector3Int(0, 0, 0), dummyTile },
                { new Vector3Int(1, 0, 0), dummyTile },
                { new Vector3Int(0, 1, 0), dummyTile },
            };

            layout._entrancePosition = new Vector3Int(1, 0, 0);

            var expectedTiles = new Dictionary<Vector3Int, TileBase>
            {
                {new Vector3Int(0, 0, 0), dummyTile},
                {new Vector3Int(-1, 0, 0), dummyTile},
                {new Vector3Int(0, 1, 0), dummyTile},
            };

            var expectedEntrancePosition = new Vector3Int(0, 1, 0);

            var actualLayout = layout.Orientate(Orientation.Up);

            CollectionAssert.AreEquivalent(expectedTiles, actualLayout._tiles);
            Assert.AreEqual(expectedEntrancePosition, actualLayout._entrancePosition);
        }

        [Test]
        public void ShouldOrientateLayoutDownwards()
        {
            var dummyTile = ScriptableObject.CreateInstance<Tile>();

            var layout = ScriptableObject.CreateInstance<BuildingLayout>();

            layout._tiles = new Dictionary<Vector3Int, TileBase>
            {
                {new Vector3Int(0, 0, 0), dummyTile},
                {new Vector3Int(1, 0, 0), dummyTile},
                {new Vector3Int(0, 1, 0), dummyTile},
            };

            layout._entrancePosition = new Vector3Int(1, 0, 0);

            var expectedTiles = new Dictionary<Vector3Int, TileBase>
            {
                {new Vector3Int(0, 0, 0), dummyTile},
                {new Vector3Int(1, 0, 0), dummyTile},
                {new Vector3Int(0, -1, 0), dummyTile},
            };

            var expectedEntrancePosition = new Vector3Int(0, -1, 0);

            var actualLayout = layout.Orientate(Orientation.Down);

            CollectionAssert.AreEquivalent(expectedTiles, actualLayout._tiles);
            Assert.AreEqual(expectedEntrancePosition, actualLayout._entrancePosition);
        }

        [Test]
        public void ShouldOrientateLayoutLeftwards()
        {
            var dummyTile = ScriptableObject.CreateInstance<Tile>();

            var layout = ScriptableObject.CreateInstance<BuildingLayout>();

            layout._tiles = new Dictionary<Vector3Int, TileBase>
            {
                {new Vector3Int(0, 0, 0), dummyTile},
                {new Vector3Int(1, 0, 0), dummyTile},
                {new Vector3Int(0, 1, 0), dummyTile},
            };

            layout._entrancePosition = new Vector3Int(1, 0, 0);

            var expectedTiles = new Dictionary<Vector3Int, TileBase>
            {
                {new Vector3Int(0, 0, 0), dummyTile},
                {new Vector3Int(-1, 0, 0), dummyTile},
                {new Vector3Int(0, -1, 0), dummyTile},
            };

            var expectedEntrancePosition = new Vector3Int(-1, 0, 0);

            var actualLayout = layout.Orientate(Orientation.Left);

            CollectionAssert.AreEquivalent(expectedTiles, actualLayout._tiles);
            Assert.AreEqual(expectedEntrancePosition, actualLayout._entrancePosition);
        }


        [Test]
        public void ShouldOrientateComplexLayout()
        {
            var dummyTile = ScriptableObject.CreateInstance<Tile>();

            var layout = ScriptableObject.CreateInstance<BuildingLayout>();

            layout._tiles = new Dictionary<Vector3Int, TileBase>
            {
                {new Vector3Int(0, 0, 0), dummyTile},
                {new Vector3Int(1, 0, 0), dummyTile},
                {new Vector3Int(0, 1, 0), dummyTile},
                {new Vector3Int(1, 1, 0), dummyTile},
                {new Vector3Int(2, 0, 0), dummyTile},
            };

            layout._entrancePosition = new Vector3Int(1, 1, 0);

            var expectedTiles = new Dictionary<Vector3Int, TileBase>
            {
                {new Vector3Int(0, 0, 0), dummyTile},
                {new Vector3Int(0, 1, 0), dummyTile},
                {new Vector3Int(0, 2, 0), dummyTile},
                {new Vector3Int(-1, 0, 0), dummyTile},
                {new Vector3Int(-1, 1, 0), dummyTile},
            };

            var expectedEntrancePosition = new Vector3Int(-1, 1, 0);

            var actualLayout = layout.Orientate(Orientation.Up);

            CollectionAssert.AreEquivalent(expectedTiles, actualLayout._tiles);
            Assert.AreEqual(expectedEntrancePosition, actualLayout._entrancePosition);
        }
    }
}