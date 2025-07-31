namespace Application.Dtos.Report
{
    public class GetRevenueReportOutput
    {
        public List<double> Revenues { get; set; } = new();

        public List<int> OrderQty { get; set; } = new();
    }
}