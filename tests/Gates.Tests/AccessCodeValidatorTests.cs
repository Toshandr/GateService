using Gates.Api.Services;

namespace Gates.Tests;

public class AccessCodeValidatorTests
{
    private readonly AccessCodeValidator _validator = new();

    [Theory]
    [InlineData("Dragon7!")] // минимальная длина (8)
    [InlineData("Abcdefghijklmn1!")] // максимальная длина (16)
    [InlineData("Ab1!Cd2@Ef3#")]
    [InlineData("Knight@42")]
    public void ValidCode_ReturnsNoErrors(string code)
    {
        Assert.Empty(_validator.Validate(code));
    }

    [Theory]
    [InlineData("Br7!", "Длина должна быть от 8 до 16 символов.")]
    [InlineData("Abcdefghijklmno1!", "Длина должна быть от 8 до 16 символов.")]
    [InlineData("dragon7!", "Нет заглавной латинской буквы.")]
    [InlineData("BERSERK7!", "Нет строчной латинской буквы.")]
    [InlineData("Berserk!", "Нет цифры.")]
    [InlineData("Berserk77", "Нет специального символа.")]
    [InlineData("I hate матанал7!", "Код содержит пробелы.")]
    [InlineData("Berseeerk7!", "Обнаружены три одинаковых символа подряд.")]
    [InlineData("Berserk777!", "Обнаружены три одинаковых символа подряд.")]
    [InlineData("Berserk123!", "Обнаружена последовательность из трёх или более подряд идущих цифр.")]
    [InlineData("Berserk789!", "Обнаружена последовательность из трёх или более подряд идущих цифр.")]
    [InlineData("1Berserk7!", "Первый символ должен быть буквой.")]
    [InlineData("!Berserk7!", "Первый символ должен быть буквой.")]
    [InlineData("Berserk7!x", "Последний символ должен быть цифрой или специальным символом.")]
    public void SingleViolation_ReturnsOnlyThatError(string code, string expected)
    {
        var error = Assert.Single(_validator.Validate(code));
        Assert.Equal(expected, error);
    }

    [Fact]
    public void SeveralViolations_ReturnsAllOfThem()
    {
        var errors = _validator.Validate("dragon123");

        Assert.Equal(
            new[]
            {
                "Нет заглавной латинской буквы.",
                "Нет специального символа.",
                "Обнаружена последовательность из трёх или более подряд идущих цифр."
            },
            errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmpty_ReturnsErrorsWithoutThrowing(string? code)
    {
        var errors = _validator.Validate(code);

        Assert.Contains("Длина должна быть от 8 до 16 символов.", errors);
    }
}
