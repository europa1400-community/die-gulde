// using System.Collections;
// using System.Collections.Generic;
// using GuldeLib.Cities;
// using GuldeLib.Maps;
// using GuldeLib.Pathfinding;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// namespace GuldeTests.Entities.Pathfinding
// {
//     public class PathfinderTests
//     {
//
//         [Test]
//         public void ShouldNotEqualNavNodeToNull()
//         {
//             var navNode = new NavNode(new Vector3Int(0, 0, 0));
//
//             Assert.False(navNode.Equals(null));
//         }
//
//         [Test]
//         public void ShouldFindPath()
//         {
//             var startPosition = new Vector3Int(0, 0, 0);
//             var endPosition = new Vector3Int(3, 3, 0);
//
//             var gameObject = new GameObject();
//             var navComponent = gameObject.AddComponent<NavComponent>();
//
//             var navMap = new List<Vector3Int>
//             {
//                 new Vector3Int(0,0,0),
//                 new Vector3Int(0,1,0),
//                 new Vector3Int(0,2,0),
//                 new Vector3Int(0,3,0),
//                 new Vector3Int(1,0,0),
//                 new Vector3Int(1, 1, 0),
//                 new Vector3Int(1, 2, 0),
//                 new Vector3Int(1, 3, 0),
//                 new Vector3Int(2,0,0),
//                 new Vector3Int(2,1,0),
//                 new Vector3Int(2,2,0),
//                 new Vector3Int(2,3,0),
//                 new Vector3Int(3,0,0),
//                 new Vector3Int(3,1,0),
//                 new Vector3Int(3,2,0),
//                 new Vector3Int(3,3,0),
//             };
//             navComponent.NavMap.AddRange(navMap);
//
//             var waypoints = Path.FindPath(startPosition, endPosition, navComponent);
//
//             var expected = new Queue<Vector3Int>(new List<Vector3Int>
//             {
//                 new Vector3Int(1, 1, 0),
//                 new Vector3Int(2, 2, 0),
//                 new Vector3Int(3, 3, 0),
//             });
//
//             CollectionAssert.AreEqual(expected, waypoints);
//         }
//
//         [Test]
//         public void ShouldNotFindPathWithInvalidNav()
//         {
//             LogAssert.ignoreFailingMessages = true;
//
//             var startPosition = new Vector3Int(0, 0, 0);
//             var endPosition = new Vector3Int(3, 3, 0);
//
//             var waypoints = Path.FindPath(startPosition, endPosition, nav: null);
//
//             Assert.AreEqual(0, waypoints.Count);
//
//             var gameObject = new GameObject();
//             var navComponent = gameObject.AddComponent<NavComponent>();
//
//             waypoints = Path.FindPath(startPosition, endPosition, navComponent);
//
//             Assert.AreEqual(0, waypoints.Count);
//
//             var mapComponent = gameObject.AddComponent<MapComponent>();
//
//             waypoints = Path.FindPath(startPosition, endPosition, mapComponent);
//
//             Assert.AreEqual(0, waypoints.Count);
//         }
//
//         [Test]
//         public void ShouldNotFindPathWithInvalidStartOrEndPosition()
//         {
//             LogAssert.ignoreFailingMessages = true;
//
//             var startPosition = new Vector3Int(0, 0, 0);
//             var endPosition = new Vector3Int(3, 3, 0);
//
//             var gameObject = new GameObject();
//             var navComponent = gameObject.AddComponent<NavComponent>();
//
//             var navMap = new List<Vector3Int>
//             {
//                 new Vector3Int(0, 1, 0),
//                 new Vector3Int(0, 2, 0),
//                 new Vector3Int(0, 3, 0),
//                 new Vector3Int(1, 0, 0),
//                 new Vector3Int(1, 1, 0),
//                 new Vector3Int(1, 2, 0),
//                 new Vector3Int(1, 3, 0),
//                 new Vector3Int(2, 0, 0),
//                 new Vector3Int(2, 1, 0),
//                 new Vector3Int(2, 2, 0),
//                 new Vector3Int(2, 3, 0),
//                 new Vector3Int(3, 0, 0),
//                 new Vector3Int(3, 1, 0),
//                 new Vector3Int(3, 2, 0),
//                 new Vector3Int(3, 3, 0),
//             };
//             navComponent.NavMap.AddRange(navMap);
//
//             var waypoints = Path.FindPath(startPosition, endPosition, navComponent);
//
//             Assert.AreEqual(0, waypoints.Count);
//
//             navComponent.NavMap.Clear();
//
//             navMap = new List<Vector3Int>
//             {
//                 new Vector3Int(0, 0, 0),
//                 new Vector3Int(0, 1, 0),
//                 new Vector3Int(0, 2, 0),
//                 new Vector3Int(0, 3, 0),
//                 new Vector3Int(1, 0, 0),
//                 new Vector3Int(1, 1, 0),
//                 new Vector3Int(1, 2, 0),
//                 new Vector3Int(1, 3, 0),
//                 new Vector3Int(2, 0, 0),
//                 new Vector3Int(2, 1, 0),
//                 new Vector3Int(2, 2, 0),
//                 new Vector3Int(2, 3, 0),
//                 new Vector3Int(3, 0, 0),
//                 new Vector3Int(3, 1, 0),
//                 new Vector3Int(3, 2, 0),
//             };
//             navComponent.NavMap.AddRange(navMap);
//
//             waypoints = Path.FindPath(startPosition, endPosition, navComponent);
//
//             Assert.AreEqual(0, waypoints.Count);
//         }
//
//         [Test]
//         public void ShouldNotCrossCorners()
//         {
//             var startPosition = new Vector3Int(0, 0, 0);
//             var endPosition = new Vector3Int(3, 3, 0);
//
//             var gameObject = new GameObject();
//             var navComponent = gameObject.AddComponent<NavComponent>();
//
//             var navMap = new List<Vector3Int>
//             {
//                 new Vector3Int(0, 0, 0),
//                 new Vector3Int(0, 2, 0),
//                 new Vector3Int(0, 3, 0),
//                 new Vector3Int(1, 0, 0),
//                 new Vector3Int(1, 2, 0),
//                 new Vector3Int(1, 3, 0),
//                 new Vector3Int(2, 0, 0),
//                 new Vector3Int(2, 2, 0),
//                 new Vector3Int(2, 3, 0),
//                 new Vector3Int(3, 0, 0),
//                 new Vector3Int(3, 1, 0),
//                 new Vector3Int(3, 2, 0),
//                 new Vector3Int(3, 3, 0),
//             };
//             navComponent.NavMap.AddRange(navMap);
//
//             var waypoints = Path.FindPath(startPosition, endPosition, navComponent);
//
//             var expected = new Queue<Vector3Int>(new List<Vector3Int>
//             {
//                 new Vector3Int(1, 0, 0),
//                 new Vector3Int(2, 0, 0),
//                 new Vector3Int(3, 0, 0),
//                 new Vector3Int(3, 1, 0),
//                 new Vector3Int(3, 2, 0),
//                 new Vector3Int(3, 3, 0),
//             });
//
//             CollectionAssert.AreEqual(expected, waypoints);
//         }
//     }
// }