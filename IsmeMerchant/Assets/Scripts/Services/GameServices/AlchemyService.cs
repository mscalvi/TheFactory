using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyService : MonoBehaviour
{
    private GameState GameState;
    private IngredientService IngredientService;

    public void Initialize(GameState game, IngredientService ingredients)
    {
        GameState = game;
        IngredientService = ingredients;
    }
}
