using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientInstance
{
    public IngredientModel Model;

    public string Id;

    public IngredientHelper.IngredientType Type;
    public GameHelper.ItemRarity Rarity;
    public IngredientHelper.IngredientClass Class;

    public string Image;
    public string Logo;

    public double Amount;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public IngredientInstance(IngredientModel model)
    {
        Id = model.Id;

        Type = model.Type;
        Rarity = model.Rarity;
        Class = model.Class;

        Image = "";
        Logo = "";

        Amount = 0;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }
}
