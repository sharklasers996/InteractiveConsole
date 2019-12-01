using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;
using System.Linq;
using System;
using System.Text;
using InteractiveConsole;
using System.Collections.Generic;
using InteractiveConsole.Models;
using System.Text.RegularExpressions;
using Console = Colorful.Console;
using System.Drawing;
using Colorful;
using System.IO;
using InteractiveConsole.Output;

namespace InteractiveConsoleAC
{
    class Program
    {
        static void Main(string[] args)
        {
            var ogre = FigletFont.Parse(PrinterFonts.Doh);
            var f = new Figlet(ogre);
            Console.WriteLine(f.ToAscii("Title!"), ColorTranslator.FromHtml("#558b2f"));

            // var fontFiles = Directory.GetFiles(@"c:\Users\PonasPx\Documents\FilgetFonts\");
            // foreach (var file in fontFiles)
            // {
            //     var font = FigletFont.Load(file);
            //     var figlet = new Figlet(font);
            //     Console.WriteLine(file);
            //     Console.WriteLine(figlet.ToAscii("Title!"), ColorTranslator.FromHtml("#558b2f"));
            // }

            // var font = FigletFont.Load("..\\fonts\\bulbhead.flf");
            // var figlet = new Figlet(font);
            // Console.WriteLine(figlet.ToAscii("Px Vapsie!"), ColorTranslator.FromHtml("#558b2f"));
            // Console.WriteAscii("Desra!", font, ColorTranslator.FromHtml("#558b2f"));
            //    Console.Write("Username: ", Color.FromArgb(32, 18, 66));
            Console.ReadLine();
            // var inputHandler = new InputHandler();

            // var a = inputHandler.Prompt("Username: ");
            // System.Console.WriteLine($"Result {a}");

            // Console.WriteLine("Enter text:");
            // while (true)
            // {
            //     var text = inputHandler.ReadLine();
            //     System.Console.WriteLine($"Text: {text}");
            // }
        }
    }

    // public class AutoCompleteHandler
    // {
    //     private readonly List<CommandInfo> _availableCommands;

    //     public AutoCompleteHandler()
    //     {
    //         var discovery = new CommandDiscovery();
    //         _availableCommands = discovery.AvailableCommands;
    //     }

    //     public string Complete(string input)
    //     {
    //         var trimmedInput = input.TrimStart();
    //         if (!trimmedInput.Contains(' '))
    //         {
    //             return CompleteCommand(trimmedInput);
    //         }

    //         return CompleteOption(input);
    //     }

    //     private string CompleteCommand(string input)
    //     {
    //         var command = _availableCommands.FirstOrDefault(x => x.NameWithoutSuffix.StartsWith(input, StringComparison.InvariantCultureIgnoreCase));
    //         if (command.NameWithoutSuffix.Equals(input))
    //         {
    //             var currentCommandIndex = _availableCommands.IndexOf(command);

    //             var nextIndex = currentCommandIndex + 1;
    //             if (nextIndex >= _availableCommands.Count)
    //             {
    //                 nextIndex = 0;
    //             }

    //             return _availableCommands[nextIndex].NameWithoutSuffix;
    //         }

    //         return command.NameWithoutSuffix;
    //     }

    //     private string CompleteOption(string input)
    //     {
    //         var completedCommand = GetCompletedCommand(input);
    //         var completedParameters = GetCompletedParamters(input).ToList();

    //         var wordToComplete = string.Empty;
    //         var lastSpaceIndex = input.LastIndexOf(' ');
    //         if (lastSpaceIndex < input.Length - 1)
    //         {
    //             wordToComplete = input
    //                 .Substring(lastSpaceIndex)
    //                 .Replace("=", "")
    //                 .Trim();
    //         }

    //         var availableOptions = completedCommand
    //             .Options
    //             .Where(x => !completedParameters.Contains(x.Name))
    //             .ToList();

    //         if (!String.IsNullOrEmpty(wordToComplete))
    //         {
    //             var optionToComplete = availableOptions.FirstOrDefault(x => x.Name.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
    //             if (optionToComplete != null)
    //             {
    //                 if (optionToComplete.Name.Equals(wordToComplete))
    //                 {
    //                     optionToComplete = availableOptions.CycleNext(optionToComplete);
    //                 }

