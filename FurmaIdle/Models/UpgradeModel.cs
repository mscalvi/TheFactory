using FurmaIdle.Helpers;
using static FurmaIdle.Helpers.UpgradeCostEnum;

namespace FurmaIdle.Models
{
    public class UpgradeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Lore { get; set; }

        public int Buys { get; set; } = 0;  
        public int MaxBuys { get; set; } = 1;
        public bool IsMaxed => Buys >= MaxBuys;
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.ExpeditionOnly;


        // State
        public bool Unlocked { get; set; }
        public string TechId { get; set; }
        public bool Avaliable { get; set; }


        // Calculados no build (ou sob demanda)
        public UpgradeCostCode CostCode { get; set; }
        public string CostResourceId { get; set; }
        public double CostBase { get; set; }
        public double CostGrowth { get; set; }
        public int Range { get; set; }

        public double GetCost() => CostBase * Math.Pow(CostGrowth, Range - 1);

        // Efeito
        public List<UpgradeEffectModel> Effects { get; init; } = new();
    }
}
