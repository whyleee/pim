using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    public class CollectionQueryFilterAttribute : CollectionFilterAttribute
    {
        public CollectionQueryFilterAttribute(string key)
            : base(key)
        {
        }
    }
}
