using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameHelper;

public class ShipConfigController : MonoBehaviour
{
    public GameDatabase Database;

    public ExpeditionState ExpeditionState;

    public TMPro.TMP_Dropdown ShipDropdown;

    public Transform RoomsParent;
    public WeaponRoomDesign WeaponRoomPrefab;
    public OtherRoomDesign OtherRoomPrefab;

    private List<ShipModel> unlockedShips;
    private List<WeaponRoomDesign> activeWeaponRooms = new List<WeaponRoomDesign>();
    private List<OtherRoomDesign> activeOtherRooms = new List<OtherRoomDesign>();

    // Start is called before the first frame update
    void Start()
    {
        unlockedShips = new List<ShipModel>();

        foreach (var ship in Database.ships)
        {
            if (ship.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
                unlockedShips.Add(ship);
        }

        ShipDropdown.ClearOptions();

        var options = new List<string>();
        foreach (var ship in unlockedShips)
            options.Add(ship.Name);

        ShipDropdown.AddOptions(options);

        ShipDropdown.onValueChanged.AddListener(OnShipSelected);

        if (unlockedShips.Count > 0)
        {
            ShipDropdown.value = 0;
            OnShipSelected(0);
        }
    }

    void OnShipSelected(int index)
    {
        var ship = unlockedShips[index];
        PopulateRooms(ship);
    }

    void ClearRooms()
    {
        foreach (Transform child in RoomsParent)
            Destroy(child.gameObject);

        activeWeaponRooms.Clear();
        activeOtherRooms.Clear();
    }

    void PopulateRooms(ShipModel ship)
    {
        ClearRooms();

        // Weapon Rooms
        foreach (var room in ship.WeaponRooms)
        {
            var roomUI = Instantiate(WeaponRoomPrefab, RoomsParent);
            roomUI.Setup(room.RoomModel, Database);

            activeWeaponRooms.Add(roomUI);
        }

        // Other Rooms
        foreach (var room in ship.OtherRooms)
        {
            var roomUI = Instantiate(OtherRoomPrefab, RoomsParent);
            roomUI.Setup(room.RoomModel, Database);

            activeOtherRooms.Add(roomUI);
        }
    }

    public void ReturnBtn()
    {
        SceneManager.LoadScene("LandingPage");
    }

    public void ConfirmBtn()
    {
        var run = new ExpeditionConfiguration();

        // Ship selecionado
        run.Ship = unlockedShips[ShipDropdown.value];

        // Weapon Rooms
        foreach (var roomUI in activeWeaponRooms)
        {
            var config = new RoomConfiguration();

            config.RoomId = roomUI.name;
            config.Tripulation = roomUI.GetSelectedTripulation();
            config.Weapon = roomUI.GetSelectedWeapon();
            config.Ammo = roomUI.GetSelectedAmmo();

            run.Rooms.Add(config);
        }

        // Other Rooms
        foreach (var roomUI in activeOtherRooms)
        {
            var config = new RoomConfiguration();

            config.RoomId = roomUI.name;
            config.Tripulation = roomUI.GetSelectedTripulation();

            run.Rooms.Add(config);
        }

        ExpeditionState.CurrentExpedition = run;

        SceneManager.LoadScene("LandingPage");
    }
}
