using System.ComponentModel.DataAnnotations;

namespace FireDotNetLibrary
{
    public enum InflationOptions
    {
        [Display(Description = "Enums_InflationOptions_NoInflation", ResourceType = typeof(Properties.Resources))]
        NoInflation = 0,
        [Display(Description = "Enums_InflationOptions_FromPeriodStart", ResourceType = typeof(Properties.Resources))]
        FromPeriodStart = 1,
        [Display(Description = "Enums_InflationOptions_FromAnalysisStart", ResourceType = typeof(Properties.Resources))]
        FromAnalysisStart = 2
    }
}