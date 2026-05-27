using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoOptionDefinition : MonoBehaviour
{
    public Button AmmoButton;
    public Image AmmoImage;

    private ShipUi Ui;

    private AmmoInstance Ammo;

    public void Setup(ShipUi ui, AmmoInstance ammo)
    {
        Ui = ui;

        Ammo = ammo;
    }

    public void OnWeaponClicked()
    {
        Ui.ShowAmmoInfo(Ammo);
    }
}
