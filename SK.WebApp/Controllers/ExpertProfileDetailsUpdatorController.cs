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
using Microsoft.EntityFrameworkCore;
using SK.Infrastructure;

namespace SK.WebApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ExpertProfileDetailsUpdatorController : ControllerBase
  {
    public class ProfileIsNotPublishedException : HttpTransferableException
    {
      private ExpertProfileDetailsUpdator.ProfileIsNotPublishedException _ex;

      protected override object SerializeExtraData()
      {
        return new { profileCheck = this._ex.ProfileCheck };
      }

      public ProfileIsNotPublishedException(Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException e)
        : base("PROFILE_IS_NOT_PUBLISHED", e.Message) {
        this._ex = e;
      }
    }

    private DatabaseContext _context;
    private ExpertProfileDetailsUpdator _updator;

    public ExpertProfileDetailsUpdatorController(DatabaseContext context, ExpertProfileDetailsUpdator updator)
    {
      this._context = context;
      this._updator = updator;
    }

    [HttpPost("[action]")]
    public async Task UpdateHeader([FromBody] ExpertProfileDetailsUpdator.UpdateHeaderReq req)
    {
      try
      {
        await this._updator.UpdateHeader(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException)
        {
          throw new ProfileIsNotPublishedException(e as Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException);
        }

        throw;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateMainInfo([FromBody] ExpertProfileDetailsUpdator.UpdateMainInfoReq req)
    {
      try
      {
        await this._updator.UpdateMainInfo(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException)
        {
          throw new ProfileIsNotPublishedException(e as Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException);
        }

        throw;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateExtraInfo([FromBody] ExpertProfileDetailsUpdator.UpdateExtraInfoReq req)
    {
      try
      {
        await this._updator.UpdateExtraInfo(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException)
        {
          throw new ProfileIsNotPublishedException(e as Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException);
        }

        throw;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateAboutMe([FromBody] ExpertProfileDetailsUpdator.UpdateAboutMeReq req)
    {
      try
      {
        await this._updator.UpdateAboutMe(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException)
        {
          throw new ProfileIsNotPublishedException(e as Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException);
        }

        throw;
      }
    }

    [HttpPost("[action]")]
    public async Task UploadPhoto([FromQuery] long expertProfileId)
    {
      var file = Request.Form.Files.FirstOrDefault();

      if (file == null)
      {
        return;
      }

      var maxFileSize = (int)(1024 * 1024 * 5);
      if (file.Length > maxFileSize)
      {
        throw new FileIsTooBigException("5МB"); ;
      }

      if (!Directory.Exists(@"wwwroot\images\expert-photos"))
      {
        Directory.CreateDirectory(@"wwwroot\images\expert-photos");
      }

      var prof = await this._context.ExpertProfiles.SingleAsync(p => p.Id == expertProfileId);
      if (prof.PhotoImageUrl != null)
      {
        var currentImageUrl = Path.Combine(new[] { "wwwroot", prof.PhotoImageUrl });
        if (System.IO.File.Exists(currentImageUrl))
        {
          System.IO.File.Delete(currentImageUrl);
        }
      }

      var ext = Path.GetExtension(file.FileName);
      var url = $@"wwwroot\images\expert-photos\{Guid.NewGuid().ToString()}{ext}";

      using (var f = System.IO.File.Create(url))
      using (var imgStream = file.OpenReadStream())
      {
        var processor = new ImageProcessor();
        processor.ExifRotate(imgStream, f);
      }

      url = url.Replace(@"wwwroot\", @"");

      await this._updator.UpdatePhoto(new ExpertProfileDetailsUpdator.UpdatePhotoReq { ExpertProfileId = expertProfileId, PhotoUrl = url }, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task Publish([FromBody] ExpertProfileDetailsUpdator.PublishReq req)
    {
      try
      {
        await this._updator.Publish(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException)
        {
          throw new ProfileIsNotPublishedException(e as Domain.ExpertProfileDetailsUpdator.ProfileIsNotPublishedException);
        }

        throw;
      }
    }

    [HttpPost("[action]")]
    public async Task Unpublish([FromBody] ExpertProfileDetailsUpdator.PublishReq req)
    {
      try
      {
        await this._updator.Unpublish(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw;
      }
    }
  }
}