namespace ContaJunsta.Models;

public class EventModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Status { get; set; } = "Open"; // "Open" | "Closed"
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o"); // ISO
    public string? ClosedAt { get; set; }
}
