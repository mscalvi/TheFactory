using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestiaryTrackerService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    private void BestiaryTracker(EnemyRuntime enemy, Vector3 position)
    {
        var Bestiary = GameState.BestiaryState.Bestiary;

        if (!Bestiary.TryGetValue(enemy.Id, out var entry))
            return;

        entry.KilledExpedition++;
        entry.KilledTotal++;
    }

    // Events

    void OnEnable()
    {
        ExpeditionEvents.OnEnemyDeath += BestiaryTracker;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnEnemyDeath -= BestiaryTracker;
    }
}
