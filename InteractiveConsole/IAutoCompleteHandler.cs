namespace InteractiveConsole
{
    public interface IAutoCompleteHandler
    {
        string Complete(string input);
        string CompleteOptionSelection(string input, int cursorPosition, bool next);
    }
}