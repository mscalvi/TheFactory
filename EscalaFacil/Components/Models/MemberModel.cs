namespace EscalaFacil.Components.Models;

public class MemberModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public HashSet<string> RoleIds { get; set; } = new(); // quais cargos pode exercer
    public bool IsActive { get; set; } = true;
}
