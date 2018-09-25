using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CollectionAttribute : Attribute
    {
        public string Path { get; set; }

        public string ItemsProperty { get; set; }
    }
}
