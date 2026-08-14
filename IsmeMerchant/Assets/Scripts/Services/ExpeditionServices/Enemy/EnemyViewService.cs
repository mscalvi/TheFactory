using System.Collections.Generic;
using UnityEngine;

public class EnemyViewService : MonoBehaviour
{
    public Transform Container;
    public Transform Ship;
    public GameObject EnemyPrefab;

    private Dictionary<EnemyRuntime, EnemyView> views = new();

    public EnemyView GetView(EnemyRuntime enemy)
    {
        return views.TryGetValue(enemy, out var view) ? view : null;
    }

    void SpawnEnemy(EnemyRuntime enemy)
    {
        var obj = Instantiate(EnemyPrefab, Container);
        var view = obj.GetComponent<EnemyView>();

        view.Setup(enemy, Ship);

        views[enemy] = view;
    }

    void RemoveEnemy(EnemyRuntime enemy, Vector3 position)
    {
        if (!views.TryGetValue(enemy, out var view))
            return;

        Destroy(view.gameObject);

        views.Remove(enemy);
    }

    void OnEnable()
    {
        ExpeditionEvents.OnEnemySpawn += SpawnEnemy;
        ExpeditionEvents.OnEnemyDeath += RemoveEnemy;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnEnemySpawn -= SpawnEnemy;
        ExpeditionEvents.OnEnemyDeath -= RemoveEnemy;
    }
}