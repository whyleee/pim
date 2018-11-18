using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Models.SelectOptions;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Pim.Meta.DataAnnotations;

namespace App.Models
{
    public class Variant
    {
        // Header

        [Display(GroupName = VariantSections.Header)]
        [Editable(false)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        [Display(GroupName = VariantSections.Header)]
        [Required]
        public string ProductId { get; set; }

        [Display(GroupName = VariantSections.Header)]
        [Required]
        public string Name { get; set; }

        [Display(GroupName = VariantSections.Header)]
        [Required]
        [RegularExpression("^[a-z0-9][a-z0-9-]*[a-z0-9]$")]
        public string Slug { get; set; }

        [Display(GroupName = VariantSections.Header)]
        public string Ean { get; set; }

        // Content

        [Display(GroupName = VariantSections.Content)]
        [Range(0, 1000000)]
        public decimal Price { get; set; }

        [Display(GroupName = VariantSections.Content)]
        [SelectOptions(typeof(CurrencySelectOptions))]
        public string Currency { get; set; }

        [Display(GroupName = VariantSections.Content)]
        [Range(0, 10000)]
        public decimal Quantity { get; set; }

        [Display(GroupName = VariantSections.Content)]
        [SelectOptions(typeof(QuantityUnitSelectOptions))]
        public string QuantityUnit { get; set; }

        // Storage

        [Display(GroupName = VariantSections.Storage)]
        [Range(0, 10000)]
        public double WeightNet { get; set; }

        [Display(GroupName = VariantSections.Storage)]
        [Range(0, 10000)]
        public double WeightGross { get; set; }

        [Display(GroupName = VariantSections.Storage)]
        public Dimensions Dimensions { get; set; } = new Dimensions();

        [Display(GroupName = VariantSections.Storage)]
        public VariantPackaging Packaging { get; set; } = new VariantPackaging();

        [Display(GroupName = VariantSections.Storage)]
        public VariantShipping Shipping { get; set; } = new VariantShipping();

        // Publishing

        [Display(GroupName = VariantSections.Publishing)]
        [Editable(false)]
        public DateTime Created { get; set; }

        [Display(GroupName = VariantSections.Publishing)]
        [Editable(false)]
        public DateTime Updated { get; set; }

        [Display(GroupName = VariantSections.Publishing)]
        [SelectOptions(typeof(PublishStatusSelectOptions))]
        public string Status { get; set; }

        [Display(GroupName = VariantSections.Publishing)]
        public DateTime? Published { get; set; }
    }
}
