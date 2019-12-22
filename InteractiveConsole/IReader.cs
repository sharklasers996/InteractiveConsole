using System.Collections.Generic;

namespace InteractiveConsole
{
    public interface IReader
    {
        string LetterSelection(Dictionary<string, string> availableActions);
        string LetterSelection(object item, Dictionary<string, string> availableActions);
        List<int> NumberSelection(string prompt, int? rangeFrom = null, int? rangeTo = null);
        bool YesNoPrompt(string text);
        string Prompt(string prompt, bool masked = false);
    }
}