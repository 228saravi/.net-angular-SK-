using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SK.Domain;
using SK.Database;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SK.WebApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class VacancyDetailsUpdatorController : ControllerBase
  {
    public class VacancyIsNotPublishedException : HttpTransferableException
    {
      private VacancyDetailsUpdator.VacancyIsNotPublishedException _ex;

      protected override object SerializeExtraData()
      {
        return new { profileCheck = this._ex.VacancyCheck };
      }

      public VacancyIsNotPublishedException(Domain.VacancyDetailsUpdator.VacancyIsNotPublishedException e)
        : base("VACANCY_IS_NOT_PUBLISHED", e.Message) {
        this._ex = e;
      }
    }

    private DatabaseContext _context;
    private VacancyDetailsUpdator _updator;

    public VacancyDetailsUpdatorController(DatabaseContext context, VacancyDetailsUpdator updator)
    {
      this._context = context;
      this._updator = updator;
    }

    [HttpPost("[action]")]
    public async Task UpdateHeader([FromBody] VacancyDetailsUpdator.UpdateHeaderReq req)
    {
      try
      {
        await this._updator.UpdateHeader(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.VacancyDetailsUpdator.VacancyIsNotPublishedException)
        {
          throw new VacancyIsNotPublishedException(e as VacancyDetailsUpdator.VacancyIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateMainInfo([FromBody] VacancyDetailsUpdator.UpdateMainInfoReq req)
    {
      try
      {
        await this._updator.UpdateMainInfo(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.VacancyDetailsUpdator.VacancyIsNotPublishedException)
        {
          throw new VacancyIsNotPublishedException(e as VacancyDetailsUpdator.VacancyIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateExtraInfo([FromBody] VacancyDetailsUpdator.UpdateExtraInfoReq req)
    {
      try
      {
        await this._updator.UpdateExtraInfo(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.VacancyDetailsUpdator.VacancyIsNotPublishedException)
        {
          throw new VacancyIsNotPublishedException(e as VacancyDetailsUpdator.VacancyIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateAboutVacancy([FromBody] VacancyDetailsUpdator.UpdateAboutVacancyReq req)
    {
      try
      {
        await this._updator.UpdateAboutVacancy(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.VacancyDetailsUpdator.VacancyIsNotPublishedException)
        {
          throw new VacancyIsNotPublishedException(e as VacancyDetailsUpdator.VacancyIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task Publish([FromBody] VacancyDetailsUpdator.PublishReq req)
    {
      try
      {
        await this._updator.Publish(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.VacancyDetailsUpdator.VacancyIsNotPublishedException)
        {
          throw new VacancyIsNotPublishedException(e as VacancyDetailsUpdator.VacancyIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task Unpublish([FromBody] VacancyDetailsUpdator.PublishReq req)
    {
      await this._updator.Unpublish(req, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task MakePublic([FromBody] VacancyDetailsUpdator.PublishReq req)
    {
      await this._updator.MakePublic(req, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task MakePrivate([FromBody] VacancyDetailsUpdator.PublishReq req)
    {
      await this._updator.MakePrivate(req, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task Delete([FromBody] VacancyDetailsUpdator.PublishReq req)
    {
      await this._updator.Delete(req, this._context);
      await this._context.SaveChangesAsync();
    }
  }
}