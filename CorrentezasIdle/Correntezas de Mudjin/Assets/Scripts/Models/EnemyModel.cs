using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy")]
public class EnemyModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public bool DayEnemy;
    public bool BossEnemy;

    public double LifeTotal;
    public double LifeRegen;
    public double Damage;
    public double AtackSpeed;
    public double Range;
    public double MovimentSpeed;
}
