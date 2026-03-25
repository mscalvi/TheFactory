using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipState
{
    public ShipInstance Ship;

    public List<WeaponRoomInstance> WeaponRooms = new();

    // Controlador do Navio
    public double BaseTicksPerRepair = 75;
}
