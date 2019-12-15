using System.Collections.Generic;
using FluentAssertions;
using InteractiveConsole.Models;
using NSubstitute;
using Xunit;

namespace InteractiveConsole.Tests
{
    public class AutoCompleteHandlerTests
    {
        private readonly ICommandDiscovery _commandDiscovery;
        private readonly IAutoCompleteHandler _autoComplete;

        public AutoCompleteHandlerTests()
        {
            _commandDiscovery = Substitute.For<ICommandDiscovery>();
            _autoComplete = new AutoCompleteHandler(_commandDiscovery);
        }

        [Fact]
        public void Complete_should_complete_command_name()
        {
            // arrange
            var commandName = "LongTestNameHere";
            _commandDiscovery.AvailableCommands.Returns(new List<CommandInfo> {
                new CommandInfo
                {
                    Name = commandName
                }
            });

            // act
            var result = _autoComplete.Complete("long", next: true);

            // assert
            result.Should().Be(commandName);
        }

        [Fact]
        public void Complete_should_cycle_next_command()
        {
            // arrange
            var commands = new List<CommandInfo>();
            for (var i = 0; i < 5; i++)
            {
                commands.Add(new CommandInfo
                {
                    Name = $"CommandName{i}"
                });
            }
            _commandDiscovery.AvailableCommands.Returns(commands);

            // act
            var result = _autoComplete.Complete("CommandName3", next: true);

            // assert
            result.Should().Be("CommandName4");
        }

        [Fact]
        public void Complete_should_cycle_previous_command()
        {
            // arrange
            var commands = new List<CommandInfo>();
            for (var i = 0; i < 5; i++)
            {
                commands.Add(new CommandInfo
                {
                    Name = $"CommandName{i}"
                });
            }
            _commandDiscovery.AvailableCommands.Returns(commands);

            // act
            var result = _autoComplete.Complete("CommandName3", next: false);

            // assert
            result.Should().Be("CommandName2");
        }

        [Fact]
        public void Complete_should_complete_next_option()
        {
            // arrange
            var commandName = "LongTestNameHere";
            _commandDiscovery.AvailableCommands.Returns(new List<CommandInfo> {
                new CommandInfo
                {
                    Name = commandName,
                    Options = new List<CommandOptionInfo>
                    {
                        new CommandOptionInfo
                        {
                            Name = "Option0"
                        },
                        new CommandOptionInfo
                        {
                            Name = "Option1"
                        }
                    }
                }
            });

            // act
            var result = _autoComplete.Complete($"{commandName} ", next: true);

            // assert
            result.Should().Be($"{commandName} Option0=");
        }

        [Fact]
        public void Complete_should_complete_previous_option()
        {
            // arrange
            var commandName = "LongTestNameHere";
            _commandDiscovery.AvailableCommands.Returns(new List<CommandInfo> {
                new CommandInfo
                {
                    Name = commandName,
                    Options = new List<CommandOptionInfo>
                    {
                        new CommandOptionInfo
                        {
                            Name = "Option0"
                        },
                        new CommandOptionInfo
                        {
                            Name = "Option1"
                        }
                    }
                }
            });

            // act
            var result = _autoComplete.Complete($"{commandName} Option0=", next: false);

            // assert
            result.Should().Be($"{commandName} Option1=");
        }

        [Fact]
        public void Complete_should_complete_required_option_first()
        {
            // arrange
            var commandName = "LongTestNameHere";
            _commandDiscovery.AvailableCommands.Returns(new List<CommandInfo> {
                new CommandInfo
                {
                    Name = commandName,
                    Options = new List<CommandOptionInfo>
                    {
                        new CommandOptionInfo
                        {
                            Name = "Option0",
                            Required = false
                        },
                        new CommandOptionInfo
                        {
                            Name = "Option1",
                            Required = true
                        }
                    }
                }
            });

            // act
            var result = _autoComplete.Complete($"{commandName} ", next: true);

            // assert
            result.Should().Be($"{commandName} Option1=");
        }

        [Fact]
        public void Complete_should_complete_option_by_name()
        {
            // arrange
            var commandName = "LongTestNameHere";
            _commandDiscovery.AvailableCommands.Returns(new List<CommandInfo> {
                new CommandInfo
                {
                    Name = commandName,
                    Options = new List<CommandOptionInfo>
                    {
                        new CommandOptionInfo
                        {
                            Name = "FirstOption"
                        },
                        new CommandOptionInfo
                        {
                            Name = "SecondOption"
                        }
                    }
                }
            });

            // act
            var result = _autoComplete.Complete($"{commandName} Secon", next: true);

            // assert
            result.Should().Be($"{commandName} SecondOption=");
        }

        [Fact]
        public void Complete_should_complete_only_uncompleted_option()
        {
            // arrange
            var commandName = "LongTestNameHere";
            _commandDiscovery.AvailableCommands.Returns(new List<CommandInfo> {
                new CommandInfo
                {
                    Name = commandName,
                    Options = new List<CommandOptionInfo>
                    {
                        new CommandOptionInfo
                        {
                            Name = "FirstOption"
                        },
                        new CommandOptionInfo
                        {
                            Name = "SecondOption"
                        },
                        new CommandOptionInfo
                        {
                            Name = "ThirdOption"
                        }
                    }
                }
            });

            // act
            var result = _autoComplete.Complete($"{commandName} FirstOption=value ThirdOption=value ", next: true);

            // assert
            result.Should().Be($"{commandName} FirstOption=value ThirdOption=value SecondOption=");
        }

        [Fact]
        public void CompleteOptionSelection_should_complete_option_selection()
        {
            // arrange
            var commandName = "LongTestNameHere";
            _commandDiscovery.AvailableCommands.Returns(new List<CommandInfo> {
                new CommandInfo
                {
                    Name = commandName,
                    Options = new List<CommandOptionInfo>
                    {
                        new CommandOptionInfo
                        {
                            Name = "Option0",
                            AvailableValues = new List<string>
                            {
                                "Value1",
                                "Value2"
                            }
                        }
                    }
                }
            });

            // act
            var input = $"{commandName} Option0=";
            var result = _autoComplete.CompleteOptionSelection(input, input.Length, next: true);

            // assert
            result.Should().Be($"{input}Value1");
        }

        [Fact]
        public void CompleteOptionSelection_should_complete_next_option_selection()
        {
            // arrange
            var commandName = "LongTestNameHere";
            _commandDiscovery.AvailableCommands.Returns(new List<CommandInfo> {
                new CommandInfo
                {
                    Name = commandName,
                    Options = new List<CommandOptionInfo>
                    {
                        new CommandOptionInfo
                        {
                            Name = "Option0",
                            AvailableValues = new List<string>
                            {
                                "Value1",
                                "Value2"
                            }
                        }
                    }
                }
            });

            // act
            var input = $"{commandName} Option0=Value1";
            var result = _autoComplete.CompleteOptionSelection(input, input.Length, next: true);

            // assert
            result.Should().Be($"{commandName} Option0=Value2");
        }
    }
}