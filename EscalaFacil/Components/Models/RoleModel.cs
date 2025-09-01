namespace EscalaFacil.Components.Models;
public class RoleModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public string? ColorHex { get; set; } // opcional p/ UI
}
