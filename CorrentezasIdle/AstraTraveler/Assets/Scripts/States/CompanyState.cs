using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyState
{
    public Dictionary<string, UpgradeInstance> CompanyUpgrades;

    public Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance> CompanyCurrency;

    public Dictionary<IngredientHelper.IngredientType, IngredientInstance> CompanyIngredients;
}
