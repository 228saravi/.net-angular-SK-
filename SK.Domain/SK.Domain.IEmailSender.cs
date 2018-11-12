using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public interface IEmailSender: IDisposable
  {
    Task SendAsync(string body, string theme, IReadOnlyCollection<string> emails);
  }
}
