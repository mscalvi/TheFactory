using Unity.VisualScripting;

public class EnemyInstance
{
    public EnemyModel Data;
    public ExpeditionService expedition;

    // Calcular o crescimento das propriedades dos inimigos aqui

    public double Distance;

    public EnemyInstance(EnemyModel data)
    {
        Data = data;
        Distance = expedition.BaseSpawnDistance * data.SpawnDistance;
    }
}