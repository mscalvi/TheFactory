using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponRoomDesign : MonoBehaviour
{
    public TMP_Text RoomName;
    public TMP_Dropdown TripulationDropdown;
    public TMP_Dropdown WeaponDropdown;
    public TMP_Dropdown AmmoDropdown;

    private List<TripulationModel> tripulationOptions = new();
    private List<WeaponModel> weaponOptions = new();
    private List<AmmoModel> ammoOptions = new();

    public void Setup(WeaponRoomModel roomModel, GameDatabase db)
    {
        RoomName.text = roomModel.Name;

        SetupTripulation(db);
        SetupWeapon(db);
        SetupAmmo(db);
    }

    void SetupTripulation(GameDatabase db)
    {
        TripulationDropdown.ClearOptions();
        tripulationOptions.Clear();

        List<string> options = new List<string> { "Empty" };

        foreach (var t in db.tripulation)
        {
            if (t.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                tripulationOptions.Add(t);
                options.Add(t.Name);
            }
        }

        TripulationDropdown.AddOptions(options);
        TripulationDropdown.value = 0; // default = Empty
    }

    void SetupWeapon(GameDatabase db)
    {
        WeaponDropdown.ClearOptions();
        weaponOptions.Clear();

        List<string> options = new List<string> { "Empty" };

        foreach (var w in db.weapons)
        {
            if (w.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                weaponOptions.Add(w);
                options.Add(w.Name);
            }
        }

        WeaponDropdown.AddOptions(options);
        WeaponDropdown.value = 0; // default = Empty
    }

    void SetupAmmo(GameDatabase db)
    {
        AmmoDropdown.ClearOptions();
        ammoOptions.Clear();

        List<string> options = new List<string> { "Empty" };

        foreach (var w in db.ammos)
        {
            if (w.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                ammoOptions.Add(w);
                options.Add(w.Name);
            }
        }

        AmmoDropdown.AddOptions(options);
        AmmoDropdown.value = 0; // default = Empty
    }

    public TripulationModel GetSelectedTripulation()
    {
        int index = TripulationDropdown.value;
        if (index == 0) return null;

        return tripulationOptions[index - 1];
    }

    public WeaponModel GetSelectedWeapon()
    {
        int index = WeaponDropdown.value;
        if (index == 0) return null;

        return weaponOptions[index - 1];
    }

    public AmmoModel GetSelectedAmmo()
    {
        int index = AmmoDropdown.value;
        if (index == 0) return null;

        return ammoOptions[index - 1];
    }
}