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
            var expectedStartMonth = new DateOnly(today.Year, today.Month, 1);
            var expectedDurationInMonths = 12 * 30; // 30 years
            var expectedEndMonth = expectedStartMonth.AddMonths(expectedDurationInMonths).AddDays(-1);

            // Act
            FireCalculator sut = new();

            // Assert
            sut.StartMonth.Should().Be(expectedStartMonth);
            sut.DurationInMonths.Should().Be(expectedDurationInMonths);
            sut.EndMonth.Should().Be(expectedEndMonth);
        }

        [TestMethod]
        [DataRow(2024, 1)]
        [DataRow(2039, 5)]
        [DataRow(2010, 11)]
        [DataRow(1999, 7)]
        public void Set_StartMonth_Updates_EndMonth(int startYear, int startMonth)
        {
            // Arrange
            FireCalculator sut = new();
            var newStartMonth = new DateOnly(startYear, startMonth, 15);

            // Act
            sut.StartMonth = newStartMonth;

            // Assert
            sut.StartMonth.Should().Be(new DateOnly(startYear, startMonth, 1));
            sut.EndMonth.Should().Be(sut.StartMonth.AddMonths(12 * 30).AddDays(-1));
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
            sut.EndMonth.Should().Be(sut.StartMonth.AddMonths(months).AddDays(-1));
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
    }
}