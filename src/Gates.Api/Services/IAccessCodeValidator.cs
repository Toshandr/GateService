namespace Gates.Api.Services;

public interface IAccessCodeValidator
{
    //Возвращает список всех нарушений. Пустой список означает, что код корректен
    List<string> Validate(string? code);
}
