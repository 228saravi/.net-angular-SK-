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
  public class ClothingSizesDirectoryController : ControllerBase
  {
    private DatabaseContext _context;
    private ClothingSizesDirectory _directory;

    public ClothingSizesDirectoryController(DatabaseContext context, ClothingSizesDirectory directory)
    {
      this._context = context;
      this._directory = directory;
    }

    [HttpGet("[action]")]
    public async Task<ClothingSizesDirectory.Res> GetAll()
    {
      var res = await this._directory.GetAll(this._context);
      return res;
    }
  }
}