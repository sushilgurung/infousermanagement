using BlazorClient.Pages;
using BlazorClient.Services.AuthService;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
namespace UserManagement.BlazorClient.UnitTests.TestPages;

public class LoginPageTests : TestContext
{
    [Fact]
    public void ShouldRenderLoginForm()
    {
        Mock<IAuthService> authServiceMock = new Mock<IAuthService>();
        base.Services.AddSingleton(authServiceMock.Object);
        IRenderedComponent<Login> cut = RenderComponent<Login>();
        cut.Markup.Contains("Login to your account");
        cut.Find("input#userName");
        cut.Find("input#password");
        cut.Find("button");
    }

    [Fact]
    public async Task ShouldShowValidationErrorsIfFormInvalid()
    {
        ServiceCollectionServiceExtensions.AddSingleton(implementationInstance: new Mock<IAuthService>().Object, services: base.Services);
        IRenderedComponent<Login> cut = RenderComponent<Login>();
        await cut.Find("form").SubmitAsync();
        Assert.Contains("UserName is required", cut.Markup);
        Assert.Contains("Password is required", cut.Markup);
    }
}