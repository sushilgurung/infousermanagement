using AngleSharp.Dom;
using BlazorBootstrap;
using BlazorClient.Common;
using BlazorClient.Dto;
using BlazorClient.Pages.UserManagement;
using BlazorClient.Services.UserService;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.BlazorClient.UnitTests.TestPages;

public class UserFormTests : TestContext
{
    private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();

    private readonly Mock<PreloadService> _preloadServiceMock = new Mock<PreloadService>();

    private readonly Mock<ToastService> _toastServiceMock = new Mock<ToastService>();

    private readonly FakeNavigationManager _fakeNavManager;

    public UserFormTests()
    {
        base.Services.AddSingleton(_userServiceMock.Object);
        base.Services.AddSingleton(_preloadServiceMock.Object);
        base.Services.AddSingleton(_toastServiceMock.Object);
        _fakeNavManager = base.Services.GetRequiredService<FakeNavigationManager>();
    }

    [Fact]
    public void Should_Render_All_Form_Fields()
    {
        IRenderedComponent<UserForm> cut = RenderComponent<UserForm>();
        cut.Markup.Contains("First Name");
        cut.Markup.Contains("Last Name");
        cut.Markup.Contains("Email");
        cut.Markup.Contains("Date of Birth");
        cut.Find("button[type=submit]");
    }

    [Fact]
    public async Task Submits_Valid_Form_To_Create_User()
    {
        _userServiceMock.Setup((IUserService s) => s.CreateUserAsync(It.IsAny<CreateUserDto>())).ReturnsAsync(new Result<CreateUserDto>
        {
            IsSuccess = true,
            Message = "User has been created"
        });
        IRenderedComponent<UserForm> cut = RenderComponent<UserForm>();
        cut.Find("input#foreName").Change("Sushil");
        cut.Find("input#sureName").Change("Gurung");
        cut.Find("input#email").Change("sushil@yopmail.com");
        cut.Find("input#dateOfBirth").Change("2015-05-14");
        cut.Find("input#isActive").Change(value: true);
        await cut.Find("form").SubmitAsync();
        _userServiceMock.Verify((IUserService s) => s.CreateUserAsync(It.IsAny<CreateUserDto>()), Times.Once);
        Assert.Equal("http://localhost/UserManagement", _fakeNavManager.Uri);
    }

    [Fact]
    public async Task Loads_User_When_In_Edit_Mode()
    {
        UsersDto userDto = new UsersDto
        {
            ForeName = "Suresh",
            SurName = "Gurung",
            Email = "suresh@yopmail.com",
            DateOfBirth = new DateOnly(1990, 1, 1),
            IsActive = true
        };
        _userServiceMock.Setup((IUserService s) => s.GetUserByIdAsync(1)).ReturnsAsync(new Result<UsersDto>
        {
            IsSuccess = true,
            Data = userDto
        });
        IRenderedComponent<UserForm> cut = RenderComponent(delegate (ComponentParameterCollectionBuilder<UserForm> p)
        {
            p.Add((UserForm x) => x.UserId, 1);
        });
        await cut.InvokeAsync(() => Task.Delay(10));
        Assert.Equal("Suresh", cut.Find("input#foreName").GetAttribute("value"));
        Assert.Equal("Gurung", cut.Find("input#sureName").GetAttribute("value"));
        Assert.Equal("suresh@yopmail.com", cut.Find("input#email").GetAttribute("value"));
        Assert.Equal("1990-01-01", cut.Find("input#dateOfBirth").GetAttribute("value"));
        IElement isActiveCheckbox = cut.Find("input#isActive");
        Assert.True(isActiveCheckbox.HasAttribute("checked"));
    }

    [Fact]
    public async Task Displays_Validation_Errors_When_Provided_By_Api()
    {
        ReturnsExtensions.ReturnsAsync(value: new Result<CreateUserDto>
        {
            IsSuccess = false,
            Message = "Validation failed",
            Errors = new Dictionary<string, string[]> {
            {
                "Email",
                new string[1] { "Invalid email format." }
            } }
        }, mock: _userServiceMock.Setup((IUserService s) => s.CreateUserAsync(It.IsAny<CreateUserDto>())));
        IRenderedComponent<UserForm> cut = RenderComponent<UserForm>();
        cut.Find("input#foreName").Change("");
        cut.Find("input#sureName").Change("");
        cut.Find("input#email").Change("");
        cut.Find("input#dateOfBirth").Change("");
        await cut.Find("form").SubmitAsync();
        Assert.Contains("Email is required", cut.Markup);
    }
}