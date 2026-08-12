using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmosService : MonoBehaviour, ITickable
{
    private GameState GameState;

    private TickService TickService;

    private Dictionary<AmmoInstance, WeaponInstance> ActiveWeapons;

    public void Initialize(GameState gameState, TickService tick)
    {
        GameState = gameState;

        TickService = tick;

        TickService.Subscribe(this);

        GameState.ExpeditionState.ActiveAmmos = new List<AmmoInstance>();
        ActiveWeapons = new Dictionary<AmmoInstance, WeaponInstance>();

        foreach (var Weapon in GameState.ExpeditionState.Ship.Weapons)
        {
            if (Weapon == null || Weapon.Ammo == null)
                continue;

            GameState.ExpeditionState.ActiveAmmos.Add(Weapon.Ammo);
            ActiveWeapons.Add(Weapon.Ammo, Weapon);
        }
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        foreach (var weapon in ActiveWeapons)
        {
            if (!weapon.Key.IsReloading)
            {
                continue;
            }

            weapon.Key.CurrentRecharge -= dt;
            ExpeditionEvents.OnRechargeProgress?.Invoke(weapon.Value);

            if (weapon.Key.CurrentRecharge <= 0)
            {
                weapon.Key.IsReloading = false;
                weapon.Key.CurrentAmmount = weapon.Key.ActualAmmount;
                weapon.Key.CurrentRecharge = weapon.Key.ActualRecharge;
                ExpeditionEvents.OnRechargeEnd?.Invoke(weapon.Value);
            }
        }
    }
}
