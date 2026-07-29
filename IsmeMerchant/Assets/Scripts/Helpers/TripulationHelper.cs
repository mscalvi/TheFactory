using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripulationHelper
{
    public enum Type
    {
        Captain,        // Principal
        Shipbuilder,    // Navio
        Hunter,         // Bestiário
        Merchant,       // Gemas
        Alchemist,      // Alquimia
        Weaponsmith,    // Armas?
        Fisherman,      // Ingredientes
        Coach,          // Treinamento
    }

    public enum Status
    {
        InShip,
        InRoom,
        InBase,
        Idle,
    }
}
