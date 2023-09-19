using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Gulde.Client.Model;
using Assets.Gulde.Client.Model.Scenes;
using Newtonsoft.Json;
using Siccity.GLTFUtility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gulde.Client
{
    public class SceneLoader : MonoBehaviour
    {
        public static void LoadScene(string basePath, string scenePath)
        {
            var objects = new Dictionary<string, string>();
            var groups = new Dictionary<string, string>();
        
            var objectsPath = Path.Combine(basePath, "objects");
            var files = Directory.GetFiles(objectsPath, "*.glb", SearchOption.AllDirectories);

            foreach (var item in files)
            {
                var key = Path.GetFileNameWithoutExtension(item);

                if (objects.ContainsKey(key))
                    continue;

                objects.Add(key, item);
            }

            var groupsPath = Path.Combine(basePath, "groups");
            files = Directory.GetFiles(groupsPath, "*.json", SearchOption.AllDirectories);

            foreach (var item in files)
            {
                var key = Path.GetFileNameWithoutExtension(item);

                if (groups.ContainsKey(key))
                    continue;

                groups.Add(key, item);
            }

            scenePath = Path.Combine(basePath, scenePath);
            var groupAsString = File.ReadAllText(scenePath);

            var gildeScene = JsonConvert.DeserializeObject<GildeScene>(groupAsString);

            foreach (var elementGroup in gildeScene.ElementGroups)
            {
                LoadElementGroup(elementGroup, objects, groups);
            }
        }

        static void LoadElementGroup(SceneElementGroup elementGroup, Dictionary<string, string> objects, Dictionary<string, string> groups)
        {
            var parentObject = LoadParentElement(elementGroup, objects, groups);
            
            foreach (var element in elementGroup.Elements)
            {
                if (element.ObjectElement is null)
                    continue;

                var objectElement = element.ObjectElement;

                if (objects.TryGetValue(objectElement.Name, out var gltfPath))
                {
                    GameObject childObject;
                    
                    try
                    {
                        childObject = Importer.LoadFromFile(gltfPath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        continue;
                    }

                    childObject.transform.position = Vector3.zero;
                    childObject.transform.rotation = Quaternion.identity;
                    childObject.transform.SetParent(parentObject.transform);
                    childObject.transform.localPosition = objectElement.Transform.PositionOffset;
                    childObject.transform.localRotation = Quaternion.Euler(Mathf.Rad2Deg * objectElement.Transform.RotationOffset);
                    childObject.name = objectElement.Name;
                }
                else
                    Console.WriteLine($"Object \"{objectElement.Name}\" not found.");
            }
            
            Debug.Log($"Loaded element group \"{elementGroup.FirstElement?.Name}\".");
        }

        static GameObject LoadParentElement(SceneElementGroup elementGroup, Dictionary<string, string> objects, Dictionary<string, string> groups)
        {
            var parentObject = new GameObject(elementGroup.FirstElement.Name);
            
            if (elementGroup.FirstElement.ObjectElement is not null)
            {
                parentObject.transform.position = elementGroup.FirstElement.ObjectElement.Transform.PositionOffset;
                parentObject.transform.rotation = Quaternion.Euler(Mathf.Rad2Deg * (elementGroup.FirstElement.ObjectElement.Transform.RotationOffset));
                
                if (objects.TryGetValue(elementGroup.FirstElement.ObjectElement.Name, out var gltfPath))
                {
                    var childObject = Importer.LoadFromFile(gltfPath, Format.GLB);
                    
                    childObject.transform.SetParent(parentObject.transform);
                    childObject.name = elementGroup.FirstElement.ObjectElement.Name;
                    childObject.transform.localPosition = Vector3.zero;
                    childObject.transform.localRotation = Quaternion.identity;
                }
                else
                    Console.WriteLine($"Object \"{elementGroup.FirstElement?.ObjectElement.Name}\" not found.");
            }
            else if (elementGroup.FirstElement.DummyElement is not null)
            {
                parentObject.transform.position = elementGroup.FirstElement.DummyElement.Transform.PositionOffset;
                parentObject.transform.rotation = Quaternion.Euler(Mathf.Rad2Deg * (elementGroup.FirstElement.DummyElement.Transform.RotationOffset));
            }

            return parentObject;
        }
    }
}