    //                 input = input.Substring(0, lastSpaceIndex + 1);
    //                 input += optionToComplete.Name + "=";

    //                 return input;
    //             }
    //         }

    //         if (availableOptions.Any())
    //         {
    //             input = input.Substring(0, lastSpaceIndex + 1);
    //             input += availableOptions.First().Name + "=";
    //         }

    //         return input;
    //     }

    //     public string CompleteOptionSelection(string input, int cursorPosition, bool next)
    //     {
    //         var completedCommand = GetCompletedCommand(input);

    //         var wordAtCursor = GetWordAtCursor(input, cursorPosition);
    //         var equalSignIndex = wordAtCursor.IndexOf("=");
    //         if (equalSignIndex == -1)
    //         {
    //             return input;
    //         }

    //         var optionNameAtCursor = wordAtCursor.Substring(0, equalSignIndex);
    //         var optionValueAtCursor = wordAtCursor.Substring(equalSignIndex + 1);

    //         var completedCommandOption = completedCommand.Options.FirstOrDefault(x => x.Name == optionNameAtCursor);
    //         if (completedCommandOption != null
    //             && completedCommandOption.AvailableValues.Any())
    //         {
    //             var completedOption = string.Empty;
    //             if (String.IsNullOrEmpty(optionValueAtCursor))
    //             {
    //                 completedOption = completedCommandOption.AvailableValues.First();
    //             }
    //             else
    //             {
    //                 var currentOptionValue = completedCommandOption.AvailableValues.FirstOrDefault(x => x.StartsWith(optionValueAtCursor, StringComparison.InvariantCultureIgnoreCase));
    //                 if (currentOptionValue != null)
    //                 {
    //                     var nextOptionValue = next
    //                         ? completedCommandOption.AvailableValues.GetNextOptionValue(currentOptionValue)
    //                         : completedCommandOption.AvailableValues.GetPreviousOptionValue(currentOptionValue);

    //                     completedOption = nextOptionValue;
    //                 }
    //             }

    //             return input.Replace(
    //                 wordAtCursor,
    //                 ToKeyValueString(
    //                     optionNameAtCursor,
    //                     completedOption
    //                 )
    //             );
    //         }

    //         return input;
    //     }

    //     private string GetWordAtCursor(string input, int cursorPosition)
    //     {
    //         if (input.Length - 1 == cursorPosition)
    //         {
    //             var lastIndexOfSpace = input.LastIndexOf(" ");
    //             return input.Substring(lastIndexOfSpace).Trim();
    //         }

    //         var textBeforeCursor = input.Substring(0, cursorPosition);
    //         var textAfterCursor = input.Substring(cursorPosition);

    //         var beforeCursorSpaceIndex = textBeforeCursor.LastIndexOf(" ");
    //         textBeforeCursor = textBeforeCursor.Substring(beforeCursorSpaceIndex);

    //         var afterCursorSpaceIndex = textAfterCursor.IndexOf(" ");
    //         if (afterCursorSpaceIndex != -1)
    //         {
    //             textAfterCursor = textAfterCursor.Substring(0, afterCursorSpaceIndex);
    //         }

    //         return (textBeforeCursor + textAfterCursor).Trim();
    //     }

    //     private CommandInfo GetCompletedCommand(string input)
    //     {
    //         var command = input.Substring(0, input.IndexOf(' '));
    //         return _availableCommands.FirstOrDefault(x => x.NameWithoutSuffix == command);
    //     }

    //     private string ToKeyValueString(string key, string value)
    //     {
    //         return $"{key}={value}";
    //     }

    //     private IEnumerable<string> GetCompletedParamters(string input)
    //     {
    //         var completedParameterMatches = Regex.Matches(input, "\\s(?<parameterName>\\w+)=[^\\s]");
    //         foreach (Match match in completedParameterMatches)
    //         {
    //             yield return match.Groups["parameterName"].ToString();
    //         }
    //     }
    // }

