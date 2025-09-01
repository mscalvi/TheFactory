namespace EscalaFacil.Components.Models;
public class ScheduleModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string TeamId { get; set; } = "";
    public string Name { get; set; } = "";
    public List<ShiftModel> Shifts { get; set; } = new();

    // Dados da edição
    public List<AvailabilityEntry> Availability { get; set; } = new(); // (MemberId, ShiftId)
                                                                       // Resultado
    public List<Assignment> Assignments { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
