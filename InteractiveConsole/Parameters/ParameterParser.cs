using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace InteractiveConsole
{
    public class ParameterParser
    {
        private const char Space = ' ';

        public static ParameterParserResult Parse(string input)
        {
            var firstSpaceIndex = input.IndexOf(Space);
            if (firstSpaceIndex == -1)
            {
                return new ParameterParserResult
                {
                    CommandName = input + "Command"
                };
            }

            var result = new ParameterParserResult
            {
                CommandName = input.Substring(0, firstSpaceIndex) + "Command",
                Parameters = new List<Parameter>()
            };

            var parametersString = input.Substring(firstSpaceIndex);

            var parameterMatches = Regex.Matches(parametersString, "\\s(?<parameterName>\\w+)=((?<parameterValue>[A-Za-z0-9~!@#$%&*\\(\\)-\\+]+)|\"(?<parameterValue>([^\"\\\\]|\\\\.)*)\")");
            foreach (Match match in parameterMatches)
            {
                result.Parameters.Add(new Parameter
                {
                    Name = match.Groups["parameterName"].ToString(),
                    Value = match.Groups["parameterValue"].ToString()
                });
            }

            return result;
        }
    }
}