    // public static class CommandInfoExtensions
    // {
    //     public static string GetNextOptionValue(this List<string> availableValues, string currentValue)
    //     {
    //         var currentOptionValueIndex = availableValues.IndexOf(currentValue);
    //         var nextIndex = currentOptionValueIndex + 1;
    //         if (nextIndex >= availableValues.Count)
    //         {
    //             nextIndex = 0;
    //         }

    //         return availableValues[nextIndex];
    //     }

    //     public static string GetPreviousOptionValue(this List<string> availableValues, string currentValue)
    //     {
    //         var currentOptionValueIndex = availableValues.IndexOf(currentValue);
    //         var nextIndex = currentOptionValueIndex - 1;
    //         if (nextIndex < 0)
    //         {
    //             nextIndex = availableValues.Count - 1;
    //         }

    //         return availableValues[nextIndex];
    //     }

    //     public static CommandOptionInfo CycleNext(this List<CommandOptionInfo> options, CommandOptionInfo currentOption)
    //     {
    //         var optionIndex = options.IndexOf(currentOption);
    //         var nextIndex = optionIndex + 1;
    //         if (nextIndex >= options.Count)
    //         {
    //             nextIndex = 0;
    //         }

    //         return options[nextIndex];
    //     }
    // }

    // public class InputHandler
    // {
    //     private readonly Dictionary<string, Action> _keyActions;

    //     private readonly StringBuilder _text = new StringBuilder();
    //     private int _cursorPosition;
    //     private int _cursorLimitRight;
    //     private int _cursorLimitLeft;

    //     private bool IsStartOfLine { get { return _cursorPosition == 0 || _cursorPosition <= _cursorLimitLeft; } }
    //     private bool IsEndOfLine { get { return _cursorPosition == _cursorLimitRight; } }

    //     private readonly AutoCompleteHandler _autoComplete;

    //     public InputHandler()
    //     {
    //         _autoComplete = new AutoCompleteHandler();

    //         _keyActions = new Dictionary<string, Action>
    //         {
    //             ["ControlA"] = MoveCursorHome,
    //             ["ControlB"] = MoveCursorLeft,
    //             ["ControlF"] = MoveCursorRight,
    //             ["ControlE"] = MoveCursorEnd,
    //             ["Backspace"] = DeleteLeft,
    //             ["ControlD"] = DeleteRight,
    //             ["Tab"] = Complete,
    //             ["ControlN"] = () => CompleteSelection(next: true),
    //             ["ControlP"] = () => CompleteSelection(next: false)
    //         };
    //     }

    //     public string ReadLine()
    //     {
    //         var keyInfo = Console.ReadKey(true);
    //         while (keyInfo.Key != ConsoleKey.Enter)
    //         {
    //             if (_keyActions.TryGetValue(keyInfo.ToKeyString(), out var action))
    //             {
    //                 action.Invoke();
    //             }
    //             else
    //             {
    //                 WriteChar(keyInfo.KeyChar);
    //             }

    //             keyInfo = Console.ReadKey(true);
    //         }

    //         Console.WriteLine();

    //         var result = _text.ToString();

    //         _text.Clear();
    //         _cursorPosition = 0;
    //         _cursorLimitRight = 0;
    //         _cursorLimitLeft = 0;

    //         return result;
    //     }

    //     public string Prompt(string prompt)
    //     {
    //         _text.Clear();
    //         _text.Append(prompt);

    //         _cursorLimitLeft = _text.Length;
    //         _cursorLimitRight = _text.Length;
    //         _cursorPosition = _text.Length;

    //         Console.Write(_text.ToString());

    //         return ReadLine().Replace(prompt, string.Empty);
    //     }

    //     private void Complete()
    //     {
    //         var result = _autoComplete.Complete(_text.ToString());
    //         _text.Clear();
    //         _text.Append(result);

    //         _cursorPosition = _text.Length;

    //         Console.SetCursorPosition(0, Console.CursorTop);
    //         Console.Write(result + new string(' ', _cursorLimitRight));
    //         Console.SetCursorPosition(_text.Length, Console.CursorTop);

