using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/OtherRoom")]
public class OtherRoomModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;


    // Na Instance
    public RoomHelper.RoomStatus Status;

    public TripulationModel User;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
