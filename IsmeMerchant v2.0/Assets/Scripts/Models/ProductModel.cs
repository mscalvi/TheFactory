using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductModel
{
    public string Id;

    public string NameEN;
    public string NamePT;
    public string DescriptionEN;
    public string DescriptionPT;

    public AlchemyHelper.IngredientType Ing1;
    public AlchemyHelper.IngredientType Ing2;
    public AlchemyHelper.IngredientType Ing3;
    public AlchemyHelper.IngredientType Ing4;
    public AlchemyHelper.IngredientType Ing5;
    public double Cost1;
    public double Cost2;
    public double Cost3;
    public double Cost4;
    public double Cost5;

    public double Time;
    public CurrencyHelper.CurrencyType IncomeType;
    public double IncomeAmmount;

    public int Level;
    
    public AlchemyHelper.LabType LabType;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

}
