namespace FireDotNetLibrary
{
    public class FireCalculator
    {
        private DateOnly _startMonth;
        private DateOnly _endMonth;
        private int _durationInMonths;
        private decimal _monthlyWithdrawalAmount;
        private decimal _annualWithdrawalAmount;

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
                    _endMonth = _startMonth.AddMonths(DurationInMonths).AddDays(-1);
            }
        }

        public DateOnly EndMonth
        {
            get => _endMonth;

            set
            {
                _endMonth = (new DateOnly(value.Year, value.Month, 1)).AddMonths(1).AddDays(-1);
                _durationInMonths = (_endMonth.Year - _startMonth.Year) * 12 + _endMonth.Month - _startMonth.Month + 1;

                if (_durationInMonths <= 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_EndMonth_Set_ArgumentOutOfRangeException);
            }
        }

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

        public decimal MonthlyWithdrawalAmount
        {
            get => _monthlyWithdrawalAmount;

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_MonthlyWithdrawalAmount_Set_ArgumentOutOfRangeException);

                _monthlyWithdrawalAmount = value;
                _annualWithdrawalAmount = value * 12;
            }
        }

        public decimal AnnualWithdrawalAmount
        {
            get => _annualWithdrawalAmount;

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_AnnualWithdrawalAmount_Set_ArgumentOutOfRangeException);

                _annualWithdrawalAmount = value;
                _monthlyWithdrawalAmount = value / 12;

            }
        }

        public decimal[] GetRemainingAmounts()
        {
            decimal[] output = new decimal[DurationInMonths + 1];

            if (StartAmount > 0)
            {
                for (int i = 0; i < output.Length; i++)
                {
                    if (i == 0)
                        output[i] = StartAmount;
                    else
                        output[i] = output[i - 1] - MonthlyWithdrawalAmount;
                }
            }

            return output;
        }
    }
}