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
  public class VacanciesSearcherController : ControllerBase
  {
    private DatabaseContext _context;
    private VacanciesSearcher _searcher;

    public VacanciesSearcherController(DatabaseContext context, VacanciesSearcher searcher)
    {
      this._context = context;
      this._searcher = searcher;
    }

    [HttpGet("[action]")]
    public async Task<VacanciesSearcher.Res> Search([FromQuery] VacanciesSearcher.Req req)
    {
      var searchResult = await this._searcher.Search(req, this._context);
      return searchResult;
    }
  }
}