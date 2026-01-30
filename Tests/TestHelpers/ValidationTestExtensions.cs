using FluentValidation;
using Xunit;

namespace Tests.TestHelpers
{
    internal static class ValidationTestExtensions
    {
        public static void ShouldHaveErrorFor<T>(this IValidator<T> validator, T model, string propertyName)
        {
            var result = validator.Validate(model);
            Assert.Contains(result.Errors, error => error.PropertyName == propertyName);
        }

        public static void ShouldNotHaveAnyErrors<T>(this IValidator<T> validator, T model)
        {
            var result = validator.Validate(model);
            Assert.Empty(result.Errors);
        }
    }
}