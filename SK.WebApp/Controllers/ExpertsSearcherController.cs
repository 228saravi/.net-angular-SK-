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
  public class ExpertsSearcherController : ControllerBase
  {
    private DatabaseContext _context;
    private ExpertsSearcher _searcher;

    public ExpertsSearcherController(DatabaseContext context, ExpertsSearcher searcher)
    {
      this._context = context;
      this._searcher = searcher;
    }

    [HttpGet("[action]")]
    public async Task<ExpertsSearcher.Res> Search([FromQuery] ExpertsSearcher.Req req)
    {
      var searchResult = await this._searcher.Search(req, this._context);
      return searchResult;
    }
  }
}