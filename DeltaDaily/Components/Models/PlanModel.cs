namespace DeltaDaily.Components.Models
{
    public class PlanModel
    {
        public string Nome { get; set; }
        public string PlanoAtual { get; set; }
        public string FaseAtual { get; set; }
        public int MetasCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int MetasCompletas { get; set; }
        public int MetasFaltando { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public DateTime? DataFechamento { get; set; }
    }
}
