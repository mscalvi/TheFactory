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

        public int RsActual { get; set;}
        public double RsFraction { get; set;} 
        public double RsPerSecond { get; set;}
        public double RsPerChar { get; set;}



    }
}
