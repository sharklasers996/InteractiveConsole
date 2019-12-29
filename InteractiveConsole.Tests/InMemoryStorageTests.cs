using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using InteractiveConsole.Storage;
using Xunit;

namespace InteractiveConsole.Tests
{
    public class InMemoryStorageTests
    {
        private readonly IInMemoryStorage _inMemoryStorage;

        public InMemoryStorageTests()
        {
            _inMemoryStorage = new InMemoryStorage();
        }

        [Fact]
        public void Add_should_add_variable()
        {
            // arrange
            var commandName = "testCommand";
            var varValue = new List<string>
            {
                "value",
                "value"
            };
            var parserResult = new ParameterParserResult
            {
                CommandName = commandName
            };

            // act
            _inMemoryStorage.Add(varValue, parserResult);

            // assert
            _inMemoryStorage.Variables.Count.Should().Be(1);
            var variable = _inMemoryStorage.Variables.First();
            variable.Id.Should().Be(1);
            variable.Value.Should().BeEquivalentTo(varValue);
            variable.Length.Should().Be(2);
            variable.TypeInfo.IsList.Should().BeTrue();
        }

        [Fact]
        public void TryGetVariable_should_return_variable()
        {
            // arrange
            var varValue = new List<string>
            {
                "value",
                "value"
            };
            var parserResult = new ParameterParserResult();

            // act
            _inMemoryStorage.Add(varValue, parserResult);

            // assert
            var variable = _inMemoryStorage.TryGetVariable("#1");
            variable.Value.Should().BeEquivalentTo(varValue);
        }
    }
}