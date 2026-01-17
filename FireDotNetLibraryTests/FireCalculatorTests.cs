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
            var today = DateOnly.FromDateTime(DateTime.Now);
            var expectedStartingMonth = new DateOnly(today.Year, today.Month, 1);
            var expectedDurationInMonths = 12 * 30; // 30 years
            var expectedEndingMonth = expectedStartingMonth.AddMonths(expectedDurationInMonths).AddDays(-1);

            // Act
            FireCalculator sut = new();

            // Assert
            sut.StartingMonth.Should().Be(expectedStartingMonth);
            sut.DurationInMonths.Should().Be(expectedDurationInMonths);
            sut.EndingMonth.Should().Be(expectedEndingMonth);
            sut.StartingAmount.Should().Be(0m);
            sut.MonthlyWithdrawalAmount.Should().Be(0m);
            sut.AnnualWithdrawalAmount.Should().Be(0m);
        }

        [TestMethod]
        [DataRow(2024, 1)]
        [DataRow(2039, 5)]
        [DataRow(2010, 11)]
        [DataRow(1999, 7)]
        public void Set_StartMonth_Updates_EndMonth(int startingYear, int startingMonth)
        {
            // Arrange
            FireCalculator sut = new();
            var newStartingMonth = new DateOnly(startingYear, startingMonth, 15);

            // Act
            sut.StartingMonth = newStartingMonth;

            // Assert
            sut.StartingMonth.Should().Be(new DateOnly(startingYear, startingMonth, 1));
            sut.EndingMonth.Should().Be(sut.StartingMonth.AddMonths(12 * 30).AddDays(-1));
        }

        [TestMethod]
        [DataRow(2 * 12)]
        [DataRow(18)]
        [DataRow(12 * 55)]
        [DataRow(33 * 12 + 4)]
        public void Set_Duration_Updates_EndMonth(int months)
        {
            // Arrange + Act
            FireCalculator sut = new()
            {
                DurationInMonths = months
            };

            // Assert
            sut.EndingMonth.Should().Be(sut.StartingMonth.AddMonths(months).AddDays(-1));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-11)]
        [DataRow(-15795035)]
        public void Set_Invalid_Duration_Throws_Exception(int months)
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            Action act = () => sut.DurationInMonths = months;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("DurationInMonths has to be positive.");
        }

        [TestMethod]
        [DataRow(1, 15)]
        [DataRow(5, 10)]
        [DataRow(11, 12)]
        [DataRow(7, 2)]
        [DataRow(12 * 10, 20)]
        [DataRow(12 * 15, 1)]
        public void Set_EndMonth_Updates_Duration(int months, int days)
        {
            // Arrange
            FireCalculator sut = new();
            var newEndingMonth = sut.StartingMonth.AddDays(-days).AddMonths(months);

            // Act
            sut.EndingMonth = newEndingMonth;

            // Assert
            sut.DurationInMonths.Should().Be(months);
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(-1, 5)]
        [DataRow(-11, 17)]
        public void Set_Invalid_EndMonth_Throws_Exception(int months, int days)
        {
            // Arrange
            FireCalculator sut = new();
            var newEndingMonth = sut.StartingMonth.AddDays(-days).AddMonths(months);

            // Act
            Action act = () => sut.EndingMonth = newEndingMonth;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("EndingMonth must not be earlier than StartingMonth.");
        }

        [TestMethod]
        [DataRow(100)]
        [DataRow(5)]
        [DataRow(12)]
        [DataRow(2.548)]
        [DataRow(12 * 10)]
        [DataRow(12.63874 * 15)]
        public void Set_MontlyWithdrawalAmount_Updates_AnnualWithdrawalAmount(double monthlyWidthdrawalAmount)
        {
            // Arrange + Act
            FireCalculator sut = new()
            {
                MonthlyWithdrawalAmount = (decimal)monthlyWidthdrawalAmount
            };

            // Assert
            sut.AnnualWithdrawalAmount.Should().Be((decimal)monthlyWidthdrawalAmount * 12);
        }

        [TestMethod]
        [DataRow(-0.00001)]
        [DataRow(-1)]
        [DataRow(-1934.2134)]
        public void Set_Invalid_MontlyWithdrawalAmount_Throws_Exception(double monthlyWidthdrawalAmount)
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            Action act = () => sut.MonthlyWithdrawalAmount = (decimal)monthlyWidthdrawalAmount;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("MonthlyWithdrawalAmount must not be less than zero.");
        }

        [TestMethod]
        [DataRow(100)]
        [DataRow(5)]
        [DataRow(12)]
        [DataRow(2.548)]
        [DataRow(12 * 10)]
        [DataRow(12.63874 * 15)]
        public void Set_AnnualWithdrawalAmount_Updates_MontlyWithdrawalAmount(double annualWidthdrawalAmount)
        {
            // Arrange + Act
            FireCalculator sut = new()
            {
                AnnualWithdrawalAmount = (decimal)annualWidthdrawalAmount
            };

            // Assert
            sut.MonthlyWithdrawalAmount.Should().Be((decimal)annualWidthdrawalAmount / 12);
        }

        [TestMethod]
        [DataRow(-0.00001)]
        [DataRow(-1)]
        [DataRow(-1934.2134)]
        public void Set_Invalid_AnnualWithdrawalAmount_Throws_Exception(double annualWidthdrawalAmount)
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            Action act = () => sut.AnnualWithdrawalAmount = (decimal)annualWidthdrawalAmount;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("AnnualWithdrawalAmount must not be less than zero.");
        }

        [TestMethod]
        public void Calculate_With_Default_Values_Returns_Collection_With_Correct_Length()
        {
            // Arrange
            FireCalculator sut = new();

            // Act
            var result = sut.GetRemainingAmounts();

            // Assert
            result.Length.Should().Be(sut.DurationInMonths + 1);
        }

        [TestMethod]
        [DataRow(0, 1000)]
        [DataRow(1000, 0)]
        [DataRow(360, 1)]
        [DataRow(1500, 20)]
        public void Calculate_Returns_Correct_Collection(double startingAmount, double monthlyWithdrawalAmount)
        {
            decimal startingAmountDecimal = (decimal)startingAmount;
            decimal monthlyWithdrawalAmountDecimal = (decimal)monthlyWithdrawalAmount;

            // Arrange
            FireCalculator sut = new()
            {
                StartingAmount = startingAmountDecimal,
                MonthlyWithdrawalAmount = monthlyWithdrawalAmountDecimal
            };

            // Act
            var result = sut.GetRemainingAmounts();

            // Assert
            result.Length.Should().Be(sut.DurationInMonths + 1);
            if (startingAmountDecimal == 0m)
            {
                result.Sum(m => m.Item2).Should().Be(0m);
            }
            else if (monthlyWithdrawalAmountDecimal == 0m)
            {
                result.All(m => m.Item2 == startingAmountDecimal).Should().BeTrue();
            }
            else
            {
                decimal expectedFinalAmount = startingAmountDecimal - (monthlyWithdrawalAmountDecimal * sut.DurationInMonths);
                DateOnly expectedFirstMonth = result[0].Item1;
                DateOnly expectedLastMonth = result[^1].Item1;
                decimal actualFinalAmount = result[^1].Item2;
                expectedFirstMonth.Should().Be(sut.StartingMonth);
                expectedLastMonth.Should().Be(sut.EndingMonth);
                actualFinalAmount.Should().Be(expectedFinalAmount);
            }
        }
    }
}