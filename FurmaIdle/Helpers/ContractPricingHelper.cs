// Helpers/ContractsPricing.cs
using FurmaIdle.Data;
using FurmaIdle.Models;
using System;

public static class ContractsPricingHelper
{
    public static bool TryGetBalance(ContractModel c, out ContractBalance bal)
        => ContractBalanceData.ByLevel.TryGetValue(c.Level, out bal);

    public static double NextPrice(ContractModel c)
    {
        if (!TryGetBalance(c, out var bal)) return double.PositiveInfinity;
        // custo da próxima unidade = C0 * Growth^(Quant atual)
        var price = bal.Cost0 * Math.Pow(bal.Growth, c.Quant);
        return Math.Ceiling(price); // arredonda pra cima
    }

    public static (string resId, double cps, double spc) ProdParams(ContractModel c)
    {
        if (!TryGetBalance(c, out var bal)) return ("", 0, 1);
        return (bal.ResourceId, bal.CoinsPerCycle, bal.SecondsPerCycle);
    }
}
