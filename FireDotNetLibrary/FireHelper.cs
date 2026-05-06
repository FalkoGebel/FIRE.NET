using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FireDotNetLibrary
{
    public static class FireHelper
    {
        public static string GetDisplayDescription(this Enum enumValue)
        {
            try
            {
                return enumValue.GetType()
                                .GetMember(enumValue.ToString())
                                .FirstOrDefault()?
                                .GetCustomAttribute<DisplayAttribute>()?
                                .GetDescription() ?? "unknown";

            }
            catch (InvalidOperationException)
            {
                return $"{enumValue}";
            }
        }
    }
}