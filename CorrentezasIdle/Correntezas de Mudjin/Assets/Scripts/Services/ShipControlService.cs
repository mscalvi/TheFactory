using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static GameHelper;

public class ShipControlService : MonoBehaviour, ITickable
{
    [SerializeField] TickService tick;
    [SerializeField] ExpeditionState expedition;

    EnemyInstance SelectedTarget = null;

    double currentLife;

    // Start is called before the first frame update
    void Start()
    {
        tick.Subscribe(this);
        currentLife = expedition.Ship.CurrentLife;
        // expedition.Ship.Cooldown = 0;
        Debug.Log($"Navio On. Vida atual: {expedition.Ship.CurrentLife}");
    }

    void OnDestroy()
    {
        tick?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        ReceiveDamage(dt);
        RepairLife(dt);

        ShootTarget(dt);
    }

    public void ReceiveDamage(float dt)
    {
        if (expedition.Ship.CurrentLife < currentLife)
        {
            Debug.Log($"Vida atual: {expedition.Ship.CurrentLife}.");
            currentLife = expedition.Ship.CurrentLife;
        }

        if (expedition.Ship.CurrentLife <= 0) 
        {
            expedition.Ship.CurrentLife = 0;
            // expedition.EndExpedition();
        }
    }

    public void SelectClosestTarget(float dt)
    {
        var enemies = expedition.ActiveEnemies;

        SelectedTarget = null;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            if (enemy.State == EnemyHelper.EnemyState.Dead)
            {
                continue;
            }

            //if (enemy.Distance <= expedition.Ship.Range)
            //{
            //    if (SelectedTarget == null)
            //    {
            //        SelectedTarget = enemy;
            //        Debug.Log("Novo inimigo selecionado");
            //    }
            //    else if (enemy.Distance < SelectedTarget.Distance)
            //    {
            //        Debug.Log("Outro inimigo selecionado");
            //        SelectedTarget = enemy;
            //    }
            //}
        }
    }

    public void RepairLife(float dt)
    {

    }

    public void ShootTarget(float dt)
    {
        //expedition.Ship.Cooldown -= dt;

        //if (expedition.Ship.Cooldown > 0)
        //    return;

        //SelectClosestTarget(dt);

        //if (SelectedTarget != null)
        //{
        //    Debug.Log($"Alvo selecionado: {SelectedTarget.Model.Name}");
        //}

        //if (SelectedTarget == null ||
        //    SelectedTarget.State == EnemyHelper.EnemyState.Dead ||
        //    SelectedTarget.CurrentLife <= 0)
        //{
        //    SelectedTarget = null;
        //    return;
        //}

        //SelectedTarget.CurrentLife -= expedition.Ship.Damage;
        //Debug.Log($"Navio atirou em um {SelectedTarget.Model.Name}, deixando-o com {SelectedTarget.CurrentLife} de vida.");
        //expedition.Ship.Cooldown = 1.0 / expedition.Ship.AttackSpeed;
    }
}
