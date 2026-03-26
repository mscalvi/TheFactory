using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static GameHelper;

public class EnemyControllerService : MonoBehaviour, ITickable
{
    private GameDatabase DataBase;
    private TickService TickService;
    private ExpeditionUiService UiService;
    private ExpeditionState Expedition;

    public void Initialize(ExpeditionState expeditionState, TickService Tick, ExpeditionUiService ui)
    {
        Expedition = expeditionState;

        TickService = Tick;

        UiService = ui;

        TickService.Subscribe(this);

        Debug.Log("EnemyControlSystem On");
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

            UiService.EnemiesTotalSet();
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
                enemy.State = EnemyHelper.EnemyState.Dead;
                Debug.Log($"Inimigo eliminado {enemy.Name}.");
                Expedition.ActiveEnemies.Remove(enemy);
                UiService.EnemiesTotalSet();
            }
        }
    }
}