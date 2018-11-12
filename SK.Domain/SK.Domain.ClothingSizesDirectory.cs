using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class ClothingSizesDirectory
  {
    public class Res
    {
      public class ClothingSize
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public IReadOnlyCollection<ClothingSize> ClothingSizes { get; set; }
    }

    public async Task<Res> GetAll(DatabaseContext database)
    {
      var clothingSizes = await database.ClothingSizes
        .OrderBy(c => c.Rank).
        Select(c => new Res.ClothingSize
        {
          Id = c.Id,
          Name = c.Name,
        }).ToArrayAsync();

      var res = new Res
      {
        ClothingSizes = clothingSizes
      };

      return res;
    }
  }
}
