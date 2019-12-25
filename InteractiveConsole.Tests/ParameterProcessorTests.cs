using System.Linq;
using System;
using System.Collections.Generic;
using FluentAssertions;
using InteractiveConsole.Models;
using InteractiveConsole.Storage;
using NSubstitute;
using Xunit;
using InteractiveConsole.Models.Storage;

namespace InteractiveConsole.Tests
{
    public class ParameterProcessorTests
    {
        private readonly IInMemoryStorage _inMemoryStorage;

        public ParameterProcessorTests()
        {
            _inMemoryStorage = Substitute.For<IInMemoryStorage>();
        }

        [Fact]
        public void SetParameters_should_set_string()
        {
            // arrange
            var propertyName = "StringProp";
            var propertyValue = "prop_value";

            // act
            var command = CreateCommandAndSetParameters(propertyName, propertyValue);

            // assert
            command.StringProp.Should().Be(propertyValue);
        }

        [Fact]
        public void SetParameters_should_set_enum()
        {
            // arrange
            var propertyName = "EnumProp";
            var propertyValue = "1";

            // act
            var command = CreateCommandAndSetParameters(propertyName, propertyValue);

            // assert
            command.EnumProp.Should().Be(TestEnum.Value1);
        }

        [Fact]
        public void SetParameters_should_set_bool()
        {
            // arrange
            var propertyName = "BoolProp";
            var propertyValue = true;

            // act
            var command = CreateCommandAndSetParameters(propertyName, propertyValue.ToString());

            // assert
            command.BoolProp.Should().Be(propertyValue);
        }

        [Fact]
        public void SetParameters_should_set_int()
        {
            // arrange
            var propertyName = "IntProp";
            var propertyValue = 15;

            // act
            var command = CreateCommandAndSetParameters(propertyName, propertyValue);

            // assert
            command.IntProp.Should().Be(propertyValue);
        }

