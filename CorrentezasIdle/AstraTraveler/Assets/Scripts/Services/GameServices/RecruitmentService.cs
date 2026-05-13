using System.Collections.Generic;
using UnityEngine;

public class RecruitmentService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
        DataState = gameState.DataState;
    }

    public void GenerateRecruitOptions()
    {
        var ship = GameState.ExpeditionState.Ship;

        if (ship.ActiveRecruits.Count > 0)
            return;

        List<TripulationInstance> available = new();

        foreach (var trip in DataState.tripulations.Values)
        {
            if (trip.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                available.Add(trip);
            }
        }

        Shuffle(available);

        int amount = Mathf.Min(4, available.Count);

        for (int i = 0; i < amount; i++)
        {
            ship.ActiveRecruits.Add(available[i]);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);

            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}