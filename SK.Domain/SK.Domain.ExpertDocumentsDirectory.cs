using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class ExpertDocumentsDirectory
  {
    public class Res
    {
      public class Document
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public IReadOnlyCollection<Document> Documents { get; set; }
    }

    public async Task<Res> GetAll(DatabaseContext database)
    {
      var documents = await database.ExpertDocuments.Select(c => new Res.Document
      {
        Id = c.Id,
        Name = c.Name,
      }).ToArrayAsync();

      var res = new Res
      {
        Documents = documents
      };

      return res;
    }
  }
}
