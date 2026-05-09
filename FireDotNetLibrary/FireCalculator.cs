using FireDotNetLibrary.Models;
using System.Collections.Concurrent;

namespace FireDotNetLibrary
{
    public class FireCalculator
    {
        private DateTime _startingMonth;
        private DateTime _endingMonth;
        private int _durationInMonths;
        private double _annualInflationRate;
        private double _annualReturn;
        private double _annualVolatility;
        private int _numberOfMultipleRuns;
        private int _numberOfRuns;
        private List<CashFlowPeriod> _cashFlowPeriods;
        private List<MonthlyCashFlowAmount> _monthlyCashFlowAmounts;

        public FireCalculator()
        {
            _numberOfMultipleRuns = 10000;
            _numberOfRuns = 1;
            _monthlyCashFlowAmounts = [];

            // TODO - Refactor using add function
            DateTime now = DateTime.Now,
                     startingMonth = new(now.Year, now.Month, 1),
                     endingMonth = startingMonth.AddMonths(12 * 30).AddDays(-1);
            _cashFlowPeriods = [new CashFlowPeriod() { StartingMonth = startingMonth, EndingMonth = endingMonth, MonthlyAmount = -500 }];
            UpdateValuesFromCashFlowPeriods();
        }

        public DateTime StartingMonth
        {
            get => _startingMonth;
        }

        public DateTime EndingMonth
        {
            get => _endingMonth;
        }

        public int DurationInMonths
        {
            get => _durationInMonths;
        }

        public double StartingAmount { get; set; }

        public double AnnualInflationRate
        {
            get => _annualInflationRate;

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_AnnualInflationRate_Set_ArgumentOutOfRangeException);

                _annualInflationRate = value;
                UpdateMonthlyWithdrawalAmounts();
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

        public int NumberOfMultipleRuns
        {
            get => _numberOfMultipleRuns;

            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_NumberOfMultipleRuns_Set_ArgumentOutOfRangeException);

