
using System.Collections.Generic;
using InteractiveConsole.Models;

namespace InteractiveConsole
{
    public interface ICommandDiscovery
    {
        List<CommandInfo> AvailableCommands { get; }
    }
}