using FireDotNetUi.ViewModels;
using FireDotNetUi.Views;

namespace FireDotNetUi.Services
{
    public class ViewHandlingService
    {
        public DateTime NewCashFlowPeriodStartingMonth { get; set; }
        public DateTime NewCashFlowPeriodEndingMonth { get; set; }
        public double NewCashFlowPeriodEndingMonthyAmount { get; set; }

        public bool ShowNewCashFlowPeriodDialog(DateTime startingMonth, DateTime endingMonth)
        {
            NewCashFlowPeriodView newCashFlowPeriodView = new()
            {
                DataContext = new NewCashFlowPeriodViewModel(startingMonth, endingMonth)
            };

            var result = newCashFlowPeriodView.ShowDialog();

            if (result == true)
            {
                NewCashFlowPeriodViewModel viewModel = (NewCashFlowPeriodViewModel)newCashFlowPeriodView.DataContext;
                NewCashFlowPeriodStartingMonth = viewModel.StartingMonth;
                NewCashFlowPeriodEndingMonth = viewModel.EndingMonth;
                NewCashFlowPeriodEndingMonthyAmount = double.Parse(viewModel.MonthlyAmountInput);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}