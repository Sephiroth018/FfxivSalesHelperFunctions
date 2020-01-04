using System.Collections.Generic;

namespace Ffxiv.Models
{
    public class Recipe
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public LocalizedNames LocalizedNames { get; set; }

        public long ResultAmount { get; set; }

        public List<Ingredient> Ingredients { get; set; }
    }
}