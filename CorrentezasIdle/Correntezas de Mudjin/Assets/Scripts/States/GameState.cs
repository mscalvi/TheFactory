using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // Estado atual do jogo, no geral. Salvo e atualizado sempre, nunca destruído. Funciona como backup para o PlayerPrefs, enquanto o jogo não for fechado

    public DataState DataState;
    public ShipState ShipState;
    public ExpeditionState ExpeditionState;

    public Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance> CompanyCurrency;
    public Dictionary<string, UpgradeInstance> CompanyUpgrades;

    public GameHelper.ExpeditionStatus ExpeditionStatus;
    public bool FirstExpedition = true;
}
