using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CollectionAttribute : Attribute
    {
        public string Path { get; set; }

        public HttpUpdateMethod UpdateMethod { get; set; }

        public bool Readonly { get; set; }

        public bool Constant { get; set; }

        public string ItemsProperty { get; set; }

        public string KeyDelimiter { get; set; }
    }
}
