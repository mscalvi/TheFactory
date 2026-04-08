using System.Collections.Generic;
using UnityEngine;

public class EnemyViewService : MonoBehaviour
{
    public Transform Container;
    public Transform Ship;
    public GameObject EnemyPrefab;

    private Dictionary<EnemyInstance, EnemyView> views = new();

    public EnemyView GetView(EnemyInstance enemy)
    {
        return views.TryGetValue(enemy, out var view) ? view : null;
    }

    void SpawnEnemy(EnemyInstance enemy)
    {
        var obj = Instantiate(EnemyPrefab, Container);
        var view = obj.GetComponent<EnemyView>();

        view.Setup(enemy, Ship);

        views[enemy] = view;
    }

    void RemoveEnemy(EnemyInstance enemy)
    {
        if (!views.TryGetValue(enemy, out var view))
            return;

        Destroy(view.gameObject);
        views.Remove(enemy);
    }

    void OnEnable()
    {
        CombatEvents.OnEnemySpawn += SpawnEnemy;
        CombatEvents.OnEnemyDeath += RemoveEnemy;
    }

    void OnDisable()
    {
        CombatEvents.OnEnemySpawn -= SpawnEnemy;
        CombatEvents.OnEnemyDeath -= RemoveEnemy;
    }
}