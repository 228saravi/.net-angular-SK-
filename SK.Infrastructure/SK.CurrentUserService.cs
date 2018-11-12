using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SK.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SK.Infrastructure
{
  public class CurrentUserService : ICurrentUserService
  {
    public static string ExpertProfileIdClaimName => "SK.ExpertProfileIdClaim";
    public static string CompanyIdClaimName => "SK.CompanyIdClaim";

    private IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
      this._httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserData GetCurrentUserData()
    {
      var principal = this._httpContextAccessor.HttpContext.User;
      var identity = principal.Identity as ClaimsIdentity;

      var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
      var expertProfileAsString = principal.FindFirstValue(CurrentUserService.ExpertProfileIdClaimName);
      var companyIdAsString = principal.FindFirstValue(CurrentUserService.CompanyIdClaimName);

      return new CurrentUserData()
      {
        Id = id,
        ExpertProfileId = expertProfileAsString != null ? new long?(long.Parse(expertProfileAsString)) : null,
        CompanyId = companyIdAsString != null ? new long?(long.Parse(companyIdAsString)) : null,
      };
    }
  }
}
