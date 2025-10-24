using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class GainModel
    {
        public string ItemId { get; set; }
        public ItemHelper.ItemType ItemType { get; set; }
        public int GainEffective { get; set; }
        public double GainTotal { get; set; }
        public double GainFraction { get; set; }


        // Modifiers
        public List<ModifierModel> Modifiers { get; set; }
    }
}
