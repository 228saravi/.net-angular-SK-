using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class VacancyDetailsProvider
  {
    public class Req
    {
      public long VacancyId { get; set; }
    }

    public class Res
    {
      public class Language
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class City
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Experience
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Document
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Segment
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class EventType
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class EventFormat
      {
        public string Id { get; set; }
        public string Name { get; set; }
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

      public class Company
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public double Rating { get; set; }
      }

      public class Event
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public Segment Segment { get; set; }
        public EventFormat Format { get; set; }
        public EventType Type { get; set; }
        public City City { get; set; }
        public string Address { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string LogoImageUrl { get; set; }
        public int? EstimatedGuestsCount { get; set; }
        public int? EstimatedAverageCheck { get; set; }

        public int? TotalCooksCount { get; set; }
        public int? TotalBarmansCount { get; set; }
        public int? TotalWaitersCount { get; set; }

        public int? ConnectedCooksCount { get; set; }
        public int? ConnectedBarmansCount { get; set; }
        public int? ConnectedWaitersCount { get; set; }

        public Company Company { get; set; }
      }

      public class Connection
      {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
      }

      public class Vacancy
      {
        public long Id { get; set; }
        public bool IsPublished { get; set; }
        public bool IsPublic { get; set; }
        public Event Event { get; set; }
        public Speciality Speciality { get; set; }
        public Language[] Languages { get; set; }
        public Document[] Documents { get; set; }
        public Experience Experience { get; set; }
        public DateTime? StartTime { get; set; }
        public int? WorkingHours { get; set; }
        public int? RatePerHour { get; set; }
        public double? Rating { get; set; }

        public int? Amount { get; set; }
        public string AboutVacancyHtml { get; set; }

        public Connection Connection { get; set; }
      }

      public Vacancy FoundVacancy { get; set; }
    }

    private ICurrentUserService _currentUserService;

    public VacancyDetailsProvider(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public async Task<Res> Get(Req req, DatabaseContext database)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vacancies = database.Vacancies.AsQueryable();

      vacancies = vacancies.Where(v => v.Id == req.VacancyId && !v.IsDeleted);

      vacancies = vacancies.Where(v => v.IsPublished && v.Event.IsPublished && v.Event.Company.IsPublished || currentUserData != null && v.Event.CompanyId == currentUserData.CompanyId);

      return new Res
      {
        FoundVacancy = await vacancies
        .Select(v =>
          new Res.Vacancy
          {
            Id = v.Id,
            IsPublished = v.IsPublished,
            IsPublic = v.IsPublic,
            Languages = v.VacancyLanguages.Select(vl => new Res.Language { Id = vl.Language.Id, Name = vl.Language.Name }).ToArray(),
            Documents = v.VacancyDocuments.Select(vd => new Res.Document { Id = vd.ExpertDocument.Id, Name = vd.ExpertDocument.Name }).ToArray(),
            Experience = v.ExperienceOptionId != null
              ? new Res.Experience { Id = v.ExperienceOption.Id, Name = v.ExperienceOption.Name }
              : null,
            StartTime = v.StartTime,
            WorkingHours = v.WorkingHours,
            RatePerHour = v.RatePerHour,
            Amount = v.Amount,
            AboutVacancyHtml = v.AboutVacancyHtml,
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
            Event = new Res.Event
            {
              Id = v.Event.Id,
              Name = v.Event.Name,
              Segment = new Res.Segment { Id = v.Event.Segment.Id, Name = v.Event.Segment.Name },
              Format = new Res.EventFormat { Id = v.Event.EventFormat.Id, Name = v.Event.EventFormat.Name },
              Type = new Res.EventType { Id = v.Event.EventType.Id, Name = v.Event.EventType.Name },
              City = v.Event.CityId != null
                ? new Res.City { Id = v.Event.City.Id, Name = v.Event.City.Name }
                : null,
              Address = v.Event.Address,
              StartTime = v.Event.StartTime,
              EndTime = v.Event.EndTime,
              LogoImageUrl = v.Event.LogoImageUrl,
              EstimatedGuestsCount = v.Event.EstimatedGuestsCount,
              EstimatedAverageCheck = v.Event.EstimatedAverageCheck,
              TotalCooksCount = v.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Cook).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              TotalBarmansCount = v.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Barman).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              TotalWaitersCount = v.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Waiter).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              ConnectedCooksCount = v.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Cook).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              ConnectedBarmansCount = v.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Barman).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              ConnectedWaitersCount = v.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Waiter).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              Company = new Res.Company
              {
                Id = v.Event.Company.Id,
                Name = v.Event.Company.Name,
                ThumbnailImageUrl = v.Event.Company.ThumbnailImageUrl,
                Rating =
                (
                  4.5 +
                  v.Event.Company.Events
                    .SelectMany(e => e.Vacancies)
                    .SelectMany(vv => vv.Connections)
                    .Where(cc => cc.FeedbackForCompany != null)
                    .Select(cc => cc.FeedbackForCompany.Rating)
                    .DefaultIfEmpty(0).Sum()
                ) /
                (
                  1 +
                  v.Event.Company.Events
                    .SelectMany(e => e.Vacancies)
                    .SelectMany(vv => vv.Connections)
                    .Where(cc => cc.FeedbackForCompany != null)
                    .Count()
                ),
              },
            },
            Connection = v.Connections
              .Where(c => c.ConnectionStatus != ConnectionStatuses.Canceled)
              .Where(c => c.ExpertProfile.UserId == currentUserData.Id).Select(c => new Res.Connection { Id = c.Id, Type = c.ConnectionType, Status = c.ConnectionStatus }).FirstOrDefault(),
          }
        ).SingleOrDefaultAsync()
      };
    }
  }
}

