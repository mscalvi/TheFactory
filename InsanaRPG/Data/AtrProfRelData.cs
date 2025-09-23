namespace InsanaRPG.Data;

public static class AtrProfRelData
{
    // Atributo (com acento) -> lista de proficiências (com acento, iguais às de ProficienciasData.Ordem)
    public static readonly Dictionary<string, string[]> Mapa = new()
    {
        ["Vigor"] = new[] { "Força", "Atletismo", "Briga", "Armamentos", "Constituição" },

        ["Presença"] = new[] { "Disfarce", "Comércio e Negócios", "Intimidação", "Carisma e Charme", "Persuasão Animal" },

        ["Inteligência"] = new[] { "Alquimia e Natureza", "Matemática e Física", "História e Religião", "Medicina Teórica", "Leitura e Decifração" },

        ["Destreza"] = new[] { "Medicina Prática", "Ofício e Engenharia", "Armas de Tiro", "Condução e Montaria", "Acrobacia e Equilíbrio" },

        ["Vontade"] = new[] { "Resiliência e Disciplina", "Foco e Concentração", "Autocontrole", "Avaliação e Riscos", "Meditação e Recobro" },

        ["Instinto"] = new[] { "Percepção", "Reflexo e Esquiva", "Navegação e Direcionamento", "Intuição", "Furtividade" }
    };
}
