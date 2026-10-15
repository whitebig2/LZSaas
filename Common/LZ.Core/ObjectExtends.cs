using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LZ.Core
{
    public static class ObjectExtends
    {
        public static string ToNullString(this object obj)
        {
            if (obj != null || obj == DBNull.Value)
                return obj.ToString().Trim();
            return string.Empty;
        }

        public static int ToInt(this object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            if (typeof(Decimal) == obj.GetType())
                obj = (object)Math.Floor((Decimal)obj);
            else if (typeof(double) == obj.GetType())
                obj = (object)Math.Floor((double)obj);
            else if (typeof(string) == obj.GetType() && obj.ToNullString().IndexOf('.') > -1)
                obj = (object)obj.ToNullString().Substring(0, obj.ToNullString().IndexOf('.'));
            string nullString = obj.ToNullString();
            int result = 0;
            int.TryParse(nullString, out result);
            return result;
        }

        public static long ToLong(this object obj)
        {
            string nullString = obj.ToNullString();
            long result = 0;
            long.TryParse(nullString, out result);
            return result;
        }

        public static bool ToBool(this object obj)
        {
            bool result = false;
            string nullString = obj.ToNullString();
            if (nullString == "1")
                return true;
            bool.TryParse(nullString, out result);
            return result;
        }

        public static Decimal ToDecimal(this object obj)
        {
            string nullString = obj.ToNullString();
            Decimal result = new Decimal();
            Decimal.TryParse(nullString, out result);
            return result;
        }

        public static double ToDouble(this object obj)
        {
            string nullString = obj.ToNullString();
            double result = 0.0;
            double.TryParse(nullString, out result);
            return result;
        }

        public static DateTime? ToDateTime(this object obj)
        {
            string nullString = obj.ToNullString();
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(nullString, out result);
            if (result == DateTime.MinValue)
                return new DateTime?();
            return new DateTime?(result);
        }

        public static bool IsDecimal(this object obj)
        {
            Decimal result = new Decimal();
            return Decimal.TryParse(obj.ToNullString(), out result);
        }

        public static bool IsInt(this object obj)
        {
            int result = 0;
            return int.TryParse(obj.ToNullString(), out result);
        }

        public static bool IsPositiveInteger(this object obj)
        {
            return Regex.IsMatch(obj.ToNullString(), "[0-9]\\d*");
        }

        public static Dictionary<string, object> ToDictionary(this object o)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo property in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                MethodInfo getMethod = property.GetGetMethod();
                if (getMethod != (MethodInfo)null && getMethod.IsPublic)
                    dictionary.Add(property.Name, getMethod.Invoke(o, new object[0]));
            }
            return dictionary;
        }
    }
}
