using Assets.Gulde.Client.Model;
using Newtonsoft.Json;
using Siccity.GLTFUtility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneLoader : MonoBehaviour
{
    private const string AssetPath = @"E:\Dateien\Source\europa-1400-tools\output\converted";

    private readonly Dictionary<string, string> objects = new();

    private void Awake()
    {
        var objectsPath = Path.Combine(AssetPath, "objects");
        var files = Directory.GetFiles(objectsPath, "*.gltf", SearchOption.AllDirectories);

        foreach (var item in files)
        {
            var key = Path.GetFileNameWithoutExtension(item);

            if (objects.ContainsKey(key))
                continue;

            objects.Add(key, item);
        }
    }

    private void Start()
    {
        var groupPath = Path.Combine(AssetPath, "scenes", "Staedte", "stadt_TUTORIAL.json");

        var groupAsString = File.ReadAllText(groupPath);

        var group = JsonConvert.DeserializeObject<Scene>(groupAsString);

        foreach (var elementGroup in group.ElementGroups)
        {
            foreach (var element in elementGroup.Elements)
            {
                if (element.Object == null)
                    continue;

                var obj = element.Object;

                if (objects.TryGetValue(obj.Name, out string gltfObjPath))
                {
                    GameObject gameObj;

                    try
                    {
                        gameObj = Importer.LoadFromFile(gltfObjPath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);

                        continue;
                    }

                    Instantiate(gameObj, obj.PositionOffset, Quaternion.Euler(obj.RotationOffset));
                }
                else
                    Console.WriteLine($"Object \"{obj.Name}\" not found.");
            }
        }
    }
}
