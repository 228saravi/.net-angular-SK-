using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class ExperienceOptionsDirectory
  {
    public class Res
    {
      public class ExperienceOption
      {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
      }

      public ExperienceOption[] ExperienceOptions { get; set; }
    }

    public async Task<Res> GetAll(DatabaseContext database)
    {
      var experienceOptions = await database.ExperienceOptions
        .OrderBy(o => o.Rank)
        .Select(o => new Res.ExperienceOption
        {
          Id = o.Id,
          Name = o.Name,
          Rank = o.Rank
        }).ToArrayAsync();

      var res = new Res
      {
        ExperienceOptions = experienceOptions
      };

      return res;
    }
  }
}
