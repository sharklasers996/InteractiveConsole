using System.Linq;
namespace InteractiveConsole.Extensions
{
    public static class StringExtensions
    {
        public static string ToFirstUpper(this string input)
        {
            if (input.Length == 0)
            {
                return input;
            }

            var firstLetter = input.First();

            return firstLetter.ToString().ToUpper() + input[1..];
        }
    }
}