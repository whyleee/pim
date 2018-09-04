using System;
using System.Collections.Generic;
using Pim.Meta.DataAnnotations;

namespace App.Models.SelectOptions
{
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
