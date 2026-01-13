using FireDotNetLibrary;
using OxyPlot;
using OxyPlot.Axes;

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

            var remainingAmountMonths = fireCalculator.GetRemainingAmounts();

            PlotModelRemainingAmounts = new PlotModel { Title = "Remaining Amounts" };
            var lineSeries = new OxyPlot.Series.LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.SkyBlue,
                ItemsSource = remainingAmountMonths.Select(m => new DataPoint(m.Item1.ToDateTime(new TimeOnly()).ToOADate(),
                                                                              (double)Math.Round(m.Item2, 2)))
                                                   .ToList(),
                TrackerFormatString = "Month: {2:MMM yyyy}\nRemaining Amount: {4:C2}"
            };
            PlotModelRemainingAmounts.Series.Add(lineSeries);

            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MMM yyyy",
                Title = "Month",
                IntervalType = DateTimeIntervalType.Months,
                MinorIntervalType = DateTimeIntervalType.Months,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Minimum = fireCalculator.StartMonth.ToDateTime(new TimeOnly()).ToOADate(),
                Maximum = fireCalculator.EndMonth.ToDateTime(new TimeOnly()).ToOADate()
            };
            PlotModelRemainingAmounts.Axes.Add(dateAxis);

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Remaining Amount",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                StringFormat = "C0",
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            PlotModelRemainingAmounts.Axes.Add(valueAxis);
        }
    }
}