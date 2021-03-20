using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gulde.Buildings
{
    [Serializable]
    public class BuildSpace
    {
        [SerializeField] int _size;
        [SerializeField] List<Vector3Int> _cellPositions;

        public BuildSpace(int size, List<Vector3Int> cellPositions) => (_size, _cellPositions) = (size, cellPositions);

        public bool HasTile(Vector3Int cellPosition) => _cellPositions.Contains(cellPosition);
    }
}