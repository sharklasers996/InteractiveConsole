using System.Collections.Generic;

namespace InteractiveConsole
{
    public interface IReader
    {
        string LetterSelection(object item, Dictionary<string, string> availableActions);
        List<int> NumberSelection(string prompt);
        bool YesNoPrompt(string text);
        string Prompt(string prompt, bool masked = false);
    }
}