using ToDoApi.Validation;
using Xunit;

namespace ToDoApi.Tests.UnitTests.Validation
{
    public class NameValidatorTests
    {

        [Theory]
        [InlineData("", "Name should not be empty.")]
        [InlineData("       ", "Name should not be only whitespace")]
        [InlineData( ".เนต คอ", "Name should only contain latin alphabet characters")]
        [InlineData( "Database-issues", "Name should not be more than 10 characters.")]
        public void NameIsNotValid(string name, string reason)
        {
            Assert.False(NameValidator.IsValid(name),
                $"Expected name '{name}' to fail validation." +
                $"Reason: {reason}");
        }

        [Theory]
        [InlineData("Valid name")]
        [InlineData("Gyldig")]
        [InlineData("TODO NAVN")]
        [InlineData("Navn")]
        public void Name_should_be_valid(string name)
        {
            Assert.True(NameValidator.IsValid(name), 
                $"Expected '{name}' to pass name validation.");
        }

        [Fact]
        public void Regex_should_be_correct()
        {
            Assert.Equal(
                @"^[a-zA-Z0-9\ ]+$",
                NameValidator.NameValidatePattern);
        }
    }
}
