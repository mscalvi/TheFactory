using FurmaIdle.Data;
using static FurmaIdle.Data.UpgradeCostEnum;

namespace FurmaIdle.Models
{
    public class UpgradeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool Unlocked { get; set; }
        public string TechId { get; set; }

        public UpgradeCostCode CostCode { get; set; }

        // Calculados no build (ou sob demanda)
        public string CostResourceId { get; set; }
        public double CostBase { get; set; }
        public double CostGrowth { get; set; }
        public int Range { get; set; }

        public double GetCost() => CostBase * Math.Pow(CostGrowth, Range - 1);
    }
}
