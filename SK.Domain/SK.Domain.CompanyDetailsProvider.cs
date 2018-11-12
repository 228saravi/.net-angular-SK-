
using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class CompanyDetailsProvider
  {
    public class Req
    {
      public long CompanyId { get; set; }
      public bool WithVacancies { get; set; }
    }

    public class Res
    {
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

      public class Vacancy
      {
        public long Id { get; set; }
        public bool IsPublished { get; set; }
        public Speciality Speciality { get; set; }
        public int? RatePerHour { get; set; }
        public DateTime? StartTime { get; set; }
        public int? WorkingHours { get; set; }
        public Experience Experience { get; set; }

        public int? Amount { get; set; }
        public int ConnectedExpertsCount { get; set; }
        public int ConnectionsCount { get; set; }
        public int IncomingConnectionsCount { get; set; }
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

      public class Event
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsPublished { get; set; }
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

        public int ConnectionsCount { get; set; }
        public int IncomingConnectionsCount { get; set; }

        public Vacancy[] Vacancies { get; set; }

      }

      public class CompanyRes
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsPublished { get; set; }
        public City City { get; set; }
        public double Rating { get; set; }
        public string LogoImageUrl { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string AboutCompanyHtml { get; set; }
        public Event[] Events { get; set; }
      }

      public Res.CompanyRes Company { get; set; }
    }

    private ICurrentUserService _currentUserService;

    public CompanyDetailsProvider(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public async Task<Res> Get(Req req, DatabaseContext context)
    {
      var curerntUserData = this._currentUserService.GetCurrentUserData();

      return await context.Companies
        .Where(c => c.Id == req.CompanyId)
        .Where(c => c.IsPublished || curerntUserData != null && c.UserId == curerntUserData.Id)
        .Select(c => new Res
        {
          Company = new Res.CompanyRes
          {
            Id = c.Id,
            Name = c.Name,
            IsPublished = c.IsPublished,
            City = c.CityId != null ? new Res.City { Id = c.City.Id, Name = c.City.Name } : null,
            Rating =
                (
                  4.5 +
                  c.Events
                    .SelectMany(ee => ee.Vacancies)
                    .SelectMany(vv => vv.Connections)
                    .Where(cc => cc.FeedbackForCompany != null)
                    .Select(cc => cc.FeedbackForCompany.Rating)
                    .DefaultIfEmpty(0).Sum()
                ) /
                (
                  1 +
                  c.Events
                    .SelectMany(ee => ee.Vacancies)
                    .SelectMany(vv => vv.Connections)
                    .Where(cc => cc.FeedbackForCompany != null)
                    .Count()
                ),
            AboutCompanyHtml = c.AboutCompanyHtml,
            LogoImageUrl = c.LogoImageUrl,
            ThumbnailImageUrl = c.ThumbnailImageUrl,
            Events = c.Events
            .Where(e => !e.IsDeleted)
            .Where(e => e.IsPublished || curerntUserData != null && c.UserId == curerntUserData.Id)
            .OrderBy(e => e.StartTime == null ? 0 : 1)
            .ThenByDescending(e => e.StartTime)
            .Select(e => new Res.Event
            {
              Id = e.Id,
              Name = e.Name,
              IsPublished = e.IsPublished,
              Segment = new Res.Segment { Id = e.Segment.Id, Name = e.Segment.Name },
              Format = new Res.EventFormat { Id = e.EventFormat.Id, Name = e.EventFormat.Name },
              Type = new Res.EventType { Id = e.EventType.Id, Name = e.EventType.Name },
              City = e.CityId != null
                ? new Res.City { Id = e.City.Id, Name = e.City.Name }
                : null,
              Address = e.Address,
              StartTime = e.StartTime,
              EndTime = e.EndTime,
              LogoImageUrl = e.LogoImageUrl,
              EstimatedGuestsCount = e.EstimatedGuestsCount,
              EstimatedAverageCheck = e.EstimatedAverageCheck,
              TotalCooksCount = e.Vacancies.Where(v => !v.IsDeleted).Where(vacancy => vacancy.SpecialityId == SpecialityIds.Cook).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              TotalBarmansCount = e.Vacancies.Where(v => !v.IsDeleted).Where(vacancy => vacancy.SpecialityId == SpecialityIds.Barman).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              TotalWaitersCount = e.Vacancies.Where(v => !v.IsDeleted).Where(vacancy => vacancy.SpecialityId == SpecialityIds.Waiter).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              ConnectedCooksCount = e.Vacancies.Where(v => !v.IsDeleted).Where(vacancy => vacancy.SpecialityId == SpecialityIds.Cook).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              ConnectedBarmansCount = e.Vacancies.Where(v => !v.IsDeleted).Where(vacancy => vacancy.SpecialityId == SpecialityIds.Barman).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              ConnectedWaitersCount = e.Vacancies.Where(v => !v.IsDeleted).Where(vacancy => vacancy.SpecialityId == SpecialityIds.Waiter).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              ConnectionsCount = e.Vacancies.Where(v => !v.IsDeleted).SelectMany(vacancy => vacancy.Connections).Count(),
              IncomingConnectionsCount = e.Vacancies.Where(v => !v.IsDeleted).SelectMany(vacancy => vacancy.Connections).Count(conn => conn.ConnectionType == ConnectionTypes.ExpertToVacancy && conn.ConnectionStatus == ConnectionStatuses.Initiated),
              Vacancies = req.WithVacancies
                ? e.Vacancies
                .Where(v => !v.IsDeleted)
                .Where(v => v.IsPublished || curerntUserData != null && c.UserId == curerntUserData.Id)
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
                  Experience = v.ExperienceOptionId != null ? new Res.Experience { Id = v.ExperienceOption.Id, Name = v.ExperienceOption.Name } : null,
                  Amount = v.Amount,
                  ConnectedExpertsCount = v.Connections.Where(conn => conn.ConnectionStatus == ConnectionStatuses.Connected).Count(),
                  ConnectionsCount = v.Connections.Count(),
                  IncomingConnectionsCount = v.Connections.Count(conn => conn.ConnectionType == ConnectionTypes.ExpertToVacancy && conn.ConnectionStatus == ConnectionStatuses.Initiated),
                }).ToArray()
                : null
            }).ToArray(),
          }
        }).SingleAsync();
    }
  }
}
