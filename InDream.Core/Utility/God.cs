
using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InDream.Core.Utility;

public static class God // helps us all
{
    public static bool IsEmailValid(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return emailRegex.IsMatch(email);
    }


    public static bool IsPasswordValid(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            return false;

        return true;
    }

    public static string GetEnumDescription<T>(this T value) where T : Enum
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

        return descriptionAttribute?.Description ?? "";
    }

    public static List<SelectListItem> GetEnumSelectList<TEnum>() where TEnum : Enum
    {
        var selectList = new List<SelectListItem>();

        foreach (var value in Enum.GetValues(typeof(TEnum)))
        {
            string description = GetEnumDescription((TEnum)value) ?? value.ToString();

            selectList.Add(new SelectListItem
            {
                Value = value.ToString(),
                Text = description
            });
        }

        return selectList;
    }
}
