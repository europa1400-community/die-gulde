using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gulde.Client.Model.Groups;
using Gulde.Client.Model.Scenes;
using Newtonsoft.Json;
using Siccity.GLTFUtility;
using TreeEditor;
using UnityEngine;
using static Siccity.GLTFUtility.GLTFAccessor.Sparse;

namespace Gulde.Client
{
    public class SceneLoader : MonoBehaviour
    {
        public static void LoadScene(string basePath, string scenePath)
        {
            var objectNames = new Dictionary<string, string>();
            var groupNames = new Dictionary<string, string>();

            var objectsPath = Path.Combine(basePath, "objects");
            var files = Directory.GetFiles(objectsPath, "*.glb", SearchOption.AllDirectories);

            foreach (var item in files)
            {
                var key = Path.GetFileNameWithoutExtension(item);

                if (objectNames.ContainsKey(key))
                    continue;

                objectNames.Add(key, item);
            }

            var groupsPath = Path.Combine(basePath, "groups");
            files = Directory.GetFiles(groupsPath, "*.json", SearchOption.AllDirectories);

            foreach (var item in files)
            {
                var key = Path.GetFileNameWithoutExtension(item);

                if (groupNames.ContainsKey(key))
                    continue;

                groupNames.Add(key, item);
            }

            var groups = new List<GildeGroup>();

            foreach (var (groupName, groupPath) in groupNames)
            {
                var groupAsString = File.ReadAllText(groupPath);
                var group = JsonConvert.DeserializeObject<GildeGroup>(groupAsString);
                groups.Add(group);
            }

            scenePath = Path.Combine(basePath, scenePath);
            var sceneAsString = File.ReadAllText(scenePath);

            var gildeScene = JsonConvert.DeserializeObject<GildeScene>(sceneAsString);

            LoadSceneElements(gildeScene, objectNames, groups);
        }

        static SceneElement FindGroupParent(Dictionary<SceneElement, GameObject> sceneElementToGameObject, SceneElement element, List<GildeGroup> groups)
        {
            var elementName = element.Name;

            var candidateElement = (SceneElement)null;

            for (var i = sceneElementToGameObject.Count - 1; i >= 0; i--)
            {
                var (fittingElement, _) = sceneElementToGameObject.ElementAt(i);

                var fittingGroups = groups.Where(group => group.Elements[0].Name == fittingElement.Name && group.Elements.Any(e => e.Name == elementName)).ToList();

                if (fittingGroups.Any())
                {
                    candidateElement = fittingElement;
                }
            }

            return candidateElement;
        }

        static void LoadSceneElements(GildeScene gildeScene, Dictionary<string, string> objects, List<GildeGroup> groups)
        {
            var mainParentObject = new GameObject("scene_elements");
            var currentFolderParent = mainParentObject.transform;
            var currentParent = mainParentObject.transform;

            var sceneElementToGameObject = new Dictionary<SceneElement, GameObject>();

            var hierarchyUp = false;

            for (var i = 0; i < gildeScene.SceneElements.Length; i++)
            {
                var element = gildeScene.SceneElements[i];

                if (element.OnesCount == 1)
                    currentParent = mainParentObject.transform;
                else if (hierarchyUp)
                {
                    var groupParent = FindGroupParent(sceneElementToGameObject, element, groups);

                    currentParent = groupParent != null ? sceneElementToGameObject[groupParent].transform : currentFolderParent;
                    hierarchyUp = false;
                }

                if (!currentParent)
                {
                    Debug.LogWarning($"The parent of element \"{element.Name}\" was not found.");
                    currentParent = mainParentObject.transform;
                }

                if (element.TransformElement is not null)
                {
                    var gameObject = LoadTransformElement(element, currentParent, objects);
                    sceneElementToGameObject.Add(element, gameObject);
                }
                else if (element.CityElement is not null)
                {
                    var gameObject = LoadCityElement(element.Width, element.Height, element.CityElement, currentParent);
                    sceneElementToGameObject.Add(element, gameObject);
                }
                else
                {
                    var gameObject = new GameObject(element.Name);
                    ApplyTransform(gameObject, null, currentParent);
                    sceneElementToGameObject.Add(element, gameObject);
                }

                if (!sceneElementToGameObject[element])
                    continue;

                if (element.Name == "ordner_BAUKLASSEN_FREI")
                {
                    Debug.Log("OIAJSDOI");
                }

                if (element.OnesCount == 1)
                    currentFolderParent = sceneElementToGameObject[element].transform;

                if (element.Hierarchy == 0)
                    currentParent = sceneElementToGameObject[element].transform;
                else if (element.Hierarchy == 2)
                    hierarchyUp = true;
            }

            Debug.Log("Loaded scene elements.");
        }

