namespace InsanaRPG.Data;

public static class OrigemData
{
    // Ordem de exibição das origens
    public static readonly string[] Ordem = new[]
    {
        "Civil", "Guildas", "Academia", "Militar", "Comércio", "Forasteiro", "Clero", "Submundo", "Nobreza"
    };

    // Descrição por origem
    public static readonly Dictionary<string, string> OrigemDesc = new()
    {
        ["Civil"] = "Habitantes das Muradas ou das grandes cidades, são a maioria em Furma, e cumprem com todos os papeis necessários para a manutenção da civilização.",
        ["Guildas"] = "Especializados na caça aos mutantes, são treinados para viajar pelos territórios dominados pela natureza.",
        ["Academia"] = "Estudiosos que dedicam a vida as diversas formas do conhecimento, como a astronomia e a alquimia. Costumam se isolar nas grandes cidades, vivendo da pesquisa e do patronato.",
        ["Militar"] = "Originais da Metrópole ou de uma das muitas cidades que tentam se manter independentes, recebem forte treinamento para combates e estratégias.",
        ["Comércio"] = "Normalmente ligados as cooperativas, passam anos no mar em busca de negócios lucrativos e produtos únicos.",
        ["Forasteiro"] = "O caos fora das muradas permite que muitos vivam de forma apátrida, sobrevivendo do que conseguirem por as mãos.",
        ["Clero"] = "A entrada na vida religiosa é difícil, e poucos são aqueles que conseguem seguir suas devoções para além das obrigações cíclicas.",
        ["Submundo"] = "Abarrotas de gente, e vazias de recursos, as cidades são um grande ninho de problemas, e muitas vezes eles andam armados.",
        ["Nobreza"] = "A pseudo-realeza de Verzassio se mantém à força, mas seus filhos ainda podem desfrutar de uma realidade diferente das dos outros cidadãos."
    };

    // Trilhas disponíveis por origem (devem bater com as usadas em TrilhaDesc/TrilhaBonus)
    public static readonly Dictionary<string, string[]> Trilha = new()
    {
        ["Civil"] = new[] { "Trabalho no Campo", "Proteção do Grupo", "Gladiador" },
        ["Guildas"] = new[] { "Caçador", "Alquimista de Campo", "Explorador" },
        ["Academia"] = new[] { "Humanidades", "Engenharia", "Ciências" },
        ["Militar"] = new[] { "Cerco e Campanhas", "Estrategista de Campo", "Agente de Linha" },
        ["Comércio"] = new[] { "Produtos Raros", "Rotas Ocultas", "Contatos" },
        ["Forasteiro"] = new[] { "Pirataria Costeira", "Nomadismo", "Desbravadores" },
        ["Clero"] = new[] { "Missionários", "Estudiosos", "Conselheiros" },
        ["Submundo"] = new[] { "Falsificadores", "Golpistas", "Ladinos" },
        ["Nobreza"] = new[] { "Oficiais e Gerentes", "Líderes", "Políticos" }
    };

    // Descrição por (origem, trilha)
    public static readonly Dictionary<(string origem, string trilha), string> TrilhaDesc = new()
    {
        // Civil
        [("Civil", "Trabalho no Campo")] = "Trabalhou nos perigosos campos, com animais e colheitas.",
        [("Civil", "Proteção do Grupo")] = "Atuou nas defesas e rondas da cidade e das caravanas.",
        [("Civil", "Gladiador")] = "Atuou como gladiador nas Arenas locais, se apresentando regularmente.",

        // Guildas
        [("Guildas", "Caçador")] = "Participou do abate de criaturas perto das muradas, em equipe.",
        [("Guildas", "Alquimista de Campo")] = "Preparou reagentes e suporte em operações.",
        [("Guildas", "Explorador")] = "Reconheceu terreno e traçou estratégias de abordagem.",

        // Academia
        [("Academia", "Humanidades")] = "Estudou idiomas, culturas e ética.",
        [("Academia", "Engenharia")] = "Estudou matemática, física e obras urbanas.",
        [("Academia", "Ciências")] = "Fez pesquisa técnica e metodologia de laboratório.",

        // Militar
        [("Militar", "Cerco e Campanhas")] = "Participou de campanhas duradouras e cercos de pressão.",
        [("Militar", "Estrategista de Campo")] = "Conheceu planejamento tático e leitura de risco.",
        [("Militar", "Agente de Linha")] = "Participou de patrulhas, combate direto e infiltração leve.",

        // Comércio
        [("Comércio", "Produtos Raros")] = "Fez a curadoria e negociação de cargas valiosas.",
        [("Comércio", "Rotas Ocultas")] = "Manteve rotas discretas por mar e terra.",
        [("Comércio", "Contatos")] = "Criou redes de intermediários e aprendeu a leitura de intenções.",

        // Forasteiro
        [("Forasteiro", "Pirataria Costeira")] = "Conheceu os mares, as cavernas e os locais secretos.",
        [("Forasteiro", "Nomadismo")] = "Sobreviveu em ambientes inóspitos, sem se apegar ao desnecessário.",
        [("Forasteiro", "Desbravadores")] = "Procurou por territórios que ainda não haviam sido clamados.",

        // Clero
        [("Clero", "Missionários")] = "Viajou para praticar a medicina e conhecer as práticas locais",
        [("Clero", "Estudiosos")] = "Estudou arquivos, códices e liturgia.",
        [("Clero", "Conselheiros")] = "Compreendeu a orientação de líderes e leitura fina de pessoas.",

        // Submundo
        [("Submundo", "Falsificadores")] = "Usou de suas habilidades para ganhar a vida de forma ilícita.",
        [("Submundo", "Golpistas")] = "Aprendeu rápido que nem tudo é o que parece, inclusive você.",
        [("Submundo", "Ladinos")] = "Usou do roubo e do furto para sobreviver no ambiente hostíl.",

        // Nobreza
        [("Nobreza", "Oficiais e Gerentes")] = "Entendeu que seu papel era o de comandar.",
        [("Nobreza", "Líderes")] = "Nasceu com poder, e aprendeu a mantê-lo.",
        [("Nobreza", "Políticos")] = "Acendeu na hierarquia social por meio da negociação e informação."
    };

