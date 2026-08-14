using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelper
{
    public enum ExpeditionStatus
    {
        Stopped,
        Paused,
        Running,
        GameOver,
        Finished,
        Loading,
    }

    public enum DecisionType
    {
        GameMode,
        Destination,
        Local,
        Event,
    }

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
        Unique,
    }

    public enum Tutorial
    {
        ExpeditionTut,
        ShipTut,
        ClickTut,
        UpgradesTut,
        BuildingsTut,
        AlchemyTut,
        BestiaryTut,
        StartTut,
        ExperienceTut,
        MarcosTut,
        DestinationsTut,
        WeaponsTut,
        KnowledgeTut
    }
}
