using FireDotNetLibrary;
using OxyPlot;

namespace FireDotNetUi.ViewModels
{
    public class MainViewModel
    {
        public PlotModel PlotModelRemainingAmounts { get; private set; }
        public MainViewModel()
        {
            FireCalculator fireCalculator = new()
            {
                StartAmount = 1000000m,
                MonthlyWithdrawalAmount = 3000m
            };

            PlotModelRemainingAmounts = new PlotModel { Title = "Remaining Amounts" };
            var lineSeries = new OxyPlot.Series.LineSeries
            {
                //Title = "Line Series",
                StrokeThickness = 2,
                Color = OxyColors.SkyBlue,
                ItemsSource = fireCalculator.GetRemainingAmounts().Select((amount, index) => new DataPoint(index, (double)Math.Round(amount, 2))).ToList()
            };
            PlotModelRemainingAmounts.Series.Add(lineSeries);
        }
    }
}