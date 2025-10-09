using FurmaIdle.Models;

namespace FurmaIdle.Helpers
{
    public class UpgradePricingHelper
    {
        public static double NextPrice(UpgradeModel u)
            => u.CostBase * Math.Pow(u.CostGrowth, u.Buys);
    }
}
