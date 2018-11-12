using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class User: IdentityUser
  {
    [Timestamp]
    public byte[] Timestamp { get; set; }

    public string DisplayName { get; set; }
    public DateTimeOffset? LastSeenTime { get; set; }

    public ExpertProfile ExpertProfile { get; set; }
    public Company Company { get; set; }
  }
}
