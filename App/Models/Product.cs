using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using App.Models.SelectOptions;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Pim.Meta.DataAnnotations;

namespace App.Models
{
    public class Product
    {
        // Header

        [Display(GroupName = ProductSections.Header)]
        [Editable(false)]
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
        public IEnumerable<ProductPairing> Pairings { get; set; } = Enumerable.Empty<ProductPairing>();

        [Display(GroupName = ProductSections.Content)]
        public IEnumerable<Wholesaler> Wholesalers { get; set; } = Enumerable.Empty<Wholesaler>();

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
        public NutritionValues NutritionValues { get; set; } = new NutritionValues();

        // Labels

        [Display(GroupName = ProductSections.Labels)]
        public ProductLabels Labels { get; set; } = new ProductLabels();

        [Display(GroupName = ProductSections.Labels)]
        public Allergy Allergy { get; set; } = new Allergy();

        // Storage

        [Display(GroupName = ProductSections.Storage)]
        [Range(0, 10000)]
        public double Weight { get; set; }

        [Display(GroupName = ProductSections.Storage)]
        [SelectOptions(typeof(WeightUnitSelectOptions))]
        public string WeightUnit { get; set; }

        [Display(GroupName = ProductSections.Storage)]
        public ProductStorageTerm StorageTerm { get; set; } = new ProductStorageTerm();

        [Display(GroupName = ProductSections.Storage)]
        public ProductStorageTemperature StorageTemperature { get; set; } = new ProductStorageTemperature();

        // Media

        [Display(GroupName = ProductSections.Media)]
        public IEnumerable<ProductImage> Images { get; set; } = Enumerable.Empty<ProductImage>();

        // Publishing

        [Display(GroupName = ProductSections.Publishing)]
        [Editable(false)]
        public DateTime Created { get; set; }

        [Display(GroupName = ProductSections.Publishing)]
        [Editable(false)]
        public DateTime Updated { get; set; }

        [Display(GroupName = ProductSections.Publishing)]
        [SelectOptions(typeof(PublishStatusSelectOptions))]
        public string Status { get; set; }

        [Display(GroupName = ProductSections.Publishing)]
        public DateTime? Published { get; set; }

        [Display(GroupName = ProductSections.Publishing)]
        public MarkAsNew MarkAsNew { get; set; } = new MarkAsNew();
    }
}
