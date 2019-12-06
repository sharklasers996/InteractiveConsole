using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InteractiveConsole.Extensions;
using InteractiveConsole.Models;

namespace InteractiveConsole
{
    public class AutoCompleteHandler : IAutoCompleteHandler
    {
        private const char Space = ' ';

        private readonly List<CommandInfo> _availableCommands;

        public AutoCompleteHandler()
        {
            var discovery = new CommandDiscovery();
            _availableCommands = discovery.AvailableCommands;
        }

        public string Complete(string input, bool next)
        {
            var trimmedInput = input.TrimStart();
            if (!trimmedInput.Contains(Space))
            {
                return CompleteCommand(trimmedInput, next);
            }

            return CompleteOption(input, next);
        }

        private string CompleteCommand(string input, bool next)
        {
            var command = _availableCommands.FirstOrDefault(x => x.NameWithoutSuffix.StartsWith(input, StringComparison.InvariantCultureIgnoreCase));
            if (command == null)
            {
                return input;
            }

            if (command.NameWithoutSuffix.Equals(input))
            {
                command = next == true
                    ? _availableCommands.CycleNext(command)
                    : _availableCommands.CyclePrevious(command);
            }

            return command.NameWithoutSuffix;
        }

        private string CompleteOption(string input, bool next)
        {
            var completedCommand = GetCompletedCommand(input);
            if (completedCommand == null)
            {
                return input;
            }

            var completedParameters = GetCompletedParamters(input).ToList();

            var wordToComplete = string.Empty;
            var lastSpaceIndex = input.LastIndexOf(Space);
            if (lastSpaceIndex < input.Length - 1)
            {
                wordToComplete = input
                    .Substring(lastSpaceIndex)
                    .Replace("=", "")
                    .Trim();
            }

            var availableOptions = completedCommand
                .Options
                .Where(x => !completedParameters.Contains(x.Name))
                .ToList();

            if (!String.IsNullOrEmpty(wordToComplete))
            {
                var optionToComplete = availableOptions.FirstOrDefault(x => x.Name.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
                if (optionToComplete != null)
                {
                    if (optionToComplete.Name.Equals(wordToComplete))
                    {
                        optionToComplete = next == true
                            ? availableOptions.CycleNext(optionToComplete)
                            : availableOptions.CyclePrevious(optionToComplete);
                    }

                    input = input.Substring(0, lastSpaceIndex + 1);
                    input += optionToComplete.Name + "=";

                    return input;
                }
            }

            if (availableOptions.Any())
            {
                input = input.Substring(0, lastSpaceIndex + 1);
                input += availableOptions.First().Name + "=";
            }

            return input;
        }

        public string CompleteOptionSelection(string input, int cursorPosition, bool next)
        {
            var completedCommand = GetCompletedCommand(input);
            if (completedCommand == null)
            {
                return input;
            }

            var wordAtCursor = GetWordAtCursor(input, cursorPosition);
            var equalSignIndex = wordAtCursor.IndexOf("=");
            if (equalSignIndex == -1)
            {
                return input;
            }

            var optionNameAtCursor = wordAtCursor.Substring(0, equalSignIndex);
            var optionValueAtCursor = wordAtCursor.Substring(equalSignIndex + 1);

            var completedCommandOption = completedCommand.Options.FirstOrDefault(x => x.Name == optionNameAtCursor);
            if (completedCommandOption != null
                && completedCommandOption.AvailableValues.Any())
            {
                var completedOption = string.Empty;
                if (String.IsNullOrEmpty(optionValueAtCursor))
                {
                    completedOption = completedCommandOption.AvailableValues.First();
                }
                else
                {
                    var currentOptionValue = completedCommandOption.AvailableValues.FirstOrDefault(x => x.StartsWith(optionValueAtCursor, StringComparison.InvariantCultureIgnoreCase));
                    if (currentOptionValue != null)
                    {
                        var nextOptionValue = next
                            ? completedCommandOption.AvailableValues.GetNextOptionValue(currentOptionValue)
                            : completedCommandOption.AvailableValues.GetPreviousOptionValue(currentOptionValue);

                        completedOption = nextOptionValue;
                    }
                }

                return input.Replace(
                    wordAtCursor,
                    ToKeyValueString(
                        optionNameAtCursor,
                        completedOption
                    )
                );
            }

            return input;
        }

        private string GetWordAtCursor(string input, int cursorPosition)
        {
            if (input.Length - 1 == cursorPosition)
            {
                var lastIndexOfSpace = input.LastIndexOf(Space);
                return input.Substring(lastIndexOfSpace).Trim();
            }

            var textBeforeCursor = input.Substring(0, cursorPosition);
            var textAfterCursor = input.Substring(cursorPosition);

            var beforeCursorSpaceIndex = textBeforeCursor.LastIndexOf(Space);
            textBeforeCursor = textBeforeCursor.Substring(beforeCursorSpaceIndex);

            var afterCursorSpaceIndex = textAfterCursor.IndexOf(Space);
            if (afterCursorSpaceIndex != -1)
            {
                textAfterCursor = textAfterCursor.Substring(0, afterCursorSpaceIndex);
            }

            return (textBeforeCursor + textAfterCursor).Trim();
        }

        private CommandInfo GetCompletedCommand(string input)
        {
            if (input.IndexOf(Space) == -1)
            {
                return null;
            }

            var command = input.Substring(0, input.IndexOf(Space));
            return _availableCommands.FirstOrDefault(x => x.NameWithoutSuffix == command);
        }

        private string ToKeyValueString(string key, string value)
        {
            return $"{key}={value}";
        }

        private IEnumerable<string> GetCompletedParamters(string input)
        {
            var completedParameterMatches = Regex.Matches(input, "\\s(?<parameterName>\\w+)=[^\\s]");
            foreach (Match match in completedParameterMatches)
            {
                yield return match.Groups["parameterName"].ToString();
            }
        }
    }
}