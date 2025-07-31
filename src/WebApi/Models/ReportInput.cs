namespace WebApi.Models
{
    public class ReportInput
    {
        public DateTime? Date { get; set; }
        public string KeySearch { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime? FinalDate { get; set; }
        public DateTime? TzDate { get; set; }
    }
}