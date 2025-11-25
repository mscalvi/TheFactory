using FurmaIdle.Data;
using FurmaIdle.Helpers;
using System.Text.Json.Serialization;

namespace FurmaIdle.Models
{
    public class CoinModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public string UnlockId { get; set; }
        public string Lore { get; set; }
        public string Description { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;


        // Modifiers
        public List<ModifierModel> Modifiers { get; set; }
    }
}
