using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 将整数或其他枚举值转换为指定的枚举类型。
        /// </summary>
        public static TEnum ToEnum<TEnum>(this object value)
            where TEnum : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Type enumType = typeof(TEnum);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException(
                    "泛型类型必须是枚举类型。");
            }

            return (TEnum)Enum.ToObject(enumType, value);
        }
    }
}
