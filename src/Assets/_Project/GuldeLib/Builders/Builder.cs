using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GuldeLib.Builders
{
    /// <summary>
    /// Base class for builders.
    /// Provides functionality for asynchronously loading game assets.
    /// </summary>
    public abstract class Builder
    {
        /// <summary>
        /// Maps properties referencing game assets to their adressable key.
        /// </summary>
        Dictionary<PropertyInfo, object> PropertyToKey { get; } = new Dictionary<PropertyInfo, object>();

        /// <summary>
        /// The coroutine used for asynchronously loading game assets referenced by subclasses.
        /// </summary>
        MonoRoutine LoadMonoRoutine { get; }

        /// <summary>
        /// Gets properties marked with the <see cref = "LoadAssetAttribute">LoadAssetAttribute</see>.
        /// </summary>
        List<PropertyInfo> LoadAssetProperties =>
            GetType()
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(property => Attribute.IsDefined(property, typeof(LoadAssetAttribute)))
                .ToList();

        /// <summary>
        /// Initializes a new instance of the <see cref = "Builder">Builder</see> class.
        /// </summary>
        public Builder()
        {
            LoadMonoRoutine = new MonoRoutine(LoadRoutine);

            FindAssetReferences();
        }

        /// <summary>
        /// Finds assets referenced by properties with the <see cref = "LoadAssetAttribute">LoadAssetAttribute</see>.
        /// </summary>
        void FindAssetReferences()
        {
            foreach (var property in LoadAssetProperties)
            {
                var key = property.CustomAttributes.First().ConstructorArguments[0].Value;
                PropertyToKey.Add(property, key);
            }
        }

        /// <summary>
        /// Asynchronously loads game assets referenced by subclasses.
        /// </summary>
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

        /// <summary>
        /// Starts the asynchronous asset loading operation.
        /// </summary>
        /// <returns>
        /// A <see cref="CustomYieldInstruction">CustomYieldInstruction</see> that waits for the loading operation's completion.
        /// </returns>
        WaitForCompletion LoadAssets() => LoadMonoRoutine.WaitForCompletion;

        /// <summary>
        /// Implements the main building function.
        /// Must call the base implementation in order to load assets.
        /// </summary>
        /// <example>
        /// public override IEnumerator Build()
        /// {
        ///     yield return base.Build();
        /// }
        /// </example>
        public virtual IEnumerator Build()
        {
            yield return LoadAssets();
        }
    }
}