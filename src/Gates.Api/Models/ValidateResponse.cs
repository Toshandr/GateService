namespace Gates.Api.Models;

public record ValidateResponse(bool IsValid, List<string> Errors, string Message);
