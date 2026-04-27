using FireDotNetLibrary;
using FluentAssertions;

namespace FireDotNetLibraryTests
{
    [TestClass]
    public sealed class FireCalculatorTests
    {
        [TestMethod]
        public void Initialize_And_Correct_Default_Values()
        {
            // Arrange
            var today = DateTime.Now;
            var expectedStartingMonth = new DateTime(today.Year, today.Month, 1);
            var expectedDurationInMonths = 12 * 30; // 30 years
            var expectedEndingMonth = expectedStartingMonth.AddMonths(expectedDurationInMonths).AddDays(-1);

            // Act
            FireCalculator sut = new();

            // Assert
            sut.StartingMonth.Should().Be(expectedStartingMonth);
            sut.DurationInMonths.Should().Be(expectedDurationInMonths);
            sut.EndingMonth.Should().Be(expectedEndingMonth);
            sut.StartingAmount.Should().Be(0);
            sut.AnnualInflationRate.Should().Be(0);
        }

        [TestMethod]
        public void Calculate_With_Default_Values_Returns_Collection_With_Correct_Length()
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            var result = sut.GetRemainingAmounts();
            var remainingAmounts = result.ElementAt(0);

            // Assert
            remainingAmounts.Count.Should().Be(sut.DurationInMonths + 1);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1000)]
        [DataRow(360)]
        [DataRow(1500)]
        public void Calculate_Returns_Correct_Collection(double startingAmount)
        {
            double monthlyCashFlowAmount = -500;

            // Arrange
            FireCalculator sut = new()
            {
                StartingAmount = startingAmount,
            };
            double expectedFinalAmount = startingAmount + (monthlyCashFlowAmount * sut.DurationInMonths);

            // Act
            var result = sut.GetRemainingAmounts();
            var remainingAmounts = result.ElementAt(0);

            // Assert
            result.Count.Should().Be(1);
            remainingAmounts.Count.Should().Be(sut.DurationInMonths + 1);

            if (startingAmount == 0)
            {
                remainingAmounts.Sum(m => m.Item2).Should().Be(0);
            }
            else if (monthlyCashFlowAmount == 0)
            {
                remainingAmounts.All(m => m.Item2 == startingAmount).Should().BeTrue();
            }
            else
            {
                remainingAmounts[0].Item1.Should().Be(sut.StartingMonth);
                remainingAmounts[^1].Item1.Should().Be(sut.EndingMonth);
                remainingAmounts[^1].Item2.Should().Be(expectedFinalAmount);
            }
        }

        [TestMethod]
        [DataRow(-0.00001)]
        [DataRow(-1)]
        [DataRow(-1934.2134)]
        public void Set_Negative_AnnualInflationRate_Throws_Exception(double annualInflationRate)
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            Action act = () => sut.AnnualInflationRate = annualInflationRate;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("AnnualInflationRate must not be less than zero.");
        }

        [TestMethod]
        [DataRow(100000, 1)]
        [DataRow(1000, 1.5)]
        [DataRow(360, 6.75)]
        [DataRow(1500, 2.2)]
        public void Calculate_With_AnnualInflationRate_Returns_Correct_Collection(double startingAmount, double annualInflationRate)
        {
            double monthlyCashFlowAmount = -500;
            double monthlyInflationFactor = Math.Pow(annualInflationRate / 100 + 1, 1d / 12);

            // Arrange
            FireCalculator sut = new()
            {
                StartingAmount = startingAmount,
                AnnualInflationRate = annualInflationRate
            };

            double[] expectedResults = new double[sut.DurationInMonths + 1];
            expectedResults[0] = startingAmount;
            for (int i = 1; i < sut.DurationInMonths + 1; i++)
            {
                monthlyCashFlowAmount *= monthlyInflationFactor;
                expectedResults[i] = expectedResults[i - 1] + monthlyCashFlowAmount;
            }

            // Act
            var result = sut.GetRemainingAmounts();
            var remainingAmounts = result.ElementAt(0);

            // Assert
            remainingAmounts[0].Item1.Should().Be(sut.StartingMonth);
            remainingAmounts[^1].Item1.Should().Be(sut.EndingMonth);
            for (int i = 0; i < sut.DurationInMonths + 1; i++)
            {
                remainingAmounts[i].Item2.Should().Be(expectedResults[i]);
            }
        }

        [TestMethod]
        [DataRow(-0.00001)]
        [DataRow(-1)]
        [DataRow(-1934.2134)]
        public void Set_Negative_AnnualReturn_Throws_Exception(double annualReturn)
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            Action act = () => sut.AnnualReturn = annualReturn;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("AnnualReturn must not be less than zero.");
        }

        [TestMethod]
        [DataRow(100000, 1)]
        [DataRow(1000, 1.5)]
        [DataRow(360, 6.75)]
        [DataRow(1500, 2.2)]
        public void Calculate_With_AnnualReturn_Returns_Correct_Collection(double startingAmount, double annualReturn)
        {
            double monthlyCashFlowAmount = -500;
            double monthlyReturnFactor = Math.Pow(annualReturn / 100 + 1, 1d / 12);

            // Arrange
            FireCalculator sut = new()
            {
                StartingAmount = startingAmount,
                AnnualReturn = annualReturn
            };

            double[] expectedResults = new double[sut.DurationInMonths + 1];
            expectedResults[0] = startingAmount;
            for (int i = 1; i < sut.DurationInMonths + 1; i++)
                expectedResults[i] = expectedResults[i - 1] * monthlyReturnFactor + monthlyCashFlowAmount;

            // Act
            var result = sut.GetRemainingAmounts();
            var remainingAmounts = result.ElementAt(0);

            // Assert
            remainingAmounts[0].Item1.Should().Be(sut.StartingMonth);
            remainingAmounts[^1].Item1.Should().Be(sut.EndingMonth);
            for (int i = 0; i < sut.DurationInMonths + 1; i++)
            {
                remainingAmounts[i].Item2.Should().Be(expectedResults[i]);
            }
        }

        [TestMethod]
        [DataRow(-0.00001)]
        [DataRow(-1)]
        [DataRow(-1934.2134)]
        public void Set_Negative_AnnualVolatility_Throws_Exception(double annualVolatility)
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            Action act = () => sut.AnnualVolatility = annualVolatility;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("AnnualVolatility must not be less than zero.");
        }

        /* Tested method is now private
        [TestMethod]
        [DataRow(7, 0)]
        [DataRow(7, 14)]
        [DataRow(15, 30)]
        public void Get_Ten_Thousand_Aacceptable_Random_Monthly_Returns_For_Given_Mean_And_Standard_Deviation(double mean, double standardDeviation)
        {
            // Arrange
            FireCalculator sut = new()
            {
                AnnualReturn = mean,
                AnnualVolatility = standardDeviation,
                DurationInMonths = 10000
            };
            double monthlyMean = Math.Pow((double)sut.AnnualReturn / 100 + 1, 1d / 12) - 1,
                   monthlyStandardDeviation = sut.AnnualVolatility > 0
                                                ? Math.Pow((double)sut.AnnualVolatility / 100 + 1, 1d / 12) - 1
                                                : 0;


            // Act
            var randomNumbers = sut.GetMonthlyReturns();

            // Assert
            randomNumbers.Length.Should().Be(sut.DurationInMonths);
            double actualMean = randomNumbers.Average();
            actualMean.Should().BeApproximately((double)monthlyMean, 0.02);
            double actualStandardDeviation = Math.Sqrt(randomNumbers.Select(x => Math.Pow(x - actualMean, 2)).Average());
            actualStandardDeviation.Should().BeApproximately((double)monthlyStandardDeviation, 0.02);
        }
        */

        [TestMethod]
        public void Calculate_Average_List_For_List_Of_Lists()
        {
            // Arrange
            Random rnd = new();
            List<List<(DateTime, double)>> lists = [];
            List<(DateTime, double)> averageList = [];
            DateTime now = DateTime.Now;

            for (int i = 0; i < 10000; i++)
            {
                List<(DateTime, double)> list = [];

                for (int j = 0; j < 1000; j++)
                {
                    double n = rnd.NextDouble() * Int64.MaxValue;
                    list.Add((now, n));
                }

                lists.Add(list);
            }

            for (int i = 0; i < lists[0].Count; i++)
            {
                double sum = 0;

                for (int j = 0; j < lists.Count; j++)
                    sum += lists[j][i].Item2;

                averageList.Add((now, sum / lists.Count));
            }

            // Act
            List<(DateTime, double)> result = FireCalculator.CalculateAverageList(lists);

            // Assert
            result.Should().BeEquivalentTo(averageList);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(-10, 10)]
        [DataRow(-1, 10)]
        [DataRow(-100, 10)]
        public void Calculate_probability_Of_Default_For_List_Of_Lists(double minValue, double maxValue)
        {
            // Arrange
            Random rnd = new();
            List<List<(DateTime, double)>> lists = [];
            DateTime now = DateTime.Now;
            int numberOfNegativeLastValues = 0;

            for (int i = 0; i < 10000; i++)
            {
                List<(DateTime, double)> list = [];

                for (int j = 0; j < 10; j++)
                {
                    double n = rnd.NextDouble() * (maxValue - minValue) + minValue;
                    list.Add((now, n));
                }

                lists.Add(list);

                if (list[^1].Item2 < 0)
                    numberOfNegativeLastValues++;
            }

            double expectedProbabilityOfDefault = (double)numberOfNegativeLastValues / lists.Count * 100;

            // Act
            double probabilityOfDefault = FireCalculator.CalculateProbabilityOfDefaultAsPercentage(lists);

            // Assert
            probabilityOfDefault.Should().Be(expectedProbabilityOfDefault);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-12)]
        [DataRow(-1213)]
        public void Set_Invalid_NumberOfMultipleRuns_Throws_Exception(int numberOfMultipleRuns)
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            Action act = () => sut.NumberOfMultipleRuns = numberOfMultipleRuns;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("NumberOfMultipleRuns has to be positive.");
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(1000)]
        [DataRow(10000)]
        [DataRow(100000)]
        public void Set_Valid_NumberOfMultipleRuns_And_Get_Correct_Number_Of_Runs_From_Calculation(int numberOfMultipleRuns)
        {
            // Arrange
            FireCalculator sut = new()
            {
                AnnualReturn = 2,
                AnnualVolatility = 1,
                NumberOfMultipleRuns = numberOfMultipleRuns
            };

            // Act
            var result = sut.GetRemainingAmounts();

            // Assert
            result.Count.Should().Be(numberOfMultipleRuns);
        }

        [TestMethod]
        public void Init_FireCalculator_And_Get_Standard_CashFlowPeriods_AndMonthlyCashFlowAmounts()
        {
            // Arrange
            FireCalculator sut = new();
            var today = DateTime.Now;
            var expectedStartingMonth = new DateTime(today.Year, today.Month, 1);
            var expectedEndingMonth = expectedStartingMonth.AddMonths(12 * 30).AddDays(-1);

            // Act
            var result = sut.CashFlowPeriods;
            var monthlyCashFlowAmounts = sut.MonthlyCashFlowAmounts;

            // Assert
            result.Count.Should().Be(1);
            result[0].StartingMonth.Should().Be(expectedStartingMonth);
            result[0].EndingMonth.Should().Be(expectedEndingMonth);
            result[0].MonthlyAmount.Should().Be(-500);
            monthlyCashFlowAmounts[0].EndOfMonth.Should().Be(sut.StartingMonth.AddMonths(1).AddDays(-1));
            monthlyCashFlowAmounts[^1].EndOfMonth.Should().Be(sut.EndingMonth);
        }

        [TestMethod]
        [DataRow(2025, 7, 1, 2020, 9, 30, -250)]
        [DataRow(2025, 7, 1, 2025, 7, 1, -250)]
        [DataRow(2025, 7, 10, 2025, 7, 1, -250)]
        [DataRow(2025, 7, 1, 1999, 8, 31, -250)]
        public void Add_Invalid_CashFlowPeriod_Throws_Exception(int startingYear, int startingMonthNumber, int startingDay,
            int endingYear, int endingMonthNumber, int endingDay, double cashFlowAmount)
        {
            // Arrange
            FireCalculator sut = new();
            var startingMonth = new DateTime(startingYear, startingMonthNumber, startingDay);
            var endingMonth = new DateTime(endingYear, endingMonthNumber, endingDay);

            // Act
            Action act = () => sut.AddCashFlowPeriod(startingMonth, endingMonth, cashFlowAmount);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("EndingMonth has to follow StartingMonth.");
        }

        [TestMethod]
        [DataRow(2020, 7, 1, 2025, 9, 30, -250, 0)]
        [DataRow(2020, 7, 1, 2025, 9, 30, 250, 0)]
        [DataRow(2020, 7, 1, 2025, 9, 30, 0, 0)]
        [DataRow(2020, 7, 1, 2085, 5, 1, -0.01, 0)]
        [DataRow(1965, 1, 30, 1965, 1, 31, -999, 0)]
        [DataRow(1966, 2, 5, 2066, 1, 5, 33.33, 0)]
        public void Add_Another_CashFlowPeriod_And_Get_Correct_CashFlowPeriods_AndMonthlyCashFlowAmounts(int startingYear, int startingMonthNumber, int startingDay,
            int endingYear, int endingMonthNumber, int endingDay, double cashFlowAmount, int indexToCheck)
        {
            // Arrange
            FireCalculator sut = new();
            var startingMonth = new DateTime(startingYear, startingMonthNumber, startingDay);
            var endingMonth = new DateTime(endingYear, endingMonthNumber, endingDay);
            var monthAfterEndingMonth = endingMonth.AddMonths(1);
            var expectedStartingMonth = new DateTime(startingYear, startingMonthNumber, 1);
            var expectedEndingMonth = new DateTime(monthAfterEndingMonth.Year, monthAfterEndingMonth.Month, 1).AddDays(-1);

            // Act
            sut.AddCashFlowPeriod(startingMonth, endingMonth, cashFlowAmount);
            var result = sut.CashFlowPeriods;
            var monthlyCashFlowAmounts = sut.MonthlyCashFlowAmounts;

            // Assert
            result.Count.Should().Be(2);
            result[indexToCheck].StartingMonth.Should().Be(expectedStartingMonth);
            result[indexToCheck].EndingMonth.Should().Be(expectedEndingMonth);
            result[indexToCheck].MonthlyAmount.Should().Be(cashFlowAmount);
            monthlyCashFlowAmounts[0].EndOfMonth.Should().Be(sut.StartingMonth.AddMonths(1).AddDays(-1));
            monthlyCashFlowAmounts[^1].EndOfMonth.Should().Be(sut.EndingMonth);
        }

        [TestMethod]
        [DataRow(2020, 7, 1, 2025, 9, 30, -250)]
        [DataRow(2020, 7, 1, 2025, 9, 30, 250)]
        [DataRow(2020, 7, 1, 2025, 9, 30, 0)]
        [DataRow(2020, 7, 1, 2085, 5, 1, -0.01)]
        [DataRow(1965, 1, 30, 1965, 1, 31, -999)]
        [DataRow(1966, 2, 5, 2066, 1, 5, 33.33)]
        public void Remove_Default_CashFlowPeriod_And_Get_Correct_CashFlowPeriods_AndMonthlyCashFlowAmounts(int startingYear, int startingMonthNumber, int startingDay,
            int endingYear, int endingMonthNumber, int endingDay, double cashFlowAmount)
        {
            // Arrange
            FireCalculator sut = new();
            var cashFlowPeriodToDelete = sut.CashFlowPeriods[0];
            var startingMonth = new DateTime(startingYear, startingMonthNumber, startingDay);
            var endingMonth = new DateTime(endingYear, endingMonthNumber, endingDay);
            var monthAfterEndingMonth = endingMonth.AddMonths(1);
            var expectedStartingMonth = new DateTime(startingYear, startingMonthNumber, 1);
            var expectedEndingMonth = new DateTime(monthAfterEndingMonth.Year, monthAfterEndingMonth.Month, 1).AddDays(-1);
            sut.AddCashFlowPeriod(startingMonth, endingMonth, cashFlowAmount);

            // Act
            sut.RemoveCashFlowPeriod(cashFlowPeriodToDelete);
            var result = sut.CashFlowPeriods;
            var monthlyCashFlowAmounts = sut.MonthlyCashFlowAmounts;

            // Assert
            result.Count.Should().Be(1);
            result[0].StartingMonth.Should().Be(expectedStartingMonth);
            result[0].EndingMonth.Should().Be(expectedEndingMonth);
            result[0].MonthlyAmount.Should().Be(cashFlowAmount);
            monthlyCashFlowAmounts[0].EndOfMonth.Should().Be(sut.StartingMonth.AddMonths(1).AddDays(-1));
            monthlyCashFlowAmounts[^1].EndOfMonth.Should().Be(sut.EndingMonth);
        }
    }
}