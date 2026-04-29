using System.Collections.Generic;
using UnityEngine;

public class EnemyMarkingService : MonoBehaviour
{
    private List<EnemyInstance> markedEnemies = new();

    private ExpeditionState ExpeditionState;

    private UnlockState UnlockState;

    public void Initialize(ExpeditionState expedition, UnlockState unlock)
    {
        ExpeditionState = expedition;

        UnlockState = unlock;
    }

    void HandleClick(EnemyInstance enemy)
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

    void Mark(EnemyInstance enemy)
    {
        enemy.MarkedEnemy = true;
        markedEnemies.Add(enemy);
    }

    void Unmark(EnemyInstance enemy)
    {
        enemy.MarkedEnemy = false;
        markedEnemies.Remove(enemy);
    }

    void HandleDeath(EnemyInstance enemy)
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