using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class EventDetailsProvider
  {
    public class Req
    {
      public long EventId { get; set; }
      public bool? WithNotFullVacancies { get; set; }
      public int? WithConnectionsForExpert { get; set; }
    }

    public class Res
    {
      public class Segment
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Type
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Format
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Company
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public string LogoImageUrl { get; set; }
        public double Rating { get; set; }
      }

      public class Skill
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class SkillsGroup
      {
        public string Name { get; set; }
        public Skill[] Skills { get; set; }
      }

      public class Specialization
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Speciality
      {
        public string Id { get; set; }
        public string Name { get; set; }
        public Specialization Specialization { get; set; }
        public SkillsGroup[] SkillsGroups { get; set; }
      }

      public class Connection
      {
        public long Id { get; set; }
      }

      public class Experience
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Vacancy
      {
        public long Id { get; set; }
        public bool IsPublished { get; set; }
        public Speciality Speciality { get; set; }
        public int? RatePerHour { get; set; }
        public DateTime? StartTime { get; set; }
        public int? WorkingHours { get; set; }
        public Experience Experience { get; set; }
        public Connection Connection { get; set; }
      }

      public class City
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class EventRes
      {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsPublic { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }

        public City City { get; set; }

        public string Address { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Res.Segment Segment { get; set; }
        public Res.Type Type { get; set; }
        public Res.Format Format { get; set; }

        public string LogoImageUrl { get; set; }
        public string DescriptionHtml { get; set; }

        public int? EstimatedGuestsCount { get; set; }
        public int? EstimatedAverageCheck { get; set; }

        public bool WithtDelivery { get; set; }
        public bool WithAccomodation { get; set; }

        public int TotalCooksCount { get; set; }
        public int TotalBarmansCount { get; set; }
        public int TotalWaitersCount { get; set; }

        public Company Company { get; set; }
        public Vacancy[] Vacancies { get; set; }
      }

      public EventRes Event { get; set; }
    }

    private ICurrentUserService _currentUserService;

    public EventDetailsProvider(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public async Task<Res> Get(Req req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      return await context.Events
        .Where(e => e.Id == req.EventId)
        .Where(e => !e.IsDeleted)
        .Where(e => e.IsPublished && e.Company.IsPublished || currentUserData != null && e.CompanyId == currentUserData.CompanyId)
        .Select(e => new Res
        {
          Event = new Res.EventRes
          {
            Id = e.Id,
            Name = e.Name,
            IsPublic = e.IsPublic,
            IsPublished = e.IsPublished,
            City = e.CityId != null
              ? new Res.City
              {
                Id = e.City.Id,
                Name = e.City.Name
              }
              : null,
            Address = e.Address,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Segment = e.Segment != null ? new Res.Segment { Id = e.Segment.Id, Name = e.Segment.Name } : null,
            Type = e.EventType != null ? new Res.Type { Id = e.EventType.Id, Name = e.EventType.Name } : null,
            Format = e.EventFormat != null ? new Res.Format { Id = e.EventFormat.Id, Name = e.EventFormat.Name } : null,
            LogoImageUrl = e.LogoImageUrl,
            DescriptionHtml = e.AboutEventHtml,
            EstimatedGuestsCount = e.EstimatedGuestsCount,
            EstimatedAverageCheck = e.EstimatedAverageCheck,
            WithtDelivery = e.WithtDelivery,
            WithAccomodation = e.WithAccomodation,
            TotalCooksCount = e.Vacancies.Where(v => v.SpecialityId == SpecialityIds.Cook).Count(),
            TotalBarmansCount = e.Vacancies.Where(v => v.SpecialityId == SpecialityIds.Barman).Count(),
            TotalWaitersCount = e.Vacancies.Where(v => v.SpecialityId == SpecialityIds.Waiter).Count(),
            Company = new Res.Company
            {
              Id = e.Company.Id,
              Name = e.Company.Name,
              LogoImageUrl = e.Company.LogoImageUrl,
              Rating =
                (
                  4.5 +
                  e.Company.Events
                    .SelectMany(ee => ee.Vacancies)
                    .SelectMany(vv => vv.Connections)
                    .Where(cc => cc.FeedbackForCompany != null)
                    .Select(cc => cc.FeedbackForCompany.Rating)
                    .DefaultIfEmpty(0).Sum()
                ) /
                (
                  1 +
                  e.Company.Events
                    .SelectMany(ee => ee.Vacancies)
                    .SelectMany(vv => vv.Connections)
                    .Where(cc => cc.FeedbackForCompany != null)
                    .Count()
                ),
            },
            Vacancies = e.Vacancies
              .Where(v => !v.IsDeleted)
              .Where(v => v.IsPublished || v.Event.CompanyId == currentUserData.CompanyId)
              .Where(v => req.WithNotFullVacancies != true || v.Connections.Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected) < v.Amount)
              .Select(v => new Res.Vacancy
              {
                Id = v.Id,
                IsPublished = v.IsPublished,
                Speciality = v.Speciality != null
                  ? new Res.Speciality
                  {
                    Id = v.Speciality.Id,
                    Name = v.Speciality.Name,
                    Specialization = v.Specialization != null
                      ? new Res.Specialization { Id = v.Specialization.Id, Name = v.Specialization.Name }
                      : null,
                    SkillsGroups = v.VacancySkills.Select(vc => vc.Skill).Where(s => s.SpecialityId == v.SpecialityId).GroupBy(s => s.GroupName)
                    .Select(g =>
                      new Res.SkillsGroup
                      {
                        Name = g.Key,
                        Skills = g.Select(s => new Res.Skill { Id = s.Id, Name = s.Name }).ToArray(),
                      }
                    ).ToArray()
                  }
                  : null,
                RatePerHour = v.RatePerHour,
                StartTime = v.StartTime,
                WorkingHours = v.WorkingHours,
                Experience = v.ExperienceOptionId != null
                  ? new Res.Experience { Id = v.ExperienceOption.Id, Name = v.ExperienceOption.Name }
                  : null,
                Connection = req.WithConnectionsForExpert != null
                  ? v.Connections
                    .Where(c => c.ConnectionStatus != ConnectionStatuses.Canceled)
                    .Where(c => c.ExpertProfileId == req.WithConnectionsForExpert)
                    .Select(c => new Res.Connection { Id = c.Id }).SingleOrDefault()
                  : v.Connections
                    .Where(c => c.ConnectionStatus != ConnectionStatuses.Canceled)
                    .Where(c => c.ExpertProfile.UserId == currentUserData.Id)
                    .Select(c => new Res.Connection { Id = c.Id }).SingleOrDefault()
              }).ToArray(),
          }
        }).SingleAsync();
    }
  }
}
