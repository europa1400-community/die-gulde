using Assets.Gulde.Client.Model;
using Newtonsoft.Json;
using Siccity.GLTFUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneLoader : MonoBehaviour
{
    private const string AssetPath = @"E:\Dateien\Source\europa-1400-tools\output\converted";

    private readonly Dictionary<string, string> objects = new();
    private readonly Dictionary<string, string> groups = new();

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

        var groupsPath = Path.Combine(AssetPath, "objects");
        files = Directory.GetFiles(groupsPath, "*.json", SearchOption.AllDirectories);

        foreach (var item in files)
        {
            var key = Path.GetFileNameWithoutExtension(item);

            if (groups.ContainsKey(key))
                continue;

            groups.Add(key, item);
        }
    }

    private void Start()
    {
        var groupPath = Path.Combine(AssetPath, "scenes", "Staedte", "stadt_TUTORIAL.json");

        var groupAsString = File.ReadAllText(groupPath);

        var group = JsonConvert.DeserializeObject<Scene>(groupAsString);

        foreach (var elementGroup in group.ElementGroups)
        {
            var groupObj = new GameObject(elementGroup.FirstElement?.Name);

            if (elementGroup.FirstElement?.Element != null)
            {
                groupObj.transform.SetPositionAndRotation(elementGroup.FirstElement.Element.PositionOffset, Quaternion.Euler(Mathf.Rad2Deg * elementGroup.FirstElement.Element.RotationOffset));
            }
            else if (elementGroup.FirstElement?.DummyElement != null)
                groupObj.transform.SetPositionAndRotation(elementGroup.FirstElement.DummyElement.Position, Quaternion.Euler(Mathf.Rad2Deg * elementGroup.FirstElement.DummyElement.Rotation));

            bool skipNext = false;

            var elementList = elementGroup.Elements.ToList();

            foreach (var element in elementList)
            {
                if (element.Object == null)
                    continue;

                var obj = element.Object;

                if (objects.TryGetValue(obj.Name, out string gltfObjPath))
                {
                    GameObject gameObj;

                    try
                    {
                        gameObj = Importer.LoadFromFile(gltfObjPath, Format.GLTF);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);

                        continue;
                    }

                    var newGameObj = Instantiate(gameObj, groupObj.transform);
                    Destroy(gameObj);

                    var idx = elementList.IndexOf(element);

                    if (obj.PositionOffset == Vector3.zero && idx > 0)
                        newGameObj.transform.position = elementList[idx - 1]?.Object?.PositionOffset ?? Vector3.zero;
                    else
                        newGameObj.transform.position = obj.PositionOffset;

                    if (obj.RotationOffset == Vector3.zero && idx > 0)
                        newGameObj.transform.rotation = Quaternion.Euler(Mathf.Rad2Deg * elementList[idx - 1]?.Object?.RotationOffset ?? Vector3.zero);
                    else
                        newGameObj.transform.rotation = Quaternion.Euler(Mathf.Rad2Deg * obj.RotationOffset);

                    newGameObj.transform.localScale = new Vector3(-1, 1, 1);

                    newGameObj.name = obj.Name;
                }
                else
                    Console.WriteLine($"Object \"{obj.Name}\" not found.");
            }
        }
    }
}
