using System.Linq;
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

        public static CommandOptionInfo CyclePrevious(this List<CommandOptionInfo> commands, CommandOptionInfo currentOption)
        {
            var orderedCommands = commands.OrderBy(x => x.Name).ToList();
            var optionIndex = orderedCommands.IndexOf(currentOption);
            var nextIndex = optionIndex - 1;
            if (nextIndex < 0)
            {
                nextIndex = orderedCommands.Count - 1;
            }

            return orderedCommands[nextIndex];
        }

        public static CommandInfo CycleNext(this List<CommandInfo> commands, CommandInfo currentCommand)
        {
            var orderedCommands = commands.OrderBy(x => x.Name).ToList();
            var currentCommandIndex = orderedCommands.IndexOf(currentCommand);

            var nextIndex = currentCommandIndex + 1;
            if (nextIndex >= orderedCommands.Count)
            {
                nextIndex = 0;
            }

            return orderedCommands[nextIndex];
        }

        public static CommandInfo CyclePrevious(this List<CommandInfo> commands, CommandInfo currentCommand)
        {
            var currentCommandIndex = commands.IndexOf(currentCommand);

            var nextIndex = currentCommandIndex - 1;
            if (nextIndex < 0)
            {
                nextIndex = commands.Count - 1;
            }

            return commands[nextIndex];
        }
    }
}