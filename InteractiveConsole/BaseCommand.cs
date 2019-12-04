using InteractiveConsole.Output;

namespace InteractiveConsole
{
    public abstract class BaseCommand
    {
        public IPrinter Printer { get; set; }
        public IInputHandler InputHandler { get; set; }
    }
}