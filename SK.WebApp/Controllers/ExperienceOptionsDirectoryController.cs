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
  public class ExperienceOptionsDirectoryController : ControllerBase
  {
    private DatabaseContext _context;
    private ExperienceOptionsDirectory _directory;

    public ExperienceOptionsDirectoryController(DatabaseContext context, ExperienceOptionsDirectory directory)
    {
      this._context = context;
      this._directory = directory;
    }

    [HttpGet("[action]")]
    public async Task<ExperienceOptionsDirectory.Res> GetAll()
    {
      var res = await this._directory.GetAll(this._context);
      return res;
    }
  }
}