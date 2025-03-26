using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Europa1400.Tools.Pipeline;
using Europa1400.Tools.Pipeline.Assets;
using Europa1400.Tools.Pipeline.Converter;
using Europa1400.Tools.Pipeline.Decoder;
using Europa1400.Tools.Pipeline.Output;
using UnityEngine;

namespace Gulde.Client
{
    public static class AssetTools
    {
        public static async Task PrepareAllAsync(
            string gamePath,
            string outputPath,
            IProgress<PipelineProgress> progress,
            CancellationToken cancellationToken = default)
        {
            PipelineSettings.CacheRoot = Path.Combine(Application.persistentDataPath, "Cache");

            await PipelineBuilder<AgebAsset>
                .Create()
                .DecodeWith<AgebDecoder>()
                .Write(new OutputHandlerOptions
                {
                    OutputRoot = Path.Combine(outputPath)
                })
                .Build()
                .ExecuteAsync(GameAssets.OfType<AgebAsset>().FromGameInstallation(gamePath), progress,
                    cancellationToken);
            await PipelineBuilder<AobjAsset>
                .Create()
                .DecodeWith<AobjDecoder>()
                .Write(new OutputHandlerOptions
                {
                    OutputRoot = Path.Combine(outputPath)
                })
                .Build()
                .ExecuteAsync(GameAssets.OfType<AobjAsset>().FromGameInstallation(gamePath), progress,
                    cancellationToken);
            await PipelineBuilder<SbfAsset>
                .Create()
                .DecodeWith<SbfDecoder>()
                .ConvertWith<SbfConverter>()
                .Write(new OutputHandlerOptions
                {
                    OutputRoot = Path.Combine(outputPath, "Sounds")
                })
                .Build()
                .ExecuteAsync(GameAssets.OfType<SbfAsset>().FromGameInstallation(gamePath), progress,
                    cancellationToken);
            await PipelineBuilder<GfxAsset>
                .Create()
                .DecodeWith<GfxDecoder>()
                .ConvertWith<GfxConverter>()
                .Write(new OutputHandlerOptions
                {
                    OutputRoot = Path.Combine(outputPath, "Graphics")
                })
                .Build()
                .ExecuteAsync(GameAssets.OfType<GfxAsset>().FromGameInstallation(gamePath), progress, 
                    cancellationToken);
        }
    }
}

