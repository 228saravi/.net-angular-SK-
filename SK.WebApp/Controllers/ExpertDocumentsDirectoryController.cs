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
  public class ExpertDocumentsDirectoryController : ControllerBase
  {
    private DatabaseContext _context;
    private ExpertDocumentsDirectory _directory;

    public ExpertDocumentsDirectoryController(DatabaseContext context, ExpertDocumentsDirectory directory)
    {
      this._context = context;
      this._directory = directory;
    }

    [HttpGet("[action]")]
    public async Task<ExpertDocumentsDirectory.Res> GetAll()
    {
      var res = await this._directory.GetAll(this._context);
      return res;
    }
  }
}