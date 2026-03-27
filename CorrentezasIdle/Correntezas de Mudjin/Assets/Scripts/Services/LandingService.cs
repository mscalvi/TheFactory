using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    public void Initialize(GameState gameState, DataState db)
    {
        GameState = gameState;
        DataState = db;

        if (GameState.ShipState == null)
        {
            Debug.Log("Nenhum Navio Carregado.");
            CreateInitialState();
            CreateFirstExpedition();
        } else
        {
            // Carregar a configuração anterior
        }

        Debug.Log("LandingService Finalizado");
    }

    public void CreateFirstExpedition()
    {
        GameState.ShipState = new ShipState();

        var ship = DataState.ships.GetValueOrDefault("s001");

        // Weapon Rooms
        foreach (var room in ship.WeaponsRooms)
        {
            room.Tripulation = DataState.tripulations.GetValueOrDefault("t001");
            room.Weapon = DataState.weapons.GetValueOrDefault("w001");
            room.Ammo = DataState.ammos.GetValueOrDefault("a001");
            room.TargetType = RoomHelper.RoomTarget.Closest;
        }

        GameState.ShipState.Ship = ship;

        Debug.Log($"Navio: {GameState.ShipState.Ship.Name} Tripulado e Equipado.");
    }

    public void CreateInitialState()
    {
        CreateCurrencies();
    }

    public void CreateCurrencies()
    {
        GameState.CompanyCurrency = new Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance>();

        foreach (var currency in DataState.currencies) 
        {
            if (currency.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                GameState.CompanyCurrency.Add(currency.Value.Type, currency.Value);

                Debug.Log($"Currency pronta: {currency.Value.Type}");
            }
        }
    }
}
