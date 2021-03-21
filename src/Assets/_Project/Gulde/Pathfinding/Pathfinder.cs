using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gulde.Pathfinding
{
    public static class Pathfinder
    {
        public static List<Vector3Int> FindPath(Vector3Int startPosition, Vector3Int endPosition, List<Vector3Int> positions)
        {
            var nodes = (from position in positions select new Node(position)).ToList();

            var startNode = nodes.Find(node => node.Position == startPosition);
            var endNode = nodes.Find(node => node.Position == endPosition);

            var openNodes = new List<Node>();
            var closedNodes = new List<Node>();

            var currentNode = startNode;

            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                currentNode = FindLowestCostNode(startNode, openNodes);

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                if (currentNode.Equals(endNode)) return RetracePath(startNode, endNode);

                foreach (var neighbour in GetNeighbours(currentNode, nodes))
                {
                    if (closedNodes.Contains(neighbour)) continue;

                    var newCost = currentNode.CostG + GetDistance(currentNode, neighbour);

                    if (newCost < neighbour.CostG || !openNodes.Contains(neighbour))
                    {
                        neighbour.CostG = newCost;
                        neighbour.CostH = GetDistance(neighbour, endNode);
                        neighbour.Parent = currentNode;

                        if (!openNodes.Contains(neighbour)) openNodes.Add(neighbour);
                    }
                }
            }

            return null;
        }

        public static List<Vector3Int> GetDirections(Vector3Int from, List<Vector3Int> waypoints)
        {
            var directions = new List<Vector3Int>();
            var current = from;

            foreach (var waypoint in waypoints)
            {
                var direction = waypoint - current;
                directions.Add(direction);
                current += direction;
            }

            return directions;
        }

        public static List<Vector3Int> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Vector3Int>();
            var currentNode = endNode;

            while (!currentNode.Equals(startNode))
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();

            return path;
        }

        public static Node FindLowestCostNode(Node startNode, IEnumerable<Node> nodes)
        {
            var nodeList = nodes.ToList();
            var lowestCostNode = nodeList[0];

            foreach (var node in nodeList)
            {
                if (node.CostF < lowestCostNode.CostF ||
                    node.CostF == lowestCostNode.CostF && node.CostH < lowestCostNode.CostH)
                {
                    lowestCostNode = node;
                }
            }

            return lowestCostNode;
        }

        public static int GetDistance(Node from, Node to)
        {
            var connection = new Vector3Int(
                Mathf.Abs(to.Position.x - from.Position.x),
                Mathf.Abs(to.Position.y - from.Position.y), 0);

            var isLargerX = Mathf.Abs(connection.x) > Mathf.Abs(connection.y);

            var larger = isLargerX ? connection.x : connection.y;
            var smaller = isLargerX ? connection.y : connection.x;

            var distance = (larger - smaller) * 10 + smaller * 14;

            return distance;
        }

        public static List<Node> GetNeighbours(Node from, List<Node> nodes)
        {
            var neighbours = new List<Node>();

            var right = nodes.Find(node => node.Position == from.Position + Vector3Int.right);
            if (nodes.Contains(right)) neighbours.Add(right);
            var left = nodes.Find(node => node.Position == from.Position + Vector3Int.left);
            if (nodes.Contains(left)) neighbours.Add(left);
            var up = nodes.Find(node => node.Position == from.Position + Vector3Int.up);
            if (nodes.Contains(up)) neighbours.Add(up);
            var down = nodes.Find(node => node.Position == from.Position + Vector3Int.down);
            if (nodes.Contains(down)) neighbours.Add(down);
            var upRight = nodes.Find(node => node.Position == from.Position + Vector3Int.right + Vector3Int.up);
            if (nodes.Contains(upRight)) neighbours.Add(upRight);
            var downRight = nodes.Find(node => node.Position == from.Position + Vector3Int.right + Vector3Int.down);
            if (nodes.Contains(downRight)) neighbours.Add(downRight);
            var upLeft = nodes.Find(node => node.Position == from.Position + Vector3Int.left + Vector3Int.up);
            if (nodes.Contains(upLeft)) neighbours.Add(upLeft);
            var downLeft = nodes.Find(node => node.Position == from.Position + Vector3Int.left + Vector3Int.down);
            if (nodes.Contains(downLeft)) neighbours.Add(downLeft);

            return neighbours;
        }
    }
}
