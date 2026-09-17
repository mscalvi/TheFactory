using System.Collections.Generic;
using UnityEngine;

public class EnemyService : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float speed = 2f;

    private PathService pathService;
    private List<TileModel> path;

    private GameObject enemyObject;
    private EnemyModel enemyModel;

    private int currentPathIndex;

    public void Initialize(PathService pathService)
    {
        this.pathService = pathService;
        path = new List<TileModel>(pathService.GetPath());

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogError("Não foi possível criar o inimigo: caminho vazio.");
            return;
        }

        enemyModel = new EnemyModel(speed, 100);

        enemyObject = Instantiate(enemyPrefab, transform);

        EnemyView view = enemyObject.GetComponent<EnemyView>();
        view.Initialize(enemyModel);

        currentPathIndex = 0;

        enemyObject.transform.position = GetTilePosition(path[currentPathIndex]);
    }

    private void Update()
    {
        if (enemyObject == null || path == null)
            return;

        MoveEnemy();
    }

    private void MoveEnemy()
    {
        if (currentPathIndex >= path.Count - 1)
            return;

        Vector3 targetPosition = GetTilePosition(path[currentPathIndex + 1]);

        enemyObject.transform.position = Vector3.MoveTowards(
            enemyObject.transform.position,
            targetPosition,
            enemyModel.Speed * Time.deltaTime
        );

        if (enemyObject.transform.position == targetPosition)
        {
            currentPathIndex++;
        }
    }

    private Vector3 GetTilePosition(TileModel tile)
    {
        return new Vector3(
            tile.X + 0.5f,
            tile.Y + 0.5f,
            0f
        );
    }
}