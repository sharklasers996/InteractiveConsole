// using System.Linq;
// using FluentAssertions;
// using Xunit;

// namespace InteractiveConsole.Tests
// {
//     public class CommandLineParserTests
//     {
//         [Theory]
//         [InlineData("\"1\" 2 \"3\"", new string[] { "1", "2", "3" })]
//         public void ParseParameters_Should_Parse_Parameters_Correctly(string input, string[] expectedParameters)
//         {
//             var parserResult = CommandLineParser.Parse(input);

//             var parserParmeterCount = parserResult.Parameters.Count;
//             parserParmeterCount.Should().Be(expectedParameters.Length);

//             for (var i = 0; i < parserParmeterCount; i++)
//             {
//                 parserResult.Parameters[i].Should().Be(expectedParameters[i]);
//             }
//         }
//     }
//}