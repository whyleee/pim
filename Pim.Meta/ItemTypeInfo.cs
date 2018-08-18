using System;
using System.Collections.Generic;
using System.Linq;

namespace Pim.Meta
{
    public class ItemTypeInfo
    {
        public string Name { get; set; }
        public IEnumerable<ItemFieldInfo> Fields { get; set; }
        public object DefaultItem { get; set; }
    }
}
