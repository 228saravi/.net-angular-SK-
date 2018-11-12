using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class CurrentUserData
  {
    public string Id { get; set; }
    public long? ExpertProfileId { get; set; }
    public long? CompanyId { get; set; }
  }

  public interface ICurrentUserService
  {
    CurrentUserData GetCurrentUserData();
  }
}
