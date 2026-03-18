using UnityEngine;
using static GameHelper;

public class EnemyControllerService : MonoBehaviour, ITickable
{
    [SerializeField] TickService tick;
    [SerializeField] ExpeditionService expedition;

    void Start()
    {
        tick.Subscribe(this);
    }

    void OnDestroy()
    {
        tick?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        if (expedition.State != GameState.Running)
            return;

        EnemyDie(dt);
        EnemyDamage(dt);
        EnemyContact(dt);
        EnemyMove(dt);
    }

    void EnemyMove(float dt)
    {
        var enemies = expedition.ActiveEnemies;

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
        }
    }

    void EnemyContact(float dt)
    {
        var enemies = expedition.ActiveEnemies;

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
        var enemies = expedition.ActiveEnemies;

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
                expedition.Ship.CurrentLife -= enemy.Model.Damage;

                enemy.Cooldown = 1.0 / enemy.Model.AttackSpeed;
                enemy.State = EnemyHelper.EnemyState.Cooldown;

                Debug.Log($"{enemy.Model.Name} atacou: {enemy.Model.Damage}");
            }
        }
    }

    void EnemyDie(float dt)
    {
        var enemies = expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            if (enemy.CurrentLife <= 0)
            {
                enemy.State = EnemyHelper.EnemyState.Dead;
                Debug.Log($"Inimigo eliminado {enemy.Model.Name}.");
                expedition.ActiveEnemies.Remove(enemy);
            }
        }
    }
}