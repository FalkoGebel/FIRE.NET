using CommunityToolkit.Mvvm.ComponentModel;
using FireDotNetLibrary;
using OxyPlot;
using OxyPlot.Axes;
using System.Windows;

namespace FireDotNetUi.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly FireCalculator _fireCalculator;
        private decimal _startingAmount = 100000;
        private decimal _monthlyWithdrawalAmount = 500;
        private decimal _annualWithdrawalAmount;

        public MainViewModel()
        {
            _fireCalculator = new()
            {
                StartingAmount = _startingAmount,
                MonthlyWithdrawalAmount = _monthlyWithdrawalAmount
            };
            _annualWithdrawalAmount = _fireCalculator.AnnualWithdrawalAmount;
            UpdatePlotModel();

            _startingAmountInput = _startingAmount.ToString("0.00");
            _monthlyWithdrawalAmountInput = _monthlyWithdrawalAmount.ToString("0.00");
            _annualWithdrawalAmountInput = _annualWithdrawalAmount.ToString("0.00");
            _startingMonth = _fireCalculator.StartingMonth;
            _endingMonth = _fireCalculator.EndingMonth;
        }

        [ObservableProperty]
        private PlotModel? _plotModelRemainingAmounts;

        [ObservableProperty]
        private string _startingAmountInput = string.Empty;

        [ObservableProperty]
        private string _monthlyWithdrawalAmountInput = string.Empty;

        [ObservableProperty]
        private string _annualWithdrawalAmountInput = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EndingMonth))]
        private DateTime _startingMonth;

        [ObservableProperty]
        private DateTime _endingMonth;

        partial void OnStartingAmountInputChanged(string? oldValue, string newValue)
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

        partial void OnMonthlyWithdrawalAmountInputChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (decimal.TryParse(newValue, System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.CurrentCulture, out decimal parsedValue))
                {
                    _monthlyWithdrawalAmount = parsedValue;
                    _monthlyWithdrawalAmountInput = _monthlyWithdrawalAmount.ToString("0.00");
                    _fireCalculator.MonthlyWithdrawalAmount = _monthlyWithdrawalAmount;
                    AnnualWithdrawalAmountInput = _fireCalculator.AnnualWithdrawalAmount.ToString("0.00");
                    UpdatePlotModel();
                }
                else
                {
                    MonthlyWithdrawalAmountInput = oldValue;
                }
            }
        }

        partial void OnAnnualWithdrawalAmountInputChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (decimal.TryParse(newValue, System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.CurrentCulture, out decimal parsedValue))
                {
                    _annualWithdrawalAmount = parsedValue;
                    _annualWithdrawalAmountInput = _annualWithdrawalAmount.ToString("0.00");
                    _fireCalculator.AnnualWithdrawalAmount = _annualWithdrawalAmount;
                    MonthlyWithdrawalAmountInput = _fireCalculator.MonthlyWithdrawalAmount.ToString("0.00");
                    UpdatePlotModel();
                }
                else
                {
                    AnnualWithdrawalAmountInput = oldValue;
                }
            }
        }

        partial void OnStartingMonthChanged(DateTime oldValue, DateTime newValue)
        {
            if (oldValue != newValue)
            {
                _fireCalculator.StartingMonth = newValue;
                _startingMonth = _fireCalculator.StartingMonth;
                _endingMonth = _fireCalculator.EndingMonth;

                UpdatePlotModel();
            }
        }

        partial void OnEndingMonthChanged(DateTime oldValue, DateTime newValue)
        {
            if (oldValue != newValue)
            {
                try
                {
                    _fireCalculator.EndingMonth = newValue;
                    _endingMonth = _fireCalculator.EndingMonth;
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
                ItemsSource = remainingAmountMonths.Select(m => new DataPoint(m.Item1.ToOADate(),
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
                Minimum = _fireCalculator.StartingMonth.ToOADate(),
                Maximum = _fireCalculator.EndingMonth.ToOADate()
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