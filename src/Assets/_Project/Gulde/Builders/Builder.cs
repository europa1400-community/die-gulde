using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Gulde.Builders
{
    public abstract class Builder
    {
        Dictionary<PropertyInfo, object> PropertyToKey { get; } = new Dictionary<PropertyInfo, object>();

        MonoRoutine LoadMonoRoutine { get; }

        List<PropertyInfo> LoadAssetProperties =>
            GetType()
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(property => Attribute.IsDefined((MemberInfo) property, typeof(LoadAssetAttribute)))
                .ToList();

        public Builder()
        {
            LoadMonoRoutine = new MonoRoutine(LoadRoutine);

            foreach (var property in LoadAssetProperties)
            {
                var key = property.CustomAttributes.First().ConstructorArguments[0].Value;
                PropertyToKey.Add(property, key);
            }
        }

        IEnumerator LoadRoutine()
        {
            foreach (var pair in PropertyToKey)
            {
                var property = pair.Key;
                var key = pair.Value;

                var asyncOperation = Addressables.LoadAssetAsync<Object>(key);
                yield return asyncOperation;
                var asset = asyncOperation.Result;

                property.SetValue(this, asset);
            }
        }

        WaitForCompletion LoadAssets() => LoadMonoRoutine.WaitForCompletion;

        public virtual IEnumerator Build()
        {
            yield return LoadAssets();
        }
    }
}