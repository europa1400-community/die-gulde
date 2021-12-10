using System.Collections.Generic;
using System.Linq;
using GuldeLib.Maps;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    public static class Path
    {
        public static Queue<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition,
            MapComponent map) =>
            map ? FindPath(startPosition, endPosition, map.Nav) : null;

        /// <summary>
        /// Calculates a traversable path from the start cell to the end cell.
        /// </summary>
        /// <param name="startPosition">The cell position to start the path from.</param>
        /// <param name="endPosition">The cell position to end the path at.</param>
        /// <param name="nav"></param>
        /// <returns>A list of cell positions to be traversed.</returns>
        public static Queue<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition, NavComponent nav)
        {
            if (!nav)
            {
                MonoLogger.Runtime.MonoLogger.Log("Pathfinder could not find path because nav was invalid.", LogType.Error);
                return new Queue<Vector2Int>();
            }

            if (nav.NavMap == null || nav.NavMap.Count == 0)
            {
                MonoLogger.Runtime.MonoLogger.Log("Pathfinder could not find path because nav map was invalid.", LogType.Error);
                return new Queue<Vector2Int>();
            }

            var navMap = new Dictionary<Vector2Int, NavNode>();

            foreach (var position in nav.NavMap)
            {
                var navNode = new NavNode(position);

                navMap.Add(position, navNode);
            }

            navMap.TryGetValue(startPosition, out var startNode);
            navMap.TryGetValue(endPosition, out var endNode);

            if (startNode == null || endNode == null)
            {
                MonoLogger.Runtime.MonoLogger.Log("Pathfinder could not find path because start or end position were not contained in nav map.", LogType.Error);
                return new Queue<Vector2Int>();
            }

            var openNodes = new HashSet<NavNode>();
            var closedNodes = new HashSet<NavNode>();

            var currentNode = startNode;

            openNodes.Add(currentNode);

            while (openNodes.Count > 0)
            {
                currentNode = FindLowestCostNode(openNodes);

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                if (currentNode.Equals(endNode)) break;

                foreach (var neighbour in GetNeighbours(currentNode, navMap))
                {
                    if (closedNodes.Contains(neighbour)) continue;

                    var newCostG = currentNode.CostG + GetDistance(currentNode, neighbour);

                    if (newCostG < neighbour.CostG || !openNodes.Contains(neighbour))
                    {
                        neighbour.CostG = newCostG;
                        neighbour.CostH = GetDistance(neighbour, endNode);
                        neighbour.Parent = currentNode;

                        if (!openNodes.Contains(neighbour)) openNodes.Add(neighbour);
                    }
                }
            }

            return RetracePath(startNode, endNode);
        }

        static Queue<Vector2Int> RetracePath(NavNode startNavNode, NavNode endNavNode)
        {
            MonoLogger.Runtime.MonoLogger.Log($"Path - Retracing path from start node {startNavNode?.Position} to {endNavNode?.Position}.");

            var path = new List<Vector2Int>();
            var currentNode = endNavNode;

            while (!currentNode?.Equals(startNavNode) ?? false)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();

            var queue = new Queue<Vector2Int>(path);

            return queue;
        }

        /// <summary>
        /// Finds the node with the lowest total cost value.
        /// </summary>
        /// <param name="nodes">The list of nodes to search.</param>
        /// <returns>The node with lowest total cost value.</returns>
        static NavNode FindLowestCostNode(IEnumerable<NavNode> nodes)
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

        /// <summary>
        /// Calculates the distance cost between to nodes.
        /// </summary>
        /// <param name="from">The node to start from.</param>
        /// <param name="to">The node to end at.</param>
        /// <returns>The distance cost value.</returns>
        static int GetDistance(NavNode from, NavNode to)
        {
            var connection = new Vector2Int(
                Mathf.Abs(to.Position.x - from.Position.x),
                Mathf.Abs(to.Position.y - from.Position.y));

            var isLargerX = Mathf.Abs(connection.x) > Mathf.Abs(connection.y);

            var larger = isLargerX ? connection.x : connection.y;
            var smaller = isLargerX ? connection.y : connection.x;

            var distance = (larger - smaller) * 10 + smaller * 14;

            return distance;
        }

        /// <summary>
        /// Finds the nodes traversable neighbour nodes.
        /// </summary>
        /// <param name="from">The node to search.</param>
        /// <param name="nodes">A list of traversable nodes.</param>
        /// <returns>The list of neighbour nodes.</returns>
        static List<NavNode> GetNeighbours(NavNode from, Dictionary<Vector2Int, NavNode> nodes)
        {
            var neighbours = new List<NavNode>();
            var walkableNeighbours = new List<NavNode>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    nodes.TryGetValue(from.Position + new Vector2Int(x, y), out var neighbour);
                    neighbours.Add(neighbour);

                    if (x == 0 && y == 0) continue;

                    if (neighbour != null) walkableNeighbours.Add(neighbour);
                }
            }

            for (var i = 0; i < neighbours.Count; i++)
            {
                var neighbour = neighbours[i];
                if (neighbour == null) continue;

                var x = -1 + i / 3;
                var y = -1 + i % 3;
                if (x == 0 || y == 0) continue;

                var a = new Vector2Int(x, 0);
                var b = new Vector2Int(0, y);
                var nodeA = walkableNeighbours.Find(node => node.Position == from.Position + a);
                var nodeB = walkableNeighbours.Find(node => node.Position == from.Position + b);

                if (nodeA == null || nodeB == null) walkableNeighbours.Remove(neighbour);
            }

            return walkableNeighbours;
        }
    }
}
