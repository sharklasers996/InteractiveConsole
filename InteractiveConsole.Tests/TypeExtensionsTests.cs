using Xunit;
using InteractiveConsole.Extensions;
using FluentAssertions;
using InteractiveConsole.Models;
using System.Collections.Generic;

namespace InteractiveConsole.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void ToTypeInfo_should_convert_string()
        {
            // arrange
            var expectedTypeInfo = new TypeInfo
            {
                IsString = true,
                Type = typeof(string)
            };

            // act
            var typeInfo = "test".ToTypeInfo();

            // assert
            typeInfo.Should().BeEquivalentTo(expectedTypeInfo);
        }

        [Fact]
        public void ToTypeInfo_should_convert_int()
        {
            // arrange
            var expectedTypeInfo = new TypeInfo
            {
                IsNumber = true,
                Type = typeof(int)
            };

            // act
            var typeInfo = 15.ToTypeInfo();

            // assert
            typeInfo.Should().BeEquivalentTo(expectedTypeInfo);
        }

        [Fact]
        public void ToTypeInfo_should_convert_enum()
        {
            // arrange
            var expectedTypeInfo = new TypeInfo
            {
                IsEnum = true,
                Type = typeof(TestEnum)
            };

            // act
            var typeInfo = TestEnum.Value.ToTypeInfo();

            // assert
            typeInfo.Should().BeEquivalentTo(expectedTypeInfo);
        }

        [Fact]
        public void ToTypeInfo_should_convert_custom_object()
        {
            // arrange
            var expectedTypeInfo = new TypeInfo
            {
                IsCustomObject = true,
                ObjectName = "Object",
                Type = typeof(object)
            };

            // act
            var typeInfo = new object().ToTypeInfo();

            // assert
            typeInfo.Should().BeEquivalentTo(expectedTypeInfo);
        }

        [Fact]
        public void ToTypeInfo_should_convert_list_of_strings()
        {
            // arrange
            var expectedTypeInfo = new TypeInfo
            {
                IsList = true,
                IsListItemString = true,
                Type = typeof(List<string>)
            };

            // act
            var typeInfo = new List<string>().ToTypeInfo();

            // assert
            typeInfo.Should().BeEquivalentTo(expectedTypeInfo);
        }

        [Fact]
        public void ToTypeInfo_should_convert_list_of_ints()
        {
            // arrange
            var expectedTypeInfo = new TypeInfo
            {
                IsList = true,
                IsListItemNumber = true,
                Type = typeof(List<int>)
            };

            // act
            var typeInfo = new List<int>().ToTypeInfo();

            // assert
            typeInfo.Should().BeEquivalentTo(expectedTypeInfo);
        }

        [Fact]
        public void ToTypeInfo_should_convert_list_of_custom_objects()
        {
            // arrange
            var expectedTypeInfo = new TypeInfo
            {
                IsList = true,
                IsListItemCustomObject = true,
                ListItemObjectName = "Object",
                Type = typeof(List<object>)
            };

            // act
            var typeInfo = new List<object>().ToTypeInfo();

            // assert
            typeInfo.Should().BeEquivalentTo(expectedTypeInfo);
        }

        [Fact]
        public void ToTypeInfo_should_convert_bool()
        {
            // arrange
            var expectedTypeInfo = new TypeInfo
            {
                IsBool = true,
                Type = typeof(bool)
            };

            // act
            var typeInfo = true.ToTypeInfo();

            // assert
            typeInfo.Should().BeEquivalentTo(expectedTypeInfo);
        }

        enum TestEnum
        {
            Value = 1,
            Value2 = 2
        }
    }
}