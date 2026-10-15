using System.ComponentModel;
using System.Reflection;

namespace LZ.Model
{

    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的描述信息
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// 根据枚举值的list 返回所有的描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Descriptionstr<T>(this List<string> list)
        {
            var des = string.Empty;
            foreach (var value in list)
            {
                des = des + ((Enum)Enum.Parse(typeof(T), value)).GetDescription() + ",";
            }
            return des.TrimEnd(',');
        }

        /// <summary>
        /// 根据枚举值的list 返回所有的描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Descriptionstr<T>(this List<int> list)
        {
            var des = string.Empty;
            foreach (var value in list)
            {
                des = des + ((Enum)Enum.ToObject(typeof(T), value)).GetDescription() + ",";
            }
            return des.TrimEnd(',');
        }
    }
}
