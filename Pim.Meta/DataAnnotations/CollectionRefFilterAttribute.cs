using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    public class CollectionRefFilterAttribute : CollectionFilterAttribute
    {
        public CollectionRefFilterAttribute(string key, string name, string refCollectionKey)
            : base(key, name)
        {
            RefCollectionKey = refCollectionKey;
        }

        public string RefCollectionKey { get; }
    }
}
