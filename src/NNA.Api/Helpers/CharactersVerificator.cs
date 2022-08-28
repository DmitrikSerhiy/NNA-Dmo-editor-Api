namespace NNA.Api.Helpers;

/// <summary>
/// Verificator code took from Microsoft.AspNetCore.Identity.PasswordValidator
/// </summary>
public static class CharactersVerificator {
    public static bool IsDigit(char c) {
        return c is >= '0' and <= '9';
    }
    
    public static bool IsLower(char c) {
        return c is >= 'a' and <= 'z';
    }
    
    public static bool IsUpper(char c) {
        return c is >= 'A' and <= 'Z';
    }
    
    public static bool IsLetterOrDigit(char c) {
        return IsUpper(c) || IsLower(c) || IsDigit(c);
    }
}