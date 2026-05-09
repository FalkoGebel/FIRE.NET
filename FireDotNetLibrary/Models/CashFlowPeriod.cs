namespace FireDotNetLibrary.Models
{
    public class CashFlowPeriod
    {
        public DateTime StartingMonth { get; set; }
        public DateTime EndingMonth { get; set; }
        public double MonthlyAmount { get; set; }
        public InflationOption InflationOption { get; set; }
    }
}