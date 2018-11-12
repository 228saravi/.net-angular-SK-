using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class SegmentsDirectory
  {
    public class Res
    {
      public class Segment
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public IReadOnlyCollection<Segment> Segments { get; set; }
    }

    public async Task<Res> GetAll(DatabaseContext database)
    {
      var segments = await database.Segments
        .OrderBy(s => s.Rank)
        .Select(s => new Res.Segment
        {
          Id = s.Id,
          Name = s.Name,
        }).ToArrayAsync();

      var res = new Res
      {
        Segments = segments
      };

      return res;
    }
  }
}
