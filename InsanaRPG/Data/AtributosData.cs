namespace InsanaRPG.Data;

public static class AtributosData
{
    public static readonly string[] AtributosOrdem = new[]
    {
        "Destreza", "Instinto", "Inteligência", "Presença", "Vigor", "Vontade"
    };

    public static Dictionary<string, int> Atributos = new Dictionary<string, int>
    {
        ["Destreza"] = 1,
        ["Instinto"] = 1,
        ["Inteligência"] = 1,
        ["Presença"] = 1,
        ["Vigor"] = 1,
        ["Vontade"] = 1
    };
}
