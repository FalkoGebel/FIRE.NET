using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace FireDotNetUi.ViewModels
{
    public partial class NewCashFlowPeriodViewModel : ObservableObject
    {
        public NewCashFlowPeriodViewModel() { }

        public NewCashFlowPeriodViewModel(DateTime startingMonth, DateTime endingMonth)
        {
            StartingMonth = startingMonth;
            EndingMonth = endingMonth;
            DurationInMonthsInput = ((EndingMonth.Year - StartingMonth.Year) * 12 + EndingMonth.Month - StartingMonth.Month + 1).ToString();
            MonthlyAmountInput = "0.00";
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

        // TODO - Add validation for the inputs -> take old code from MainViewModel
        // TODO - submit button only enabled if the inputs are valid
    }
}