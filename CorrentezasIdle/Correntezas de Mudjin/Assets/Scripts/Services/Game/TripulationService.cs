using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripulationService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }
    public void AddTripulationToCrew(TripulationInstance crew)
    {
        Debug.Log($"Game TripulationService - Tentando Adicionar {crew.Name}");

        if (crew == null)
            return;

        GameState.TripulationState.InactiveTripulation.Add(crew);

        GameEvents.OnTripulationChange?.Invoke(crew);
    }


    public void AddTripulationToActive(TripulationInstance crew)
    {
        if (crew == null)
            return;

        if (GameState.TripulationState.ActiveTripulation.Count >= GameState.ShipState.Ship.MaxNamedTripulation)
            return;

        GameState.TripulationState.ActiveTripulation.Add(crew);
        GameState.TripulationState.InactiveTripulation.Remove(crew);

        GameEvents.OnTripulationChange?.Invoke(crew);
    }

    public void AddUnnamedTripulation(TripulationInstance crew)
    {
        if (crew == null)
            return;

        if (GameState.ShipState.Ship.ActualUnnamedTripulation >= GameState.ShipState.Ship.MaxUnnamedTripulation)
            return;

        GameState.ShipState.Ship.ActualUnnamedTripulation++;
    }

    public void AlocTripulationInWeaponRoom(TripulationInstance crew, WeaponRoomInstance room)
    {
        if (crew == null || room == null)
            return;

        if (GameState.TripulationState.TripulationWeaponAssignment.ContainsKey(crew))
            return;

        if (GameState.TripulationState.TripulationWeaponAssignment.ContainsValue(room))
            return;

        GameState.TripulationState.TripulationWeaponAssignment.Add(crew, room);
    }

    public void RemoveTripulationFromWeaponRoom(TripulationInstance crew, WeaponRoomInstance room)
    {
        if (crew == null || room == null)
            return;

        if (!GameState.TripulationState.TripulationWeaponAssignment.ContainsKey(crew))
            return;

        if (!GameState.TripulationState.TripulationWeaponAssignment.ContainsValue(room))
            return;

        GameState.TripulationState.TripulationWeaponAssignment.Remove(crew);
    }

    public void FreeWeaponRoom(WeaponRoomInstance room)
    {
        if (!GameState.TripulationState.TripulationWeaponAssignment.ContainsValue(room))
            return;

        foreach (var ocupiedRoom in GameState.TripulationState.TripulationWeaponAssignment)
        {
            if (ocupiedRoom.Value == room)
            {
                GameState.TripulationState.TripulationWeaponAssignment.Remove(ocupiedRoom.Key);
            }
        }

    }
}
