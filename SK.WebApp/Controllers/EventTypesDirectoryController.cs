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
  public class EventTypesDirectoryController : ControllerBase
  {
    private DatabaseContext _context;
    private EventTypesDirectory _store;

    public EventTypesDirectoryController(DatabaseContext context, EventTypesDirectory store)
    {
      this._context = context;
      this._store = store;
    }

    [HttpGet("[action]")]
    public async Task<EventTypesDirectory.Res> GetAll()
    {
      var res = await this._store.GetAll(this._context);
      return res;
    }
  }
}