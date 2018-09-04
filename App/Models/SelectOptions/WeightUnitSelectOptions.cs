using System;
using System.Collections.Generic;
using Pim.Meta.DataAnnotations;

namespace App.Models.SelectOptions
{
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
}
