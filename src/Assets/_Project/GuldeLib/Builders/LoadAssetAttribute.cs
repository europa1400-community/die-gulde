using System;

namespace GuldeLib.Builders
{
    /// <summary>
    /// Attribute for properties referencing adressable assets.
    /// TO be used in conjunction with the <see cref = "Builder">Builder</see> classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LoadAssetAttribute : Attribute
    {
        /// <summary>
        /// Gets the key of the adressable.
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "LoadAssetAttribute">LoadAssetAttribute</see> class.
        /// </summary>
        /// <param name="key">The <see cref = "Key">Key</see> of the adressable.</param>
        public LoadAssetAttribute(object key)
        {
            Key = key;
        }
    }
}