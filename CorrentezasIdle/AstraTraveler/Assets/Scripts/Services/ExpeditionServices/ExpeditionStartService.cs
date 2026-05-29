using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameHelper;

public class ExpeditionStartService : MonoBehaviour
{

    private GameState GameState;

    private PathService PathService;

    public void Initialize(GameState game, PathService path)
    {
        GameState = game;

        PathService = path;
               
        LoadExpedition(GameState);
        LoadShip(GameState);
        PathService.GenerateNextDestination();
    }

    void LoadExpedition(GameState Game)
    {
        var Expedition = Game.ExpeditionState;

        Expedition.DayCounter = Expedition.StartDay;
        Expedition.IsDay = true;
        Expedition.NextDestination = 0;
        Expedition.ActualDestination = 0;
        Expedition.ReachedDestinations = 0;
        Expedition.LastPath = new PathHelper.PathTagSet();
        Expedition.ActualPath = new PathHelper.PathTagSet();
        Expedition.phaseTimer = 0f;

        Expedition.ActiveEnemies.Clear();
        Expedition.DamageTaken = false;

        Expedition.ActualIngredientRarityWeights = Expedition.BaseIngredientRarityWeights;

        Expedition.ActualSpawnChance = Expedition.BaseSpawnChance;
        Expedition.ActualSpawnInterval = Expedition.BaseSpawnInterval;
        Expedition.ActualSpawnBudget = Expedition.BaseSpawnBudget;
        Expedition.ActualSpawnBudgetGrowth = Expedition.BaseSpawnBudgetGrowth;
        Expedition.ActualBossThreshold = Expedition.BaseBossThreshold;

        Expedition.ActualStartExperience = Expedition.BaseStartExperience;
        Expedition.ActualExperienceKillBonus = Expedition.BaseExperienceKillBonus;
        Expedition.ActualDayReward = Expedition.BaseDayReward;
        Expedition.ActualNightReward = Expedition.BaseNightReward;
        Expedition.ActualDestinationReward = Expedition.BaseDestinationReward;

        Expedition.ActualMaxMarkedEnemies = Expedition.BaseMaxMarkedEnemies;
        Expedition.ActualMaxMarkedLoot = Expedition.BaseMaxMarkedLoot;
        Expedition.ActualNextLootChance = Expedition.BaseNextLootChance;
        Expedition.ActualNextLootDecay = Expedition.BaseNextLootDecay;

        Expedition.ActualDestinationGapIncrease = Expedition.BaseDestinationGapIncrease;
        Expedition.ActualMaximalDestinationGap = Expedition.BaseMaximalDestinationGap;
        Expedition.ActualMinimalDestinationGap = Expedition.BaseMinimalDestinationGap;

        foreach (var upgrade in GameState.DataState.upgrades.Values)
        {
            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                upgrade.ActualBuy = 0;
                upgrade.ActualCost = upgrade.BaseCost;
                upgrade.ActualUpgradeValue = upgrade.BaseUpgradeValue;

                if (upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Finished)
                {
                    upgrade.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                }

                upgrade.CurrentValue = upgrade.StartUpgradeValue;
            }
        }

        GameState.UpgradesState.Modifiers.RemoveAll(
            x => x.Scope == UpgradeHelper.UpgradeScope.Expedition
        );
    }

    void LoadShip(GameState Game)
    {
        Game.ExpeditionState.Ship.ActualArmor = Game.ExpeditionState.Ship.BaseArmor;
        Game.ExpeditionState.Ship.ActualResistence = Game.ExpeditionState.Ship.BaseResistence;

        Game.ExpeditionState.Ship.ActualLife = Game.ExpeditionState.Ship.BaseLife;
        Game.ExpeditionState.Ship.ActualRepairPerTripulation = Game.ExpeditionState.Ship.BaseRepairPerTripulation;

        Game.ExpeditionState.Ship.ActualSpeed = Game.ExpeditionState.Ship.BaseSpeed;

        foreach (var weapon in Game.ExpeditionState.Ship.Weapons)
        {
            weapon.ActualDamage = weapon.BaseDamage;
            weapon.ActualAttackSpeed = weapon.BaseAttackSpeed;
            weapon.ActualRange = weapon.BaseRange;
            weapon.ActualCriticalDamage = weapon.BaseCriticalDamage;
            weapon.ActualPrecision = weapon.BasePrecision;
            weapon.Ammo.ActualAmmount = weapon.Ammo.BaseAmmount;
            weapon.Ammo.ActualDamage = weapon.Ammo.BaseDamage;
            weapon.Ammo.ActualProjectileSpeed = weapon.Ammo.BaseProjectileSpeed;
            weapon.Ammo.ActualRecharge = weapon.Ammo.BaseRecharge;
            weapon.Ammo.CurrentRecharge = weapon.Ammo.BaseRecharge;
            weapon.CurrentTarget = null;
        }

        Game.ExpeditionState.Ship.CurrentLife = Game.ExpeditionState.Ship.ActualLife;
    }
}