        static GameObject LoadTransformElement(SceneElement sceneElement, Transform parent, Dictionary<string, string> objects)
        {
            var gameObject = (GameObject)null;
            
            if (sceneElement.TransformElement is DummyElement dummyElement)
            {
                gameObject = new GameObject(sceneElement.Name);
            }
            else if (sceneElement.TransformElement is ObjectElement objectElement)
            {
                if (objectElement.Name is not null && objects.TryGetValue(objectElement.Name, out var gltfPath))
                {
                    gameObject = LoadObject(gltfPath);

                    if (gameObject is null)
                    {
                        return null;
                    }
                    
                    gameObject.name = objectElement.Name;
                }
                else
                {
                    Debug.LogWarning($"Object \"{objectElement.Name}\" not found.");
                }
            }
            
            if (gameObject is null)
                return null;
            
            ApplyTransform(gameObject, sceneElement, parent);
            
            return gameObject;
        }

        static void ApplyTransform(GameObject gameObject, SceneElement sceneElement = null, Transform parentObject = null)
        {
            if (parentObject is not null)
            {
                gameObject.transform.SetParent(parentObject);
            }
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            var transformElement = sceneElement?.TransformElement;
            if (transformElement is null)
                return;
            
            var gildeTransform = gameObject.AddComponent<GildeTransform>();
            
            var position = new Vector3(-transformElement.Transform.Position.x, transformElement.Transform.Position.y, -transformElement.Transform.Position.z);
            var rotationRad = new Vector3(transformElement.Transform.Rotation.x, -transformElement.Transform.Rotation.y, transformElement.Transform.Rotation.z);
            // var rotationRad = new Vector3(0, transformElement.Transform.Rotation.y+ transformElement.Transform.Rotation.x, 0);
            var rotationDeg = rotationRad * Mathf.Rad2Deg;
            var hasXZRotation = rotationDeg.x >= 1 || rotationDeg.z >= 1;
            if (hasXZRotation)
                rotationDeg.y *= -1;
            // rotationDeg = Quaternion.Euler(rotationDeg).eulerAngles;

            gildeTransform.position = position;
            gildeTransform.rotation = new Vector3(transformElement.Transform.Rotation.x, transformElement.Transform.Rotation.y, transformElement.Transform.Rotation.z) * Mathf.Rad2Deg;

            // if (parentObject)
            // {
            //     var parentGildeTransform = parentObject.GetComponent<GildeTransform>();
            //
            //     if (parentGildeTransform)
            //     {
            //         rotationDeg += parentGildeTransform.rotation;
            //     }
            // }
            
            gameObject.transform.localPosition = position;
            gameObject.transform.localEulerAngles = rotationDeg;
            gameObject.transform.localScale = Vector3.one;

            if (sceneElement?.Name == "ordner_LICHT_INNEN")
            {
                Debug.Log($"Actual Rotation: {gameObject.transform.eulerAngles}");
                Debug.Log($"Gilde Rotation: {gildeTransform.rotation}");
            }
        }
        
        static GameObject LoadCityElement(int width, int height, CityElement cityElement, Transform parentObject)
        {
            var terrainObject = new GameObject("Terrain");
            terrainObject.transform.SetParent(parentObject.transform);
            var terrain = terrainObject.AddComponent<Terrain>();
            var terrainData = new TerrainData();
            
            var heights = new float[width, height];
            var maxHeight = cityElement.HeightData1.Max();
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    heights[x, y] = (float)cityElement.HeightData1[x + y * width] / maxHeight;
                }
            }
            
            terrainData.heightmapResolution = width;
            terrainData.size = new Vector3(width, 100, height);
            terrainData.SetHeights(0, 0, heights);
            terrain.terrainData = terrainData;
            
            return terrainObject;
        }

        static GameObject LoadObject(string path)
        {
            if (path.EndsWith("ub_BLUMEN.glb"))
                return new GameObject("Blumen");

            if (path.EndsWith("ub_KRAUT.glb"))
                return new GameObject("Kraut");

            try
            {
                return Importer.LoadFromFile(path);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not load object {path}\n{e}");
                return new GameObject($"[Error] {Path.GetFileNameWithoutExtension(path)}");
            }
        }
    }
}
