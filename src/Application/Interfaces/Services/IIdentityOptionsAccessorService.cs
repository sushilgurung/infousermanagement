namespace Application.Interfaces.Services;

public interface IIdentityOptionsAccessorService
{
    string GetAllowedUserNameCharacters();
    Task<bool> IsUserNameValid(string userName);
    int GetPasswordRequiredLength();
    bool GetPasswordRequireNonAlphanumeric();
    bool GetPasswordRequireDigit();
    bool GetPasswordRequireLowercase();
    bool GetPasswordRequireUppercase();
    Task<bool> IsPasswordValid(string password);

}