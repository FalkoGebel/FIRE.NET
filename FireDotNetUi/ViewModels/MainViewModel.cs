using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FireDotNetLibrary;
using FireDotNetLibrary.Models;
using FireDotNetUi.Services;
using OxyPlot;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FireDotNetUi.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // TODO - Finalize cash flow period view and view model
        // TODO - Add remove button for cash flow periods

        private readonly FireCalculator _fireCalculator;

        public MainViewModel()
        {
            _fireCalculator = new()
            {
                StartingAmount = 100000,
            };
            UpdatePlotModel();

            _startingAmountInput = _fireCalculator.StartingAmount.ToString("0.00");
            _cashFlowPeriods = [.. _fireCalculator.CashFlowPeriods];
            _startingMonth = _fireCalculator.StartingMonth;
            _endingMonth = _fireCalculator.EndingMonth;
            _durationInMonthsInput = _fireCalculator.DurationInMonths.ToString();
            _annualInflationRate = _fireCalculator.AnnualInflationRate.ToString("0.00");
            _annualReturn = _fireCalculator.AnnualReturn.ToString("0.00");
            _annualVolatility = _fireCalculator.AnnualVolatility.ToString("0.00");
            _numberOfMultipleRuns = [.. _numberOfMultipleRuns.Select(n => int.Parse(n).ToString("#,0"))];
            _selectedNumberOfMultipleRuns = int.Parse(_selectedNumberOfMultipleRuns).ToString("#,0");
        }

        [ObservableProperty]
        private PlotModel? _plotModelRemainingAmounts;

        [ObservableProperty]
        private string _startingAmountInput;

        [ObservableProperty]
        private ObservableCollection<CashFlowPeriod> _cashFlowPeriods;

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

        [ObservableProperty]
        private string[] _numberOfMultipleRuns = ["1000", "10000", "100000", "1000000"];

        [ObservableProperty]
        private string _selectedNumberOfMultipleRuns = "10000";

        [RelayCommand]
        private void Calculate()
        {
            UpdatePlotModel();
        }

        [RelayCommand]
        private void AddNewCashFlowPeriods()
        {
            ViewHandlingService viewHandlingService = new();

            if (viewHandlingService.ShowNewCashFlowPeriodDialog(StartingMonth, EndingMonth))
            {
                _fireCalculator.AddCashFlowPeriod(viewHandlingService.NewCashFlowPeriodStartingMonth,
                                                  viewHandlingService.NewCashFlowPeriodEndingMonth,
                                                  viewHandlingService.NewCashFlowPeriodEndingMonthyAmount);
                CashFlowPeriods = [.. _fireCalculator.CashFlowPeriods];
                StartingMonth = _fireCalculator.StartingMonth;
                EndingMonth = _fireCalculator.EndingMonth;
                DurationInMonthsInput = _fireCalculator.DurationInMonths.ToString();
                UpdatePlotModel();
            }
        }

        partial void OnSelectedNumberOfMultipleRunsChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                _fireCalculator.NumberOfMultipleRuns = int.Parse(newValue, NumberStyles.AllowThousands);
                UpdatePlotModel();
            }
        }

        partial void OnAnnualVolatilityChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (double.TryParse(newValue, NumberStyles.Number,
                    CultureInfo.CurrentCulture, out double parsedValue) &&
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
                if (double.TryParse(newValue, NumberStyles.Number,
                    CultureInfo.CurrentCulture, out double parsedValue) &&
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
                if (double.TryParse(newValue, NumberStyles.Number,
                    CultureInfo.CurrentCulture, out double parsedValue) &&
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

        partial void OnStartingAmountInputChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (double.TryParse(newValue, NumberStyles.Number,
                    CultureInfo.CurrentCulture, out double parsedValue) &&
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

        partial void OnStartingMonthChanged(DateTime oldValue, DateTime newValue)
        {
            if (oldValue != newValue)
            {
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
            DateTime startTime = DateTime.Now;
            var runs = _fireCalculator.GetRemainingAmounts();
            double probabilityOfDefault = FireCalculator.CalculateProbabilityOfDefaultAsPercentage(runs);

            PlotModelRemainingAmounts = new PlotModel
            {
                Title = $"{Properties.Resources.MainView_PlotModel_RemainingAmount} -> {Properties.Resources.MainView_PlotModel_ProbabilityOfDefault}: {probabilityOfDefault:0.00}%",
                TitlePadding = 20,
                TitleFontSize = 20,
                DefaultFont = "Verdana",
                DefaultFontSize = 16
            };

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

            TimeSpan duration = DateTime.Now - startTime;
            PlotModelRemainingAmounts.Subtitle = $"{runs.Count:#,0} {Properties.Resources.MainView_PlotModel_Runs} -> {duration.TotalSeconds:0.00} {Properties.Resources.MainView_PlotModel_Seconds}";

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