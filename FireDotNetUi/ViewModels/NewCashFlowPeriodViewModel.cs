using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FireDotNetLibrary;
using System.Windows;

namespace FireDotNetUi.ViewModels
{
    public partial class NewCashFlowPeriodViewModel : ObservableObject
    {
        public class InflationOptionComboBoxItem(InflationOption inflationOption)
        {
            public string Description { get => Value.GetDisplayDescription(); }
            public InflationOption Value { get; private set; } = inflationOption;
        }

        public NewCashFlowPeriodViewModel() { }

        public NewCashFlowPeriodViewModel(DateTime startingMonth, DateTime endingMonth)
        {
            _startingMonth = startingMonth;
            _endingMonth = endingMonth;
            _durationInMonthsInput = ((EndingMonth.Year - StartingMonth.Year) * 12 + EndingMonth.Month - StartingMonth.Month + 1).ToString();
            _monthlyAmountInput = 0.ToString("0.00");
            _inflationOptions = [.. Enum.GetValues<InflationOption>().Select(option => new InflationOptionComboBoxItem(option))];
            _selectedInflationOption = new InflationOptionComboBoxItem(InflationOption.FromAnalysisStart);
        }

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
        private string _monthlyAmountInput = string.Empty;

        [ObservableProperty]
        private InflationOptionComboBoxItem? _selectedInflationOption;

        [ObservableProperty]
        private InflationOptionComboBoxItem[]? _inflationOptions;

        [RelayCommand]
        private static void Cancel(Window window)
        {
            window.Close();
        }

        [RelayCommand]
        private static void Submit(Window window)
        {

            window.DialogResult = true;
            window.Close();
        }

        partial void OnStartingMonthChanged(DateTime oldValue, DateTime newValue)
        {
            if (oldValue != newValue)
                _endingMonth = StartingMonth.AddMonths(int.Parse(DurationInMonthsInput)).AddDays(-1);
        }

        partial void OnEndingMonthChanged(DateTime oldValue, DateTime newValue)
        {
            if (oldValue != newValue)
            {
                if (newValue >= StartingMonth)
                {
                    _endingMonth = newValue;
                    DurationInMonthsInput = ((EndingMonth.Year - StartingMonth.Year) * 12 + EndingMonth.Month - StartingMonth.Month + 1).ToString();
                }
                else
                {
                    _endingMonth = oldValue;
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
                    _endingMonth = StartingMonth.AddMonths(parsedValue).AddDays(-1);
                }
                else
                {
                    DurationInMonthsInput = oldValue;
                }
            }
        }

        partial void OnMonthlyAmountInputChanged(string? oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue != newValue)
            {
                if (double.TryParse(newValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out double parsedValue))
                {
                    _monthlyAmountInput = parsedValue.ToString("0.00");
                }
                else
                {
                    MonthlyAmountInput = oldValue;
                }
            }
        }
    }
}