using FireDotNetLibrary;
using FluentAssertions;
using System.Globalization;

namespace FireDotNetLibraryTests
{
    [TestClass]
    public sealed class FireHelperTests
    {
        [TestMethod]
        [DataRow("en-US", "No inflation")]
        public void TestTranslationOfDescriptionAttribute(string culture, string expectedValue)
        {
            // Arrange
            CultureInfo cultureInfo = new(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            InflationOptions inflationOption = InflationOptions.NoInflation;

            // Act
            string description = inflationOption.GetDisplayDescription();

            // Assert
            description.Should().Be(expectedValue);
        }
    }
}