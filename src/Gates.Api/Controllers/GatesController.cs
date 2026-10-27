using Gates.Api.Models;
using Gates.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gates.Api.Controllers;

[ApiController]
[Route("api/gates")] 
public class GatesController(IAccessCodeValidator validator) : ControllerBase
{
    [HttpPost("validate")] //единственный endpoint для валидации пароля
    [ProducesResponseType<ValidateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ValidateResponse> Validate(ValidateRequest request)
    {
        var errors = validator.Validate(request.AccessCode);
        var isValid = errors.Count == 0;

        return new ValidateResponse(
            isValid,
            errors,
            isValid ? "Код принят. Ворота открываются." : "Код отклонён."); //такую фишку без if else вспомнил с пар C++
    }
}
