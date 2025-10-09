// Helpers/ContractsPricing.cs
using FurmaIdle.Data;
using FurmaIdle.Models;
using System;

public static class ContractsPricingHelper
{
    public static bool TryGetBalance(ContractModel c, out ContractBalance bal)
        => ContractBalanceData.ByLevel.TryGetValue(c.Level, out bal);

    // custo da PRÓXIMA unidade (para a 1ª ativação, Quant=0 -> Cost0)
    public static double NextPrice(ContractModel c)
    {
        if (!TryGetBalance(c, out var bal)) return double.PositiveInfinity;
        var price = bal.Cost0 * Math.Pow(bal.Growth, c.Quant);
        return Math.Ceiling(price);
    }

    public static (string resId, double cps, double spc) ProdParams(ContractModel c)
    {
        if (!TryGetBalance(c, out var bal)) return ("", 0, 1);
        return (bal.ResourceId, bal.CoinsPerCycle, bal.SecondsPerCycle);
    }

    // produção por segundo considerando Quant atual
    public static double ProdPerSecond(ContractModel c)
    {
        var (_, cps, spc) = ProdParams(c);
        if (!(cps > 0) || !(spc > 0) || c.Quant <= 0) return 0;
        return (cps / spc) * c.Quant;
    }
}
