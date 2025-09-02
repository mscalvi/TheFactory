namespace EscalaFacil.Components.Models;
public class ShiftModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime Start { get; set; } // use DateTime para evitar converters extra
    public DateTime End { get; set; }
    public string RoleId { get; set; } = "";
    public string? Notes { get; set; }
}
