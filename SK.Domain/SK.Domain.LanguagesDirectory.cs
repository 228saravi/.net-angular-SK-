using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class LanguagesDirectory
  {
    public class Res
    {
      public class Language
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public IReadOnlyCollection<Language> Languages { get; set; }
    }

    public async Task<Res> GetAll(DatabaseContext database)
    {
      var languages = await database.Languages.Select(l => new Res.Language
      {
        Id = l.Id,
        Name = l.Name,
      }).ToArrayAsync();

      var res = new Res
      {
        Languages = languages
      };

      return res;
    }
  }
}
