using System;
using System.Collections.Generic;
using System.Linq;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SelectOptionsAttribute : Attribute
    {
        public SelectOptionsAttribute(Type optionProvider)
        {
            OptionProvider = optionProvider;
        }

        public Type OptionProvider { get; }
    }
}
