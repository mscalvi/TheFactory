namespace EscalaFacil.Components.Models;
public class TeamModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public List<MemberModel> Members { get; set; } = new();
    public List<RoleModel> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
