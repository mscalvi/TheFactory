namespace DeltaDaily.Components.Models
{
    public class WorkModel
        {
        public string? Plano { get; set; }          // nome livre
        public TimeOnly? Duracao { get; set; }      // usar HH:mm como duração (simples)
        public string? Tipo { get; set; }           // ex.: Dedicado/Paralelo (livre)
        public string? Foco { get; set; }           // ex.: Baixo/Médio/Alto (livre)
        public string? Tecnica { get; set; }        // ex.: Pomodoro/Timeboxing/Livre (livre)
    }

}
