using System;

namespace Gulde.Builders
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LoadAssetAttribute : Attribute
    {
        public object Key { get; }

        public LoadAssetAttribute(object key)
        {
            Key = key;
        }
    }
}