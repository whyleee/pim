using System.Collections.Generic;

namespace App.Models.Scheme.DataAnnotations
{
    public interface ISelectOptionProvider
    {
        IEnumerable<SelectOption> GetOptions();
    }
}
