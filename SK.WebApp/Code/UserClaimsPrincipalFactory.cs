using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SK.Database;
using SK.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SK.WebApp
{
  public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
  {
    private readonly DatabaseContext _databaseContext;

    public UserClaimsPrincipalFactory(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            DatabaseContext databaseContext
      ) : base(userManager, roleManager, optionsAccessor)
    {
      this._databaseContext = databaseContext;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
      user = await this._databaseContext.Users
        .Include(u => u.ExpertProfile)
        .Include(u => u.Company)
        .SingleAsync(u => u.Id == user.Id);

      var identity = await base.GenerateClaimsAsync(user);

      if (user.ExpertProfile != null)
      {
        identity.AddClaim(new Claim(CurrentUserService.ExpertProfileIdClaimName, user.ExpertProfile.Id.ToString()));
      }

      if (user.Company != null)
      {
        identity.AddClaim(new Claim(CurrentUserService.CompanyIdClaimName, user.Company.Id.ToString()));
      }

      return identity;
    }
  }
}
