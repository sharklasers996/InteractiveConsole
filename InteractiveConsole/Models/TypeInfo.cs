using System;

namespace InteractiveConsole.Models
{
    public class TypeInfo : IEquatable<TypeInfo>
    {
        public Type Type { get; set; }
        public bool IsNumber { get; set; }
        public bool IsString { get; set; }
        public bool IsEnum { get; set; }
        public bool IsCustomObject { get; set; }
        public string ObjectName { get; set; }
        public bool IsBool { get; set; }

        public bool IsList { get; set; }
        public bool IsListItemNumber { get; set; }
        public bool IsListItemString { get; set; }
        public bool IsListItemCustomObject { get; set; }
        public string ListItemObjectName { get; set; }

        public bool Equals(TypeInfo other)
        {
            if ((other.IsNumber && !IsNumber)
                || (other.IsString && !IsString)
                || (other.IsEnum && !IsEnum)
                || (other.IsBool && !IsBool)
                || (other.IsCustomObject && !IsCustomObject)
                || (other.IsList && !IsList))
            {
                return false;
            }

            if (other.IsList)
            {
                if ((other.IsListItemNumber && !IsListItemNumber)
                    || (other.IsListItemString && !IsListItemString)
                    || (other.IsListItemCustomObject && !IsListItemCustomObject))
                {
                    return false;
                }
            }

            return true;
        }

        public bool EqualsListType(TypeInfo other)
        {
            if (!other.IsList)
            {
                return false;
            }

            if ((other.IsListItemNumber && !IsNumber)
                || (other.IsListItemString && !IsString)
                || (other.IsListItemCustomObject && !IsCustomObject))
            {
                return false;
            }

            return true;
        }
    }
}