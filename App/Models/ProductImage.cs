using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Pim.Meta.DataAnnotations;

namespace App.Models
{
    public class ProductImage
    {
        [Required]
        [SelectOptions(typeof(ProductImageTypeSelectOptions))]
        public string Type { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        public string Url { get; set; }

        public bool IsTransparent { get; set; }
    }

    public class ProductImageTypeSelectOptions : ISelectOptionProvider
    {
        public IEnumerable<SelectOption> GetOptions()
        {
            return new[]
            {
                new SelectOption("Pack Shot: Front", "pack-shot-front"),
                new SelectOption("Pack Shot: Right", "pack-shot-right"),
                new SelectOption("Pack Shot: Left", "pack-shot-left"),
                new SelectOption("Pack Shot: Top", "pack-shot-top"),
                new SelectOption("Pack Shot: Back", "pack-shot-back"),
                new SelectOption("Environmental: In context", "environmental-in-context"),
                new SelectOption("Environmental: Product unpacked (not maninupated)", "environmental-unpacked"),
                new SelectOption("Environmental: Product being prepared", "environmental-prepared"),
                new SelectOption("Environmental: Product in cooking", "environmental-in-cooking")
            };
        }
    }
}