    // Bônus de proficiências por (origem, trilha): +15 / +10 / +5
    public record TrilhaBonus(string ProfMais15, string ProfMais10, string ProfMais5);

    public static readonly Dictionary<(string origem, string trilha), TrilhaBonus> TrilhaProfs = new()
    {
        // Civil
        [("Civil", "Trabalho no Campo")] = new("Persuasão Animal", "Resiliência e Disciplina", "Alquimia e Natureza"),
        [("Civil", "Proteção do Grupo")] = new("Percepção", "Resiliência e Disciplina", "Armamentos"),
        [("Civil", "Gladiador")] = new("Briga", "Resiliência e Disciplina", "Atletismo"),

        // Guildas
        [("Guildas", "Caçador")] = new("Armas de Tiro", "Avaliação e Riscos", "Furtividade"),
        [("Guildas", "Alquimista de Campo")] = new("Medicina Prática", "Avaliação e Riscos", "Alquimia e Natureza"),
        [("Guildas", "Explorador")] = new("Atletismo", "Avaliação e Riscos", "Percepção"),

        // Academia
        [("Academia", "Humanidades")] = new("Leitura e Decifração", "Foco e Concentração", "Comércio e Negócios"),
        [("Academia", "Engenharia")] = new("Matemática e Física", "Foco e Concentração", "Ofício e Engenharia"),
        [("Academia", "Ciências")] = new("Alquimia e Natureza", "Foco e Concentração", "Ofício e Engenharia"),

        // Militar
        [("Militar", "Cerco e Campanhas")] = new("Resiliência e Disciplina", "Armamentos", "Armas de Tiro"),
        [("Militar", "Estrategista de Campo")] = new("Intuição", "Armamentos", "Avaliação e Riscos"),
        [("Militar", "Agente de Linha")] = new("Força", "Armamentos", "Intimidação"),

        // Comércio
        [("Comércio", "Produtos Raros")] = new("Comércio e Negócios", "Intuição", "Carisma e Charme"),
        [("Comércio", "Rotas Ocultas")] = new("Acrobacia e Equilíbrio", "Intuição", "Navegação e Direcionamento"),
        [("Comércio", "Contatos")] = new("Carisma e Charme", "Intuição", "Disfarce"),

        // Forasteiro
        [("Forasteiro", "Pirataria Costeira")] = new("Condução e Montaria", "Atletismo", "Reflexo e Esquiva"),
        [("Forasteiro", "Nomadismo")] = new("Navegação e Direcionamento", "Atletismo", "Resiliência e Disciplina"),
        [("Forasteiro", "Desbravadores")] = new("Reflexo e Esquiva", "Atletismo", "Avaliação e Riscos"),

        // Clero
        [("Clero", "Missionários")] = new("Medicina Teórica", "História e Religião", "Medicina Prática"),
        [("Clero", "Estudiosos")] = new("Foco e Concentração", "História e Religião", "Leitura e Decifração"),
        [("Clero", "Conselheiros")] = new("Meditação e Recobro", "História e Religião", "Carisma e Charme"),

        // Submundo
        [("Submundo", "Falsificadores")] = new("Ofício e Engenharia", "Briga", "Disfarce"),
        [("Submundo", "Golpistas")] = new("Disfarce", "Briga", "Carisma e Charme"),
        [("Submundo", "Ladinos")] = new("Furtividade", "Briga", "Reflexo e Esquiva"),

        // Nobreza
        [("Nobreza", "Oficiais e Gerentes")] = new("Intimidação", "Carisma e Charme", "Armamentos"),
        [("Nobreza", "Líderes")] = new("Autocontrole", "Carisma e Charme", "Meditação e Recobro"),
        [("Nobreza", "Políticos")] = new("Avaliação e Riscos", "Carisma e Charme", "Intuição")
    };
}
