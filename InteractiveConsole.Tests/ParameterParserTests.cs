using System.Linq;
using FluentAssertions;
using Xunit;

namespace InteractiveConsole.Tests
{
    public class ParameterParserTests
    {
        [Fact]
        public void Parse_should_parse_command()
        {
            // arrange
            var commandName = "testCommand";

            // act
            var result = ParameterParser.Parse(commandName);

            // assert
            result.CommandName.Should().Be($"{commandName}Command");
        }

        [Theory]
        [InlineData("param1", "value")]
        [InlineData("param2", "'value'")]
        [InlineData("param3", "\"value\"")]
        [InlineData("param4", "1")]
        public void Parse_should_parse_parameters(string parameterName, string parameterValue)
        {
            // arrange
            var input = $"commandName {parameterName}={parameterValue}";

            // act
            var result = ParameterParser.Parse(input);

            // assert
            result.Parameters.Count.Should().Be(1);
            result.Success.Should().BeTrue();
            var parameter = result.Parameters.First();
            parameter.Name.Should().Be(parameterName);
            parameter.Value.Should().Be(parameterValue.Replace("\"", string.Empty));
        }

        [Fact]
        public void Parse_should_parse_variable_index()
        {
            // arrange
            var index = 15;
            var input = $"commandName parameter=#1[{index}]";

            // act
            var result = ParameterParser.Parse(input);

            // assert
            result.Success.Should().BeTrue();
            var parameter = result.Parameters.First();
            parameter.IndexFrom.Should().Be(index);
        }

        [Fact]
        public void Parse_should_parse_variable_range()
        {
            // arrange
            var indexFrom = 15;
            var indexTo = 30;
            var input = $"commandName parameter=#1[{indexFrom}..{indexTo}]";

            // act
            var result = ParameterParser.Parse(input);

            // assert
            result.Success.Should().BeTrue();
            var parameter = result.Parameters.First();
            parameter.IndexFrom.Should().Be(indexFrom);
            parameter.IndexTo.Should().Be(indexTo);
        }

        [Fact]
        public void Parse_should_return_success_false_if_failed_to_parse_string()
        {
            // arrange
            var input = "commandName someString";

            // act
            var result = ParameterParser.Parse(input);

            // assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public void Parse_should_parse_multiple_parameters()
        {
            // arrange
            var input = "commandName ";
            for (var i = 0; i < 5; i++)
            {
                input += $"parameter{i}=value{i} ";
            }

            // act
            var result = ParameterParser.Parse(input);

            // assert
            result.Success.Should().BeTrue();
            result.Parameters.Count.Should().Be(5);
        }

        [Fact]
        public void Parse_should_return_success_false_if_failed_to_parse_part_of_string()
        {
            // arrange
            var input = "commandName parameter1=some value";

            // act
            var result = ParameterParser.Parse(input);

            // assert
            result.Success.Should().BeFalse();
            var parameter = result.Parameters.First();
            parameter.Value.Should().Be("some");
        }
    }
}