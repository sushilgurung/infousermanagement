
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Services;

public class IdentityOptionsAccessorService : IIdentityOptionsAccessorService
{
    private readonly ILogger<IdentityOptionsAccessorService> _logger;
    private readonly string _allowedUserNameChars;
    private readonly PasswordOptions _passwordOptions;
    public IdentityOptionsAccessorService(ILogger<IdentityOptionsAccessorService> logger, IOptions<IdentityOptions> identityOptions)
    {
        _logger = logger;
        var identityOptionsValue = identityOptions.Value;
        _allowedUserNameChars = identityOptionsValue.User.AllowedUserNameCharacters;
        _passwordOptions = identityOptionsValue.Password;
    }

    /// <summary>
    /// this method returns the allowed characters for usernames as defined in IdentityOptions.
    /// </summary>
    /// <returns></returns>
    public string GetAllowedUserNameCharacters() => _allowedUserNameChars;

    /// <summary>
    /// This method checks if the provided username is valid based on the allowed characters defined in IdentityOptions.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public Task<bool> IsUserNameValid(string userName)
    {
        var isValid = userName.All(c => _allowedUserNameChars.Contains(c));
        return Task.FromResult(isValid);
    }

    public int GetPasswordRequiredLength() => _passwordOptions.RequiredLength;

    public bool GetPasswordRequireNonAlphanumeric() => _passwordOptions.RequireNonAlphanumeric;

    public bool GetPasswordRequireDigit() => _passwordOptions.RequireDigit;

    public bool GetPasswordRequireLowercase() => _passwordOptions.RequireLowercase;

    public bool GetPasswordRequireUppercase() => _passwordOptions.RequireUppercase;

    /// <summary>
    /// This method checks if the provided password is valid based on the rules defined in IdentityOptions.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public Task<bool> IsPasswordValid(string password)
    {
        if (string.IsNullOrEmpty(password))
            return Task.FromResult(false);

        bool isValid = password.Length >= _passwordOptions.RequiredLength;

        if (_passwordOptions.RequireNonAlphanumeric)
            isValid &= password.Any(ch => !char.IsLetterOrDigit(ch));

        if (_passwordOptions.RequireDigit)
            isValid &= password.Any(char.IsDigit);

        if (_passwordOptions.RequireLowercase)
            isValid &= password.Any(char.IsLower);

        if (_passwordOptions.RequireUppercase)
            isValid &= password.Any(char.IsUpper);

        return Task.FromResult(isValid);
    }
}


