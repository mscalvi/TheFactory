using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientInstance
{
    public IngredientModel Model;

    public string Id;
    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public IngredientHelper.IngredientType Type;
    public GameHelper.ItemRarity Rarity;
    public IngredientHelper.IngredientClass Class;

    public double Amount;
    public int VisualAmount => (int)Amount;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public IngredientInstance(IngredientModel model)
    {
        Model = model;

        Id = model.Id;

        NamePT = model.NamePT;
        NameEN = model.NameEN;
        DescriptionPT = model.DescriptionPT;
        DescriptionEN = model.DescriptionEN;

        Type = model.Type;
        Rarity = model.Rarity;
        Class = model.Class;

        Amount = 0;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public IngredientInstance()
    {

    }
}
