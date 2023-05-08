namespace Utilitar.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public string? Nume { get; set; }
        public string? Marca { get; set; }
        public string? Role { get; set; }
        public DayOfWeek Day { get; set; }
        public string? TipConcediu { get; set; }
        public int DurataConcediu { get; set; }
    }
}
