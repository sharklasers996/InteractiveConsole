using System.Collections.Generic;
using InteractiveConsole.Models;

namespace InteractiveConsole.Extensions
{
    public static class CommandInfoExtensions
    {
        public static string GetNextOptionValue(this List<string> availableValues, string currentValue)
        {
            var currentOptionValueIndex = availableValues.IndexOf(currentValue);
            var nextIndex = currentOptionValueIndex + 1;
            if (nextIndex >= availableValues.Count)
            {
                nextIndex = 0;
            }

            return availableValues[nextIndex];
        }

        public static string GetPreviousOptionValue(this List<string> availableValues, string currentValue)
        {
            var currentOptionValueIndex = availableValues.IndexOf(currentValue);
            var nextIndex = currentOptionValueIndex - 1;
            if (nextIndex < 0)
            {
                nextIndex = availableValues.Count - 1;
            }

            return availableValues[nextIndex];
        }

        public static CommandOptionInfo CycleNext(this List<CommandOptionInfo> options, CommandOptionInfo currentOption)
        {
            var optionIndex = options.IndexOf(currentOption);
            var nextIndex = optionIndex + 1;
            if (nextIndex >= options.Count)
            {
                nextIndex = 0;
            }

            return options[nextIndex];
        }
    }
}