using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace SK.Domain
{
  public class ConnectionsManager
  {
    public class VacancyConnectionsReq
    {
      public long VacancyId { get; set; }
    }

    public class ExpertConnectionsReq
    {
      public long ExpertProfileId { get; set; }
    }

    public class VacancyConnectionsRes
    {
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

      public class Expert
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhotoImageUrl { get; set; }
        public City City { get; set; }
        public double Rating { get; set; }
        public int? RatePerHour { get; set; }
        public Speciality Speciality { get; set; }
        public Experience Experience { get; set; }
      }

      public class Feedback
      {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string CommentHtml { get; set; }
      }

      public class Connection
      {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public Expert Expert { get; set; }
        public Feedback FeedbackForExpert { get; set; }
        public Feedback FeedbackForCompany { get; set; }
      }

      public Connection[] Connections { get; set; }
    }

    public class ExpertConnectionsRes
    {
      public class City
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
        public string LogoImageUrl { get; set; }
        public Segment Segment { get; set; }
        public EventFormat Format { get; set; }
        public EventType Type { get; set; }
        public City City { get; set; }
        public string Address { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? EstimatedGuestsCount { get; set; }
        public int? EstimatedAverageCheck { get; set; }

        public int? TotalCooksCount { get; set; }
        public int? TotalBarmansCount { get; set; }
        public int? TotalWaitersCount { get; set; }

        public int ConnectedCooksCount { get; set; }
        public int ConnectedBarmansCount { get; set; }
        public int ConnectedWaitersCount { get; set; }

        public Company Company { get; set; }
      }

      public class Experience
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Language
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
        public Event Event { get; set; }
        public Speciality Speciality { get; set; }
        public ExpertConnectionsRes.Language[] Languages { get; set; }
        public Experience Experience { get; set; }
        public DateTime? StartTime { get; set; }
        public int? WorkingHours { get; set; }
        public int? RatePerHour { get; set; }
        public double? Rating { get; set; }
      }

      public class Feedback
      {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string CommentHtml { get; set; }
      }

      public class Connection
      {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public Vacancy Vacancy { get; set; }
        public Feedback FeedbackForExpert { get; set; }
        public Feedback FeedbackForCompany { get; set; }
      }

      public Connection[] Connections { get; set; }
    }

    public class RegisterExpertToVacancyConnectionReq
    {
      public long ExpertProfileId { get; set; }
      public long VacancyId { get; set; }
    }

    public class RegisterVacancyToExpertConnectionReq
    {
      public long VacancyId { get; set; }
      public long ExpertProfileId { get; set; }
    }

    public class CancelConnectionReq
    {
      public long ConnectionId { get; set; }
    }

    public class ApproveConnectionReq
    {
      public long ConnectionId { get; set; }
    }

    public class PostFeedbackReq
    {
      public long ConnectionId { get; set; }
      public int Rating { get; set; }
      public string CommentHtml { get; set; }
    }

    public class AmooutIsFullException : ApplicationException
    {
      public AmooutIsFullException() : base("Amount is full!") { }
    }

    public class AlreadyConnectedException : ApplicationException
    {
      public AlreadyConnectedException() : base("Already connected!") { }
    }

    public class NotYourProfileException : ApplicationException
    {
      public NotYourProfileException() : base("You can update your profile only!") { }
    }

    public class NotYourCompanyException : ApplicationException
    {
      public NotYourCompanyException() : base("You can update your company only!") { }
    }

    public class CompanyIsNotPublishedException : ApplicationException
    {
      public CompanyIsNotPublishedException() : base("Company is not published!") { }
    }

    public class NotYourConnectionException : ApplicationException
    {
      public NotYourConnectionException() : base("Not your connection!") { }
    }

    public class TooLateToCancelConnectionException : ApplicationException
    {
      public TooLateToCancelConnectionException() : base("Too late to cancel connection!") { }
    }

    private ICurrentUserService _currentUserService;
    private IEmailSender _emailSender;
    private CompanyDetailsUpdator _companyDetailsUpdator;
    private ExpertProfileDetailsUpdator _expertProfileDetailsUpdator;

    private string GenerateEmailBody(Connection connection)
    {
      var sb = new StringBuilder();
      sb.Append($"<div><strong>Эксперт</strong></div>");
      sb.Append($"<div>Id профиля: {connection.ExpertProfile.Id}</div>");
      sb.Append($"<div>Имя: {connection.ExpertProfile.User.DisplayName}</div>");
      sb.Append($"<div>Емейл: {connection.ExpertProfile.User.Email}</div>");

      sb.Append($"<div><strong>Вакансия</strong></div>");
      sb.Append($"<div>Id вакансии: {connection.Vacancy.Id}</div>");
      sb.Append($"<div>Специальность: {connection.Vacancy.Speciality?.Name}</div>");
      sb.Append($"<div>Специализация: {connection.Vacancy.Specialization?.Name}</div>");
      sb.Append($"<div>Компания: {connection.Vacancy.Event.Company.Name}</div>");

      return sb.ToString();
    }

    public ConnectionsManager(ICurrentUserService currentUserService, IEmailSender emailSender, CompanyDetailsUpdator companyDetailsUpdator, ExpertProfileDetailsUpdator expertProfileDetailsUpdator)
    {
      this._currentUserService = currentUserService;
      this._emailSender = emailSender;
      this._companyDetailsUpdator = companyDetailsUpdator;
      this._expertProfileDetailsUpdator = expertProfileDetailsUpdator;
    }

    public async Task<ExpertConnectionsRes> GetExpertConnections(ExpertConnectionsReq req, DatabaseContext context)
    {

      var currentUserData = this._currentUserService.GetCurrentUserData();

      if (req.ExpertProfileId != currentUserData.ExpertProfileId)
      {
        throw new NotYourProfileException();
      }

      var connections = await context.Connections
        .Where(c => c.ConnectionStatus != ConnectionStatuses.Canceled)
        .Where(c => c.ExpertProfileId == req.ExpertProfileId)
        .OrderBy(c => c.Vacancy.StartTime)
        .Select(c => new ExpertConnectionsRes.Connection
        {
          Id = c.Id,
          Type = c.ConnectionType,
          Status = c.ConnectionStatus,
          Vacancy = new ExpertConnectionsRes.Vacancy
          {
            Id = c.Vacancy.Id,
            Languages = c.Vacancy.VacancyLanguages.Select(vl => new ExpertConnectionsRes.Language { Id = vl.Language.Id, Name = vl.Language.Name }).ToArray(),
            Event = new ExpertConnectionsRes.Event
            {
              Id = c.Vacancy.Event.Id,
              Name = c.Vacancy.Event.Name,
              LogoImageUrl = c.Vacancy.Event.LogoImageUrl,
              Segment = c.Vacancy.Event.SegmentId != null
                ? new ExpertConnectionsRes.Segment { Id = c.Vacancy.Event.Segment.Id, Name = c.Vacancy.Event.Segment.Name }
                : null,
              Format = c.Vacancy.Event.EventFormatId != null
                ? new ExpertConnectionsRes.EventFormat { Id = c.Vacancy.Event.EventFormat.Id, Name = c.Vacancy.Event.EventFormat.Name }
                : null,
              Type = c.Vacancy.Event.EventTypeId != null
                ? new ExpertConnectionsRes.EventType { Id = c.Vacancy.Event.EventType.Id, Name = c.Vacancy.Event.EventType.Name }
                : null,
              City = c.Vacancy.Event.CityId != null
                ? new ExpertConnectionsRes.City { Id = c.Vacancy.Event.City.Id, Name = c.Vacancy.Event.City.Name }
                : null,
              Address = c.Vacancy.Event.Address,
              StartTime = c.Vacancy.Event.StartTime,
              EndTime = c.Vacancy.Event.EndTime,
              EstimatedGuestsCount = c.Vacancy.Event.EstimatedGuestsCount,
              EstimatedAverageCheck = c.Vacancy.Event.EstimatedAverageCheck,
              TotalCooksCount = c.Vacancy.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Cook).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              TotalBarmansCount = c.Vacancy.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Barman).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              TotalWaitersCount = c.Vacancy.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Waiter).Select(vv => vv.Amount ?? 0).DefaultIfEmpty(0).Sum(),
              ConnectedCooksCount = c.Vacancy.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Cook).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              ConnectedBarmansCount = c.Vacancy.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Barman).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              ConnectedWaitersCount = c.Vacancy.Event.Vacancies.Where(vacancy => vacancy.SpecialityId == SpecialityIds.Waiter).SelectMany(vv => vv.Connections).Count(conn => conn.ConnectionStatus == ConnectionStatuses.Connected),
              Company = new ExpertConnectionsRes.Company
              {
                Id = c.Vacancy.Event.Company.Id,
                Name = c.Vacancy.Event.Company.Name,
                ThumbnailImageUrl = c.Vacancy.Event.Company.ThumbnailImageUrl,
                Rating =
                (
                  4.5 +
                  c.Vacancy.Event.Company.Events
                    .SelectMany(ee => ee.Vacancies)
                    .SelectMany(vv => vv.Connections)
                    .Where(cc => cc.FeedbackForCompany != null)
                    .Select(cc => cc.FeedbackForCompany.Rating)
                    .DefaultIfEmpty(0).Sum()
                ) /
                (
                  1 +
                  c.Vacancy.Event.Company.Events
                    .SelectMany(ee => ee.Vacancies)
                    .SelectMany(vv => vv.Connections)
                    .Where(cc => cc.FeedbackForCompany != null)
                    .Count()
                ),
              },
            },

            Speciality = c.Vacancy.Speciality != null
              ? new ExpertConnectionsRes.Speciality
              {
                Id = c.Vacancy.Speciality.Id,
                Name = c.Vacancy.Speciality.Name,
                Specialization = c.Vacancy.SpecializationId != null
                  ? new ExpertConnectionsRes.Specialization
                  {
                    Id = c.Vacancy.Specialization.Id,
                    Name = c.Vacancy.Specialization.Name
                  }
                  : null,
                SkillsGroups = c.Vacancy.VacancySkills.Select(vc => vc.Skill).Where(s => s.SpecialityId == c.Vacancy.SpecialityId).GroupBy(s => s.GroupName)
                .Select(g =>
                  new ExpertConnectionsRes.SkillsGroup
                  {
                    Name = g.Key,
                    Skills = g.Select(s => new ExpertConnectionsRes.Skill { Id = s.Id, Name = s.Name }).ToArray(),
                  }
                ).ToArray()
              }
              : null,
            Experience = c.Vacancy.ExperienceOptionId != null
              ? new ExpertConnectionsRes.Experience { Id = c.Vacancy.ExperienceOption.Id, Name = c.Vacancy.ExperienceOption.Name }
              : null,
            StartTime = c.Vacancy.StartTime,
            WorkingHours = c.Vacancy.WorkingHours,
            RatePerHour = c.Vacancy.RatePerHour,
          },
          FeedbackForExpert = c.FeedbackForExpert != null
            ? new ExpertConnectionsRes.Feedback
            {
              Id = c.FeedbackForExpert.Id,
              Rating = c.FeedbackForExpert.Rating,
              CommentHtml = c.FeedbackForExpert.CommentHtml,
            }
            : null,
          FeedbackForCompany = c.FeedbackForCompany != null
            ? new ExpertConnectionsRes.Feedback
            {
              Id = c.FeedbackForCompany.Id,
              Rating = c.FeedbackForCompany.Rating,
              CommentHtml = c.FeedbackForCompany.CommentHtml,
            }
            : null
        }).ToArrayAsync();

      var res = new ExpertConnectionsRes
      {
        Connections = connections
      };

      return res;
    }

    public async Task<VacancyConnectionsRes> GetVacancyConnecctions(VacancyConnectionsReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      if (!await context.Vacancies.Where(v => v.Id == req.VacancyId && v.Event.CompanyId == currentUserData.CompanyId).AnyAsync())
      {
        throw new NotYourCompanyException();
      }

      var connections = await context.Connections
        .Where(c => c.ConnectionStatus != ConnectionStatuses.Canceled)
        .Where(c => c.VacancyId == req.VacancyId)
        .Select(c => new VacancyConnectionsRes.Connection
        {
          Id = c.Id,
          Type = c.ConnectionType,
          Status = c.ConnectionStatus,
          Expert = new VacancyConnectionsRes.Expert
          {
            Id = c.ExpertProfile.Id,
            Name = c.ExpertProfile.User.DisplayName,
            PhotoImageUrl = c.ExpertProfile.PhotoImageUrl,
            RatePerHour = c.ExpertProfile.RatePerHour,
            Rating = (4.5 + c.ExpertProfile.Connections.Where(cc => cc.FeedbackForExpertId != null).Select(cc => cc.FeedbackForExpert.Rating).DefaultIfEmpty(0).Sum()) /
              (1 + c.ExpertProfile.Connections.Count(cc => cc.FeedbackForExpert != null)),
            City = c.ExpertProfile.CityId != null
              ? new VacancyConnectionsRes.City { Id = c.ExpertProfile.City.Id, Name = c.ExpertProfile.City.Name }
              : null,
            Speciality = c.ExpertProfile.SpecialityId != null
              ? new VacancyConnectionsRes.Speciality
              {
                Id = c.ExpertProfile.Speciality.Id,
                Name = c.ExpertProfile.Speciality.Name,
                Specialization = c.ExpertProfile.SpecializationId != null
                  ? new VacancyConnectionsRes.Specialization
                  {
                    Id = c.ExpertProfile.Specialization.Id,
                    Name = c.ExpertProfile.Specialization.Name
                  }
                  : null,
                SkillsGroups = c.ExpertProfile.ExpertProfileSkills
                  .Select(ps => ps.Skill)
                  .Where(s => s.SpecialityId == c.ExpertProfile.SpecialityId)
                  .GroupBy(s => s.GroupName)
                  .Select(g => new VacancyConnectionsRes.SkillsGroup
                  {
                    Name = g.Key,
                    Skills = g.Select(s => new VacancyConnectionsRes.Skill { Id = s.Id, Name = s.Name }).ToArray()
                  }).ToArray()
              }
              : null,
            Experience = c.ExpertProfile.ExperienceOptionId != null
              ? new VacancyConnectionsRes.Experience { Id = c.ExpertProfile.ExperienceOption.Id, Name = c.ExpertProfile.ExperienceOption.Name }
              : null
          },
          FeedbackForExpert = c.FeedbackForExpert != null
            ? new VacancyConnectionsRes.Feedback
            {
              Id = c.FeedbackForExpert.Id,
              Rating = c.FeedbackForExpert.Rating,
              CommentHtml = c.FeedbackForExpert.CommentHtml,
            }
            : null,
          FeedbackForCompany = c.FeedbackForCompany != null
            ? new VacancyConnectionsRes.Feedback
            {
              Id = c.FeedbackForCompany.Id,
              Rating = c.FeedbackForCompany.Rating,
              CommentHtml = c.FeedbackForCompany.CommentHtml,
            }
            : null
        }).ToArrayAsync();

      var res = new VacancyConnectionsRes { Connections = connections };

      return res;
    }

    public async Task RegisterExpertToVacancyConnection(DatabaseContext context, RegisterExpertToVacancyConnectionReq req)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var profile = await context.ExpertProfiles.Include(p => p.User).SingleAsync(p => p.Id == req.ExpertProfileId);

      if (profile.UserId != currentUserData.Id)
      {
        throw new NotYourProfileException();
      }

      this._expertProfileDetailsUpdator.ThrowIfPublishingBroken(profile);

      if (context.Connections.Any(c => c.ExpertProfileId == req.ExpertProfileId && c.VacancyId == req.VacancyId && c.ConnectionStatus != ConnectionStatuses.Canceled))
      {
        throw new AlreadyConnectedException();
      }

      if (context.Vacancies.Where(v => v.Id == req.VacancyId && v.Connections.Where(c => c.ConnectionStatus == ConnectionStatuses.Connected).Count() >= v.Amount).Any())
      {
        throw new AmooutIsFullException();
      }

      var connection = new Connection
      {
        ConnectionType = ConnectionTypes.ExpertToVacancy,
        ConnectionStatus = ConnectionStatuses.Initiated,
        RequestingDate = DateTime.Now,
        ExpertProfileId = req.ExpertProfileId,
        VacancyId = req.VacancyId
      };

      context.Connections.Add(connection);
    }

    public async Task RegisterVacancyToExpertConnection(DatabaseContext context, RegisterVacancyToExpertConnectionReq req)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      if (! await context.Vacancies.AnyAsync(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id))
      {
        throw new NotYourCompanyException();
      }

      if (await context.Connections.AnyAsync(c => c.ExpertProfileId == req.ExpertProfileId && c.VacancyId == req.VacancyId && c.ConnectionStatus != ConnectionStatuses.Canceled))
      {
        throw new AlreadyConnectedException();
      }

      var vacancy = await context.Vacancies
        .Include(v => v.Event.Company)
        .Include(v => v.Connections)
        .SingleAsync(v => v.Id == req.VacancyId);

      if (vacancy.Connections.Where(c => c.ConnectionStatus == ConnectionStatuses.Connected).Count() >= vacancy.Amount)
      {
        throw new AmooutIsFullException();
      }

      this._companyDetailsUpdator.ThrowIfPublishingBroken(vacancy.Event.Company);


      var connection = new Connection
      {
        ConnectionType = ConnectionTypes.VacancyToExpert,
        ConnectionStatus = ConnectionStatuses.Initiated,
        RequestingDate = DateTime.Now,
        ExpertProfileId = req.ExpertProfileId,
        VacancyId = req.VacancyId
      };

      context.Connections.Add(connection);
    }

    public async Task CancelConnection(CancelConnectionReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var connection = await context.Connections
        .Include(c => c.ExpertProfile)
        .Include(c => c.Vacancy.Event.Company)
        .SingleAsync(c => c.Id == req.ConnectionId);

      if (connection.ExpertProfile.UserId != currentUserData.Id && connection.Vacancy.Event.Company.UserId != currentUserData.Id)
      {
        throw new NotYourConnectionException();
      }

      if (DateTime.Now >= connection.Vacancy.StartTime)
      {
        throw new TooLateToCancelConnectionException();
      }

      connection.ConnectionStatus = ConnectionStatuses.Canceled;
    }

    public async Task ApproveConnection(ApproveConnectionReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var connection = await context.Connections
        .Include(c => c.ExpertProfile)
        .Include(c => c.Vacancy.Event.Company)
        .SingleAsync(c => c.Id == req.ConnectionId);

      if (connection.ExpertProfile.UserId != currentUserData.Id && connection.Vacancy.Event.Company.UserId != currentUserData.Id)
      {
        throw new NotYourConnectionException();
      }

      connection.ConnectionStatus = ConnectionStatuses.Connected;
    }

    public async Task PostFeedback(PostFeedbackReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var connection = await context.Connections
        .Include(c => c.ExpertProfile)
        .Include(c => c.Vacancy.Event.Company)
          .ThenInclude(c => c.Events)
          .ThenInclude(e => e.Vacancies)
          .ThenInclude(v => v.Connections)
        .Include(c => c.FeedbackForExpert)
        .Include(c => c.FeedbackForCompany)
        .SingleAsync(c => c.Id == req.ConnectionId);

      if (connection.ExpertProfile.UserId != currentUserData.Id && connection.Vacancy.Event.Company.UserId != currentUserData.Id)
      {
        throw new NotYourConnectionException();
      }

      if (currentUserData.ExpertProfileId != null && currentUserData.ExpertProfileId == connection.ExpertProfileId)
      {
        var feedbackForCompany = new FeedbackForCompany
        {
          Rating = req.Rating,
          CommentHtml = req.CommentHtml
        };

        connection.FeedbackForCompany = feedbackForCompany;

        var allFeedbacks = connection.Vacancy.Event.Company.Events
          .SelectMany(e => e.Vacancies)
          .SelectMany(v => v.Connections)
          .Where(c => c.FeedbackForCompany != null)
          .Select(c => c.FeedbackForCompany)
          .ToArray();

        // var rating = 4.5 + allFeedbacks.Sum(f => f.Rating) / 1 + allFeedbacks.Count();
      }

      if (currentUserData.CompanyId != null && currentUserData.CompanyId == connection.Vacancy.Event.CompanyId)
      {
        var feedbackForExpert = new FeedbackForExpert
        {
          Rating = req.Rating,
          CommentHtml = req.CommentHtml
        };

        connection.FeedbackForExpert = feedbackForExpert;
      }
    }
  }
}
