using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class LocalModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string? UnlockId { get; set; }
        public int? Level { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public string StageId { get; set; }
    }
}
