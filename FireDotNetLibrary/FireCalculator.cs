namespace FireDotNetLibrary
{
    public class FireCalculator
    {
        private DateOnly _startMonth;
        private int _durationInMonths;

        public FireCalculator()
        {
            StartMonth = DateOnly.FromDateTime(DateTime.Now);
            DurationInMonths = 12 * 30; // Default to 30 years
        }

        public DateOnly StartMonth
        {
            get => _startMonth;

            set
            {
                _startMonth = new DateOnly(value.Year, value.Month, 1);

                if (DurationInMonths > 0)
                    EndMonth = _startMonth.AddMonths(DurationInMonths).AddDays(-1);
            }
        }

        public DateOnly EndMonth { get; set; }

        public int DurationInMonths
        {
            get => _durationInMonths;

            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_Duration_Set_ArgumentOutOfRangeException);

                _durationInMonths = value;
                EndMonth = _startMonth.AddMonths(_durationInMonths).AddDays(-1);
            }
        }

        public decimal StartAmount { get; set; }
        public decimal MonthlyWithdrawalAmount { get; set; }
        public decimal AnnualyWithdrawalAmount { get; set; }
    }
}