        [Fact]
        public void SetParameters_should_set_custom_object()
        {
            // arrange
            var propertyName = "ObjectProp";
            var inMemoryVariable = CreateTestObject(15, "prop_value");
            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsCustomObject = true,
                    ObjectName = "TestObject"
                },
                Value = inMemoryVariable
            });

            // act 
            var command = CreateCommandAndSetParameters(propertyName, string.Empty);

            // assert
            command.ObjectProp.Should().BeEquivalentTo(inMemoryVariable);
        }

        [Fact]
        public void SetParameters_should_set_list_of_custom_objects()
        {
            // arrange
            var propertyName = "ObjectsProp";
            var inMemoryVariable = CreateListOfTestObjects(4);

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsList = true,
                    IsListItemCustomObject = true,
                    ListItemObjectName = "TestObject"
                },
                Value = inMemoryVariable
            });

            // act 
            var command = CreateCommandAndSetParameters(propertyName, string.Empty);

            // assert
            command.ObjectsProp.Should().BeEquivalentTo(inMemoryVariable);
        }

        [Fact]
        public void SetParameters_should_set_single_object_as_list_if_types_match()
        {
            // arrange
            var propertyName = "StringsProp";
            var propertyValue = "prop_value";

            // act
            var command = CreateCommandAndSetParameters(propertyName, propertyValue);

            // assert
            command.StringsProp.Count.Should().Be(1);
            command.StringsProp.FirstOrDefault().Should().Be(propertyValue);
        }

        [Fact]
        public void SetParameters_should_set_list_of_custom_objects_with_index_range()
        {
            // arrange
            var propertyName = "ObjectsProp";
            var inMemoryVariable = CreateListOfTestObjects(10);

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsList = true,
                    IsListItemCustomObject = true,
                    Type = typeof(List<TestObject>)
                },
                Value = inMemoryVariable,
            });

            // act 
            var command = CreateCommandAndSetParameters(new Parameter
            {
                Name = propertyName,
                Value = string.Empty,
                IndexFrom = 3,
                IndexTo = 6
            });

            // assert
            var expectedVariable = inMemoryVariable.Skip(3).Take(3).ToList();
            command.ObjectsProp.Should().BeEquivalentTo(expectedVariable);
        }

        [Fact]
        public void SetParameters_should_set_custom_object_from_list_with_index()
        {
            // arrange
            var propertyName = "ObjectProp";
            var inMemoryVariable = CreateListOfTestObjects(10);

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsListItemCustomObject = true,
                    IsList = true,
                    Type = typeof(List<TestObject>)
                },
                Value = inMemoryVariable,
            });

            // act 
            var command = CreateCommandAndSetParameters(new Parameter
            {
                Name = propertyName,
                Value = string.Empty,
                IndexFrom = 5,
            });

            // assert
            command.ObjectProp.Should().BeEquivalentTo(inMemoryVariable[5]);
        }

        [Fact]
        public void SetParameters_should_return_error_when_indexFrom_is_out_of_bounds()
        {
            // arrange
            var propertyName = "ObjectProp";
            var inMemoryVariable = CreateListOfTestObjects(1);

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsListItemCustomObject = true,
                    IsList = true,
                    Type = typeof(List<TestObject>)
                },
                Value = inMemoryVariable,
            });

            var processor = CreateParameterProcessor(new Parameter
            {
                Name = propertyName,
                Value = string.Empty,
                IndexFrom = 5,
            });
            // act 
            var error = processor.SetParameters();

            // assert
            error.Should().Be("Index is out of bounds");
        }

        [Fact]
        public void SetParameters_should_return_error_when_indexTo_is_without_indexFrom()
        {
            // arrange
            var propertyName = "ObjectProp";
            var inMemoryVariable = CreateListOfTestObjects(1);

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsListItemCustomObject = true,
                    IsList = true
                },
                Value = inMemoryVariable,
            });

            var processor = CreateParameterProcessor(new Parameter
            {
                Name = propertyName,
                Value = string.Empty,
                IndexTo = 5,
            });
            // act 
            var error = processor.SetParameters();

            // assert
            error.Should().Be("Starting index is required");
        }

        [Fact]
        public void SetParameters_should_return_error_when_object_list_type_does_not_match()
        {
            // arrange
            var propertyName = "ObjectsProp";
            var inMemoryVariable = new List<string> { "test1", "test2" };

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsListItemString = true,
                    IsList = true
                },
                Value = inMemoryVariable,
            });

            var processor = CreateParameterProcessor(new Parameter
            {
                Name = propertyName,
                Value = string.Empty
            });
            // act 
            var error = processor.SetParameters();

            // assert
            error.Should().Be("Parameter type does not match");
        }

        [Fact]
        public void SetParameters_should_return_error_when_object_type_does_not_match()
        {
            // arrange
            var propertyName = "ObjectProp";
            var inMemoryVariable = "test_string_value";

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsString = true
                },
                Value = inMemoryVariable
            });

            var processor = CreateParameterProcessor(new Parameter
            {
                Name = propertyName,
                Value = string.Empty
            });
            // act 
            var error = processor.SetParameters();

            // assert
            error.Should().Be("Parameter type does not match");
        }

        [Fact]
        public void SetParameters_should_return_error_when_primitive_type_does_not_match()
        {
            // arrange
            var propertyName = "IntProp";
            var propertyValue = "not_int";

            // act
            var processor = CreateParameterProcessor(new Parameter
            {
                Name = propertyName,
                Value = propertyValue
            });
            // act 
            var error = processor.SetParameters();

            // assert
            error.Should().Be("Parameter type does not match");
        }

        [Fact]
        public void SetParameters_should_set_inMemoryStorageVariable_property_as_inMemoryStorageVariable_object()
        {
            // arrange
            var propertyName = "InMemoryStorageVariableProp";
            var inMemoryVariable = new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    ObjectName = "testObj",
                    Type = typeof(object)
                }
            };

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(inMemoryVariable);

            // act
            var command = CreateCommandAndSetParameters(new Parameter
            {
                Name = propertyName,
                Value = string.Empty
            });

            // assert
            command.InMemoryStorageVariableProp.Should().BeEquivalentTo(inMemoryVariable);
        }

        [Fact]
        public void SetParameters_should_set_inMemoryStorageVariable_property_with_index()
        {
            // arrange
            var propertyName = "InMemoryStorageVariableProp";
            var inMemoryVariable = new List<InMemoryStorageVariable>{
                new InMemoryStorageVariable
                {
                    TypeInfo = new TypeInfo
                    {
                        ObjectName = "testObj",
                        Type = typeof(InMemoryStorageVariable)
                    }
                }
            };

            _inMemoryStorage.TryGetVariable(Arg.Any<string>()).Returns(new InMemoryStorageVariable
            {
                TypeInfo = new TypeInfo
                {
                    IsList = true,
                    IsListItemCustomObject = true,
                    Type = typeof(List<InMemoryStorageVariable>)
                },
                Value = inMemoryVariable,
            });

            // act
            var command = CreateCommandAndSetParameters(new Parameter
            {
                Name = propertyName,
                Value = string.Empty,
                IndexFrom = 0
            });

            // assert
            command.InMemoryStorageVariableProp.Should().BeEquivalentTo(inMemoryVariable.First());
        }

        private TestCommand CreateCommandAndSetParameters(string propertyName, object propertyValue)
        {
            return CreateCommandAndSetParameters(new Parameter { Name = propertyName, Value = propertyValue.ToString() });
        }

        private TestCommand CreateCommandAndSetParameters(Parameter parameter)
        {
            var parserResult = new ParameterParserResult
            {
                CommandName = "TestCommand",
                Parameters = new List<Parameter>
                {
                    parameter
                }
            };
            var commandInfo = new CommandInfo
            {
                Options = new List<CommandOptionInfo>
                {
                    new CommandOptionInfo {
                        Name = parameter.Name
                    }
                }
            };
            var command = (TestCommand)Activator.CreateInstance(typeof(TestCommand));
            var processor = new ParameterProcessor(_inMemoryStorage)
            {
                CommandInstance = command,
                ParserResult = parserResult,
                CommandInfo = commandInfo
            };

            processor.SetParameters();

            return command;
        }

        private ParameterProcessor CreateParameterProcessor(Parameter parameter)
        {
            var parserResult = new ParameterParserResult
            {
                CommandName = "TestCommand",
                Parameters = new List<Parameter>
                {
                    parameter
                }
            };
            var commandInfo = new CommandInfo
            {
                Options = new List<CommandOptionInfo>
                {
                    new CommandOptionInfo {
                        Name = parameter.Name
                    }
                }
            };
            var command = (TestCommand)Activator.CreateInstance(typeof(TestCommand));
            return new ParameterProcessor(_inMemoryStorage)
            {
                CommandInstance = command,
                ParserResult = parserResult,
                CommandInfo = commandInfo
            };
        }

        private TestObject CreateTestObject(int property1, string property2)
        {
            return new TestObject
            {
                Property1 = property1,
                Property2 = property2
            };
        }

        private List<TestObject> CreateListOfTestObjects(int count)
        {
            var objects = new List<TestObject>();
            for (var i = 0; i < count; i++)
            {
                objects.Add(CreateTestObject(i, $"prop_value_{i}"));
            }
            return objects;
        }
    }

    class TestCommand : BaseCommand
    {
        public string StringProp { get; set; }
        public TestEnum EnumProp { get; set; }
        public bool BoolProp { get; set; }
        public int IntProp { get; set; }
        public TestObject ObjectProp { get; set; }
        public List<TestObject> ObjectsProp { get; set; }
        public List<string> StringsProp { get; set; }
        public InMemoryStorageVariable InMemoryStorageVariableProp { get; set; }

        public override object Execute()
        {
            return null;
        }
    }

    class TestObject
    {
        public int Property1 { get; set; }
        public string Property2 { get; set; }
    }

    enum TestEnum
    {
        Value1 = 1,
        Value2 = 2
    }
}
