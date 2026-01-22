using CommunityToolkit.Mvvm.ComponentModel;
using FireDotNetLibrary;
using OxyPlot;
using OxyPlot.Axes;
using System.Windows;

namespace FireDotNetUi.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // TODO - Too wide calendar popup in MonthPicker control -> style needed

        private readonly FireCalculator _fireCalculator;
        private decimal _startingAmount = 100000;

        public MainViewModel()
        {
            _fireCalculator = new()
            {
                StartingAmount = _startingAmount,
                MonthlyWithdrawalAmount = MonthlyWithdrawalAmount
            };
            AnnualWithdrawalAmount = _fireCalculator.AnnualWithdrawalAmount;
            UpdatePlotModel();

            _startingAmountInput = _startingAmount.ToString("0.00");
            _startingMonth = _fireCalculator.StartingMonth.ToDateTime(new TimeOnly());
            _endingMonth = _fireCalculator.EndingMonth.ToDateTime(new TimeOnly());
        }

        [ObservableProperty]
        private PlotModel? _plotModelRemainingAmounts;

        [ObservableProperty]
        private string _startingAmountInput = string.Empty;

        [ObservableProperty]
        private decimal _monthlyWithdrawalAmount = 500;

        [ObservableProperty]
        private decimal _annualWithdrawalAmount;

        [ObservableProperty]
        private DateTime _startingMonth;

        [ObservableProperty]
        private DateTime _endingMonth;

        partial void OnStartingAmountInputChanging(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (decimal.TryParse(newValue, System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.CurrentCulture, out decimal parsedValue))
                {
                    _startingAmount = parsedValue;
                    _startingAmountInput = _startingAmount.ToString("0.00");
                    _fireCalculator.StartingAmount = _startingAmount;
                    UpdatePlotModel();
                }
                else
                {
                    StartingAmountInput = oldValue;
                }
            }
        }

        partial void OnMonthlyWithdrawalAmountChanged(decimal oldValue, decimal newValue)
        {
            if (oldValue != newValue)
            {
                _fireCalculator.MonthlyWithdrawalAmount = newValue;
                AnnualWithdrawalAmount = _fireCalculator.AnnualWithdrawalAmount;
                UpdatePlotModel();
            }
        }

        partial void OnAnnualWithdrawalAmountChanged(decimal oldValue, decimal newValue)
        {
            if (oldValue != newValue)
            {
                _fireCalculator.AnnualWithdrawalAmount = newValue;
                MonthlyWithdrawalAmount = _fireCalculator.MonthlyWithdrawalAmount;
                UpdatePlotModel();
            }
        }

        partial void OnStartingMonthChanged(DateTime oldValue, DateTime newValue)
        {
            if (oldValue != newValue)
            {
                _fireCalculator.StartingMonth = DateOnly.FromDateTime(newValue);
                _startingMonth = _fireCalculator.StartingMonth.ToDateTime(new TimeOnly());

                // TODO - Update EndingMonth on MonthPicker does not work
                _endingMonth = _fireCalculator.EndingMonth.ToDateTime(new TimeOnly());

                UpdatePlotModel();
            }
        }

        partial void OnEndingMonthChanged(DateTime oldValue, DateTime newValue)
        {
            if (oldValue != newValue)
            {
                try
                {
                    _fireCalculator.EndingMonth = DateOnly.FromDateTime(newValue);
                    _endingMonth = _fireCalculator.EndingMonth.ToDateTime(new TimeOnly());
                    UpdatePlotModel();
                }
                catch (ArgumentOutOfRangeException)
                {
                    _endingMonth = oldValue;
                    MessageBox.Show("Ending Month must not be earlier than Starting Month", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdatePlotModel()
        {
            var remainingAmountMonths = _fireCalculator.GetRemainingAmounts();

            PlotModelRemainingAmounts = new PlotModel { Title = "Remaining Amounts" };
            var lineSeries = new OxyPlot.Series.LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.SkyBlue,
                ItemsSource = remainingAmountMonths.Select(m => new DataPoint(m.Item1.ToDateTime(new TimeOnly()).ToOADate(),
                                                                              m.Item2 > 0 ? (double)Math.Round(m.Item2, 2) : 0))
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
                Minimum = _fireCalculator.StartingMonth.ToDateTime(new TimeOnly()).ToOADate(),
                Maximum = _fireCalculator.EndingMonth.ToDateTime(new TimeOnly()).ToOADate()
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
                IsPanEnabled = false,
                Minimum = 0
            };
            PlotModelRemainingAmounts.Axes.Add(valueAxis);
            PlotModelRemainingAmounts.InvalidatePlot(true);
        }
    }
}