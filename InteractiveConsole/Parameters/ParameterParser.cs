using System.Linq;
using System;
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

            var parametersString = input.Substring(firstSpaceIndex).Trim();

            var parameterMatches = Regex.Matches(parametersString, "\\s?(?<parameterName>\\w+)=((?<parameterValue>[A-Za-z0-9/~!@#$'%:\\\\&*\\(\\)-\\+\\[\\]\\.]+)|\"(?<parameterValue>([^\"\\\\]|\\\\.)*)\")");
            if (!parameterMatches.Any()
                && !String.IsNullOrEmpty(parametersString.Trim()))
            {
                result.Success = false;
                return result;
            }

            var matchLength = 0;
            foreach (Match match in parameterMatches)
            {
                matchLength += match.Length;

                var value = match.Groups["parameterValue"].ToString();

                var param = new Parameter
                {
                    Name = match.Groups["parameterName"].ToString(),
                    Value = match.Groups["parameterValue"].ToString()
                };

                var indexMatch = Regex.Match(value, @"#(?<parameterValue>\d+)\[(?<indexFrom>\d+)(\.\.)?(?<indexTo>\d+)?\]?");
                if (indexMatch.Success)
                {
                    var indexFromString = indexMatch.Groups["indexFrom"].ToString();
                    var indexToString = indexMatch.Groups["indexTo"].ToString();

                    param.IndexFrom = int.Parse(indexFromString);
                    if (!String.IsNullOrEmpty(indexToString))
                    {
                        param.IndexTo = int.Parse(indexToString);
                    }
                }

                result.Parameters.Add(param);
            }

            result.Success = parametersString.Length == matchLength;

            return result;
        }
    }
}