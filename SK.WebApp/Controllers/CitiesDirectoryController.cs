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
  public class CitiesDirectoryController : ControllerBase
  {
    private DatabaseContext _context;
    private CitiesDirectory _directory;

    public CitiesDirectoryController(DatabaseContext context, CitiesDirectory store)
    {
      this._context = context;
      this._directory = store;
    }

    [HttpGet("[action]")]
    public async Task<CitiesDirectory.Res> GetAll()
    {
      var res = await this._directory.GetAll(this._context);
      return res;
    }
  }
}