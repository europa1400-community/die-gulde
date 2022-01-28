using UnityEngine;

namespace GuldeLib.Pathfinding
{
    public class NavNode
    {
        public Vector2Int Position { get; }
        public NavNode Parent { get; set; }
        public int CostG { get; set; }
        public int CostH { get; set; }
        public int CostF => CostG + CostH;

        public NavNode(Vector2Int position) => Position = position;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((NavNode) obj);
        }

        bool Equals(NavNode other) => Position.Equals(other.Position);
        public override int GetHashCode() => Position.GetHashCode();
    }
}