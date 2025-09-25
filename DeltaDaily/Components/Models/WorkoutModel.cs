using DeltaDaily.Components.Data;

namespace DeltaDaily.Components.Models
{
    public class WorkoutModel
    {
            public TimeOnly? Hora { get; set; }
            public TimeOnly? Duracao { get; set; }
            public WorkoutData.Workout? Modalidade { get; set; }
            public WorkoutData.Tired? Cansaco { get; set; }
            public WorkoutData.Happy? Animo { get; set; }
       
    }
}
