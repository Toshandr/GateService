namespace Gates.Api.Services;

public class AccessCodeValidator : IAccessCodeValidator
{
    private const string Specials = "!@#$%";

    public List<string> Validate(string? code)
    {
        code ??= "";
        var errors = new List<string>();

        if (code.Length < 8 || code.Length > 16)
            errors.Add("Длина должна быть от 8 до 16 символов.");

        if (!code.Any(c => c is >= 'A' and <= 'Z'))
            errors.Add("Нет заглавной латинской буквы.");

        if (!code.Any(c => c is >= 'a' and <= 'z'))
            errors.Add("Нет строчной латинской буквы.");

        if (!code.Any(IsDigit))
            errors.Add("Нет цифры.");

        if (!code.Any(IsSpecial))
            errors.Add("Нет специального символа.");

        if (code.Any(char.IsWhiteSpace))
            errors.Add("Код содержит пробелы.");

        if (HasTripleRepeat(code))
            errors.Add("Обнаружены три одинаковых символа подряд.");

        if (HasDigitSequence(code))
            errors.Add("Обнаружена последовательность из трёх или более подряд идущих цифр.");

        if (code.Length > 0 && !char.IsLetter(code[0]))
            errors.Add("Первый символ должен быть буквой.");

        if (code.Length > 0 && !IsDigit(code[^1]) && !IsSpecial(code[^1]))
            errors.Add("Последний символ должен быть цифрой или специальным символом.");

        return errors;
    }

    private static bool IsDigit(char c) => c is >= '0' and <= '9';

    private static bool IsSpecial(char c) => Specials.Contains(c);

    private static bool HasTripleRepeat(string s)
    {
        for (var i = 2; i < s.Length; i++)
            if (s[i] == s[i - 1] && s[i] == s[i - 2])
                return true;
        return false;
    }

    //Проверка на монотонность
    private static bool HasDigitSequence(string s)
    {
        for (var i = 2; i < s.Length; i++)
            if (IsDigit(s[i]) && s[i - 1] == s[i] - 1 && s[i - 2] == s[i] - 2)
                return true;
        return false;
    }
}
