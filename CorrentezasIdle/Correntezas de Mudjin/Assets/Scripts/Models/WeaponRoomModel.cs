using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/WeaponRoom")]
public class WeaponRoomModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public double RangeFactor;
    public double Angle;
    public double AngleMin;
    public double AngleMax;

    // Na Instance
    public RoomHelper.WeaponRoomType Type;
    public RoomHelper.RoomStatus Status;
    public RoomHelper.RoomTarget Target;

    public TripulationModel User;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
