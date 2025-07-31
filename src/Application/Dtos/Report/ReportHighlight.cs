namespace Application.Dtos.Report
{
    public class ReportHighlight
    {
        public int TotalNewOrder { get; set; }
        public double DailySales { get; set; }
        public int TotalOrder { get; set; }
        public double SalesRevenue { get; set; }

        public List<int> OrderQtyByStatus { get; set; } = new List<int>();
        public List<int> OrderQty { get; set; } = new List<int>();
        public List<double> Revenues { get; set; } = new List<double>();
    }
}