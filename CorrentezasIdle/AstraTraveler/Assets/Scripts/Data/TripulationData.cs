using System.Collections.Generic;

public static class TripulationData
{
    public static readonly Dictionary<string, TripulationModel> All = new()
    {
        ["t001"] = new TripulationModel
        {
            Id = "t001",
            Name = "Matias",
        }
    };
}