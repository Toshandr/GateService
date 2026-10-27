using System.ComponentModel.DataAnnotations;

namespace Gates.Api.Models;

public class ValidateRequest
{
    [Required(ErrorMessage = "Код доступа не указан или пуст.")]
    public string? AccessCode { get; set; }
}
