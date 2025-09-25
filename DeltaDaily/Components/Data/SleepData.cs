using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Data
{
    public class SleepData
    {
        public enum SleepQuality
        {
            [Display(Name = "Muitos sonhos")] MuitosSonhos,
            [Display(Name = "Muito calor")] MuitoCalor,
            [Display(Name = "Muito frio")] MuitoFrio,
            [Display(Name = "Insônia")] Insonia,
            [Display(Name = "Tranquilo")] Tranquilo,
            [Display(Name = "Ótimo")] Otimo,
            [Display(Name = "Normal")] Normal
        }
    }
}
