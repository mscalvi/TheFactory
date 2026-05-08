using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripulationHelper
{
    [Flags]
    public enum Jobs
    {
        None = 0,
        Shooter = 1 << 0,
        Merchant = 1 << 1,
        Cartographer = 1 << 2,
        Sailor = 1 << 3,
    }

    public enum Type
    {
        Captain,    // Principal
        Shipbuilder,// Navio     
        Hunter,     // Bestiário
        Merchant,   // Gemas
        Alchemist,  // Alquimia
        Weaponsmith,// Armas?
        Fisherman,  // Ingredientes
        Coach,      // Treinamento
    }

    public enum Status
    {
        InShip,
        InRoom,
        InBase,
        Idle,
    }
}
