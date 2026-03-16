using UnityEngine;

[CreateAssetMenu(menuName = "Game/Game Database")]
public class GameDatabase : ScriptableObject
{
    public ShipModel[] ships;
    public EnemyModel[] enemies;
}