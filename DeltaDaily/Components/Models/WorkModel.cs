using DeltaDaily.Components.Data;

namespace DeltaDaily.Components.Models
{
    public class WorkModel
        {
        public ProjectModel Plano { get; set; }          
        public string? Duracao { get; set; }
        public WorkData.WorkType? Tipo { get; set; }
        public WorkData.Focus? Foco { get; set; }
        public WorkData.WorkMethod? Metodo { get; set; }
    }

}
