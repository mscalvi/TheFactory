using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanelDefinition : MonoBehaviour
{
    public TMP_Text WeaponName;
    public TMP_Text AmmoName;

    public Button WeaponButton;
    public Button AmmoButton;

    public Image WeaponImage;
    public Image AmmoImage;

    private ShipUi Ui;

    private WeaponInstance Weapon;
    private AmmoInstance Ammo;

    int Index;

    public void Setup(ShipUi ui, WeaponInstance weapon, int index)
    {
        Ui = ui;

        Weapon = weapon; 
        
        if (Weapon == null)
        {
            WeaponName.text = "Empty";
            AmmoName.text = "-";

            AmmoButton.interactable = false;
        }
        else
        {
            Ammo = Weapon.Ammo;
        }

        Index = index;
    }

    public void OnWeaponClicked()
    {
        Ui.SelectWeaponSlot(Index);
        Ui.ShowWeapons();
        Ui.ShowWeaponInfo(Weapon);
    }
    public void OnAmmoClicked()
    {
        if (Weapon == null)
            return;

        Ui.SelectAmmoSlot(Index);
        Ui.ShowAmmos();

        if (Ammo != null)
        {
            Ui.ShowAmmoInfo(Ammo);
        }
    }
}
