using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SK.Database;
using SK.Domain;

namespace SK.WebApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    public class RegisterExpertReq
    {
      public string Email { get; set; }
      public string Password { get; set; }
      public string Name { get; set; }
    }

    public class RegisterCompanyReq
    {
      public string Email { get; set; }
      public string Password { get; set; }
      public string Name { get; set; }
      public string CompanyName { get; set; }
    }

    public class SignInReq
    {
      public string Email { get; set; }
      public string Password { get; set; }
    }

    public class CurrentUserDataRes
    {
      public class ExpertProfileRes
      {
        public long Id { get; set; }
        public bool IsPublished { get; set; }
        public string ThumbnailImageUrl { get; set; }
      }

      public class CompanyRes
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsPublished { get; set; }
        public string ThumbnailImageUrl { get; set; }
      }

      public string Email { get; set; }
      public string DisplayName { get; set; }
      public ExpertProfileRes ExpertProfile { get; set; }
      public CompanyRes Company { get; set; }
    }

    public class InitPasswordResetReq
    {
      public string Email { get; set; }
    }

    public class FinishPasswordResetReq
    {
      public string Email { get; set; }
      public string Token { get; set; }
      public string NewPassword { get; set; }
    }

    public class EmailAlreadyUsedException : HttpTransferableException
    {
      public EmailAlreadyUsedException() : base("EMAIL_ALREADY_USED", "This email is already used!") { }
    }

    public class WrongEmailOrPasswordException : HttpTransferableException
    {
      public WrongEmailOrPasswordException() : base("WRONG_EMAIL_OR_PASSWORD", "Wrong email or password!") { }
    }

    public class UserNotFoundException : HttpTransferableException
    {
      public UserNotFoundException() : base("USER_NOT_FOUND", "User not found!") { }
    }

    private readonly DatabaseContext _databaseContext;

    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IEmailSender _emailSender;

    public AccountController(DatabaseContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IEmailSender emailSender)
    {
      this._databaseContext = context;

      this._userManager = userManager;
      this._roleManager = roleManager;
      this._signInManager = signInManager;

      this._emailSender = emailSender;
    }

    [HttpPost("[action]")]
    public async Task RegisterExpert([FromBody]RegisterExpertReq req)
    {
      var user = new User
      {
        UserName = req.Email,
        Email = req.Email,
        DisplayName = req.Name,
        ExpertProfile = new ExpertProfile
        {
        },
      };

      var result = await this._userManager.CreateAsync(user, req.Password);

      if (result.Succeeded == false && result.Errors.Any(e => e.Code == "DuplicateUserName"))
      {
        throw new EmailAlreadyUsedException();
      }

      await this._userManager.AddToRoleAsync(_userManager.Users.Single(u => u.Id == user.Id), "expert");
    }

    [HttpPost("[action]")]
    public async Task RegisterCompany([FromBody]RegisterCompanyReq req)
    {
      var user = new User
      {
        UserName = req.Email,
        Email = req.Email,
        DisplayName = req.Name,
        Company = new Company
        {
          Name = req.CompanyName,
        }
      };

      var result = await this._userManager.CreateAsync(user, req.Password);

      if (result.Succeeded == false && result.Errors.Any(e => e.Code == "DuplicateUserName"))
      {
        throw new EmailAlreadyUsedException();
      }

      await this._userManager.AddToRoleAsync(_userManager.Users.Single(u => u.Id == user.Id), "company-admin");
    }

    [HttpPost("[action]")]
    public async Task SignIn([FromBody]SignInReq req)
    {
      var acc = new User { UserName = req.Email, Email = req.Email };
      var result = await _signInManager.PasswordSignInAsync(req.Email, req.Password, true, false);

      if (!result.Succeeded)
      {
        throw new WrongEmailOrPasswordException();
      }
    }

    [HttpPost("[action]")]
    public async Task SignOut()
    {
      await this._signInManager.SignOutAsync();
    }

    [HttpPost("[action]")]
    public async Task InitPasswordReset([FromBody] InitPasswordResetReq req)
    {
      var user = await this._userManager.FindByEmailAsync(req.Email);

      if (user == null)
      {
        throw new UserNotFoundException();
      }

      var resetToken = HttpUtility.UrlEncode(await this._userManager.GeneratePasswordResetTokenAsync(user));

      var uriBuilder = new UriBuilder(
        Request.Scheme,
        Request.Host.Host);

      if (Request.Host.Port.HasValue)
      {
        uriBuilder.Port = Request.Host.Port.Value;
      }

      uriBuilder.Path = "login";
      uriBuilder.Query = $@"?email={ user.Email }&token={ resetToken }";

      await this._emailSender.SendAsync($@"<a href=""{ uriBuilder.ToString()  }"">Сбросить пароль</a>", "Сброс пароля", new[] { user.Email });
    }

    [HttpPost("[action]")]
    public async Task FinishPasswordReset([FromBody] FinishPasswordResetReq req)
    {
      var user = await this._userManager.FindByEmailAsync(req.Email);
      var res = await this._userManager.ResetPasswordAsync(user, req.Token, req.NewPassword);
    }

    [HttpGet("[action]")]
    public async Task<CurrentUserDataRes> GetCurrentUserData()
    {
      var userName = this.User.Identity.Name;
      var roleClaim = this.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);

      var user = await this._databaseContext.Users
        .Include(u => u.ExpertProfile)
        .Include(u => u.Company)
        .SingleOrDefaultAsync(u => u.UserName == this.User.Identity.Name);

      if (user == null)
      {
        await this._signInManager.SignOutAsync();
        return await Task.FromResult<CurrentUserDataRes>(null);
      }

      return await Task.FromResult(new CurrentUserDataRes
      {
        Email = user.Email,
        DisplayName = user.DisplayName,
        ExpertProfile = user.ExpertProfile != null
          ? new CurrentUserDataRes.ExpertProfileRes
          {
            Id = user.ExpertProfile.Id,
            IsPublished = user.ExpertProfile.IsPublished,
            ThumbnailImageUrl = user.ExpertProfile.ThumbnailImageUrl,
          }
          : null,
        Company = user.Company != null
          ? new CurrentUserDataRes.CompanyRes
          {
            Id = user.Company.Id,
            Name = user.Company.Name,
            IsPublished = user.Company.IsPublished,
            ThumbnailImageUrl = user.Company.ThumbnailImageUrl,
          }
          : null
      });
    }
  }
}