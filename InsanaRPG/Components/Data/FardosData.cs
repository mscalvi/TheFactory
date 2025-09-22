namespace InsanaRPG.Components.Data;

public static class FardosData
{
    // Ordem para exibir como radios
    public static readonly string[] Ordem = new[]
    {
        "Lesão Antiga",
        "Contrações Musculares",
        "Sequelas de Doenças",
        "Falta de Flexibilidade",
        "Noção Espacial Ruim",
        "Ansiedade Crônica",
        "Traumas de Brigas",
        "Fobia Social",
        "Pânico para Decisões",
        "Fobia da Natureza",
        "Alergias Severas",
        "Educação Limitada",
        "Analfabetismo Funcional",
        "Miopia Severa",
        "Vertigem e Enjoo",
        "Atenção Dispersa",
        "Processamento Lento",
        "Insônia",
        "Estigma Visível",
        "Mãos Rígidas"
    };

    // Mapeamento: Fardo -> (3) Proficiências afetadas (nomes iguais aos de ProficienciasData.Ordem)
    public static readonly Dictionary<string, string[]> Afeta = new()
    {
        ["Lesão Antiga"] = new[] { "Força", "Briga", "Armamentos" },
        ["Contrações Musculares"] = new[] { "Força", "Constituição", "Percepção" },
        ["Sequelas de Doenças"] = new[] { "Atletismo", "Constituição", "Percepção" },
        ["Falta de Flexibilidade"] = new[] { "Atletismo", "Acrobacia e Equilíbrio", "Furtividade" },
        ["Noção Espacial Ruim"] = new[] { "Disfarce", "Armas de Tiro", "Navegação e Direcionamento" },
        ["Ansiedade Crônica"] = new[] { "Intimidação", "Comércio e Negócios", "Meditação e Recobro" },
        ["Traumas de Brigas"] = new[] { "Briga", "Intimidação", "Avaliação e Riscos" },
        ["Fobia Social"] = new[] { "Carisma e Charme", "História e Religião", "Foco e Concentração" },
        ["Pânico para Decisões"] = new[] { "Comércio e Negócios", "Resiliência e Disciplina", "Intuição" },
        ["Fobia da Natureza"] = new[] { "Persuasão Animal", "Alquimia e Natureza", "Medicina Prática" },
        ["Alergias Severas"] = new[] { "Persuasão Animal", "Condução e Montaria", "Autocontrole" },
        ["Educação Limitada"] = new[] { "Alquimia e Natureza", "Matemática e Física", "Medicina Teórica" },
        ["Analfabetismo Funcional"] = new[] { "Matemática e Física", "História e Religião", "Leitura e Decifração" },
        ["Miopia Severa"] = new[] { "Medicina Teórica", "Ofício e Engenharia", "Armas de Tiro" },
        ["Vertigem e Enjoo"] = new[] { "Condução e Montaria", "Acrobacia e Equilíbrio", "Navegação e Direcionamento" },
        ["Atenção Dispersa"] = new[] { "Resiliência e Disciplina", "Foco e Concentração", "Autocontrole" },
        ["Processamento Lento"] = new[] { "Avaliação e Riscos", "Reflexo e Esquiva", "Intuição" },
        ["Insônia"] = new[] { "Leitura e Decifração", "Meditação e Recobro", "Reflexo e Esquiva" },
        ["Estigma Visível"] = new[] { "Disfarce", "Carisma e Charme", "Furtividade" },
        ["Mãos Rígidas"] = new[] { "Armamentos", "Medicina Prática", "Ofício e Engenharia" }
    };
}