                _numberOfMultipleRuns = value;
                UpdateNumberOfRuns();
            }
        }

        public List<CashFlowPeriod> CashFlowPeriods { get => _cashFlowPeriods; }
        public List<MonthlyCashFlowAmount> MonthlyCashFlowAmounts { get => _monthlyCashFlowAmounts; }

        private void UpdateMonthlyWithdrawalAmounts()
        {
            MonthlyCashFlowAmount[] newMonthlyWithdrawalAmounts = new MonthlyCashFlowAmount[DurationInMonths];

            for (int i = 0; i < DurationInMonths; i++)
                newMonthlyWithdrawalAmounts[i] = new MonthlyCashFlowAmount() { EndOfMonth = StartingMonth.AddMonths(i + 1).AddDays(-1), MonthlyAmount = 0 };

            double monthlyInflationFactorDecimal = Math.Pow(AnnualInflationRate / 100 + 1, 1d / 12);

            foreach (var cashFlowPeriod in _cashFlowPeriods)
            {
                double currentMonthlyWithdrawalAmount = cashFlowPeriod.MonthlyAmount;
                int firstIndex = ((cashFlowPeriod.StartingMonth.Year - StartingMonth.Year) * 12 + cashFlowPeriod.StartingMonth.Month - StartingMonth.Month),
                    lastIndex = DurationInMonths - 1 - ((EndingMonth.Year - cashFlowPeriod.EndingMonth.Year) * 12 + EndingMonth.Month - cashFlowPeriod.EndingMonth.Month);

                for (int i = 0; i < DurationInMonths; i++)
                {
                    if (cashFlowPeriod.InflationOption == InflationOptions.FromAnalysisStart)
                        currentMonthlyWithdrawalAmount *= monthlyInflationFactorDecimal;

                    if (i >= firstIndex && i <= lastIndex)
                    {
                        if (cashFlowPeriod.InflationOption == InflationOptions.FromPeriodStart)
                            currentMonthlyWithdrawalAmount *= monthlyInflationFactorDecimal;

                        newMonthlyWithdrawalAmounts[i].MonthlyAmount += currentMonthlyWithdrawalAmount;
                    }
                }
            }

            _monthlyCashFlowAmounts = [.. newMonthlyWithdrawalAmounts];
        }

        private void UpdateNumberOfRuns()
        {
            if (AnnualVolatility != 0 && AnnualReturn != 0)
                _numberOfRuns = NumberOfMultipleRuns;
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

        private double[] GetMonthlyReturns(Random random)
        {
            double[] output = new double[DurationInMonths];

            if (AnnualReturn == 0)
                return output;

            double monthlyReturn = Math.Pow(AnnualReturn / 100 + 1, 1d / 12) - 1;

            if (AnnualVolatility == 0)
            {
                for (int i = 0; i < DurationInMonths; i++)
                    output[i] = monthlyReturn;
            }
            else
            {
                double monthlyVolatility = Math.Pow(AnnualVolatility / 100 + 1, 1d / 12) - 1;
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
            ConcurrentBag<List<(DateTime, double)>> output = [];
            int seed = (int)DateTime.Now.Ticks;

            Parallel.For(0, _numberOfRuns, run =>
            {
                var currentRun = new (DateTime, double)[DurationInMonths + 1];
                DateTime currentMonth = StartingMonth;
                var monthlyReturns = GetMonthlyReturns(new Random(seed + run));

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
                            currentRun[i] = (currentMonth, currentRun[i - 1].Item2 * (1 + monthlyReturns[i - 1]) + _monthlyCashFlowAmounts[i - 1].MonthlyAmount);
                            currentMonth = currentMonth.AddDays(1).AddMonths(1).AddDays(-1);
                        }
                    }
                }

                output.Add([.. currentRun]);
            });

            return [.. output];
        }

        /// <summary>
        /// Calculates the average values for a sequence of date-value pairs across multiple lists.
        /// </summary>
        /// <remarks>The method assumes that all inner lists have the same length. If the input
        /// lists are empty, the method will return an empty list.</remarks>
        /// <param name="lists">A list of lists, where each inner list contains tuples of a <see langword="DateTime"/>
        /// and a double value. Each inner list must have the same number of elements.</param>
        /// <returns>A list of tuples, each containing the <see langword="DateTime"/> of the given first list
        /// and the average of the corresponding double values from the input lists.</returns>
        public static List<(DateTime, double)> CalculateAverageList(List<List<(DateTime, double)>> lists)
        {
            List<(DateTime, double)> output = [];

            for (int j = 0; j < lists[0].Count; j++)
            {
                double sum = 0;

                for (int i = 0; i < lists.Count; i++)
                    sum += lists[i][j].Item2;

                output.Add((lists[0][j].Item1, sum / lists.Count));
            }

            return output;
        }

        /// <summary>
        /// Calculates the probability of default for a sequence of multiple lists.
        /// The probability of default is defined as the percentage of lists where the last double value is negative.
        /// </summary>
        /// <param name="lists">A list of lists, where each inner list contains tuples of a <see langword="DateTime"/>
        /// and a double value. Each inner list must have the same number of elements.</param>
        /// <returns>The probality of default for the given list of lists.</returns>
        public static double CalculateProbabilityOfDefaultAsPercentage(List<List<(DateTime, double)>> lists)
            => lists.Count(x => x.Last().Item2 < 0) / (double)lists.Count * 100;

        /// <summary>
        /// Adds a new cash flow period with starting month and ending month based on the given values and the specified monthly amount.
        /// </summary>
        /// <param name="startingMonth">The starting month of the cash flow period. The day is ignored; the first day of the month is always used.</param>
        /// <param name="endingMonth">The ending month of the cash flow period. Must follow the starting month. The day is ignored;
        /// the last day of the month is always used.
        /// <param name="amount">The monthly amount that will be the base for the amounts of each month during the specified period.</param>
        /// <param name="inflationOption">The inflation option to apply to the cash flow period.</param>
        /// <exception cref="ArgumentOutOfRangeException">This is triggered if the end month does not follow the start month.</exception>
        public void AddCashFlowPeriod(DateTime startingMonth, DateTime endingMonth, double amount, InflationOptions inflationOption = InflationOptions.FromAnalysisStart)
        {
            if (endingMonth <= startingMonth)
                throw new ArgumentOutOfRangeException(null, Properties.Resources.FireCalculator_AddCashFlowPeriod_ArgumentException);

            _cashFlowPeriods.Add(new CashFlowPeriod()
            {
                StartingMonth = new DateTime(startingMonth.Year, startingMonth.Month, 1),
                EndingMonth = (new DateTime(endingMonth.Year, endingMonth.Month, 1)).AddMonths(1).AddDays(-1),
                MonthlyAmount = amount,
                InflationOption = inflationOption
            });

            UpdateValuesFromCashFlowPeriods();
        }

        private void UpdateValuesFromCashFlowPeriods()
        {
            _cashFlowPeriods = [.. _cashFlowPeriods.OrderBy(x => x.StartingMonth).ThenBy(x => x.EndingMonth).ThenBy(x => x.MonthlyAmount)];
            _startingMonth = _cashFlowPeriods.Min(x => x.StartingMonth);
            _endingMonth = _cashFlowPeriods.Max(x => x.EndingMonth);
            _durationInMonths = (_endingMonth.Year - _startingMonth.Year) * 12 + _endingMonth.Month - _startingMonth.Month + 1;
            UpdateMonthlyWithdrawalAmounts();
        }

        /// <summary>
        /// Removes the first occurence of the given cash flow period from the cash flow periods, if there are at least two cash flow periods.
        /// If the given cash flow period is not in the list of cash flow periods, no changes will be made.
        /// </summary>
        /// <param name="cashFlowPeriodToDelete">The cash flow period to remove.</param>
        public void RemoveCashFlowPeriod(CashFlowPeriod cashFlowPeriodToDelete)
        {
            if (_cashFlowPeriods.Count <= 1)
                return;

            _cashFlowPeriods.Remove(cashFlowPeriodToDelete);
            UpdateValuesFromCashFlowPeriods();
        }
    }
}