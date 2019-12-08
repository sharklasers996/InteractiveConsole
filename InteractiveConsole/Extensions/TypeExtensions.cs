using System.Linq;
using System;
using System.Collections.Generic;

namespace InteractiveConsole.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsString(this Type type)
        {
            return type.Equals(typeof(string));
        }

        public static bool IsList(this Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static Type GetListItemType(this Type type)
        {
            return type.GetGenericArguments().FirstOrDefault();
        }
    }
}