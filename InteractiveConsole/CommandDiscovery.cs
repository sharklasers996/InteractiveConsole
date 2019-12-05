using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using InteractiveConsole.Attributes;
using InteractiveConsole.Models;
using Console = Colorful.Console;

namespace InteractiveConsole
{
    public class CommandDiscovery : ICommandDiscovery
    {
        public List<CommandInfo> AvailableCommands { get; private set; }

        public CommandDiscovery()
        {
            AvailableCommands = new List<CommandInfo>();

            var commands = GetCommands();
            foreach (var cmd in commands)
            {
                var commandOptions = GetCommandOptions(cmd)
                    .OrderBy(x => x.Required)
                    .ToList();
                AvailableCommands.Add(new CommandInfo
                {
                    Name = cmd.Name,
                    Options = commandOptions,
                    Type = cmd
                });
            }
        }

        public void PrintAvailableCommands()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine();

            foreach (var command in AvailableCommands)
            {
                Console.WriteLine(command.NameWithoutSuffix);
                foreach (var option in command.Options)
                {
                    Console.WriteLine($"\t> {option.Name}");
                }
            }
        }

        private IEnumerable<Type> GetCommands()
        {
            var types = Assembly.GetEntryAssembly().GetTypes().ToList();
            types.AddRange(Assembly.GetCallingAssembly().GetTypes());

            foreach (var type in types)
            {
                if (type.GetCustomAttribute(typeof(CommandAttribute)) != null)
                {
                    yield return type;
                }
            }
        }

        private IEnumerable<CommandOptionInfo> GetCommandOptions(Type commandType)
        {
            var properties = commandType.GetProperties();
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(CommandParameterAttribute)) != null)
                {
                    var optionSelections = new List<string>();
                    if (property.PropertyType.IsEnum)
                    {
                        foreach (var e in Enum.GetValues(property.PropertyType))
                        {
                            optionSelections.Add(e.ToString());
                        }
                    }

                    yield return new CommandOptionInfo
                    {
                        Name = property.Name,
                        Required = property.GetCustomAttribute(typeof(RequiredAttribute)) != null,
                        AvailableValues = optionSelections,
                        Type = property.PropertyType
                    };
                }
            }
        }
    }
}