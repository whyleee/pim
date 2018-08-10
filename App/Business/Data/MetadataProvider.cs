using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using App.Models.Scheme;
using App.Models.Scheme.DataAnnotations;

namespace App.Business.Data
{
    public class MetadataProvider
    {
        public ItemTypeInfo GetTypeInfo(Type type)
        {
            return new ItemTypeInfo
            {
                Name = type.Name,
                Fields = type.GetProperties().Select(GetTypeFieldInfo).ToList(),
                DefaultItem = Activator.CreateInstance(type)
            };
        }

        private ItemFieldInfo GetTypeFieldInfo(PropertyInfo prop)
        {
            var field = new ItemFieldInfo
            {
                Name = char.ToLower(prop.Name.First()) + prop.Name.Substring(1),
                Type = GetTypeName(prop.PropertyType),
                Kind = GetFieldKind(prop),
                Attributes = GetAttributes(prop)
            };

            var itemType = IsCollection(prop.PropertyType)
                ? prop.PropertyType.GetGenericArguments().First()
                : prop.PropertyType;

            if (itemType.Namespace.StartsWith("App.Models"))
            {
                field.ComplexType = GetTypeInfo(itemType);
            }

            return field;
        }

        private IDictionary<string, object> GetAttributes(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes();
            var dict = new Dictionary<string, object>();

            dict.Add("displayName", ToSentenceCase(prop.Name));

            foreach (var attr in attrs)
            {
                if (attr is ReadOnlyAttribute)
                {
                    dict.Add("readonly", true);
                }
                if (attr is RequiredAttribute)
                {
                    dict.Add("required", true);
                }
                if (attr is RangeAttribute range)
                {
                    dict.Add("range", new { Min = range.Minimum, Max = range.Maximum });
                }
                if (attr is RegularExpressionAttribute regex)
                {
                    dict.Add("regex", regex.Pattern);
                }
                if (attr is DisplayAttribute display)
                {
                    if (!string.IsNullOrEmpty(display.Name))
                    {
                        dict["displayName"] = display.Name;
                    }
                    if (!string.IsNullOrEmpty(display.GroupName))
                    {
                        dict.Add("groupName", display.GroupName);
                    }
                }
                if (attr is SelectOptionsAttribute selectOptions)
                {
                    var optionProvider = (ISelectOptionProvider) Activator.CreateInstance(selectOptions.OptionProvider);
                    dict.Add("selectOptions", optionProvider.GetOptions());
                }
            }

            return dict;
        }

        private string GetFieldKind(PropertyInfo prop)
        {
            var dataTypeAttr = prop.GetCustomAttribute<DataTypeAttribute>();
            return dataTypeAttr?.DataType.ToString();
        }

        private string GetTypeName(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string))
            {
                return "string";
            }
            if (type == typeof(bool))
            {
                return "bool";
            }
            if (type == typeof(int))
            {
                return "integer";
            }
            if (type == typeof(double) || type == typeof(decimal))
            {
                return "number";
            }
            if (type == typeof(DateTime))
            {
                return "datetime";
            }
            if (IsCollection(type))
            {
                return "array";
            }

            return type.Name;
        }

        private bool IsCollection(Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        private string ToSentenceCase(string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }
    }
}
