using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProductInstance
{
    public ProductModel Model;

    public string Id;

    public string NameEN;
    public string NamePT;
    public string DescriptionEN;
    public string DescriptionPT;

    public Dictionary<AlchemyHelper.IngredientType, double> StartCosts;
    public Dictionary<AlchemyHelper.IngredientType, double> BaseCosts;
    public Dictionary<AlchemyHelper.IngredientType, double> ActualCosts;
    public AlchemyHelper.IngredientType Ing1;
    public AlchemyHelper.IngredientType Ing2;
    public AlchemyHelper.IngredientType Ing3;
    public AlchemyHelper.IngredientType Ing4;
    public AlchemyHelper.IngredientType Ing5;
    public double StartCost1;
    public double StartCost2;
    public double StartCost3;
    public double StartCost4;
    public double StartCost5;
    public double BaseCost1;
    public double BaseCost2;
    public double BaseCost3;
    public double BaseCost4;
    public double BaseCost5;
    public double ActualCost1;
    public double ActualCost2;
    public double ActualCost3;
    public double ActualCost4;
    public double ActualCost5;

    public double StartTime;
    public double BaseTime;
    public double ActualTime;
    public DateTime NextProduction;

    public CurrencyHelper.CurrencyType IncomeType;
    public double IncomeAmmount;

    public int Level;
    public int BuyCount;

    public bool CanBuy;

    public AlchemyHelper.LabType LabType;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public ProductInstance(ProductModel model)
    {
        Model = model;

        Id = model.Id;

        NameEN = model.NameEN;
        NamePT = model.NamePT;
        DescriptionEN = model.DescriptionEN;
        DescriptionPT = model.DescriptionPT;

        Ing1 = model.Ing1;
        Ing2 = model.Ing2;
        Ing3 = model.Ing3;
        Ing4 = model.Ing4;
        Ing5 = model.Ing5;
        StartCost1 = model.Cost1;
        StartCost2 = model.Cost2;
        StartCost3 = model.Cost3;
        StartCost4 = model.Cost4;
        StartCost5 = model.Cost5;
        BaseCost1 = model.Cost1;
        BaseCost2 = model.Cost2;
        BaseCost3 = model.Cost3;
        BaseCost4 = model.Cost4;
        BaseCost5 = model.Cost5;
        ActualCost1 = model.Cost1;
        ActualCost2 = model.Cost2;
        ActualCost3 = model.Cost3;
        ActualCost4 = model.Cost4;
        ActualCost5 = model.Cost5;

        StartCosts = new Dictionary<AlchemyHelper.IngredientType, double>();
        if (StartCost1 > 0)
            StartCosts[Ing1] = StartCost1;

        if (StartCost2 > 0)
            StartCosts[Ing2] = StartCost2;

        if (StartCost3 > 0)
            StartCosts[Ing3] = StartCost3;

        if (StartCost4 > 0)
            StartCosts[Ing4] = StartCost4;

        if (StartCost5 > 0)
            StartCosts[Ing5] = StartCost5;

        BaseCosts = new Dictionary<AlchemyHelper.IngredientType, double>();
        if (StartCost1 > 0)
            BaseCosts[Ing1] = StartCost1;

        if (StartCost2 > 0)
            BaseCosts[Ing2] = StartCost2;

        if (StartCost3 > 0)
            BaseCosts[Ing3] = StartCost3;

        if (StartCost4 > 0)
            BaseCosts[Ing4] = StartCost4;

        if (StartCost5 > 0)
            BaseCosts[Ing5] = StartCost5;

        ActualCosts = new Dictionary<AlchemyHelper.IngredientType, double>();
        if (StartCost1 > 0)
            ActualCosts[Ing1] = StartCost1;

        if (StartCost2 > 0)
            ActualCosts[Ing2] = StartCost2;

        if (StartCost3 > 0)
            ActualCosts[Ing3] = StartCost3;

        if (StartCost4 > 0)
            ActualCosts[Ing4] = StartCost4;

        if (StartCost5 > 0)
            ActualCosts[Ing5] = StartCost5;

        StartTime = model.Time;
        BaseTime = model.Time;
        ActualTime = model.Time;

        IncomeType = model.IncomeType;
        IncomeAmmount = model.IncomeAmmount;
            
        Level = model.Level;
        BuyCount = 0;

        CanBuy = false;

        LabType = model.LabType;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public ProductInstance()
    {

    }
}
