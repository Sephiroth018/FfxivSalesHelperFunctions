using System.Collections.Generic;
using Ffxiv.Common;
using Newtonsoft.Json;

namespace Ffxiv.Models
{
    public class Item
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(StringJsonConverter))]
        public long Id { get; set; }

        public string Name { get; set; }

        public LocalizedNames LocalizedNames { get; set; }

        public List<Recipe> Recipes { get; set; }

        public ItemKind ItemKind { get; set; }

        public ClassJobCategory ClassJobCategory { get; set; }

        public bool IsEndProduct { get; set; }
    }
}