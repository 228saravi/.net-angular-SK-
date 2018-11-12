using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class EventDetailsUpdator
  {
    public class UpdateHeaderReq
    {
      public long EventId { get; set; }
      public string Name { get; set; }
      public string TypeId { get; set; }
      public string FormatId { get; set; }
    }

    public class UpdateMainInfoReq
    {
      public long EventId { get; set; }
      public DateTime? StartTime { get; set; }
      public DateTime? EndTime { get; set; }
      public string SegmentId { get; set; }
      public string CityId { get; set; }
      public string Address { get; set; }
      public int? EstimatedGuestsCount { get; set; }
      public int? EstimatedAverageCheck { get; set; }
      public bool WithDelivery { get; set; }
      public bool WithAccomodation { get; set; }
    }

    public class UpdateAboutEventReq
    {
      public long EventId { get; set; }
      public string AboutEventHtml { get; set; }
    }

    public class UpdatePhotoReq
    {
      public long EventId { get; set; }
      public string PhotoUrl { get; set; }
    }

    public class PublishReq
    {
      public long EventId { get; set; }
    }

    public class RegisterVacancyReq
    {
      public long EventId { get; set; }
    }

    public class RegisterVacancyRes
    {
      public long VacancyId { get; set; }
    }

    public class EventCheckBeforePublish
    {
      public EventCheckBeforePublish(Event e)
      {
        this.NameSet = !String.IsNullOrWhiteSpace(e.Name);
        this.LogoSet = e.LogoImageUrl != null;
        this.TypeSet = e.EventTypeId != null;
        this.FormatSet = e.EventFormatId != null;
        this.StartTimeSet = e.StartTime != null;
        this.EndTimeSet = e.EndTime != null;
        this.SegmentSet = e.SegmentId != null;
        this.CitySet = e.CityId != null;
        this.AddrerssSet = e.Address != null;
      }

      public bool NameSet { get; private set; }
      public bool LogoSet { get; private set; }
      public bool TypeSet { get; private set; }
      public bool FormatSet { get; private set; }
      public bool StartTimeSet { get; private set; }
      public bool EndTimeSet { get; private set; }
      public bool SegmentSet { get; private set; }
      public bool CitySet { get; private set; }
      public bool AddrerssSet { get; private set; }

      public bool IsReadyForPublish
      {
        get
        {
          return this.NameSet &&
            this.LogoSet &&
            this.TypeSet &&
            this.FormatSet &&
            this.StartTimeSet &&
            this.EndTimeSet &&
            this.SegmentSet &&
            this.CitySet &&
            this.AddrerssSet;
        }
      }
    }

    public class EventIsNotPublishedException : ApplicationException
    {
      private EventCheckBeforePublish _eventCheck;

      public EventIsNotPublishedException(EventCheckBeforePublish eventCheck) : base("Can not publish!")
      {
        this._eventCheck = eventCheck;
      }

      public EventCheckBeforePublish EventCheck
      {
        get
        {
          return this._eventCheck;
        }
      }
    }

    private ICurrentUserService _currentUserService;

    public EventDetailsUpdator(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public void ThrowIfPublishingBroken(Event e)
    {
      var check = new EventCheckBeforePublish(e);
      var isOk = e.IsPublished && check.IsReadyForPublish;
      if (!isOk)
      {
        throw new EventIsNotPublishedException(check);
      }
    }

    public async Task UpdateHeader(UpdateHeaderReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      e.Name = req.Name;
      e.EventTypeId = req.TypeId;
      e.EventFormatId = req.FormatId;

      if (e.IsPublished)
      {
        this.ThrowIfPublishingBroken(e);
      }
      
    }

    public async Task UpdateMainInfo(UpdateMainInfoReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .Include(ee => ee.Vacancies)
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      var startTimeDistance = req.StartTime - e.StartTime;

      foreach (var vac in e.Vacancies)
      {
        vac.StartTime += startTimeDistance;
      }

      e.StartTime = req.StartTime;
      e.EndTime = req.EndTime;
      e.SegmentId = req.SegmentId;
      e.CityId = req.CityId;
      e.Address = req.Address;
      e.EstimatedGuestsCount = req.EstimatedGuestsCount;
      e.EstimatedAverageCheck = req.EstimatedAverageCheck;
      e.WithtDelivery = req.WithDelivery;
      e.WithAccomodation = req.WithAccomodation;

      if (e.IsPublished)
      {
        this.ThrowIfPublishingBroken(e);
      }
    }

    public async Task UpdateAboutEvent(UpdateAboutEventReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      e.AboutEventHtml = req.AboutEventHtml;

      if (e.IsPublished)
      {
        this.ThrowIfPublishingBroken(e);
      }
    }

    public async Task UpdatePhoto(UpdatePhotoReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      e.LogoImageUrl = req.PhotoUrl;

      if (e.IsPublished)
      {
        this.ThrowIfPublishingBroken(e);
      }
    }

    public async Task Publish(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      e.IsPublished = true;
      this.ThrowIfPublishingBroken(e);
    }

    public async Task Unpublish(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      e.IsPublished = false;
    }

    public async Task MakePublic(PublishReq req, DatabaseContext context)
    {
        var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      e.IsPublic = true;
    }

    public async Task MakePrivate(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      e.IsPublic = false;
    }

    public async Task Delete(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .Include(ee => ee.Vacancies)
        .ThenInclude(v => v.Connections)
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      e.IsDeleted = true;

      foreach (var v in e.Vacancies)
      {
        v.IsDeleted = true;

        foreach (var c in v.Connections)
        {
          c.ConnectionStatus = ConnectionStatuses.Canceled;
        }
      }
    }

    public async Task<RegisterVacancyRes> RegistetrVacancy(RegisterVacancyReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var e = await context.Events
        .Include(ee => ee.Vacancies)
        .SingleAsync(ee => ee.Id == req.EventId && ee.Company.UserId == currentUserData.Id);

      var newVacancy = new Vacancy() { IsPublic = true };

      e.Vacancies.Add(newVacancy);

      this.ThrowIfPublishingBroken(e);

      await context.SaveChangesAsync();

      var res = new RegisterVacancyRes { VacancyId = newVacancy.Id };

      return res;
    }
  }
}
