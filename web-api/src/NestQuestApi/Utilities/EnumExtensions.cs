using System.ComponentModel;

namespace NestQuestApi.Utilities;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attribute = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                  .FirstOrDefault() as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }
}
