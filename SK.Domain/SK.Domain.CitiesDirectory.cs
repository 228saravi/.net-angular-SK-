using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class CitiesDirectory
  {
    public class Res
    {
      public class City
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public IReadOnlyCollection<City> Cities { get; set; }
    }

    public async Task<Res> GetAll(DatabaseContext database)
    {
      var cities = await database.Cities.Select(c => new Res.City
      {
        Id = c.Id,
        Name = c.Name,
      }).ToArrayAsync();

      var res = new Res
      {
        Cities = cities
      };

      return res;
    }
  }
}
