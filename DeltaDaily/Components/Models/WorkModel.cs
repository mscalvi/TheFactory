using DeltaDaily.Components.Data;

namespace DeltaDaily.Components.Models
{
    public class WorkModel
        {
        public PlanModel Plano { get; set; }          
        public TimeOnly? Duracao { get; set; }
        public WorkTypeData.WorkType? Tipo { get; set; }
        public FocusData.Focus? Foco { get; set; }
        public WorkMethodData.WorkMethod? Metodo { get; set; }
    }

}
