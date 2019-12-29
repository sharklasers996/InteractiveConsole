using InteractiveConsole.Output;

namespace InteractiveConsole
{
    public abstract class BaseCommand
    {
        public IPrinter Printer { get; set; }
        public IReader Reader { get; set; }

        public virtual bool IsValid()
        {
            return true;
        }

        public abstract object Execute();

        public object ExecuteChildCommand(BaseCommand command)
        {
            command.Reader = Reader;
            command.Printer = Printer;

            return command.Execute();
        }
    }
}