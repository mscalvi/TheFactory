using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripulationState
{
    public List<TripulationInstance> ActiveTripulation = new List<TripulationInstance>();

    public Dictionary<TripulationInstance, WeaponRoomInstance> TripulationWeaponAssignment = new Dictionary<TripulationInstance, WeaponRoomInstance>();
    //public Dictionary<TripulationInstance, OtherRoomInstance> TripulationOtherAssignment = new Dictionary<TripulationInstance, OtherRoomInstance>();
}

