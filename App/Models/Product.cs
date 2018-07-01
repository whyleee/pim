using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using App.Models.Scheme.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace App.Models
{
    public class Product
    {
        // Header

        [Display(GroupName = ProductSections.Header)]
        [ReadOnly(true)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        [Display(GroupName = ProductSections.Header)]
        [Required]
        public string Name { get; set; }

        [Display(GroupName = ProductSections.Header)]
        [Required]
        [RegularExpression("^[a-z0-9][a-z0-9-]*[a-z0-9]$")]
        public string Slug { get; set; }

        // Content

        [Display(GroupName = ProductSections.Content)]
        [DataType(DataType.MultilineText)]
        public string Tagline { get; set; }

        [Display(GroupName = ProductSections.Content)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(GroupName = ProductSections.Content)]
        [DataType(DataType.MultilineText)]
        public string OtherDetails { get; set; }

        [Display(GroupName = ProductSections.Content)]
        public IEnumerable<ProductPairing> Pairings { get; set; }

        [Display(GroupName = ProductSections.Content)]
        public IEnumerable<Wholesaler> Wholesalers { get; set; }

        // Nutrition

        [Display(GroupName = ProductSections.Nutrition)]
        public string TasteSignature { get; set; }

        [Display(GroupName = ProductSections.Nutrition)]
        public int? TasteStrength { get; set; }

        [Display(GroupName = ProductSections.Nutrition)]
        [DataType(DataType.MultilineText)]
        public string Ingredients { get; set; }

        [Display(GroupName = ProductSections.Nutrition)]
        [DataType(DataType.MultilineText)]
        public string AllergicIngredients { get; set; }

        [Display(GroupName = ProductSections.Nutrition)]
        public NutritionValues NutritionValues { get; set; }

        // Labels

        [Display(GroupName = ProductSections.Labels)]
        public ProductLabels Labels { get; set; }

        [Display(GroupName = ProductSections.Labels)]
        public Allergy Allergy { get; set; }

        // Storage

        [Display(GroupName = ProductSections.Storage)]
        [Range(0, 10000)]
        public double Weight { get; set; }

        [Display(GroupName = ProductSections.Storage)]
        [SelectOptions(typeof(WeightUnitSelectOptions))]
        public string WeightUnit { get; set; }

        [Display(GroupName = ProductSections.Storage)]
        public ProductStorageTerm StorageTerm { get; set; }

        [Display(GroupName = ProductSections.Storage)]
        public ProductStorageTemperature StorageTemperature { get; set; }

        // Media

        [Display(GroupName = ProductSections.Media)]
        public IEnumerable<ProductImage> Images { get; set; }

        // Publishing

        [Display(GroupName = ProductSections.Publishing)]
        [ReadOnly(true)]
        public DateTime Created { get; set; }

        [Display(GroupName = ProductSections.Publishing)]
        [ReadOnly(true)]
        public DateTime Updated { get; set; }

        [Display(GroupName = ProductSections.Publishing)]
        [SelectOptions(typeof(PublishStatusSelectOptions))]
        public string Status { get; set; }

        [Display(GroupName = ProductSections.Publishing)]
        public DateTime? Published { get; set; }

        [Display(GroupName = ProductSections.Publishing)]
        public MarkAsNew MarkAsNew { get; set; }
    }

    public class WeightUnitSelectOptions : ISelectOptionProvider
    {
        public IEnumerable<SelectOption> GetOptions()
        {
            return new[]
            {
                new SelectOption("kg", "kg"),
                new SelectOption("l", "l")
            };
        }
    }

    public class PublishStatusSelectOptions : ISelectOptionProvider
    {
        public IEnumerable<SelectOption> GetOptions()
        {
            return new[]
            {
                new SelectOption("Draft", "draft"),
                new SelectOption("Published", "published")
            };
        }
    }
}
