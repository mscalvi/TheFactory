using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShipUi : MonoBehaviour
{
    private GameState GameState;

    [SerializeField] Transform SelectedShipPanel;
    public TMP_Text SelectedShipName;
    public Image ShipImage;
    public Button ShipButton;

    [SerializeField] Transform SelectedItemInfo;
    public TMP_Text SelectedInfoName;

    [SerializeField] Transform SelectedWeaponPanel;
    [SerializeField] WeaponPanelDefinition SelectedWeaponPrefab;

    [SerializeField] Transform ShipOptionPanel;
    [SerializeField] ShipOptionDefinition ShipOptionPrefab;

    [SerializeField] Transform WeaponOptionPanel;
    [SerializeField] WeaponPanelDefinition WeaponOptionPrefab;

    [SerializeField] Transform AmmoOptionPanel;
    [SerializeField] AmmoOptionDefinition AmmoOptionPrefab;

    Dictionary<string, ShipOptionDefinition> shipUi = new();
    Dictionary<string, WeaponOptionDefinition> weaponUi = new();
    Dictionary<string, AmmoOptionDefinition> ammoUi = new();

    private enum SelectionMode
    {
        None,
        Ship,
        Weapon,
        Ammo
    }
    private SelectionMode CurrentMode;

    private ShipInstance PreviewShip;
    private WeaponInstance PreviewWeapon;
    private AmmoInstance PreviewAmmo;

    private int SelectedWeaponSlot = -1;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        RefreshAllUi();
    }

    private void BuildCurrent()
    {
        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            SelectedShipName.text = GameState.ExpeditionState.Ship.NamePT;
        }

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            SelectedShipName.text = GameState.ExpeditionState.Ship.NameEN;
        }

        BuildWeapons(GameState.ExpeditionState.Ship);
    }
    private void BuildWeapons(ShipInstance Ship)
    {
        for (int i = 0; i < Ship.Weapons.Count; i++)
        {
            var weapon = Ship.Weapons[i];

            if (weapon == null)
                continue;

            var go = Instantiate(SelectedWeaponPrefab, SelectedWeaponPanel);
            var ui = go.GetComponent<WeaponPanelDefinition>();

            ui.Setup(this, weapon, i);
        }
    }

    public void SelectWeaponSlot(int Index)
    {
        CurrentMode = SelectionMode.Weapon;
        SelectedWeaponSlot = Index;

        ShowWeaponInfo(GameState.ExpeditionState.Ship.Weapons[Index]);
    }
    public void SelectAmmoSlot(int Index)
    {
        CurrentMode = SelectionMode.Ammo;
        SelectedWeaponSlot = Index;

        ShowAmmoInfo(GameState.ExpeditionState.Ship.Weapons[Index].Ammo);
    }

    public void ShowShips()
    {
        ClearPanel(ShipOptionPanel);

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            SelectedInfoName.text = GameState.ExpeditionState.Ship.NamePT;
        }

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            SelectedInfoName.text = GameState.ExpeditionState.Ship.NameEN;
        }

        foreach (var ship in GameState.DataState.ships.Values)
        {
            if (ship.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                continue;

            var go = Instantiate(ShipOptionPrefab, ShipOptionPanel);
            var ui = go.GetComponent<ShipOptionDefinition>();

            ui.Setup(this, ship);
        }
    }
    public void ShowWeapons()
    {
        ClearPanel(WeaponOptionPanel);

        foreach (var weapon in GameState.DataState.weapons.Values)
        {
            if (weapon.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                continue;

            if (GameState.ExpeditionState.Ship.Weapons.Contains(weapon))
                continue;

            var go = Instantiate(WeaponOptionPrefab, WeaponOptionPanel);
            var ui = go.GetComponent<WeaponOptionDefinition>();

            ui.Setup(this, weapon);
        }
    }
    public void ShowAmmos()
    {
        if (SelectedWeaponSlot < 0)
            return;
        if (SelectedWeaponSlot >= GameState.ExpeditionState.Ship.Weapons.Count)
            return;

        var weapon = GameState.ExpeditionState.Ship.Weapons[SelectedWeaponSlot];

        if (weapon == null)
            return;

        ClearPanel(AmmoOptionPanel);

        foreach (var ammo in GameState.DataState.ammos.Values)
        {
            if (ammo.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                continue;

            if (ammo.Type != weapon.AmmoType)
                continue;

            var go = Instantiate(AmmoOptionPrefab, AmmoOptionPanel);
            var ui = go.GetComponent<AmmoOptionDefinition>();

            ui.Setup(this, ammo);
        }
    }

    public void ShowShipInfo(ShipInstance Ship)
    {
        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            SelectedInfoName.text = Ship.NamePT;
        }

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            SelectedInfoName.text = Ship.NameEN;
        }

        PreviewShip = Ship;
        CurrentMode = SelectionMode.Ship;
    }
    public void ShowWeaponInfo(WeaponInstance Weapon)
    {
        if (Weapon == null)
        {
            SelectedInfoName.text = "Empty Weapon Slot";
            PreviewWeapon = null;
            return;
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            SelectedInfoName.text = Weapon.NamePT;
        }

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            SelectedInfoName.text = Weapon.NameEN;
        }

        PreviewWeapon = Weapon;
        CurrentMode = SelectionMode.Weapon;
    }
    public void ShowAmmoInfo(AmmoInstance Ammo)
    {
        if (Ammo == null)
        {
            SelectedInfoName.text = "Empty Weapon Slot";
            PreviewWeapon = null;
            return;
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            SelectedInfoName.text = Ammo.NamePT;
        }

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            SelectedInfoName.text = Ammo.NameEN;
        }

        PreviewAmmo = Ammo;
        CurrentMode = SelectionMode.Ammo;
    }

    public void Select()
    {
        switch (CurrentMode)
        {
            case SelectionMode.Ship:
                if (PreviewShip == null)
                    return;
                EquipShip();
                break;

            case SelectionMode.Weapon:
                if (PreviewWeapon == null)
                    return;
                EquipWeapon();
                break;

            case SelectionMode.Ammo:
                if (PreviewAmmo == null)
                    return;
                EquipAmmo();
                break;
        }

        PreviewWeapon = null;
        PreviewAmmo = null;
        PreviewShip = null;

        CurrentMode = SelectionMode.None;

        SelectedWeaponSlot = -1;

        RefreshAllUi();
    }

    private void EquipShip()
    {
        bool alreadyEquipped = GameState.ExpeditionState.Ship == PreviewShip;
        if (alreadyEquipped)
            return;

        GameState.ExpeditionState.Ship = PreviewShip;
        GameState.ExpeditionState.Ship.Weapons = new List<WeaponInstance>();

        for (int i = 0; i < GameState.ExpeditionState.Ship.WeaponSlots; i++)
        {
            GameState.ExpeditionState.Ship.Weapons.Add(null);
        }
    }
    private void EquipWeapon()
    {
        bool alreadyEquipped = GameState.ExpeditionState.Ship.Weapons.Contains(PreviewWeapon);
        if (alreadyEquipped)
            return;

        GameState.ExpeditionState.Ship.Weapons[SelectedWeaponSlot] = PreviewWeapon;
    }
    private void EquipAmmo()
    {
        bool alreadyEquipped = false;

        foreach (var weapon in GameState.ExpeditionState.Ship.Weapons)
        {
            if (weapon == null)
                continue;

            if (weapon.Ammo == PreviewAmmo)
                alreadyEquipped = true;
        }

        if (alreadyEquipped)
            return;

        if (PreviewAmmo.Type != GameState.ExpeditionState.Ship.Weapons[SelectedWeaponSlot].AmmoType)
            return;

        GameState.ExpeditionState.Ship.Weapons[SelectedWeaponSlot].Ammo = PreviewAmmo;
    }

    public void EmptySlot()
    {
        switch (CurrentMode)
        {
            case SelectionMode.Weapon:
                EmptyWeapon();
                break;

            case SelectionMode.Ammo:
                EmptyAmmo();
                break;
        }

        RefreshAllUi();
    }

    private void EmptyWeapon()
    {
        GameState.ExpeditionState.Ship.Weapons[SelectedWeaponSlot] = null;
    }
    private void EmptyAmmo()
    {
        GameState.ExpeditionState.Ship.Weapons[SelectedWeaponSlot].Ammo = null;
    }

    private void RefreshAllUi()
    {
        ClearPanel(SelectedWeaponPanel);
        ClearPanel(ShipOptionPanel);
        ClearPanel(WeaponOptionPanel);
        ClearPanel(AmmoOptionPanel);

        BuildCurrent();
        ShowShips();
        ShowWeapons();
        ShowAmmos();
    }

    private void ClearPanel(Transform panel)
    {
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }
    }

    public void Return()
    {
        SceneManager.LoadScene("LandingScene");
    }


    // Eventos
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }
}
