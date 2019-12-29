using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using InteractiveConsole.Attributes;
using InteractiveConsole.Models;
using InteractiveConsole.Extensions;

namespace InteractiveConsole
{
    public class CommandDiscovery : ICommandDiscovery
    {
        public List<CommandInfo> AvailableCommands { get; private set; }

        private readonly ITypeProvider _typeProvider;

        public CommandDiscovery(ITypeProvider typeProvider)
        {
            _typeProvider = typeProvider;
            AvailableCommands = new List<CommandInfo>();

            var commands = GetCommands();
            foreach (var cmd in commands)
            {
                cmd.Options = GetCommandOptions(cmd.Type)
                    .OrderBy(x => x.Required)
                    .ToList();
                AvailableCommands.Add(cmd);
            }
        }

        private IEnumerable<CommandInfo> GetCommands()
        {
            var types = _typeProvider.GetTypes();
            var commandTypes = types.Where(x => x.GetTypeInfo().IsSubclassOf(typeof(BaseCommand)));

            foreach (var type in commandTypes)
            {
                var commandInfo = new CommandInfo
                {
                    Name = type.Name,
                    Type = type
                };

                var attribute = type.GetCustomAttribute(typeof(CommandAttribute));
                if (attribute != null)
                {
                    var commandAttribute = attribute as CommandAttribute;
                    commandInfo.Description = commandAttribute.Description;
                    commandInfo.Category = commandAttribute.Category;
                }

                yield return commandInfo;
            }
        }

        private IEnumerable<CommandOptionInfo> GetCommandOptions(Type commandType)
        {
            var properties = commandType.GetProperties();
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(CommandParameterAttribute)) != null)
                {
                    var propertyType = property.PropertyType;
                    var optionSelections = new List<string>();
                    if (propertyType.IsEnum)
                    {
                        foreach (var e in Enum.GetValues(property.PropertyType))
                        {
                            optionSelections.Add(e.ToString());
                        }
                    }

                    var optionInfo = new CommandOptionInfo
                    {
                        Name = property.Name,
                        Required = property.GetCustomAttribute(typeof(RequiredAttribute)) != null,
                        AvailableValues = optionSelections,
                        TypeInfo = propertyType.ToTypeInfo()
                    };

                    yield return optionInfo;
                }
            }
        }
    }
}