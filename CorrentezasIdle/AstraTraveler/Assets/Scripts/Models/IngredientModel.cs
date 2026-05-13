
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ingredient")]
public class IngredientModel : ScriptableObject
{
    public string Id;
    public IngredientHelper.IngredientType Type;
    public GameHelper.ItemRarity Rarity;
    public IngredientHelper.IngredientClass Class;
    public string Image;
    public string Logo;

    public UnlockHelper.UnlockStatus UnlockStatus;
}