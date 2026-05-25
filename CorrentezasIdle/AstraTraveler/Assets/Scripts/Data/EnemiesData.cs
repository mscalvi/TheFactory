using System.Collections.Generic;
using UnityEngine;

public static class EnemiesData
{
    public static Dictionary<string, EnemyModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Enemies");

        foreach (var row in rows)
        {
            EnemyModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.DayEnemy = ParseHelper.Bool(row["DayEnemy"]);
            model.BossEnemy = ParseHelper.Bool(row["BossEnemy"]);

            model.EnemyType =
                ParseHelper.Enum<EnemyHelper.EnemyType>(
                    row["EnemyType"]
                );

            model.Life = ParseHelper.Double(row["Life"]);
            model.LifeGrowth = ParseHelper.Double(row["LifeGrowth"]);

            model.LifeRegen = ParseHelper.Double(row["LifeRegen"]);
            model.LifeRegenGrowth = ParseHelper.Double(row["LifeRegenGrowth"]);

            model.Speed = ParseHelper.Double(row["Speed"]);
            model.SpeedGrowth = ParseHelper.Double(row["SpeedGrowth"]);

            model.Armor = ParseHelper.Double(row["Armor"]);
            model.ArmorGrowth = ParseHelper.Double(row["ArmorGrowth"]);

            model.Range = ParseHelper.Double(row["Range"]);
            model.RangeGrowth = ParseHelper.Double(row["RangeGrowth"]);

            model.Damage = ParseHelper.Double(row["Damage"]);
            model.DamageGrowth = ParseHelper.Double(row["DamageGrowth"]);

            model.AttackSpeed = ParseHelper.Double(row["AttackSpeed"]);
            model.AttackSpeedGrowth = ParseHelper.Double(row["AttackSpeedGrowth"]);

            model.SpawnDistance = ParseHelper.Double(row["SpawnDistance"]);
            model.SpawnDistanceGrowth = ParseHelper.Double(row["SpawnDistanceGrowth"]);

            model.Experience = ParseHelper.Double(row["ExperienceKill"]);

            model.Special =
                ParseHelper.Enum<EnemyHelper.EnemySpecial>(
                    row["Special"]                    
                );

            model.CommonIngredient =
                ParseHelper.Enum<IngredientHelper.IngredientType>(
                    row["CommonIngredient"]
                );

            model.UncommonIngredient =
                ParseHelper.Enum<IngredientHelper.IngredientType>(
                    row["UncommonIngredient"]
                );

            model.RareIngredient =
                ParseHelper.Enum<IngredientHelper.IngredientType>(
                    row["RareIngredient"]
                );

            model.LegendaryIngredient =
                ParseHelper.Enum<IngredientHelper.IngredientType>(
                    row["LegendaryIngredient"]
                );

            model.Rarity = ParseHelper.Double(row["Rarity"]);

            model.SpawnCost = ParseHelper.Double(row["SpawnCost"]);

            model.Stage = new();

            if (!string.IsNullOrWhiteSpace(row["Stage"]))
            {
                string[] stages = row["Stage"].Split('|');

                model.Stage =
                    ParseHelper.EnumList<EnemyHelper.EnemyStage>(
                        row["Stage"]
                    );
            }

            model.PathTypes = new();

            if (!string.IsNullOrWhiteSpace(row["PathTypes"]))
            {
                string[] types = row["PathTypes"].Split('|');

                model.PathTypes =
                    ParseHelper.EnumFlags<PathHelper.PathType>(
                        row["PathTypes"]
                    );
            }

            model.PathEnvironments = new();

            if (!string.IsNullOrWhiteSpace(row["PathEnvironments"]))
            {
                string[] envs = row["PathEnvironments"].Split('|');

                model.PathEnvironments =
                    ParseHelper.EnumFlags<PathHelper.PathEnvironment>(
                        row["PathEnvironments"]
                    );
            }

            model.PathModifiers = new();

            if (!string.IsNullOrWhiteSpace(row["PathModifiers"]))
            {
                string[] mods = row["PathModifiers"].Split('|');

                model.PathModifiers =
                    ParseHelper.EnumFlags<PathHelper.PathModifier>(
                        row["PathModifiers"]
                    );
            }

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus =
                ParseHelper.Enum<UnlockHelper.UnlockStatus>(
                    row["UnlockStatus"]
                );

            All[model.Id] = model;
        }
    }
}