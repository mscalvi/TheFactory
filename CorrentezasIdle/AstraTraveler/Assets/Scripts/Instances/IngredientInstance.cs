using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientInstance
{
    public IngredientModel Model;

    public string Id;

    public IngredientHelper.IngredientType Type;
    public IngredientHelper.IngredientRarity Rarity;
    public IngredientHelper.IngredientClass Class;

    public string Image;
    public string Logo;

    public double Amount;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public IngredientInstance(IngredientModel model)
    {
        Id = model.Id;

        Type = model.Type;
        Rarity = model.Rarity;
        Class = model.Class;

        Image = model.Image;
        Logo = model.Logo;

        Amount = 0;

        UnlockStatus = model.UnlockStatus;
    }

    public IngredientInstance(IngredientInstance model)
    {
        Id = model.Id;
        Type = model.Type;
        Rarity = model.Rarity;
        Class = model.Class;

        Image = model.Image;
        Logo = model.Logo;

        Amount = 0;

        UnlockStatus = model.UnlockStatus;
    }
}
