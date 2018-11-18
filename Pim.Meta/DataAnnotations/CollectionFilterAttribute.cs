using System;
using System.Collections.Generic;
using System.Text;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class CollectionFilterAttribute : Attribute
    {
        protected CollectionFilterAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }
    }
}
