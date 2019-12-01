namespace InteractiveConsole
{
    public interface IInputHandler
    {
        string ReadLine();
        string Prompt(string prompt);
    }
}