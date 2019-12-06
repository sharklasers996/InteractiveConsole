namespace InteractiveConsole
{
    public interface IAutoCompleteHandler
    {
        string Complete(string input, bool next);
        string CompleteOptionSelection(string input, int cursorPosition, bool next);
    }
}