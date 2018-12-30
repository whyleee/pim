using System;
using System.Collections.Generic;

namespace Pim.Meta
{
    public class CollectionRefInfo
    {
        public string CollectionKey { get; set; }
        public IDictionary<string, object> Filters { get; set; }
    }
}
