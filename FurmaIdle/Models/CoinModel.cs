using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class CoinModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string UnlockId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
    }
}
