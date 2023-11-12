using System.ComponentModel.DataAnnotations;

namespace Data.Models.Request;

public class LoginRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} precisa ser um email válido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 8)]
    public string Password { get; set; }
}

public class CreateUserRequest : LoginRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 4)]
    public string Name { get; set; }
}


public class ResetPassword : LoginRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string ResetPasswordToken { get; set; }
}