using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OtherRoomUI : MonoBehaviour
{
    public TMP_Text RoomName;
    public TMP_Dropdown TripulationDropdown;

    private List<TripulationModel> tripulationOptions = new();

    public void Setup(OtherRoomModel roomModel, GameDatabase db)
    {
        RoomName.text = roomModel.Name;

        SetupTripulation(db);
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


    public TripulationModel GetSelectedTripulation()
    {
        int index = TripulationDropdown.value;
        if (index == 0) return null;

        return tripulationOptions[index - 1];
    }

}