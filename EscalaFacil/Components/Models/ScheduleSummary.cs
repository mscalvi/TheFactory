namespace EscalaFacil.Components.Models;
public class ScheduleSummary
{
    public int TotalShifts { get; set; }
    public int FilledShifts { get; set; }
    public int DistinctPeople { get; set; }
    public Dictionary<string, int> CountByMember { get; set; } = new();
}
