using System.Collections.Generic;
using UnityEngine;

public class EnemyMarkingService : MonoBehaviour
{
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    private UnlockState UnlockState;

    private List<EnemyRuntime> markedEnemies = new();

    public void Initialize(GameState game)
    {
        GameState = game;

        ExpeditionState = GameState.ExpeditionState;

        UnlockState = GameState.UnlockState;
    }

    void HandleClick(EnemyRuntime enemy)
    {
        if (enemy == null) return;

        if (!UnlockState.Click) return;

        if (!enemy.MarkedEnemy && markedEnemies.Count >= ExpeditionState.ActualMaxMarkedEnemies)
        {
            Unmark(markedEnemies[0]);
        }

        if (enemy.MarkedEnemy)
        {
            return;
        }
        else
        {
            Mark(enemy);
        }
    }

    void Mark(EnemyRuntime enemy)
    {
        enemy.MarkedEnemy = true;
        markedEnemies.Add(enemy);
    }

    void Unmark(EnemyRuntime enemy)
    {
        enemy.MarkedEnemy = false;
        markedEnemies.Remove(enemy);
    }

    void HandleDeath(EnemyRuntime enemy)
    {
        if (enemy.MarkedEnemy)
        {
            ExpeditionEvents.OnMarkedEnemyDeath?.Invoke(enemy);
            enemy.MarkedEnemy = false;
            markedEnemies.Remove(enemy);
        }
    }

    void OnEnable()
    {
        ExpeditionEvents.OnEnemyClicked += HandleClick;
        ExpeditionEvents.OnEnemyDeath += HandleDeath;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnEnemyClicked -= HandleClick;
        ExpeditionEvents.OnEnemyDeath -= HandleDeath;
    }
}