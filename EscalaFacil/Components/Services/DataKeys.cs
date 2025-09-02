namespace EscalaFacil.Components.Services;

public static class DataKeys
{
    // teams/{teamId}.json
    public static string Team(string teamId) => $"teams/{teamId}";

    // teams/{teamId}/schedules/{scheduleId}.json
    public static string Schedule(string teamId, string scheduleId)
        => $"teams/{teamId}/schedules/{scheduleId}";
}
