using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionWeaponDefinition : MonoBehaviour
{
    public TMP_Text WeaponName;
    public TMP_Text WeaponDamage;
    public TMP_Text WeaponAtackSpeed;
    public TMP_Text WeaponRange;
    public TMP_Text WeaponSpecial;
    public TMP_Text WeaponCriticalDamage;
    public TMP_Text WeaponCriticalChance;

    public TMP_Text AmmoName;
    public TMP_Text AmmoDamage;
    public TMP_Text AmmoSpeed;
    public TMP_Text AmmoCurrentAmmount;
    public TMP_Text AmmoMaxAmmount;
    public TMP_Text TimeText;

    public GameObject RechargeContainer;
    public Slider RechargeBar;

    private WeaponInstance Weapon;
    private GameState GameState;

    public void Setup(WeaponInstance weapon, GameState game)
    {
        Weapon = weapon;
        GameState = game;

        if (weapon == null)
        {
            gameObject.SetActive(false);
            return;
        }

        TextChange();

        if (Weapon.Ammo != null)
        {
            AmmoCurrentAmmount.text = weapon.Ammo.CurrentAmmount.ToString();
            AmmoMaxAmmount.text = weapon.Ammo.ActualAmmount.ToString();

            string time = FormatTime(Weapon.Ammo.ActualRecharge);
            TimeText.text = time;
        } else
        {
            AmmoCurrentAmmount.text = "-";
            AmmoMaxAmmount.text = "-";
            TimeText.text = "-:-";
        }
    }

    string FormatTime(double seconds)
    {
        if (seconds < 0) seconds = 0;

        TimeSpan t = TimeSpan.FromSeconds(seconds);

        if (t.TotalHours >= 1)
            return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds / 10:D2}";

        if (t.TotalMinutes >= 1)
            return $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds / 10:D2}";

        return $"{t.Seconds:D2}:{t.Milliseconds / 10:D2}";
    }

    void OnStart(AmmoInstance ammo)
    {
        if (ammo == null)
            return;

        if (Weapon == null || Weapon.Ammo == null)
            return;

        if (ammo.Id != Weapon.Ammo.Id) return;

        // Efeitos
    }

    void TextChange()
    {
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            WeaponName.text = Weapon.NameEN;
            WeaponDamage.text = "Damage: " + Weapon.ActualDamage.ToString("F2");
            WeaponAtackSpeed.text = "Speed: " + Weapon.ActualAttackSpeed.ToString("F2");
            WeaponRange.text = "Range: " + Weapon.ActualRange.ToString("F2");
            WeaponSpecial.text = "Special: " + Weapon.Special.ToString();
            WeaponCriticalDamage.text = "Crit.: " + Weapon.ActualCriticalDamage.ToString("F2");
            WeaponCriticalChance.text = "Crit. Chance: " + (Weapon.ActualPrecision * 100).ToString("F2");

            if (Weapon.Ammo != null)
            {
                AmmoName.text = Weapon.Ammo.NameEN;

                AmmoDamage.text = "Damage: " + Weapon.Ammo.ActualDamage.ToString("F2");
                AmmoSpeed.text = "Projectile Speed: " + Weapon.Ammo.ActualProjectileSpeed.ToString("F2");
            }
            else
            {
                AmmoName.text = "No Ammo";

                AmmoDamage.text = "-";
                AmmoSpeed.text = "-";
            }
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            WeaponName.text = Weapon.NamePT;
            WeaponDamage.text = "Dano: " + Weapon.ActualDamage.ToString("N2");
            WeaponAtackSpeed.text = "Velocidade: " + Weapon.ActualAttackSpeed.ToString("N2");
            WeaponRange.text = "Alcance: " + Weapon.ActualRange.ToString("N2");
            WeaponSpecial.text = "Especial: " + Weapon.Special.ToString();
            WeaponCriticalDamage.text = "Crit.: " + Weapon.ActualCriticalDamage.ToString("N2");
            WeaponCriticalChance.text = "Crit. %: " + (Weapon.ActualPrecision * 100).ToString("N2");

            if (Weapon.Ammo != null)
            {
                AmmoName.text = Weapon.Ammo.NamePT;

                AmmoDamage.text = "Dano: " + Weapon.Ammo.ActualDamage.ToString("N2");
                AmmoSpeed.text = "Vel. Projétil: " + Weapon.Ammo.ActualProjectileSpeed.ToString("N2");
            }
            else
            {
                AmmoName.text = "Sem Munição";

                AmmoDamage.text = "-";
                AmmoSpeed.text = "-";
            }
        }
    }

    void OnProgress(AmmoInstance ammo)
    {
        if (ammo == null)
            return;

        if (Weapon == null || Weapon.Ammo == null)
            return;

        if (ammo.Id != Weapon.Ammo.Id) return;

        float progress = 1f - (float)(ammo.CurrentRecharge / ammo.ActualRecharge);
        float remaining = (float)ammo.CurrentRecharge;

        RechargeBar.value = progress;
        TimeText.text = FormatTime(remaining);
    }

    void OnShoot(WeaponInstance weapon, EnemyRuntime enemy)
    {
        if (weapon != Weapon)
            return;

        AmmoCurrentAmmount.text = weapon.Ammo.CurrentAmmount.ToString();
    }

    void OnEnd(AmmoInstance ammo)
    {
        if (ammo == null)
            return;

        AmmoCurrentAmmount.text = ammo.CurrentAmmount.ToString();

        RechargeBar.value = 0f;

        string time = FormatTime(Weapon.Ammo.ActualRecharge);
        TimeText.text = time;
    }

    void OnChange(UpgradeInstance upgrade)
    {
        if (upgrade.TargetType == UpgradeHelper.TargetType.Weapon || upgrade.TargetType == UpgradeHelper.TargetType.Ammo)
        {
            TextChange();
        }
    }

    void OnEnable()
    {
        ExpeditionEvents.OnRechargeStart += OnStart;
        ExpeditionEvents.OnRechargeProgress += OnProgress;
        ExpeditionEvents.OnRechargeEnd += OnEnd;
        ExpeditionEvents.OnShoot += OnShoot;
        GameEvents.OnUpgradeBought += OnChange;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnRechargeStart -= OnStart;
        ExpeditionEvents.OnRechargeProgress -= OnProgress;
        ExpeditionEvents.OnRechargeEnd -= OnEnd;
        ExpeditionEvents.OnShoot -= OnShoot;

    }
}
