using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Gulde.Builders;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GuldePlayTests.Builders
{
    public class AssetTests
    {
        [LoadAsset("prefab_player")]
        GameObject PlayerPrefab { get; set; }

        [UnityTest]
        public IEnumerator ShouldLoadAssets()
        {
            var loadAssetProperties = GetType()
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(property => Attribute.IsDefined((MemberInfo) property, typeof(LoadAssetAttribute)))
                .ToList();

            foreach (var property in loadAssetProperties)
            {
                var key = property.CustomAttributes.First().ConstructorArguments[0].Value;
                var asyncOperation = Addressables.LoadAssetAsync<Object>(key);
                yield return asyncOperation;
                var asset = asyncOperation.Result;

                property.SetValue(this, asset);
                Assert.NotNull(property.GetValue(this));
            }

        }
    }
}