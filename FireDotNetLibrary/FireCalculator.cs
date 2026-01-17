namespace FireDotNetLibrary
{
    public class FireCalculator
    {
        private DateOnly _startingMonth;
        private DateOnly _endingMonth;
        private int _durationInMonths;
        private decimal _monthlyWithdrawalAmount;
        private decimal _annualWithdrawalAmount;

        public FireCalculator()
        {
            StartingMonth = DateOnly.FromDateTime(DateTime.Now);
            DurationInMonths = 12 * 30; // Default to 30 years
        }

        public DateOnly StartingMonth
        {
            get => _startingMonth;

            set
            {
                _startingMonth = new DateOnly(value.Year, value.Month, 1);

                if (DurationInMonths > 0)
                    _endingMonth = _startingMonth.AddMonths(DurationInMonths).AddDays(-1);
            }
        }

        public DateOnly EndingMonth
        {
            get => _endingMonth;

            set
            {
                DateOnly oldEndingMonth = _endingMonth;

                _endingMonth = (new DateOnly(value.Year, value.Month, 1)).AddMonths(1).AddDays(-1);
                int newDurationInMonths = (_endingMonth.Year - _startingMonth.Year) * 12 + _endingMonth.Month - _startingMonth.Month + 1;

                if (newDurationInMonths <= 0)
                {
                    _endingMonth = oldEndingMonth;
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_EndingMonth_Set_ArgumentOutOfRangeException);
                }

                _durationInMonths = newDurationInMonths;
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
                EndingMonth = _startingMonth.AddMonths(_durationInMonths).AddDays(-1);
            }
        }

        public decimal StartingAmount { get; set; }

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
            DateOnly currentMonth = StartingMonth;

            if (StartingAmount > 0)
            {

                for (int i = 0; i < DurationInMonths + 1; i++)
                {
                    if (i == 0)
                    {
                        output[i] = (currentMonth, StartingAmount);
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