using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Data
{
    public class WorkData
    {
        public enum WorkMethod
        {
            Pomodoro,
            Livre
        }
        public enum WorkType
        {
            [Display(Name = "Home-Office")] HomeOffice,
            [Display(Name = "Em Paralelo")] Paralelo,
            [Display(Name = "Hora Extra")] HoraExtra
        }
        public enum Focus
        {
            Péssimo,
            Baixo,
            Médio,
            Bom,
            Maravilhoso
        }
    }
}
