namespace InteractiveConsole
{
    public interface IInputHandler
    {
        string ReadLine(bool masked = false);
        string Prompt(string prompt, bool masked = false);
    }
}