namespace InsanaRPG.Components.Data;

public static class DescricaoData
{
    public static readonly Dictionary<string, string[]> Mapa = new()
    {
        ["Vigor"] = new[] { "Franzino", "Fraco", "Normal", "Robusto", "Parrudo", "Atleta" },
        ["Presença"] = new[] { "Horroroso", "Feio", "Normal", "Agradável", "Bonito", "Belíssimo" },
        ["Inteligência"] = new[] { "Estúpido", "Burro", "Normal", "Astuto", "Inteligente", "Genial" },
        ["Vontade"] = new[] { "Nem Tenta Direito", "Desiste Fácil", "Normal", "Determinado", "Determinação de Ferro", "Inabalável" },
        ["Destreza"] = new[] { "Desastrado", "Descoordenado", "Normal", "Hábil", "Bastante Habilidoso", "Mestre de Ofício" },
        ["Instinto"] = new[] { "Alheio ao Mundo", "Distraído", "Normal", "Atento", "Esperto", "Uma verdadeira Antena" }
    };

    public static string Get(string atributo, int valor)
    {
        if (!Mapa.TryGetValue(atributo, out var arr)) return "-";
        if (valor < 1 || valor > 6) return "-";
        return arr[valor - 1];
    }
}
