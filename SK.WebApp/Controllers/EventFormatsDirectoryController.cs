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
  public class EventFormatsDirectoryController : ControllerBase
  {
    private DatabaseContext _context;
    private EventFormatsDirectory _directory;

    public EventFormatsDirectoryController(DatabaseContext context, EventFormatsDirectory store)
    {
      this._context = context;
      this._directory = store;
    }

    [HttpGet("[action]")]
    public async Task<EventFormatsDirectory.Res> GetAll()
    {
      var res = await this._directory.GetAll(this._context);
      return res;
    }
  }
}