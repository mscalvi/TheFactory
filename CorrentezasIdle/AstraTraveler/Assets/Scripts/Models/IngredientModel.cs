
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ingredient")]
public class IngredientModel : ScriptableObject
{
    public string Id;
    public IngredientHelper.IngredientType Type;
    public IngredientHelper.IngredientRarity Rarity;
    public IngredientHelper.IngredientClass Class;
    public string Image;
    public string Logo;

    public UnlockHelper.UnlockStatus UnlockStatus;
}