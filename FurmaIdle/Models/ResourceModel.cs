using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ResourceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UnlockId { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string Lore { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public UnlockHelper.State State { get; set; }

        public double AddMod {  get; set; } = 0;
        public double MultMod { get; set; } = 1;

        public double RsPerSecond { get; set;}
        public int RsPerChar { get; set;}
    }
}
