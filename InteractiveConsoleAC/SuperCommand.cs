using System.Threading.Tasks;
using InteractiveConsole;
using InteractiveConsole.Attributes;

namespace InteractiveConsoleAC
{
    public enum SomEnum
    {
        EnumVal1,
        EnumVal2
    }

    [Command]
    public class SuperCommand : ICommand
    {
        [CommandParameter]
        public SomEnum SuperParameter1 { get; set; }

        [CommandParameter]
        public string SuperParameter2 { get; set; }

        [CommandParameter]
        public string SuperParameter3 { get; set; }

        public object Execute()
        {
            return null;
        }
    }

    [Command]
    public class FineCommand : ICommand
    {
        public object Execute()
        {
            return null;
        }
    }

    [Command]
    public class InterestingCommand : ICommand
    {
        public object Execute()
        {
            return null;
        }
    }
}