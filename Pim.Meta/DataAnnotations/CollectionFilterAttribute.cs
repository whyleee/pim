using System;
using System.Collections.Generic;
using System.Text;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class CollectionFilterAttribute : Attribute
    {
        protected CollectionFilterAttribute(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public string Key { get; }

        public string Name { get; }

        public bool Required { get; set; }
    }
}
