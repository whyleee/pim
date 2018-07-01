using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models.Scheme.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SelectOptionsAttribute : Attribute
    {
        public SelectOptionsAttribute(Type optionProvider, bool multiselect = false)
        {
            OptionProvider = optionProvider;
            Multiselect = multiselect;
        }

        public Type OptionProvider { get; }
        public bool Multiselect { get; }
    }
}
