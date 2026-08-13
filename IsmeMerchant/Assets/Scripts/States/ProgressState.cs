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
    public bool Studies = false;
    public bool Company = false;
    public bool Constructions = false;
    public bool Training = false;
    public bool Ship = false;
    public bool Alchemy = false;

    public bool Missions = false;
    public bool Bestiary = false;

    public bool Click = true;
    public bool Ingredients = true;
    public bool Recruiting = false;

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
}