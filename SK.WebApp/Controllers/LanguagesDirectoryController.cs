using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SK.Domain;
using SK.Database;

namespace SK.WebApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LanguagesDirectoryController : ControllerBase
  {
    private DatabaseContext _context;
    private LanguagesDirectory _store;

    public LanguagesDirectoryController(DatabaseContext context, LanguagesDirectory store)
    {
      this._context = context;
      this._store = store;
    }

    [HttpGet("[action]")]
    public async Task<LanguagesDirectory.Res> GetAll()
    {
      var res = await this._store.GetAll(this._context);
      return res;
    }
  }
}