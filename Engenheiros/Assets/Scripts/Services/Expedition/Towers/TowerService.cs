using UnityEngine;

public class TowerService : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;

    private MapService mapService;

    public void Initialize(MapService mapService)
    {
        this.mapService = mapService;
    }

    private void TryPlaceTower(TileModel tile)
    {
        if (tile == null)
            return;

        if (tile.Type != TileHelper.TileType.Ground)
            return;

        if (tile.OccupiedTile)
            return;

        TowerModel model = new TowerModel();

        GameObject towerObject = Instantiate(
            towerPrefab,
            new Vector3(tile.X + 0.5f, tile.Y + 0.5f, 0f),
            Quaternion.identity
        );

        TowerView view = towerObject.GetComponent<TowerView>();

        view.Initialize(model);

        tile.OccupiedTile = true;
    }

    private void OnEnable()
    {
        ExpeditionEvents.TileClicked += TryPlaceTower;
    }

    private void OnDisable()
    {
        ExpeditionEvents.TileClicked -= TryPlaceTower;
    }
}