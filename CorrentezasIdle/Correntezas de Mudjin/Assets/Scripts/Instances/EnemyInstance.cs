using Unity.VisualScripting;
using UnityEngine;

public class EnemyInstance
{
    public EnemyModel Model;

    // Calcular o crescimento das propriedades dos inimigos aqui

    public double Distance;
    public double Cooldown;
    public double CurrentLife;
    public EnemyHelper.EnemyState State;

    public EnemyInstance(EnemyModel model, double BaseSpawnDistance)
    {
        Model = model;
        Distance = BaseSpawnDistance * model.SpawnDistance;
        Cooldown = 1 / model.AttackSpeed;
        CurrentLife = model.Life;
        State = EnemyHelper.EnemyState.Moving;
    }
}