    //         _cursorLimitRight = _text.Length;
    //     }

    //     private void CompleteSelection(bool next)
    //     {
    //         string result = _autoComplete.CompleteOptionSelection(_text.ToString(), _cursorPosition, next);

    //         _text.Clear();
    //         _text.Append(result);

    //         Console.SetCursorPosition(0, Console.CursorTop);
    //         Console.Write(result + new string(' ', _cursorLimitRight));

    //         _cursorLimitRight = _text.Length;
    //         MoveCursorRightUntilFirstSpaceOrEnd();
    //     }

    //     private void WriteChar(char input)
    //     {
    //         if (IsEndOfLine)
    //         {
    //             _text.Append(input);
    //             Console.Write(input.ToString());
    //             _cursorPosition++;
    //         }
    //         else
    //         {
    //             var textAfterCursor = _text.ToString().Substring(_cursorPosition);
    //             _text.Insert(_cursorPosition, input);

    //             using (new PersistConsolePosition())
    //             {
    //                 Console.Write(input.ToString() + textAfterCursor);
    //             }

    //             MoveCursorRight();
    //         }

    //         _cursorLimitRight++;
    //     }

    //     private void DeleteLeft()
    //     {
    //         if (IsStartOfLine)
    //         {
    //             return;
    //         }

    //         MoveCursorLeft();

    //         _text.Remove(_cursorPosition, 1);
    //         var replacement = _text.ToString(_cursorPosition, _text.Length - _cursorPosition);

    //         using (new PersistConsolePosition())
    //         {
    //             Console.Write($"{replacement} ");
    //         }

    //         _cursorLimitRight--;
    //     }

    //     private void DeleteRight()
    //     {
    //         if (IsEndOfLine)
    //         {
    //             return;
    //         }

    //         MoveCursorRight();
    //         DeleteLeft();
    //     }

    //     private void MoveCursorHome()
    //     {
    //         Console.SetCursorPosition(_cursorLimitLeft, Console.CursorTop);
    //         _cursorPosition = _cursorLimitLeft;
    //     }

    //     private void MoveCursorEnd()
    //     {
    //         Console.SetCursorPosition(_cursorLimitRight, Console.CursorTop);
    //         _cursorPosition = _cursorLimitRight;
    //     }

    //     private void MoveCursorLeft()
    //     {
    //         if (IsStartOfLine)
    //         {
    //             return;
    //         }

    //         Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

    //         _cursorPosition--;
    //     }

    //     private void MoveCursorRight()
    //     {
    //         if (IsEndOfLine)
    //         {
    //             return;
    //         }

    //         Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);

    //         _cursorPosition++;
    //     }

    //     private void MoveCursorRightUntilFirstSpaceOrEnd()
    //     {
    //         var textBeforeCursor = _text.ToString(0, _cursorPosition);
    //         var textAfterCursor = _text.ToString(_cursorPosition, _text.Length - _cursorPosition);
    //         var nextSpaceIndex = textAfterCursor.IndexOf(" ");
    //         if (nextSpaceIndex == -1)
    //         {
    //             MoveCursorEnd();
    //             return;
    //         }

    //         _cursorPosition = textBeforeCursor.Length + nextSpaceIndex;
    //         Console.SetCursorPosition(_cursorPosition, Console.CursorTop);
    //     }
    // }

    // public class PersistConsolePosition : IDisposable
    // {
    //     private readonly int _cursorPosition;

    //     public PersistConsolePosition()
    //     {
    //         _cursorPosition = Console.CursorLeft;
    //     }

    //     public void Dispose()
    //     {
    //         Console.SetCursorPosition(_cursorPosition, Console.CursorTop);
    //     }
    // }

    // public static class InputHandlerExtensions
    // {
    //     public static string ToKeyString(this ConsoleKeyInfo keyInfo)
    //     {
    //         return (keyInfo.Modifiers != ConsoleModifiers.Control && keyInfo.Modifiers != ConsoleModifiers.Shift) ?
    //             keyInfo.Key.ToString() : keyInfo.Modifiers.ToString() + keyInfo.Key.ToString();
    //     }
    // }
}
