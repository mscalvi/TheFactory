using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static GameHelper;

public class EnemyControllerService : MonoBehaviour, ITickable
{
    private GameDatabase DataBase;
    private TickService TickService;
    private ExpeditionState Expedition;

    [SerializeField] TextMeshProUGUI AliveEnemies;
    [SerializeField] TextMeshProUGUI KilledEnemies;

    int Alive = 0;
    int Killed = 0;

    public void Initialize(ExpeditionState expeditionState, TickService Tick)
    {
        Expedition = expeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        Debug.Log("EnemyControlSystem On");

        AliveEnemies.text = "Nenhum Inimigo Vivo.";
        KilledEnemies.text = "Nenhum Inimigo Eliminado.";
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        EnemyDie(dt);
        EnemyDamage(dt);
        EnemyContact(dt);
        EnemyMove(dt);
    }

    void EnemyMove(float dt)
    {
        Alive = 0;

        var enemies = Expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            // Chegou na Área de Ataque ao Navio
            if (enemy.State == EnemyHelper.EnemyState.Moving)
            {
                if (enemy.Distance - enemy.Model.Range > enemy.Model.Speed * dt)
                {
                    enemy.Distance -= enemy.Model.Speed * dt;
                } else
                {
                    enemy.Distance = enemy.Model.Range;
                }

                if (enemy.Distance <= enemy.Model.Range)
                {
                    enemy.State = EnemyHelper.EnemyState.Arrival;
                }
            }

            if (enemy.State != EnemyHelper.EnemyState.Dead)
            {
                Alive++;
                AliveEnemies.text = "Vivos: " + Alive;
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
                Debug.Log($"{enemy.Model.Name} preparado para atacar.");
            }
        }
    }

    void EnemyDamage(float dt)
    {
        var enemies = Expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            enemy.Cooldown -= dt;

            if (enemy.State == EnemyHelper.EnemyState.Cooldown && enemy.Cooldown <= 0)
            {
                enemy.State = EnemyHelper.EnemyState.Damaging;
            }

            if (enemy.State == EnemyHelper.EnemyState.Damaging)
            {
                Expedition.Ship.CurrentLife -= enemy.Model.Damage;

                enemy.Cooldown = 1.0 / enemy.Model.AttackSpeed;
                enemy.State = EnemyHelper.EnemyState.Cooldown;

                Debug.Log($"{enemy.Model.Name} atacou: {enemy.Model.Damage}");
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
                Debug.Log($"Inimigo eliminado {enemy.Model.Name}.");
                Expedition.ActiveEnemies.Remove(enemy);

                // Mover pra UI
                Killed++;
                KilledEnemies.text = "Eliminados: " + Killed;
                Alive--;
                AliveEnemies.text = "Vivos: " + Alive;
            }
        }
    }
}