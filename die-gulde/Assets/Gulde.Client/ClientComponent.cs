using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Europa1400.Tools.Pipeline;
using Europa1400.Tools.Pipeline.Assets;
using Europa1400.Tools.Pipeline.Converter;
using Europa1400.Tools.Pipeline.Decoder;
using Europa1400.Tools.Pipeline.Output;
using Europa1400.Tools.Structs.Gfx;
using SimpleFileBrowser;
using UnityEngine;

namespace Gulde.Client
{
    public class ClientComponent : MonoBehaviour
    {
        private void Start()
        {
            return;
            FileBrowser.ShowLoadDialog(result =>
                {
                    var selectedPath = result[0];
                    Debug.Log("Selected folder: " + selectedPath);
            
                    var outputPath = Path.Combine(Application.persistentDataPath, "ConvertedAssets");
            
                    Task.Run(() => ConvertAllAssets(selectedPath, outputPath))
                        .ContinueWith(t =>
                        {
                            if (t.Exception != null)
                                Debug.LogError("Asset conversion failed: " + t.Exception);
                            else
                                Debug.Log("Asset conversion completed.");
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                },
                () =>
                {
                    Debug.LogWarning("No folder selected.");
                }, FileBrowser.PickMode.Folders, title: "Select game folder");
        }

        private static void ConvertAllAssets(string gamePath, string outputPath)
        {
            var gameDirectory = new DirectoryInfo(gamePath);
            var outputDirectory = new DirectoryInfo(outputPath);
            var graphicsOutputPath = Path.Combine(outputPath, "graphics");
            
            Debug.Log($"Converting assets from {gameDirectory.FullName} to {outputDirectory.FullName}");   
            
            PipelineBuilder<GfxAsset>
                .Create()
                .DecodeWith<GfxDecoder, GfxStruct>()
                .ConvertWith<GfxConverter, GfxStruct, List<IFileExport>>()
                .WriteTo(graphicsOutputPath)
                .Build()
                .Execute(GameAssets.OfType<GfxAsset>().FromGameInstallation(gamePath));
        }
    }
}