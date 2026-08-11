using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductInstance
{
    public ProductModel Model;

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
    public double Income;

    public int Level;

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
        Cost1 = model.Cost1;
        Cost2 = model.Cost2;
        Cost3 = model.Cost3;
        Cost4 = model.Cost4;
        Cost5 = model.Cost5;

        Time = model.Time;

        Income = model.Income;
        Level = model.Level;

        CanBuy = false;

        LabType = model.LabType;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public ProductInstance()
    {

    }
}
