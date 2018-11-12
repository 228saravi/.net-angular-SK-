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
  public class FileIsTooBigException : HttpTransferableException
  {
    public FileIsTooBigException(string maxFileSizeAsString)
      : base("FILE_IS_TOO_BIG", $@"Файл слишком большой. Макс размер {maxFileSizeAsString}.")
    {
    }
  }


  [Route("api/[controller]")]
  [ApiController]
  public class CompanyDetailsUpdatorController : ControllerBase
  {
    public class CompanyIsNotPublishedException : HttpTransferableException
    {
      private CompanyDetailsUpdator.CompanyIsNotPublishedException _ex;

      protected override object SerializeExtraData()
      {
        return new { companyCheck = this._ex.CompanyCheck };
      }

      public CompanyIsNotPublishedException(Domain.CompanyDetailsUpdator.CompanyIsNotPublishedException e)
        : base("COMPANY_IS_NOT_PUBLISHED", e.Message)
      {
        this._ex = e;
      }
    }

    private DatabaseContext _context;
    private CompanyDetailsUpdator _updator;

    public CompanyDetailsUpdatorController(DatabaseContext context, CompanyDetailsUpdator updator)
    {
      this._context = context;
      this._updator = updator;
    }

    [HttpPost("[action]")]
    public async Task UpdateHeader([FromBody] CompanyDetailsUpdator.UpdateHeaderReq req)
    {
      try
      {
        await this._updator.UpdateHeader(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.CompanyDetailsUpdator.CompanyIsNotPublishedException)
        {
          throw new CompanyIsNotPublishedException(e as Domain.CompanyDetailsUpdator.CompanyIsNotPublishedException);
        }

        throw;
      }
    }

    [HttpPost("[action]")]
    public async Task UpdateAboutCompany([FromBody] CompanyDetailsUpdator.UpdateAboutCompanyReq req)
    {
      await this._updator.UpdateAboutCompany(req, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task UploadLogo([FromQuery] long companyId)
    {
      var file = Request.Form.Files.FirstOrDefault();

      if (file == null || file.Length == 0)
      {
        throw new ApplicationHttpTransferableException("No file posted!");
      }

      var maxFileSize = (int)(1024 * 1024 * 5);
      if (file.Length > maxFileSize)
      {
        throw new FileIsTooBigException("5МB");;
      }

      if (!file.ContentType.Contains("image"))
      {
        throw new ApplicationHttpTransferableException("Invalid file type");
      }

      if (!Directory.Exists(@"wwwroot\images\company-photos"))
      {
        Directory.CreateDirectory(@"wwwroot\images\company-photos");
      }

      var comp = await this._context.Companies.SingleAsync(c => c.Id == companyId);
      if (comp.LogoImageUrl != null)
      {
        var currentImageUrl = Path.Combine(new[] { "wwwroot", comp.LogoImageUrl });
        if (System.IO.File.Exists(currentImageUrl))
        {
          System.IO.File.Delete(currentImageUrl);
        }
      }

      var ext = Path.GetExtension(file.FileName);
      var url = $@"wwwroot\images\company-photos\{Guid.NewGuid().ToString()}{ext}";

      using (var f = System.IO.File.Create(url))
      using (var imgStream = file.OpenReadStream())
      {
        var processor = new ImageProcessor();
        processor.ExifRotate(imgStream, f);
      }

      url = url.Replace(@"wwwroot\", @"");

      await this._updator.UpdatePhoto(new CompanyDetailsUpdator.UpdatePhotoReq { CompanyId = companyId, PhotoUrl = url }, this._context);
      await this._context.SaveChangesAsync();
    }

    [HttpPost("[action]")]
    public async Task Publish([FromBody] CompanyDetailsUpdator.PublishReq req)
    {
      try
      {
        await this._updator.Publish(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        if (e is Domain.CompanyDetailsUpdator.CompanyIsNotPublishedException)
        {
          throw new CompanyIsNotPublishedException(e as Domain.CompanyDetailsUpdator.CompanyIsNotPublishedException);
        }

        throw;
      }
    }

    [HttpPost("[action]")]
    public async Task<CompanyDetailsUpdator.RegisterEventRes> RegisterEvent([FromBody] CompanyDetailsUpdator.RegisterEventReq req)
    {
      try
      {
        var res = await this._updator.RegisterEvent(req, this._context);
        await this._context.SaveChangesAsync();
        return res;
      }
      catch (Exception e)
      {
        if (e is Domain.CompanyDetailsUpdator.CompanyIsNotPublishedException)
        {
          throw new CompanyIsNotPublishedException(e as Domain.CompanyDetailsUpdator.CompanyIsNotPublishedException);
        }

        throw;
      }
    }
  }
}