using System;
using System.Collections.Generic;

namespace Pim.Meta
{
    public abstract class CollectionFilterInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Required { get; set; }
    }
}
