using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SK.Domain;
using SK.Database;
using System.IO;
using Microsoft.EntityFrameworkCore;
using SK.Infrastructure;

namespace SK.WebApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EventDetailsUpdatorController : ControllerBase
  {
    public class EventIsNotPublishedException : HttpTransferableException
    {
      private EventDetailsUpdator.EventIsNotPublishedException _ex;

      protected override object SerializeExtraData()
      {
        return new { eventCheck = this._ex.EventCheck };
      }

      public EventIsNotPublishedException(EventDetailsUpdator.EventIsNotPublishedException e)
        : base("EVENT_IS_NOT_PUBLISHED", e.Message)
      {
        this._ex = e;
      }
    }

    private DatabaseContext _context;
    private EventDetailsUpdator _updator;

    public EventDetailsUpdatorController(DatabaseContext context, EventDetailsUpdator updator)
    {
      this._context = context;
      this._updator = updator;
    }

    [HttpPost("[action]")]
    public async Task UpdateHeader([FromBody] EventDetailsUpdator.UpdateHeaderReq req)
    {
      try
      {
        await this._updator.UpdateHeader(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is EventDetailsUpdator.EventIsNotPublishedException)
        {
          throw new EventIsNotPublishedException(e as EventDetailsUpdator.EventIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateMainInfo([FromBody] EventDetailsUpdator.UpdateMainInfoReq req)
    {
      try
      {
        await this._updator.UpdateMainInfo(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is EventDetailsUpdator.EventIsNotPublishedException)
        {
          throw new EventIsNotPublishedException(e as EventDetailsUpdator.EventIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateAboutEvent([FromBody] EventDetailsUpdator.UpdateAboutEventReq req)
    {
      try
      {
        await this._updator.UpdateAboutEvent(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is EventDetailsUpdator.EventIsNotPublishedException)
        {
          throw new EventIsNotPublishedException(e as EventDetailsUpdator.EventIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task UploadLogo([FromQuery] long eventId)
    {
      var file = Request.Form.Files.FirstOrDefault();

      if (file == null || file.Length == 0)
      {
        throw new ApplicationHttpTransferableException("No file posted!");
      }

      var maxFileSize = (int)(1024 * 1024 * 5);
      if (file.Length > maxFileSize)
      {
        throw new FileIsTooBigException("5МB"); ;
      }

      if (!file.ContentType.Contains("image"))
      {
        throw new ApplicationHttpTransferableException("Invalid file type");
      }

      if (!Directory.Exists(@"wwwroot\images\event-photos"))
      {
        Directory.CreateDirectory(@"wwwroot\images\event-photos");
      }

      var ev = await this._context.Events.SingleAsync(e => e.Id == eventId);
      if (ev.LogoImageUrl != null)
      {
        var currentImageUrl = Path.Combine(new[] { "wwwroot", ev.LogoImageUrl });
        if (System.IO.File.Exists(currentImageUrl))
        {
          System.IO.File.Delete(currentImageUrl);
        }
      }

      var ext = Path.GetExtension(file.FileName);
      var url = $@"wwwroot\images\event-photos\{Guid.NewGuid().ToString()}{ext}";

      using (var f = System.IO.File.Create(url))
      using (var imgStream = file.OpenReadStream())
      {
        var processor = new ImageProcessor();
        processor.ExifRotate(imgStream, f);
      }

      url = url.Replace(@"wwwroot\", @"");

      await this._updator.UpdatePhoto(new EventDetailsUpdator.UpdatePhotoReq { EventId = eventId, PhotoUrl = url }, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task Publish([FromBody] EventDetailsUpdator.PublishReq req)
    {
      try
      {
        await this._updator.Publish(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is EventDetailsUpdator.EventIsNotPublishedException)
        {
          throw new EventIsNotPublishedException(e as EventDetailsUpdator.EventIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    public async Task Unpublish([FromBody] EventDetailsUpdator.PublishReq req)
    {
      await this._updator.Unpublish(req, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task MakePublic([FromBody] EventDetailsUpdator.PublishReq req)
    {
      await this._updator.MakePublic(req, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task MakePrivate([FromBody] EventDetailsUpdator.PublishReq req)
    {
      await this._updator.MakePrivate(req, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task Delete([FromBody] EventDetailsUpdator.PublishReq req)
    {
      await this._updator.Delete(req, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task<EventDetailsUpdator.RegisterVacancyRes> RegisterVacancy([FromBody] EventDetailsUpdator.RegisterVacancyReq req)
    {
      try
      {
        var res = await this._updator.RegistetrVacancy(req, this._context);
        await this._context.SaveChangesAsync();
        return res;
      }
      catch (Exception e)
      {
        if (e is EventDetailsUpdator.EventIsNotPublishedException)
        {
          throw new EventIsNotPublishedException(e as EventDetailsUpdator.EventIsNotPublishedException);
        }

        throw e;
      }
    }
  }
}