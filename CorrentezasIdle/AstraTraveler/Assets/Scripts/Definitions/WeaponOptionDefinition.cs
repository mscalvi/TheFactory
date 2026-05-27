using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponOptionDefinition : MonoBehaviour
{
    public Button WeaponButton;
    public Image WeaponImage;

    private ShipUi Ui;

    private WeaponInstance Weapon;

    public void Setup(ShipUi ui, WeaponInstance weapon)
    {
        Ui = ui;

        Weapon = weapon;
    }

    public void OnWeaponClicked()
    {
        Ui.ShowWeaponInfo(Weapon);
    }
}
