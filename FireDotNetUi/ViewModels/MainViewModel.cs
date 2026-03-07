using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            _annualInflationRate = _fireCalculator.AnnualInflationRate.ToString("0.00");
            _annualReturn = _fireCalculator.AnnualReturn.ToString("0.00");
            _annualVolatility = _fireCalculator.AnnualVolatility.ToString("0.00");
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

        [ObservableProperty]
        private string _annualInflationRate;

        [ObservableProperty]
        private string _annualReturn;

        [ObservableProperty]
        private string _annualVolatility;

        [RelayCommand]
        private void Calculate()
        {
            UpdatePlotModel();
        }

        partial void OnAnnualVolatilityChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (double.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out double parsedValue) &&
                    parsedValue >= 0)
                {
                    _annualVolatility = parsedValue.ToString("0.00");
                    _fireCalculator.AnnualVolatility = parsedValue;
                    UpdatePlotModel();
                }
                else
                {
                    AnnualVolatility = oldValue;
                }
            }
        }

        partial void OnAnnualReturnChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (double.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out double parsedValue) &&
                    parsedValue >= 0)
                {
                    _annualReturn = parsedValue.ToString("0.00");
                    _fireCalculator.AnnualReturn = parsedValue;
                    UpdatePlotModel();
                }
                else
                {
                    AnnualReturn = oldValue;
                }
            }
        }

        partial void OnAnnualInflationRateChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (double.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out double parsedValue) &&
                    parsedValue >= 0)
                {
                    _annualInflationRate = parsedValue.ToString("0.00");
                    _fireCalculator.AnnualInflationRate = parsedValue;
                    UpdatePlotModel();
                }
                else
                {
                    AnnualInflationRate = oldValue;
                }
            }
        }

        partial void OnDurationInMonthsInputChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (int.TryParse(newValue, System.Globalization.NumberStyles.Integer,
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
                if (double.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out double parsedValue) &&
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
                if (double.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out double parsedValue) &&
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
                if (double.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out double parsedValue) &&
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
            PlotModelRemainingAmounts = new PlotModel
            {
                Title = Properties.Resources.MainView_PlotModel_RemainingAmount,
                TitlePadding = 20,
                TitleFontSize = 20,
                DefaultFont = "Verdana",
                DefaultFontSize = 16
            };

            var runs = _fireCalculator.GetRemainingAmounts();
            var remainingAmounts = runs;

            if (runs.Count > 10)
            {
                runs = [.. runs.OrderByDescending(r => r.Sum(ra => ra.Item2))];
                int numberOfOnePercent = runs.Count / 100;

                remainingAmounts.Clear();
                remainingAmounts.Add(FireCalculator.CalculateAverageList([.. runs.Take(numberOfOnePercent)]));
                remainingAmounts.Add(FireCalculator.CalculateAverageList(runs[(runs.Count / 2 - numberOfOnePercent / 2)..(runs.Count / 2 + numberOfOnePercent / 2)]));
                remainingAmounts.Add(FireCalculator.CalculateAverageList([.. runs.Skip(runs.Count - numberOfOnePercent)]));
            }

            foreach (var ra in remainingAmounts)
            {
                var lineSeries = new OxyPlot.Series.LineSeries
                {
                    StrokeThickness = 2,
                    Color = OxyColors.SkyBlue,
                    ItemsSource = ra.Select(m => new DataPoint(m.Item1.ToOADate(), m.Item2 > 0 ? (double)Math.Round(m.Item2, 2) : 0))
                                     .ToList(),
                    TrackerFormatString = Properties.Resources.MainView_PlotModel_Month +
                                          ": {2:dd.MM.yyyy}\n" +
                                          Properties.Resources.MainView_PlotModel_RemainingAmount +
                                          ": {4:#,0.00}",
                    CanTrackerInterpolatePoints = false
                };
                PlotModelRemainingAmounts.Series.Add(lineSeries);
            }

            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MMM yyyy",
                Title = Properties.Resources.MainView_PlotModel_Month,
                AxisTitleDistance = 20,
                IntervalType = DateTimeIntervalType.Months,
                MinorIntervalType = DateTimeIntervalType.Months,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Minimum = _fireCalculator.StartingMonth.ToOADate(),
                Maximum = _fireCalculator.EndingMonth.ToOADate(),
                IntervalLength = 80
            };
            PlotModelRemainingAmounts.Axes.Add(dateAxis);

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = Properties.Resources.MainView_PlotModel_RemainingAmount,
                AxisTitleDistance = 20,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                StringFormat = "#,0.00",
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Minimum = 0
            };
            PlotModelRemainingAmounts.Axes.Add(valueAxis);
            PlotModelRemainingAmounts.InvalidatePlot(true);
        }
    }
}