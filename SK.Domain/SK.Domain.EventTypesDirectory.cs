using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class EventTypesDirectory
  {
    public class Res
    {
      public class EventType
      {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
      }

      public IReadOnlyCollection<EventType> EventTypes { get; set; }
    }

    public async Task<Res> GetAll(DatabaseContext database)
    {
      var eventTypes = await database.EventTypes.Select(c => new Res.EventType
      {
        Id = c.Id,
        Name = c.Name,
        Rank = c.Rank,
      }).ToArrayAsync();

      var res = new Res
      {
        EventTypes = eventTypes
      };

      return res;
    }
  }
}
