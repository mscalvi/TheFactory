using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class StartExpeditionService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        LoadExpedition(GameState);
        LoadShip(GameState);

        Debug.Log("ShipControlService On");
    }

    void LoadShip(GameState Game)
    {
        var ShipConfiguration = Game.ShipInitialConfiguration;

        ShipInstance activeShip = new ShipInstance(ShipConfiguration.Ship);

        Game.ShipState.Ship = activeShip;

        Game.ShipState.WeaponRooms = new List<WeaponRoomInstance>();

        for (int i = 0; i < activeShip.Model.WeaponRooms.Count; i++)
        {
            var modelRoom = activeShip.Model.WeaponRooms[i];
            var instanceRoom = new WeaponRoomInstance(modelRoom.WeaponRoomModel);

            if (ShipConfiguration.WeaponRooms != null && i < ShipConfiguration.WeaponRooms.Count)
            {
                var roomConfig = ShipConfiguration.WeaponRooms[i];

                instanceRoom.Weapon = roomConfig.Weapon;
                instanceRoom.Tripulation = roomConfig.Tripulation;
                instanceRoom.Ammo = roomConfig.Ammo;
                instanceRoom.TargetType = roomConfig.TargetType;

                instanceRoom.Setup();
            }

            Game.ShipState.WeaponRooms.Add(instanceRoom);
        }

        // Repetir para Others
        Debug.Log($"Navio Ativo: {Game.ShipState.Ship.Model.Name}");
        Debug.Log($"Rooms carregadas: {Game.ShipState.WeaponRooms.Count}");
    }

    void LoadExpedition(GameState Game)
    {
        Game.ExpeditionState = new ExpeditionState();

        Debug.Log($"Expedition Ativa");
    }
}
