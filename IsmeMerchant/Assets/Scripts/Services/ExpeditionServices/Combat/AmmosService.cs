using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmosService : MonoBehaviour, ITickable
{
    private GameState GameState;

    private TickService TickService;

    public void Initialize(GameState gameState, TickService tick)
    {
        GameState = gameState;

        TickService = tick;

        TickService.Subscribe(this);

        GameState.ExpeditionState.ActiveAmmos = new List<AmmoInstance>();

        foreach (var Weapon in GameState.ExpeditionState.Ship.Weapons)
        {
            if (Weapon == null || Weapon.Ammo == null)
                continue;

            GameState.ExpeditionState.ActiveAmmos.Add(Weapon.Ammo);
        }
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        foreach (var ammo in GameState.ExpeditionState.ActiveAmmos)
        {
            if (!ammo.IsReloading)
            {
                continue;
            }

            ammo.CurrentRecharge -= dt;
            ExpeditionEvents.OnRechargeProgress?.Invoke(ammo);

            if (ammo.CurrentRecharge <= 0)
            {
                ammo.IsReloading = false;
                ammo.CurrentAmmount = ammo.ActualAmmount;
                ammo.CurrentRecharge = ammo.ActualRecharge;
                ExpeditionEvents.OnRechargeEnd?.Invoke(ammo);
            }
        }
    }
}
