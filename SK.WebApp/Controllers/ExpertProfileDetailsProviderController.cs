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
  public class ExpertProfileDetailsProviderController : ControllerBase
  {
    private DatabaseContext _context;
    private ExpertProfileDetailsProvider _provider;

    public ExpertProfileDetailsProviderController(DatabaseContext context, ExpertProfileDetailsProvider provider)
    {
      this._context = context;
      this._provider = provider;
    }

    [HttpGet("[action]")]
    public async Task<ExpertProfileDetailsProvider.Res> Get([FromQuery] ExpertProfileDetailsProvider.Req req)
    {
      var res = await this._provider.Get(req, this._context);
      return res;
    }
  }
}