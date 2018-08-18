using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    public interface ISelectOptionProvider
    {
        IEnumerable<SelectOption> GetOptions();
    }
}
