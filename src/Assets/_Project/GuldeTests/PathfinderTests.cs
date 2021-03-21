using NUnit.Framework;
using Gulde.Pathfinding;
using UnityEngine;
using System.Collections.Generic;

namespace GuldeTests
{
    public class PathfinderTests
    {
        [Test]
        public void ShouldCalculateDistance()
        {
            var nodeA = new Node(new Vector3Int(1, 1, 0));
            var nodeB = new Node(new Vector3Int(3, 5, 0));

            var actual = Pathfinder.GetDistance(nodeA, nodeB);

            Assert.AreEqual(48, actual);
        }

        [Test]
        public void ShouldFindNeighbours()
        {
            var nodeA = new Node(new Vector3Int(3, 3, 0));

            var nodes = new List<Node>();
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    var node = new Node(new Vector3Int(x, y, 0));
                    nodes.Add(node);
                }
            }

            Debug.Log(nodes.Count);

            var expected = new List<Node>()
            {
                new Node(new Vector3Int(4, 3, 0)),
                new Node(new Vector3Int(2, 3, 0)),
                new Node(new Vector3Int(3, 4, 0)),
                new Node(new Vector3Int(3, 2, 0)),
                new Node(new Vector3Int(4, 4, 0)),
                new Node(new Vector3Int(4, 2, 0)),
                new Node(new Vector3Int(2, 4, 0)),
                new Node(new Vector3Int(2, 2, 0)),
            };

            var actual = Pathfinder.GetNeighbours(nodeA, nodes);

            foreach (var node in actual) Debug.Log(node.Position);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldFindLowestCostNode()
        {
            var nodes = new List<Node>()
            {
                new Node(new Vector3Int(4, 3, 0)),
                new Node(new Vector3Int(2, 3, 0)),
                new Node(new Vector3Int(3, 4, 0)),
                new Node(new Vector3Int(3, 2, 0)),
                new Node(new Vector3Int(4, 4, 0)),
                new Node(new Vector3Int(4, 2, 0)),
                new Node(new Vector3Int(2, 4, 0)),
                new Node(new Vector3Int(2, 2, 0)),
            };
            var startNode = new Node(new Vector3Int(1, 0, 0));

            foreach (var node in nodes) node.CostG = Pathfinder.GetDistance(startNode, node);

            var expected = new Node(new Vector3Int(2, 2, 0));
            var actual = Pathfinder.FindLowestCostNode(startNode, nodes);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetracePath()
        {
            var nodes = new List<Node>()
            {
                new Node(new Vector3Int(5, 1, 0)),
                new Node(new Vector3Int(4, 0, 0)),
                new Node(new Vector3Int(3, 0, 0)),
                new Node(new Vector3Int(2, 0, 0)),
                new Node(new Vector3Int(1, 0, 0)),
                new Node(new Vector3Int(0, 0, 0)),
            };

            for (var i = 0; i < nodes.Count - 1; i++)
            {
                var node = nodes[i];
                node.Parent = nodes[i + 1];
            }

            var expected = new List<Vector3Int>()
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(2, 0, 0),
                new Vector3Int(3, 0, 0),
                new Vector3Int(4, 0, 0),
                new Vector3Int(5, 1, 0),
            };

            var actual = Pathfinder.RetracePath(nodes[nodes.Count - 1], nodes[0]);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldFindPath()
        {
            var positions = new List<Vector3Int>();
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    var position = new Vector3Int(x, y, 0);
                    positions.Add(position);
                }
            }

            var startPosition = new Vector3Int(1, 0, 0);
            var endPosition = new Vector3Int(4, 2, 0);

            var expected = new List<Vector3Int>()
            {
                new Vector3Int(2, 1, 0),
                new Vector3Int(3, 2, 0),
                new Vector3Int(4, 2, 0),
            };

            var actual = Pathfinder.FindPath(startPosition, endPosition, positions);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetDirections()
        {
            var startPosition = new Vector3Int(1, 0, 0);

            var waypoints = new List<Vector3Int>()
            {
                new Vector3Int(2, 1, 0),
                new Vector3Int(3, 2, 0),
                new Vector3Int(4, 2, 0),
            };

            var expected = new List<Vector3Int>()
            {
                new Vector3Int(1, 1, 0),
                new Vector3Int(1, 1, 0),
                new Vector3Int(1, 0, 0),
            };

            var actual = Pathfinder.GetDirections(startPosition, waypoints);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
