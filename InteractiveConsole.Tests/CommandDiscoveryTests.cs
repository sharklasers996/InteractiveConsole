using System.Linq;
using System;
using System.Collections.Generic;
using InteractiveConsole.Attributes;
using InteractiveConsole.Extensions;
using NSubstitute;
using Xunit;
using FluentAssertions;
using InteractiveConsole.Models;

namespace InteractiveConsole.Tests
{
    public class CommandDiscoveryTests
    {
        private readonly TestCommand _command;
        private readonly ICommandDiscovery _commandDiscovery;

        public CommandDiscoveryTests()
        {
            // arrange for all tests
            _command = new TestCommand();

            var typeProvider = Substitute.For<ITypeProvider>();
            typeProvider
                .GetTypes()
                .Returns(new List<Type> { typeof(TestCommand) });

            _commandDiscovery = new CommandDiscovery(typeProvider);
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_command()
        {
            // assert
            _commandDiscovery.AvailableCommands.Count.Should().Be(1);

            var testCommand = _commandDiscovery.AvailableCommands.First();
            testCommand.Description.Should().Be("TestDescription");
            testCommand.Name.Should().Be("TestCommand");
            testCommand.NameWithoutSuffix.Should().Be("Test");
            testCommand.Type.Should().Be(typeof(TestCommand));
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_required_string_property()
        {
            // arrange
            var expectedCommandOptionInfo = CreateCommandOptionInfo(nameof(_command.StringProp));
            expectedCommandOptionInfo.TypeInfo.IsString = true;
            expectedCommandOptionInfo.TypeInfo.Type = typeof(string);
            expectedCommandOptionInfo.Required = true;

            // act
            var commandOptionInfo = GetCommandOptionInfo(nameof(_command.StringProp));

            // assert
            commandOptionInfo.Should().BeEquivalentTo(expectedCommandOptionInfo);
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_int_property()
        {
            // arrange
            var expectedCommandOptionInfo = CreateCommandOptionInfo(nameof(_command.IntProp));
            expectedCommandOptionInfo.TypeInfo.IsNumber = true;
expectedCommandOptionInfo.TypeInfo.Type = typeof(int);

            // act
            var commandOptionInfo = GetCommandOptionInfo(nameof(_command.IntProp));

            // assert
            commandOptionInfo.Should().BeEquivalentTo(expectedCommandOptionInfo);
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_object_property()
        {
            // arrange
            var expectedCommandOptionInfo = CreateCommandOptionInfo(nameof(_command.ObjectProp));
            expectedCommandOptionInfo.TypeInfo.IsCustomObject = true;
            expectedCommandOptionInfo.TypeInfo.Type = typeof(object);
            expectedCommandOptionInfo.TypeInfo.ObjectName = "Object";

            // act
            var commandOptionInfo = GetCommandOptionInfo(nameof(_command.ObjectProp));

            // assert
            commandOptionInfo.Should().BeEquivalentTo(expectedCommandOptionInfo);
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_string_list_property()
        {
            // arrange
            var expectedCommandOptionInfo = CreateCommandOptionInfo(nameof(_command.StringListProp));
            expectedCommandOptionInfo.TypeInfo.IsList = true;
            expectedCommandOptionInfo.TypeInfo.IsListItemString = true;
            expectedCommandOptionInfo.TypeInfo.Type = typeof(List<string>);

            // act
            var commandOptionInfo = GetCommandOptionInfo(nameof(_command.StringListProp));

            // assert
            commandOptionInfo.Should().BeEquivalentTo(expectedCommandOptionInfo);
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_int_list_property()
        {
            // arrange
            var expectedCommandOptionInfo = CreateCommandOptionInfo(nameof(_command.IntListProp));
            expectedCommandOptionInfo.TypeInfo.IsList = true;
            expectedCommandOptionInfo.TypeInfo.IsListItemNumber = true;
            expectedCommandOptionInfo.TypeInfo.Type = typeof(List<int>);

            // act
            var commandOptionInfo = GetCommandOptionInfo(nameof(_command.IntListProp));

            // assert
            commandOptionInfo.Should().BeEquivalentTo(expectedCommandOptionInfo);
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_object_list_property()
        {
            // arrange
            var expectedCommandOptionInfo = CreateCommandOptionInfo(nameof(_command.ObjectListProp));
            expectedCommandOptionInfo.TypeInfo.IsList = true;
            expectedCommandOptionInfo.TypeInfo.IsListItemCustomObject = true;
            expectedCommandOptionInfo.TypeInfo.ListItemObjectName = "Object";
            expectedCommandOptionInfo.TypeInfo.Type = typeof(List<object>);

            // act
            var commandOptionInfo = GetCommandOptionInfo(nameof(_command.ObjectListProp));

            // assert
            commandOptionInfo.Should().BeEquivalentTo(expectedCommandOptionInfo);
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_enum_property()
        {
            // arrange
            var expectedCommandOptionInfo = CreateCommandOptionInfo(nameof(_command.EnumProp));
            expectedCommandOptionInfo.TypeInfo.IsEnum = true;
            expectedCommandOptionInfo.TypeInfo.Type = typeof(TestEnum);

            foreach (var value in Enum.GetValues(typeof(TestEnum)))
            {
                expectedCommandOptionInfo.AvailableValues.Add(value.ToString());
            }

            // act
            var commandOptionInfo = GetCommandOptionInfo(nameof(_command.EnumProp));

            // assert
            commandOptionInfo.Should().BeEquivalentTo(expectedCommandOptionInfo);
        }

        [Fact]
        public void CommandDiscovery_should_get_metadata_about_bool_property()
        {
            // arrange
            var expectedCommandOptionInfo = CreateCommandOptionInfo(nameof(_command.BoolProp));
            expectedCommandOptionInfo.TypeInfo.IsBool = true;
            expectedCommandOptionInfo.TypeInfo.Type = typeof(bool);

            // act
            var commandOptionInfo = GetCommandOptionInfo(nameof(_command.BoolProp));

            // assert
            commandOptionInfo.Should().BeEquivalentTo(expectedCommandOptionInfo);
        }

        private CommandOptionInfo CreateCommandOptionInfo(string name)
        {
            return new CommandOptionInfo
            {
                Name = name,
                AvailableValues = new List<string>(),
                TypeInfo = new TypeInfo()
            };
        }

        private CommandOptionInfo GetCommandOptionInfo(string optionName)
        {
            var testCommand = _commandDiscovery.AvailableCommands.First();
            return testCommand.Options.FirstOrDefault(x => x.Name == optionName);
        }

        [Command(Description = "TestDescription")]
        class TestCommand : BaseCommand
        {
            [CommandParameter]
            [Required]
            public string StringProp { get; set; }

            [CommandParameter]
            public int IntProp { get; set; }

            [CommandParameter]
            public object ObjectProp { get; set; }

            [CommandParameter]
            public List<string> StringListProp { get; set; }

            [CommandParameter]
            public List<int> IntListProp { get; set; }

            [CommandParameter]
            public List<object> ObjectListProp { get; set; }

            [CommandParameter]
            public TestEnum EnumProp { get; set; }

            [CommandParameter]
            public bool BoolProp { get; set; }

            public override object Execute()
            {
                return null;
            }
        }

        enum TestEnum
        {
            Value1,
            Value2
        }
    }
}