using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Models.Scheme.DataAnnotations;

namespace App.Models
{
    public class ProductPairing
    {
        [Required]
        [SelectOptions(typeof(ProductPairingNameSelectOptions))]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class ProductPairingNameSelectOptions : ISelectOptionProvider
    {
        public IEnumerable<SelectOption> GetOptions()
        {
            return new[]
            {
                new SelectOption("Beer (Beverage)", "beer"),
                new SelectOption("Ale (Beverage)", "ale"),
                new SelectOption("Wine (Beverage)", "wine"),
                new SelectOption("Red Wine (Beverage)", "red-wine"),
                new SelectOption("Port Wine (Beverage)", "port-wine"),
                new SelectOption("Sparkling Wine (Beverage)", "sparkling-wine"),
                new SelectOption("Generic Beverage (Beverage)", "generic-beverage"),
                new SelectOption("Bread (Food)", "bread"),
                new SelectOption("Bread Multi Grain (Food)", "bread-multi-grain"),
                new SelectOption("Bread Rye (Food)", "bread-rye"),
                new SelectOption("Crackers (Food)", "crackers"),
                new SelectOption("Grapes (Food)", "grapes"),
                new SelectOption("Apples (Food)", "apples"),
                new SelectOption("Fruits (Food)", "fruits"),
                new SelectOption("Raisins (Food)", "raisins"),
                new SelectOption("Pear (Food)", "pear"),
                new SelectOption("Peaches (Food)", "peaches"),
                new SelectOption("Citrus (Food)", "citrus"),
                new SelectOption("Grapefruit (Food)", "grapefruit"),
                new SelectOption("Grapes Red (Food)", "grapes-red"),
                new SelectOption("Berries (Food)", "berries"),
                new SelectOption("Berry Compote (Food)", "berry-compote"),
                new SelectOption("Strawberries (Food)", "strawberries"),
                new SelectOption("Corn Cob (Food)", "corn-cob"),
                new SelectOption("Vegetables (Food)", "vegetables"),
                new SelectOption("Asparagus White (Food)", "asparagus-white"),
                new SelectOption("Asparagus Green (Food)", "asparagus-green"),
                new SelectOption("Baked Potato (Food)", "baked-potato"),
                new SelectOption("Salad (Food)", "salad"),
                new SelectOption("Celery (Food)", "celery"),
                new SelectOption("Chili (Food)", "chili"),
                new SelectOption("Fennel (Food)", "fennel"),
                new SelectOption("Onion (Food)", "onion"),
                new SelectOption("Pepper (Food)", "pepper"),
                new SelectOption("Pasta (Food)", "pasta"),
                new SelectOption("Rice (Food)", "rice"),
                new SelectOption("Nuts (Food)", "nuts"),
                new SelectOption("Walnuts (Food)", "walnuts"),
                new SelectOption("Candied Walnuts (Food)", "candied-walnuts"),
                new SelectOption("Preserves (Food)", "preserves"),
                new SelectOption("Caramelized Onion Chutney (Food)", "caramelized-onion-chutney"),
                new SelectOption("Pickled Figs (Food)", "pickled-figs"),
                new SelectOption("Mustard (Food)", "mustard"),
                new SelectOption("Poultry (Food)", "poultry"),
                new SelectOption("Chicken Drumstick (Food)", "chicken-drumstick"),
                new SelectOption("Beef (Food)", "beef"),
                new SelectOption("Pork (Food)", "pork"),
                new SelectOption("Fish (Food)", "fish"),
                new SelectOption("Shellfish (Food)", "shellfish"),
                new SelectOption("Scallop (Food)", "scallop"),
                new SelectOption("Herbs (Food)", "herbs"),
                new SelectOption("Honey (Food)", "honey"),
                new SelectOption("Syrup (Food)", "syrup"),
                new SelectOption("Figs (Food)", "figs"),
                new SelectOption("Dates (Food)", "dates"),
                new SelectOption("Jar (Food)", "jar")
            };
        }
    }
}
