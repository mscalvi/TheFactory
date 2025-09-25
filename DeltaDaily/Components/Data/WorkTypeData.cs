using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Data
{
    public class WorkTypeData
    {
        public enum WorkType
        {
            [Display(Name = "Home-Office")] HomeOffice,
            [Display(Name = "Em Paralelo")] Paralelo,
            [Display(Name = "Hora Extra")] HoraExtra
        }
    }
}
