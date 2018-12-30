using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CollectionRefAttribute : Attribute
    {
        public CollectionRefAttribute(string collectionKey)
        {
            CollectionKey = collectionKey;
        }

        public string CollectionKey { get; }

        public IDictionary<string, object> Filters { get; set; }
    }
}
