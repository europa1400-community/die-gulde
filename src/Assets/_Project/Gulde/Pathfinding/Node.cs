using System;
using UnityEngine;

namespace Gulde.Pathfinding
{
    public class Node
    {

        public readonly Vector3Int Position;
        public Node Parent;
        public int CostG;
        public int CostH;
        public int CostF => CostG + CostH;

        public Node(Vector3Int position) => Position = position;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Node) obj);
        }

        bool Equals(Node other) => Position.Equals(other.Position);
        public override int GetHashCode() => Position.GetHashCode();
        public override string ToString() => Position.ToString();
    }
}