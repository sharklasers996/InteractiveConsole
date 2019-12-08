using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using InteractiveConsole.Attributes;
using InteractiveConsole.Models;
using Console = Colorful.Console;
using InteractiveConsole.Extensions;

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
                cmd.Options = GetCommandOptions(cmd.Type)
                    .OrderBy(x => x.Required)
                    .ToList();
                AvailableCommands.Add(cmd);
            }
        }

        private IEnumerable<CommandInfo> GetCommands()
        {
            var types = Assembly.GetEntryAssembly().GetTypes().ToList();
            types.AddRange(Assembly.GetCallingAssembly().GetTypes());

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute(typeof(CommandAttribute));
                if (attribute != null)
                {
                    var commandAttribute = attribute as CommandAttribute;
                    yield return new CommandInfo
                    {
                        Description = commandAttribute.Description,
                        Name = type.Name,
                        Type = type
                    };
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
                        IsList = propertyType.IsList(),
                        IsNumber = propertyType.IsNumericType(),
                        IsString = propertyType.IsString(),
                        IsEnum = propertyType.IsEnum
                    };

                    if (optionInfo.IsList)
                    {
                        var listItemType = propertyType.GetListItemType();
                        optionInfo.IsListItemNumber = listItemType.IsNumericType();
                        optionInfo.IsListItemString = listItemType.IsString();
                        optionInfo.IsListItemEnum = listItemType.IsEnum;

                        if (!optionInfo.IsListItemNumber
                            && !optionInfo.IsListItemString
                            && !optionInfo.IsListItemEnum)
                        {
                            optionInfo.IsListItemCustomObject = true;
                            optionInfo.ListItemObjectName = listItemType.Name;
                        }
                    }

                    if (!optionInfo.IsList
                        && !optionInfo.IsNumber
                        && !optionInfo.IsString
                        && !optionInfo.IsEnum)
                    {
                        optionInfo.IsCustomObject = true;
                        optionInfo.ObjectName = propertyType.Name;
                    }


                    yield return optionInfo;
                }
            }
        }
    }
}