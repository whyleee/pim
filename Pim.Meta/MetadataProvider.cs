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
        public BackendInfo GetBackendInfo(Type backendType)
        {
            return new BackendInfo
            {
                Collections = backendType.GetProperties()
                    .Select(prop => new
                    {
                        Property = prop,
                        ItemType = TryGetCollectionItemType(prop.PropertyType)
                    })
                    .Where(prop => prop.ItemType != null)
                    .Select(prop => GetCollectionInfo(prop.Property, prop.ItemType))
                    .ToList()
            };
        }

        private Type TryGetCollectionItemType(Type type)
        {
            return new[] { type }.Union(type.GetInterfaces())
                .Where(t => t != typeof(string) &&
                            t.IsGenericType &&
                            t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GetGenericArguments().First())
                .FirstOrDefault();
        }

        private CollectionInfo GetCollectionInfo(PropertyInfo collectionProp, Type itemType)
        {
            return new CollectionInfo
            {
                Key = collectionProp.Name.ToLowerInvariant(),
                Name = GetCollectionName(collectionProp),
                Path = GetCollectionPath(collectionProp),
                Readonly = GetCollectionReadOnly(collectionProp),
                Constant = GetCollectionConstant(collectionProp),
                ItemsProperty = GetCollectionItemsProperty(collectionProp),
                ItemType = GetTypeInfo(itemType),
                Filters = GetCollectionFilterInfos(collectionProp)
            };
        }

        private string GetCollectionName(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<DisplayAttribute>()?.Name
                ?? Helpers.ToSentenceCase(prop.Name);
        }

        private string GetCollectionPath(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<CollectionAttribute>()?.Path
                ?? $"/{prop.Name.ToLowerInvariant()}";
        }

        private bool GetCollectionReadOnly(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<CollectionAttribute>()?.Readonly ?? false;
        }

        private bool GetCollectionConstant(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<CollectionAttribute>()?.Constant ?? false;
        }

        private string GetCollectionItemsProperty(PropertyInfo prop)
        {
            return Helpers.ToCamelCase(prop.GetCustomAttribute<CollectionAttribute>()?.ItemsProperty);
        }

        private IEnumerable<CollectionFilterInfo> GetCollectionFilterInfos(PropertyInfo prop)
        {
            return prop.GetCustomAttributes<CollectionFilterAttribute>()
                .Select(attr =>
                {
                    if (attr is CollectionQueryFilterAttribute queryAttr)
                    {
                        return (CollectionFilterInfo) GetCollectionQueryFilter(queryAttr);
                    }

                    if (attr is CollectionRefFilterAttribute refAttr)
                    {
                        var filterValues = prop.GetCustomAttributes<CollectionRefFilterValueAttribute>()
                            .Where(valueAttr => valueAttr.RefKey == refAttr.Key)
                            .ToList();

                        return (CollectionFilterInfo) GetCollectionRefFilter(refAttr, filterValues);
                    }

                    throw new ArgumentException("Unknown filter attribute");
                })
                .ToList();
        }

        private CollectionQueryFilterInfo GetCollectionQueryFilter(CollectionQueryFilterAttribute attr)
        {
            return new CollectionQueryFilterInfo
            {
                Key = attr.Key,
                Name = attr.Name ?? Helpers.ToSentenceCase(attr.Key),
                Description = attr.Description,
                Type = "query",
                Required = attr.Required
            };
        }

        private CollectionRefFilterInfo GetCollectionRefFilter(CollectionRefFilterAttribute attr, IEnumerable<CollectionRefFilterValueAttribute> filterValues)
        {
            return new CollectionRefFilterInfo
            {
                Key = attr.Key,
                Name = attr.Name ?? Helpers.ToSentenceCase(attr.Key),
                Description = attr.Description,
                Type = "ref",
                Required = attr.Required,
                RefCollectionKey = attr.RefCollectionKey.ToLowerInvariant(),
                Multiple = attr.Multiple,
                Filters = filterValues.ToDictionary(f => f.Key, f => f.Value)
            };
        }

        private ItemTypeInfo GetTypeInfo(Type type)
        {
            return new ItemTypeInfo
            {
                Name = type.Name,
                Fields = type.GetProperties()
                    .Where(ShouldScaffold)
                    .Select(GetTypeFieldInfo)
                    .ToList(),
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
                Attributes = GetAttributes(prop),
                Ref = GetCollectionRefInfo(prop)
            };

            if (field.Kind == DataType.Text.ToString())
            {
                field.Type = GetTypeName(typeof(string));
                field.Kind = "array";
            }

            var itemType = IsCollection(prop.PropertyType)
                ? prop.PropertyType.GetGenericArguments().First()
                : prop.PropertyType;

            if (!IsSimpleType(itemType))
            {
                field.ComplexType = GetTypeInfo(itemType);
            }

            return field;
        }

        private CollectionRefInfo GetCollectionRefInfo(PropertyInfo prop)
        {
            var collectionRef = prop.GetCustomAttribute<CollectionRefAttribute>();

            if (collectionRef == null)
            {
                return null;
            }

            var filterValues = prop.GetCustomAttributes<CollectionRefFilterValueAttribute>()
                .Where(attr => attr.RefKey == collectionRef.CollectionKey)
                .ToList();

            return new CollectionRefInfo
            {
                CollectionKey = collectionRef.CollectionKey.ToLowerInvariant(),
                Filters = filterValues.ToDictionary(f => f.Key, f => f.Value)
            };
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
                if (attr is ReadFromAttribute readFrom)
                {
                    dict["readFrom"] = Helpers.ToCamelCase(readFrom.PropertyName);
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

        private bool ShouldScaffold(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<ScaffoldColumnAttribute>()?.Scaffold ?? true;
        }
    }
}
