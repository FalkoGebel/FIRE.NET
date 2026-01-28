using CommunityToolkit.Mvvm.ComponentModel;
using FireDotNetLibrary;
using OxyPlot;
using OxyPlot.Axes;

namespace FireDotNetUi.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly FireCalculator _fireCalculator;

        public MainViewModel()
        {
            _fireCalculator = new()
            {
                StartingAmount = 100000,
                MonthlyWithdrawalAmount = 500
            };
            UpdatePlotModel();

            _startingAmountInput = _fireCalculator.StartingAmount.ToString("0.00");
            _monthlyWithdrawalAmountInput = _fireCalculator.MonthlyWithdrawalAmount.ToString("0.00");
            _annualWithdrawalAmountInput = _fireCalculator.AnnualWithdrawalAmount.ToString("0.00");
            _startingMonth = _fireCalculator.StartingMonth;
            _endingMonth = _fireCalculator.EndingMonth;
            _durationInMonthsInput = _fireCalculator.DurationInMonths.ToString();
        }

        [ObservableProperty]
        private PlotModel? _plotModelRemainingAmounts;

        [ObservableProperty]
        private string _startingAmountInput;

        [ObservableProperty]
        private string _monthlyWithdrawalAmountInput = string.Empty;

        [ObservableProperty]
        private string _annualWithdrawalAmountInput = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EndingMonth))]
        private DateTime _startingMonth;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DurationInMonthsInput))]
        private DateTime _endingMonth;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EndingMonth))]
        private string _durationInMonthsInput = string.Empty;

        partial void OnDurationInMonthsInputChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (int.TryParse(newValue, System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.CurrentCulture, out int parsedValue) &&
                    parsedValue > 0)
                {
                    _durationInMonthsInput = parsedValue.ToString();
                    _fireCalculator.DurationInMonths = parsedValue;
                    _endingMonth = _fireCalculator.EndingMonth;
                    UpdatePlotModel();
                }
                else
                {
                    DurationInMonthsInput = oldValue;
                }
            }
        }

        partial void OnStartingAmountInputChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (decimal.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out decimal parsedValue) &&
                    parsedValue > 0)
                {
                    _startingAmountInput = parsedValue.ToString("0.00");
                    _fireCalculator.StartingAmount = parsedValue;
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
                if (decimal.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out decimal parsedValue) &&
                    parsedValue >= 0)
                {
                    _monthlyWithdrawalAmountInput = parsedValue.ToString("0.00");
                    _fireCalculator.MonthlyWithdrawalAmount = parsedValue;
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
                if (decimal.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out decimal parsedValue) &&
                    parsedValue >= 0)
                {
                    _annualWithdrawalAmountInput = parsedValue.ToString("0.00");
                    _fireCalculator.AnnualWithdrawalAmount = parsedValue;
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
                    _durationInMonthsInput = _fireCalculator.DurationInMonths.ToString();
                    UpdatePlotModel();
                }
                catch (ArgumentOutOfRangeException)
                {
                    _endingMonth = oldValue;
                }
            }
        }

        private void UpdatePlotModel()
        {
            var remainingAmountMonths = _fireCalculator.GetRemainingAmounts();

            PlotModelRemainingAmounts = new PlotModel
            {
                Title = Properties.Resources.MainView_PlotModel_RemainingAmount,
                DefaultFont = "Verdana",
                DefaultFontSize = 16
            };
            var lineSeries = new OxyPlot.Series.LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.SkyBlue,
                ItemsSource = remainingAmountMonths.Select(m => new DataPoint(m.Item1.ToOADate(),
                                                                              m.Item2 > 0 ? (double)Math.Round(m.Item2, 2) : 0))
                                                   .ToList(),
                TrackerFormatString = Properties.Resources.MainView_PlotModel_Month +
                                      ": {2:dd.MM.yyyy}\n" +
                                      Properties.Resources.MainView_PlotModel_RemainingAmount +
                                      ": {4:C2}",
                CanTrackerInterpolatePoints = false,
            };
            PlotModelRemainingAmounts.Series.Add(lineSeries);

            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MMM yyyy",
                Title = Properties.Resources.MainView_PlotModel_Month,
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
                Title = Properties.Resources.MainView_PlotModel_RemainingAmount,
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