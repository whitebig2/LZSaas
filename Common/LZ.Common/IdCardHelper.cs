using System;
using System.Globalization;

public class IdCardHelper
{

    public static int GetAge(string idNumber)
    {
        // 验证身份证号长度（仅处理18位）
        if (idNumber?.Length != 18)
        {
            return -1;
        }
        //throw new ArgumentException("身份证号长度无效，应为18位。");

        // 提取出生日期部分（yyyyMMdd格式）
        string birthDateStr = idNumber.Substring(6, 8);
        if (!DateTime.TryParseExact(birthDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
        {
            return -1;
        }
        //throw new ArgumentException("身份证号中包含无效的出生日期。");

        // 计算年龄
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;
        // 如果今年生日还未过，年龄减1
        if (birthDate.Date > today.AddYears(-age)) age--;

        return age;
    }

    public static string GetGender(string idNumber)
    {
        char genderCode = idNumber.ToCharArray()[16];
        if (!char.IsDigit(genderCode))
            throw new ArgumentException("身份证号性别码无效。");
        string gender = (int)char.GetNumericValue(genderCode) % 2 == 0 ? "女" : "男";
        return gender;
    }

    public static string GetBirthDate(string idNumber)
    {
        // 验证身份证号长度（仅处理18位）
        if (idNumber?.Length != 18)
        {
            return "number length error";
        }
        //throw new ArgumentException("身份证号长度无效，应为18位。");

        // 提取出生日期部分（yyyyMMdd格式）
        string birthDateStr = idNumber.Substring(6, 8);
        if (!DateTime.TryParseExact(birthDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
        {
            return "birthDate error";
        }

        return birthDate.ToString("yyyy-MM-dd");
    }

    public static (int Age, string Gender) GetAgeAndGender(string idNumber)
    {
        // 验证身份证号长度（仅处理18位）
        if (idNumber?.Length != 18)
            throw new ArgumentException("身份证号长度无效，应为18位。");

        // 提取出生日期部分（yyyyMMdd格式）
        string birthDateStr = idNumber.Substring(6, 8);
        if (!DateTime.TryParseExact(birthDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            throw new ArgumentException("身份证号中包含无效的出生日期。");

        // 计算年龄
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;
        // 如果今年生日还未过，年龄减1
        if (birthDate.Date > today.AddYears(-age)) age--;

        // 提取性别信息（第17位，0-based索引16）
        char genderCode = idNumber.ToCharArray()[16];
        if (!char.IsDigit(genderCode))
            throw new ArgumentException("身份证号性别码无效。");

        string gender = (int)char.GetNumericValue(genderCode) % 2 == 0 ? "女" : "男";

        return (age, gender);
    }


    public static DateTime GetRetireDate(string idNumber)
    {
        idNumber = idNumber.Trim();
        // 验证身份证号长度（仅处理18位）
        if (idNumber?.Length != 18)
            throw new ArgumentException("身份证号长度无效，应为18位。");

        // 提取出生日期部分（yyyyMMdd格式）
        string birthDateStr = idNumber.Substring(6, 8);
        if (!DateTime.TryParseExact(birthDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            throw new ArgumentException("身份证号中包含无效的出生日期。");
        // 提取性别信息（第17位，0-based索引16）
        char genderCode = idNumber.ToCharArray()[16];
        if (!char.IsDigit(genderCode))
            throw new ArgumentException("身份证号性别码无效。");

        string gender = (int)char.GetNumericValue(genderCode) % 2 == 0 ? "女" : "男";
        if (gender == "女")
        {
            return birthDate.AddYears(50);
        }
        else {
            return birthDate.AddYears(60);
        }
    }

    public static (int Age, string Gender) GetAgeAndGender(string idNumber,DateTime date)
    {
        // 验证身份证号长度（仅处理18位）
        if (idNumber?.Length != 18)
            throw new ArgumentException("身份证号长度无效，应为18位。");

        // 提取出生日期部分（yyyyMMdd格式）
        string birthDateStr = idNumber.Substring(6, 8);
        if (!DateTime.TryParseExact(birthDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            throw new ArgumentException("身份证号中包含无效的出生日期。");

        // 计算年龄
        DateTime today = date;
        int age = today.Year - birthDate.Year;
        // 如果今年生日还未过，年龄减1
        if (birthDate.Date > today.AddYears(-age)) age--;

        // 提取性别信息（第17位，0-based索引16）
        char genderCode = idNumber.ToCharArray()[16];
        if (!char.IsDigit(genderCode))
            throw new ArgumentException("身份证号性别码无效。");

        string gender = (int)char.GetNumericValue(genderCode) % 2 == 0 ? "女" : "男";

        return (age, gender);
    }



    public static (int Age, string Gender,DateTime birthDate) GetAgeAndGenderAndBirthDate(string idNumber)
    {
        // 验证身份证号长度（仅处理18位）
        if (idNumber?.Length != 18)
            throw new ArgumentException("身份证号长度无效，应为18位。");

        // 提取出生日期部分（yyyyMMdd格式）
        string birthDateStr = idNumber.Substring(6, 8);
        if (!DateTime.TryParseExact(birthDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            throw new ArgumentException("身份证号中包含无效的出生日期。");

        // 计算年龄
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;
        // 如果今年生日还未过，年龄减1
        if (birthDate.Date > today.AddYears(-age)) age--;

        // 提取性别信息（第17位，0-based索引16）
        char genderCode = idNumber.ToCharArray()[16];
        if (!char.IsDigit(genderCode))
            throw new ArgumentException("身份证号性别码无效。");

        string gender = (int)char.GetNumericValue(genderCode) % 2 == 0 ? "女" : "男";

        return (age, gender,birthDate);
    }


}
