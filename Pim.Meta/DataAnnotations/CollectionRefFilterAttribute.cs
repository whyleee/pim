using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    public class CollectionRefFilterAttribute : CollectionFilterAttribute
    {
        public CollectionRefFilterAttribute(string key, string refCollectionKey)
            : base(key)
        {
            RefCollectionKey = refCollectionKey;
        }

        public string RefCollectionKey { get; }

        public string BackendKey { get; set; }

        public bool Multiple { get; set; }
    }
}
