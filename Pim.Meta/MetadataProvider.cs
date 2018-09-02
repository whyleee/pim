using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Pim.Meta.DataAnnotations;

namespace Pim.Meta
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

            if (!IsSimpleType(itemType))
            {
                field.ComplexType = GetTypeInfo(itemType);
            }

            return field;
        }

        private IDictionary<string, object> GetAttributes(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes();
            var dict = new Dictionary<string, object>();

            dict["displayName"] = Helpers.ToSentenceCase(prop.Name);

            foreach (var attr in attrs)
            {
                if (attr is KeyAttribute)
                {
                    dict["key"] = true;
                }
                if (attr is ReadOnlyAttribute)
                {
                    dict["readonly"] = true;
                }
                if (attr is EditableAttribute editable && !editable.AllowEdit)
                {
                    if (editable.AllowInitialValue)
                    {
                        dict["constant"] = true;
                    }
                    else
                    {
                        dict["readonly"] = true;
                    }
                }
                if (attr is RequiredAttribute)
                {
                    dict["required"] = true;
                }
                if (attr is RangeAttribute range)
                {
                    dict["range"] = new { Min = range.Minimum, Max = range.Maximum };
                }
                if (attr is RegularExpressionAttribute regex)
                {
                    dict["regex"] = regex.Pattern;
                }
                if (attr is DisplayAttribute display)
                {
                    if (!string.IsNullOrEmpty(display.Name))
                    {
                        dict["displayName"] = display.Name;
                    }
                    if (!string.IsNullOrEmpty(display.GroupName))
                    {
                        dict["groupName"] = display.GroupName;
                    }
                }
                if (attr is SelectOptionsAttribute selectOptions)
                {
                    var optionProvider = (ISelectOptionProvider) Activator.CreateInstance(selectOptions.OptionProvider);
                    dict["selectOptions"] = optionProvider.GetOptions();
                }
            }

            if (prop.PropertyType.IsEnum && !dict.ContainsKey("selectOptions"))
            {
                var optionProvider = new EnumSelectOptionProvider(prop.PropertyType);
                dict["selectOptions"] = optionProvider.GetOptions();
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

            if (type == typeof(string) || type.IsEnum)
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

        private bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments().Single();
            }

            var systemSimpleTypes = new[]
            {
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(Decimal),
                typeof(Enum),
                typeof(Guid),
                typeof(String),
                typeof(TimeSpan)
            };

            return type.IsPrimitive ||
                type.IsEnum ||
                systemSimpleTypes.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
        }

        private bool IsCollection(Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}
