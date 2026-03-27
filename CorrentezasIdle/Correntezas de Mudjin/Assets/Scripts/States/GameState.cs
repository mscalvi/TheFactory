using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public DataState DataState;

    // Estado atual do jogo, no geral. Salvo e atualizado sempre, nunca destruído. Funciona como backup para o PlayerPrefs, enquanto o jogo não for fechado

    public bool FirstExpedition = true;

    public ShipState ShipState;
    public ExpeditionState ExpeditionState;

    //public ShipInitialConfiguration ShipInitialConfiguration;

    public GameHelper.ExpeditionStatus ExpeditionStatus;

    // Currency
    public Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance> CompanyCurrency;
}
