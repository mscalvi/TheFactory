using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnlockHelper;

public class ProgressState
{
    // Dados da Campanha
    public int MaxDaysTraveling = 0;

    // Unlocks de Personagens
    public bool Shipbuilder = false;
    public bool Hunter = false;
    public bool Merchant = false;
    public bool Alchemist = false;
    public bool Fisherman = false;
    public bool Coach = false;
    public bool Weaponsmith = false;

    // Unlcoks de Upgrades
    public int UnlockableCompanyUpgrades = 0;
    public int UnlockableExpeditionUpgrades = 0;

    // Unlock de Mecânicas
    public bool Ship = false;
    public bool Alchemy = true;
    public bool Bestiary = true;
    public bool Click = true;
    public bool Ingredients = true;
    public bool Fumac = false;

    // Tutorial
    public bool ExpeditionTut = false;
    public bool ShipTut = false;
    public bool ClickTut = false;
    public bool UpgradesTut = false;
    public bool BuildingsTut = false;
    public bool AlchemyTut = false;
    public bool BestiaryTut = false;
    public bool StartTut = false;
    public bool MarcosTut = false;
    public bool ExperienceTut = false;
    public bool DestinationsTut = false;
    public bool WeaponsTut = false;
    public bool KnowledgeTut = false;
    public bool FumacTut = false;
    public bool ShopTut = false;
}