namespace FurmaIdle.Models
{
    public sealed class TraitModel
    {
        public string Id { get; set; }           // tr00, tr01...
        public string Name { get; set; } = "";
        public string? Image { get; set; }
        public string? Description { get; set; }

        // efeitos simples (keep it simple)
        public string? ResourceId { get; set; }  // para “Gera Mantimentos”
        public string? KnowledgeId { get; set; } // para “Conhecimento Cultural/Sobrevivência/Geográfico”
        public double AddPerSecond { get; set; } // +X/s no ResourceId/KnowledgeId
        public double GainMult { get; set; } = 1.0; // multiplicador de ganho do Resource/Knowledge
        public double CharacterCostMult { get; set; } = 1.0; // “Reduz Custo de Contratação” (mult global)
    }
}
