using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    /// <summary>
    /// Helper class for Enum functionality
    /// </summary>
    public class EnumHelper
    {
        public static string GetEnumKey<T>(int enumValue)
        {
            return Enum.GetName(typeof(T), enumValue);
        }

        public static string GetDescription(Type enumType, object enumValue)
        {
            FieldInfo fi = enumType.GetField(enumValue.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attributes.Length > 0) ? attributes[0].Description : enumValue.ToString();
        }

        public static string GetDescription(Enum value)
        {
            try
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetDescription<T>(int enumValue)
        {
            Type type = typeof(T);
            FieldInfo fi = type.GetField(Enum.ToObject(typeof(T), enumValue).ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : enumValue.ToString();
        }


        public static IEnumerable<SelectItem> ConvertEnumToList<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("Type given T must be an Enum");
            }

            var result = Enum.GetValues(typeof(T))
                                .Cast<T>()
                                .Select(x => new SelectItem
                                {
                                    Id = Convert.ToInt32(x),
                                    //Text = x.ToString(new CultureInfo("en"))
                                    Text = GetDescription<T>(Convert.ToInt32(x))
                                })
                                .ToList()
                                .AsReadOnly();

            return result;
        }
    }
}
