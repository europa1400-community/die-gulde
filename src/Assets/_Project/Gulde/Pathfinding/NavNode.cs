using System;
using UnityEngine;

namespace Gulde.Pathfinding
{
    public class NavNode
    {
        public readonly Vector3Int Position;
        public NavNode Parent;
        public int CostG;
        public int CostH;
        public int CostF => CostG + CostH;

        public NavNode(Vector3Int position) => Position = position;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((NavNode) obj);
        }

        bool Equals(NavNode other) => Position.Equals(other.Position);
        public override int GetHashCode() => Position.GetHashCode();
        public override string ToString() => Position.ToString();
    }
}