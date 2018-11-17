using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CollectionFilterAttribute : Attribute
    {
        public CollectionFilterAttribute(string key, string name, string refCollectionKey)
        {
            Key = key;
            Name = name;
            RefCollectionKey = refCollectionKey;
        }

        public string Key { get; }

        public string Name { get; }

        public string RefCollectionKey { get; set; }
    }
}
