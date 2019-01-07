using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Pim.Meta.DataAnnotations
{
    public class EnumSelectOptionProvider : ISelectOptionProvider
    {
        private readonly Type _enumType;

        public EnumSelectOptionProvider(Type enumType)
        {
            _enumType = enumType;
        }

        public IEnumerable<SelectOption> GetOptions()
        {
            return Enum.GetNames(_enumType)
                .Select(name => new SelectOption(
                    text: Helpers.ToSentenceCase(name),
                    value: GetValue(name)
                ))
                .ToList();
        }

        private string GetValue(string name)
        {
            var member = _enumType.GetMember(name).Single();
            var attr = member.GetCustomAttribute<EnumMemberAttribute>();

            if (attr != null)
            {
                return attr.Value;
            }

            return Helpers.ToCamelCase(name);
        }
    }
}
