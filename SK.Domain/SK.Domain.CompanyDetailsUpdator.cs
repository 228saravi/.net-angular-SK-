using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class CompanyDetailsUpdator
  {
    public class UpdateHeaderReq
    {
      public long CompanyId { get; set; }
      public string Name { get; set; }
      public string CityId { get; set; }
    }

    public class UpdateAboutCompanyReq
    {
      public long CompanyId { get; set; }
      public string AboutCompanyHtml { get; set; }
    }

    public class UpdatePhotoReq
    {
      public long CompanyId { get; set; }
      public string PhotoUrl { get; set; }
    }

    public class PublishReq
    {
      public long CompanyId { get; set; }
    }

    public class RegisterEventReq
    {
      public long CompanyId { get; set; }
    }

    public class RegisterEventRes
    {
      public long EventId { get; set; }
    }

    public class CompanyCheckBeforePublish
    {
      public CompanyCheckBeforePublish(Company company)
      {
        this.NameSet = !String.IsNullOrWhiteSpace(company.Name);
        this.LogoSet = !String.IsNullOrWhiteSpace(company.LogoImageUrl);
        this.CitySet = !String.IsNullOrWhiteSpace(company.CityId);
      }

      public bool NameSet { get; private set; }
      public bool LogoSet { get; private set; }
      public bool CitySet { get; private set; }

      public bool IsReadyForPublish
      {
        get
        {
          return this.NameSet &&
            this.LogoSet &&
            this.CitySet;
        }
      }
    }

    public class CompanyIsNotPublishedException : ApplicationException
    {
      private CompanyCheckBeforePublish _companyCheck;

      public CompanyIsNotPublishedException(CompanyCheckBeforePublish companyCheck) : base("Can not publish!") {
        this._companyCheck = companyCheck;
      }

      public CompanyCheckBeforePublish CompanyCheck
      {
        get
        {
          return this._companyCheck;
        }
      }
    }

    private ICurrentUserService _currentUserService;

    public CompanyDetailsUpdator(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public void ThrowIfPublishingBroken(Company company)
    {
      var check = new CompanyCheckBeforePublish(company);
      var isOk = company.IsPublished && check.IsReadyForPublish;
      if (!isOk)
      {
        throw new CompanyIsNotPublishedException(check);
      }
    }

    public async Task UpdateHeader(UpdateHeaderReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var company = await context.Companies
        .SingleAsync(c => c.Id == req.CompanyId && c.UserId == currentUserData.Id);

      company.Name = req.Name;
      company.CityId = req.CityId;

      if (company.IsPublished)
      {
        this.ThrowIfPublishingBroken(company);
      }
    }

    public async Task UpdateAboutCompany(UpdateAboutCompanyReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var company = await context.Companies
        .SingleAsync(c => c.Id == req.CompanyId && c.UserId == currentUserData.Id);

      company.AboutCompanyHtml = req.AboutCompanyHtml;

      if (company.IsPublished)
      {
        this.ThrowIfPublishingBroken(company);
      }
      
    }

    public async Task UpdatePhoto(UpdatePhotoReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var company = await context.Companies
        .SingleAsync(c => c.Id == req.CompanyId && c.UserId == currentUserData.Id);

      company.LogoImageUrl = company.ThumbnailImageUrl = req.PhotoUrl;

      if (company.IsPublished)
      {
        this.ThrowIfPublishingBroken(company);
      }
    }

    public async Task Publish(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var company = await context.Companies
        .Include(c => c.User)
        .SingleAsync(c => c.Id == req.CompanyId && c.UserId == currentUserData.Id);

      company.IsPublished = true;
      this.ThrowIfPublishingBroken(company);
    }

    public async Task<RegisterEventRes> RegisterEvent(RegisterEventReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var company = await context.Companies
        .Include(c => c.Events)
        .SingleAsync(c => c.Id == req.CompanyId && c.UserId == currentUserData.Id);

      var newEvent = new Event() { IsPublic = true };

      company.Events.Add(newEvent);

      this.ThrowIfPublishingBroken(company);

      await context.SaveChangesAsync();

      var res = new RegisterEventRes { EventId = newEvent.Id };

      return res;
    }
  }
}
