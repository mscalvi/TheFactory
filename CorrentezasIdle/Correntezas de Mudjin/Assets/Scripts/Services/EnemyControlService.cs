using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static GameHelper;

public class EnemyControllerService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ExpeditionState Expedition;

    public void Initialize(ExpeditionState expeditionState, TickService Tick)
    {
        Expedition = expeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        Debug.Log("EnemyControlService On");
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        EnemyDie(dt);
        EnemyContact(dt);
        EnemyMove(dt);
    }

    void EnemyMove(float dt)
    {
        var enemies = Expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            // Chegou na Área de Ataque ao Navio
            if (enemy.State == EnemyHelper.EnemyState.Moving)
            {
                if (enemy.Distance - enemy.Range > enemy.Speed * dt)
                {
                    enemy.Distance -= enemy.Speed * dt;
                } else
                {
                    enemy.Distance = enemy.Range;
                }

                if (enemy.Distance <= enemy.Range)
                {
                    enemy.State = EnemyHelper.EnemyState.Arrival;
                }
            }
        }
    }

    void EnemyContact(float dt)
    {
        var enemies = Expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            // Chegou na Área de Ataque ao Navio
            if (enemy.State == EnemyHelper.EnemyState.Arrival)
            {
                enemy.Cooldown = 0.1;
                enemy.State = EnemyHelper.EnemyState.Damaging;
            }
        }
    }

    void EnemyDie(float dt)
    {
        var enemies = Expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            if (enemy.CurrentLife <= 0)
            {
                KillEnemy(enemy);
                enemies.RemoveAt(i);
            }
        }
    }

    void KillEnemy(EnemyInstance enemy)
    {
        if (enemy.State == EnemyHelper.EnemyState.Dead)
            return;

        enemy.State = EnemyHelper.EnemyState.Dead;

        CombatEvents.OnEnemyDeath?.Invoke(enemy);
    }
}