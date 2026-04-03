using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace FireDotNetUi.ViewModels
{
    public partial class NewCashFlowPeriodViewModel : ObservableObject
    {
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
        private void Cancel(Window window)
        {
            MessageBox.Show("Cancel. This functionality is not implemented yet.");
        }

        [RelayCommand]
        private void Submit(Window window)
        {
            MessageBox.Show("Submit. This functionality is not implemented yet.");
        }
    }
}