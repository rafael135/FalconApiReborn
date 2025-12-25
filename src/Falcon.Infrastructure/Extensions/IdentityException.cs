using Microsoft.AspNetCore.Identity;

namespace Falcon.Infrastructure.Extensions;

/// <summary>
/// Extensions for IdentityError handling.
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// Converts IdentityErrors to a friendly dictionary format.
    /// </summary>
    /// <param name="errors">The collection of IdentityError objects to convert.</param>
    /// <returns>A dictionary mapping error keys to user-friendly error messages.</returns>
    public static Dictionary<string, string> ToFriendlyDictionary(
        this IEnumerable<IdentityError> errors
    )
    {
        Dictionary<string, string> errorMessages = new();

        foreach (IdentityError error in errors)
        {
            string message = error.Code switch
            {
                "PasswordTooShort" => "A senha deve ter no mínimo 8 caracteres",
                "PasswordRequiresDigit" => "A senha deve conter pelo menos um número",
                "PasswordRequiresLower" => "A senha deve conter letras minúsculas",
                "PasswordRequiresUpper" => "A senha deve conter letras maiúsculas",
                "PasswordRequiresNonAlphanumeric" => "A senha deve conter caracteres especiais",
                "DuplicateUserName" => "Este e-mail já está em uso",
                "DuplicateEmail" => "Este e-mail já está em uso",
                "InvalidEmail" => "Formato de e-mail inválido",
                "InvalidUserName" => "Nome de usuário inválido",
                _ => error.Description,
            };

            if (error.Code.StartsWith("Password"))
            {
                errorMessages["password"] = message;
            }
            else if (error.Code.StartsWith("Email"))
            {
                errorMessages["email"] = message;
            }
            else
            {
                errorMessages["form"] = message;
            }
        }

        return errorMessages;
    }
}
