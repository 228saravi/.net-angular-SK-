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
  public class SkillsMatrixDirectoryController : ControllerBase
  {
    private DatabaseContext _context;
    private SkillsMatrixDirectory _skillsMatrixProvider;

    public SkillsMatrixDirectoryController(DatabaseContext context, SkillsMatrixDirectory skillsMatrixProvider)
    {
      this._context = context;
      this._skillsMatrixProvider = skillsMatrixProvider;
    }

    [HttpGet("[action]")]
    public async Task<SkillsMatrix> Get()
    {
      var res = await this._skillsMatrixProvider.Get(this._context);
      return res;
    }
  }
}