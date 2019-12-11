using System.Linq;
using System;
using System.Collections.Generic;
using InteractiveConsole.Models;

namespace InteractiveConsole.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNumber(this Type type)
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

        public static bool IsBool(this Type type)
        {
            return type.Equals(typeof(bool));
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

        public static TypeInfo ToTypeInfo(this object obj)
        {
            return obj.GetType().ToTypeInfo();
        }

        public static TypeInfo ToTypeInfo(this Type type)
        {
            var typeInfo = new TypeInfo
            {
                IsEnum = type.IsEnum,
                IsList = type.IsList(),
                IsString = type.IsString(),
                IsBool = type.IsBool()
            };

            if (!typeInfo.IsEnum)
            {
                typeInfo.IsNumber = type.IsNumber();
            }

            if (typeInfo.IsList)
            {
                var listItemType = type.GetListItemType();
                typeInfo.IsListItemString = listItemType.IsString();
                typeInfo.IsListItemNumber = listItemType.IsNumber();

                if (!typeInfo.IsListItemNumber
                    && !typeInfo.IsListItemString)
                {
                    typeInfo.IsListItemCustomObject = true;
                    typeInfo.ListItemObjectName = listItemType.Name;
                }
            }

            if (!typeInfo.IsList
                && !typeInfo.IsNumber
                && !typeInfo.IsString
                && !typeInfo.IsEnum
                && !typeInfo.IsBool)
            {
                typeInfo.IsCustomObject = true;
                typeInfo.ObjectName = type.Name;
            }

            return typeInfo;
        }
    }
}