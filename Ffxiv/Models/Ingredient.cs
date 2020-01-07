namespace Ffxiv.Models
{
    public class Ingredient : Item
    {
        public long Amount { get; set; }

        public bool IsCrystal { get; set; }
    }
}