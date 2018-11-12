using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class EventFormatsDirectory
  {
    public class Res
    {
      public class EventFormat
      {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
      }

      public EventFormat[] EventFormats { get; set; }
    }

    public async Task<Res> GetAll(DatabaseContext database)
    {
      var eventFormats = await database.EventFormats.Select(c => new Res.EventFormat
      {
        Id = c.Id,
        Name = c.Name,
        Rank = c.Rank,
      }).ToArrayAsync();

      var res = new Res
      {
        EventFormats = eventFormats
      };

      return res;
    }
  }
}
