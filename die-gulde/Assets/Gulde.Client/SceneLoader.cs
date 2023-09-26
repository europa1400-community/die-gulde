using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gulde.Client.Model.Groups;
using Gulde.Client.Model.Scenes;
using Newtonsoft.Json;
using Siccity.GLTFUtility;
using UnityEngine;

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

            foreach (var elementGroup in gildeScene.ElementGroups)
            {
                LoadElementGroup(elementGroup, objectNames, groups);
            }
        }

        static void PrepareScene(GildeScene scene)
        {
            foreach (var elementGroup in scene.ElementGroups)
            {
                for (int i = 1; i < elementGroup.Elements.Count; i++)
                {
                    var element = elementGroup.Elements[i];
                    var objectElement = element.ObjectElement;
                    var dummyElement = element.DummyElement;

                    if (objectElement is null && dummyElement is null) continue;

                    var definingElement = elementGroup.Elements[i - 1];
                    for (int j = i - 1; j >= 0; j--)
                    {
                        var previousElement = elementGroup.Elements[j];
                        var previousObjectElement = previousElement.ObjectElement;
                        var previousDummyElement = previousElement.DummyElement;

                        if ((previousObjectElement is null ||
                             previousObjectElement.Transform.Position == Vector3.zero) &&
                            (previousDummyElement is null || previousDummyElement.Transform.Position == Vector3.zero))
                            continue;
                        
                        definingElement = previousElement;
                        break;
                    }

                    if (objectElement is not null && objectElement.Transform.Position == Vector3.zero)
                    {
                        objectElement.Transform = definingElement.ObjectElement?.Transform ?? definingElement.DummyElement.Transform;
                    }
                }
            }
        }

        static void LoadElementGroup(SceneElementGroup elementGroup, Dictionary<string, string> objects, List<GildeGroup> groups)
        {
            var mainParentObject = LoadParentElement(elementGroup, objects);
            
            var sceneElementToGameObject = new Dictionary<SceneElement, GameObject>();
            sceneElementToGameObject.Add(elementGroup.FirstElement, mainParentObject);

            for (var i = 0; i < elementGroup.Elements.Count; i++)
            {
                var element = elementGroup.Elements[i];
                var parentElement = elementGroup.FirstElement;
                var gildeGroup = groups.FirstOrDefault(group => group.Name == element.Name);
                
                if (gildeGroup is null)
                {
                    var elementsWithSameName = 0;
                    // Search for elements before this element that define a group containing this element
                    for (int j = i - 1; j >= 0; j--)
                    {
                        var possibleParentElement = elementGroup.Elements[j];
                        if (possibleParentElement.Name == element.Name)
                        {
                            elementsWithSameName += 1;
                            continue;
                        }

                        var group = groups.FirstOrDefault(g =>
                            g.Name == possibleParentElement.Name || g.Elements.Count > 0 &&
                            (g.Elements[0].Name == possibleParentElement.Name || 
                             possibleParentElement.ObjectElement is not null && g.Elements[0].Name == possibleParentElement.ObjectElement.Name));
                        if (group is null) continue;

                        if (group.Name == "ub_LEUCHTER_3ARM")
                        {
                            Debug.Log("");
                        }
                        
                        var groupElement = group.Elements.FirstOrDefault(groupElement => groupElement.Name == element.Name);
                        if (groupElement is null) continue;

                        var groupElementIndex = group.Elements.IndexOf(groupElement);
                        if (i - j != groupElementIndex + elementsWithSameName) continue;
                        
                        if (group.Name != element.Name)
                        {
                            parentElement = possibleParentElement;
                        }
                        gildeGroup = group;
                        break;
                    }
                }

                if (!sceneElementToGameObject.ContainsKey(parentElement))
                {
                    Debug.LogWarning($"The parent element \"{parentElement.Name}\" of element \"{element.Name}\" was not found.");
                    continue;
                }
                var parentObject = sceneElementToGameObject[parentElement];
                if (element.TransformElement is not null)
                {
                    var gameObject = LoadTransformElement(element, parentObject, objects, gildeGroup);
                    sceneElementToGameObject.Add(element, gameObject);
                }
                else if (element.CityElement is not null)
                {
                    var gameObject = LoadCityElement(element.Width, element.Height, element.CityElement, parentObject);
                    sceneElementToGameObject.Add(element, gameObject);
                }
                else
                {
                    var gameObject = new GameObject(element.Name);
                    ApplyTransform(gameObject, null, parentObject, gildeGroup);
                    sceneElementToGameObject.Add(element, gameObject);
                }
            }
            
            Debug.Log($"Loaded element group \"{elementGroup.FirstElement?.Name}\".");
        }

        static GameObject LoadTransformElement(SceneElement sceneElement, GameObject parentObject, Dictionary<string, string> objects, GildeGroup gildeGroup = null)
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
            
            ApplyTransform(gameObject, sceneElement, parentObject, gildeGroup);
            
            return gameObject;
        }

        static void ApplyTransform(GameObject gameObject, SceneElement sceneElement = null, GameObject parentObject = null, GildeGroup gildeGroup = null)
        {
            if (parentObject is not null)
            {
                gameObject.transform.SetParent(parentObject.transform);
            }
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            var transformElement = sceneElement?.TransformElement;
            if (transformElement is null)
                return;
            
            var position = new Vector3(-transformElement.Transform.Position.x, transformElement.Transform.Position.y, -transformElement.Transform.Position.z);
            var rotation = new Vector3(transformElement.Transform.Rotation.x, -transformElement.Transform.Rotation.y,
                transformElement.Transform.Rotation.z);

            // if (gildeGroup is not null)
            // {
            //     var groupElement = gildeGroup.Elements.FirstOrDefault(element => element.Name == sceneElement.Name);
            //
            //     if (groupElement is not null)
            //     {
            //         var groupPosition = new Vector3(-groupElement.Transform.Position.x, groupElement.Transform.Position.y, -groupElement.Transform.Position.z);
            //         var groupRotation = new Vector3(groupElement.Transform.Rotation.x, -groupElement.Transform.Rotation.y,
            //             groupElement.Transform.Rotation.z);
            //         
            //         position += groupPosition;
            //         rotation += groupRotation;
            //     }
            // }

            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = Quaternion.Euler(Mathf.Rad2Deg * rotation);
            gameObject.transform.localScale = Vector3.one;
        }
        
        static GameObject LoadCityElement(int width, int height, CityElement cityElement, GameObject parentObject)
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

        static GameObject LoadParentElement(SceneElementGroup elementGroup, Dictionary<string, string> objects)
        {
            var parentObject = new GameObject(elementGroup.FirstElement.Name);
            var sceneElement = elementGroup.FirstElement;
            var transformElement = sceneElement.TransformElement;

            if (sceneElement is null)
            {
                Debug.LogWarning("Element group has no first element.");
                return null;
            }
            
            if (transformElement is not null)
            {
                ApplyTransform(parentObject, sceneElement);
                
                if (transformElement is not ObjectElement objectElement ||
                    objectElement.Name == null ||
                    !objects.TryGetValue(objectElement.Name, out var gltfPath)) return parentObject;
                
                var childObject = Importer.LoadFromFile(gltfPath, Format.GLB);
                var name = transformElement is ObjectElement element
                    ? element.Name
                    : sceneElement.Name;
                childObject.name = name;
                ApplyTransform(childObject, null, parentObject);
            }
            else if (sceneElement.CityElement is not null)
            {
                LoadCityElement(elementGroup.FirstElement.Width, elementGroup.FirstElement.Height, elementGroup.FirstElement.CityElement, parentObject);
            }

            return parentObject;
        }

        static GameObject LoadObject(string path)
        {
            try
            {
                return Importer.LoadFromFile(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
    }
}
