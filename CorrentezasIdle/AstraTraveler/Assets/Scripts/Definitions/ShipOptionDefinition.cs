using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipOptionDefinition : MonoBehaviour
{
    public Button ShipButton;
    public Image ShipImage;

    private ShipUi Ui;

    private ShipInstance Ship;

    public void Setup(ShipUi ui, ShipInstance ship)
    {
        Ui = ui;

        Ship = ship;
    }

    public void OnWeaponClicked()
    {
        Ui.ShowShipInfo(Ship);
    }
}
