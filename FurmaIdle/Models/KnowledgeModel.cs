using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class KnowledgeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double GainBase { get; set; }
        public double GainCurve { get; set; }
        public double GainMultiplier { get; set; }
        public int GainFactor { get; set; } = 0;
        public double KnowCoinGain { get; set; }
        public double KnowCoinGainPenaltie { get; set; }
        public string Image { get; set; }
        public bool Avaliable { get; set; } = false;
        public int Points { get; set; } = 0;
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.ExpansionOnly;


    }
}
