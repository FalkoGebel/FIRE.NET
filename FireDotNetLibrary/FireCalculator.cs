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

        public (DateOnly, decimal)[] GetRemainingAmounts()
        {
            var output = new (DateOnly, decimal)[DurationInMonths + 1];
            DateOnly currentMonth = StartMonth;

            if (StartAmount > 0)
            {

                for (int i = 0; i < DurationInMonths + 1; i++)
                {
                    if (i == 0)
                    {
                        output[i] = (currentMonth, StartAmount);
                        currentMonth = currentMonth.AddMonths(1).AddDays(-1);
                    }
                    else
                    {
                        output[i] = (currentMonth, output[i - 1].Item2 - MonthlyWithdrawalAmount);
                        currentMonth = currentMonth.AddDays(1).AddMonths(1).AddDays(-1);
                    }
                }
            }

            return output;
        }
    }
}