using System;
using System.Collections.Generic;
using Pim.Meta.DataAnnotations;

namespace App.Models.SelectOptions
{
    public class CurrencySelectOptions : ISelectOptionProvider
    {
        public IEnumerable<SelectOption> GetOptions()
        {
            return new[]
            {
                new SelectOption("DKK", "DKK"),
                new SelectOption("EUR", "EUR"),
                new SelectOption("SEK", "SEK"),
                new SelectOption("USD", "USD")
            };
        }
    }
}
