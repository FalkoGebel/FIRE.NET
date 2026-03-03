namespace FireDotNetLibrary
{
    public class FireCalculator
    {
        private const int _maxNumberOfRuns = 10000;

        private DateTime _startingMonth;
        private DateTime _endingMonth;
        private int _durationInMonths;
        private double _monthlyWithdrawalAmount;
        private double _annualWithdrawalAmount;
        private double _annualInflationRate;
        private double _annualReturn;
        private double _annualVolatility;
        private int _numberOfRuns;

        public FireCalculator()
        {
            StartingMonth = DateTime.Now;
            DurationInMonths = 12 * 30; // Default to 30 years
            _numberOfRuns = 1;
        }

        public DateTime StartingMonth
        {
            get => _startingMonth;

            set
            {
                _startingMonth = new DateTime(value.Year, value.Month, 1);

                if (DurationInMonths > 0)
                    _endingMonth = _startingMonth.AddMonths(DurationInMonths).AddDays(-1);
            }
        }

        public DateTime EndingMonth
        {
            get => _endingMonth;

            set
            {
                DateTime oldEndingMonth = _endingMonth;

                _endingMonth = (new DateTime(value.Year, value.Month, 1)).AddMonths(1).AddDays(-1);
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

        public double StartingAmount { get; set; }

        public double MonthlyWithdrawalAmount
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

        public double AnnualWithdrawalAmount
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

        public double AnnualInflationRate
        {
            get => _annualInflationRate;

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_AnnualInflationRate_Set_ArgumentOutOfRangeException);

                _annualInflationRate = value;
            }
        }

        public double AnnualReturn
        {
            get => _annualReturn;

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_AnnualReturn_Set_ArgumentOutOfRangeException);

                _annualReturn = value;
                UpdateNumberOfRuns();
            }
        }

        public double AnnualVolatility
        {
            get => _annualVolatility;

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_AnnualVolatility_Set_ArgumentOutOfRangeException);

                _annualVolatility = value;
                UpdateNumberOfRuns();
            }
        }

        private void UpdateNumberOfRuns()
        {
            if (AnnualVolatility != 0 && AnnualReturn != 0)
                _numberOfRuns = _maxNumberOfRuns;
            else
                _numberOfRuns = 1;
        }

        /// <summary>
        /// Returns two normal distributed random numbers with the given mean and standard deviation using the Box-Muller transformation.
        /// </summary>
        /// <param name="random">The random object to use for generating the random numbers.</param>
        /// <param name="mean">The given mean.</param>
        /// <param name="standardDeviation">The given standard deviation.</param>
        /// <returns>A tuple with two random double numbers matching a normal distribution with the given parameters.</returns>
        private static (double, double) GetTwoNormalDistributedRandomNumbers(Random random, double mean, double standardDeviation)
        {
            double x1 = 1 - random.NextDouble();
            double x2 = 1 - random.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            double y2 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Sin(2.0 * Math.PI * x2);
            return (y1 * standardDeviation + mean, y2 * standardDeviation + mean);
        }

        private double[] GetMonthlyReturns()
        {
            double[] output = new double[DurationInMonths];

            if (AnnualReturn == 0)
                return output; // [.. output.Select(x => 1d)];

            double monthlyReturn = Math.Pow(AnnualReturn / 100 + 1, 1d / 12) - 1;

            if (AnnualVolatility == 0)
            {
                for (int i = 0; i < DurationInMonths; i++)
                    output[i] = monthlyReturn;
            }
            else
            {
                double monthlyVolatility = Math.Pow(AnnualVolatility / 100 + 1, 1d / 12) - 1;
                Random random = new();
                List<double> monthlyReturns = [];

                while (monthlyReturns.Count < DurationInMonths)
                {
                    (var x1, var x2) = GetTwoNormalDistributedRandomNumbers(random, monthlyReturn, monthlyVolatility);

                    monthlyReturns.Add(x1);

                    if (monthlyReturns.Count < DurationInMonths)
                        monthlyReturns.Add(x2);
                }

                output = [.. monthlyReturns];
            }

            return output;
        }

        public List<List<(DateTime, double)>> GetRemainingAmounts()
        {
            List<List<(DateTime, double)>> output = [];

            for (int run = 0; run < _numberOfRuns; run++)
            {
                var currentRun = new (DateTime, double)[DurationInMonths + 1];
                DateTime currentMonth = StartingMonth;
                double currentMonthlyWithdrawalAmount = MonthlyWithdrawalAmount;
                double monthlyInflationFactorDecimal = Math.Pow(AnnualInflationRate / 100 + 1, 1d / 12);
                var monthlyReturns = GetMonthlyReturns();

                if (StartingAmount > 0)
                {
                    for (int i = 0; i < DurationInMonths + 1; i++)
                    {
                        if (i == 0)
                        {
                            currentRun[i] = (currentMonth, StartingAmount);
                            currentMonth = currentMonth.AddMonths(1).AddDays(-1);
                        }
                        else
                        {
                            currentMonthlyWithdrawalAmount *= monthlyInflationFactorDecimal;
                            currentRun[i] = (currentMonth, currentRun[i - 1].Item2 * (1 + monthlyReturns[i - 1]) - currentMonthlyWithdrawalAmount);
                            currentMonth = currentMonth.AddDays(1).AddMonths(1).AddDays(-1);
                        }
                    }
                }

                output.Add([.. currentRun]);
            }

            return output;
        }
    }
}