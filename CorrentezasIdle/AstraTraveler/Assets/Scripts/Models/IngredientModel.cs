
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ingredient")]
public class IngredientModel : ScriptableObject
{
    public string Id;

    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public IngredientHelper.IngredientType Type;

    public GameHelper.ItemRarity Rarity;

    public IngredientHelper.IngredientClass Class;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}