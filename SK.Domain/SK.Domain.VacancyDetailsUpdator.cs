using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class VacancyDetailsUpdator
  {
    public class UpdateHeaderReq
    {
      public long VacancyId { get; set; }
      public string Name { get; set; }
      public string SpecialityId { get; set; }
      public string SpecializationId { get; set; }
      public int? Amount { get; set; }
      public int? RatePerHour { get; set; }
    }

    public class UpdateMainInfoReq
    {
      public long VacancyId { get; set; }
      public DateTime? StartTime { get; set; }
      public int? WorkingHours { get; set; }
      public string ExperienceOptionId { get; set; }

    }

    public class UpdateExtraInfoReq
    {
      public long VacancyId { get; set; }
      public string[] LanguagesIds { get; set; }
      public string[] DocumentsIds { get; set; }
      public string[] SkillsIds { get; set; }
    }

    public class UpdateAboutVacancyReq
    {
      public long VacancyId { get; set; }
      public string AboutVacancyHtml { get; set; }
    }

    public class PublishReq
    {
      public long VacancyId { get; set; }
    }

    public class VacancyCheckBeforePublish
    {
      public VacancyCheckBeforePublish(Vacancy vac)
      {
        this.SpecialitySet = vac.SpecialityId != null;
        this.SpecializationSet = vac.SpecializationId != null;
        this.AmountSet = vac.Amount != null;
        this.RatePerHourSet = vac.RatePerHour != null;
        this.StartTimeSet = vac.StartTime != null;
        this.WorkingHoursSet = vac.WorkingHours != null;
        this.ExperienceSet = vac.ExperienceOptionId != null;
      }

      public bool SpecialitySet { get; private set; }
      public bool SpecializationSet { get; private set; }
      public bool AmountSet { get; private set; }
      public bool RatePerHourSet { get; private set; }
      public bool StartTimeSet { get; private set; }
      public bool WorkingHoursSet { get; private set; }
      public bool ExperienceSet { get; private set; }

      public bool IsReadyForPublish
      {
        get
        {
          return this.SpecialitySet &&
            this.SpecializationSet &&
            this.AmountSet &&
            this.RatePerHourSet &&
            this.StartTimeSet &&
            this.WorkingHoursSet &&
            this.ExperienceSet;
        }
      }
    }

    public class VacancyIsNotPublishedException : ApplicationException
    {
      private VacancyCheckBeforePublish _vacancyCheck;

      public VacancyIsNotPublishedException(VacancyCheckBeforePublish vacancyCheck) : base("Can not publish!")
      {
        this._vacancyCheck = vacancyCheck;
      }

      public VacancyCheckBeforePublish VacancyCheck
      {
        get
        {
          return this._vacancyCheck;
        }
      }
    }

    private ICurrentUserService _currentUserService;

    public VacancyDetailsUpdator(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public void ThrowIfPublishingBroken(Vacancy v)
    {
      var check = new VacancyCheckBeforePublish(v);
      var isOk = v.IsPublished && check.IsReadyForPublish;
      if (!isOk)
      {
        throw new VacancyIsNotPublishedException(check);
      }
    }

    public async Task UpdateHeader(UpdateHeaderReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = context.Vacancies
        .Single(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);

      vac.SpecialityId = req.SpecialityId;
      vac.SpecializationId = req.SpecializationId;
      vac.Amount = req.Amount;
      vac.RatePerHour = req.RatePerHour;

      if (vac.IsPublished)
      {
        this.ThrowIfPublishingBroken(vac);
      }
    }

    public async Task UpdateMainInfo(UpdateMainInfoReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = context.Vacancies
        .Single(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);

      vac.StartTime = req.StartTime;
      vac.WorkingHours = req.WorkingHours;
      vac.ExperienceOptionId = req.ExperienceOptionId;

      if (vac.IsPublished)
      {
        this.ThrowIfPublishingBroken(vac);
      }
    }

    public async Task UpdateExtraInfo(UpdateExtraInfoReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = context.Vacancies
        .Include(v => v.VacancyLanguages)
        .Include(v => v.VacancyDocuments)
        .Include(v => v.VacancySkills)
        .Single(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);


      var languagesIdsToAdd = req.LanguagesIds.Where(id => vac.VacancyLanguages.All(l => l.LanguageId != id)).ToArray();
      var vacLanguagesToDelete = vac.VacancyLanguages.Where(lang => !req.LanguagesIds.Contains(lang.LanguageId)).ToArray();

      foreach (var profileLang in vacLanguagesToDelete)
      {
        vac.VacancyLanguages.Remove(profileLang);
      }

      foreach (var langId in languagesIdsToAdd)
      {
        vac.VacancyLanguages.Add(new VacancyLanguage { LanguageId = langId });
      }


      var documentIdsToAdd = req.DocumentsIds.Where(id => vac.VacancyDocuments.All(d => d.ExpertDocumentId != id)).ToArray();
      var documentsToDelete = vac.VacancyDocuments.Where(d => !req.DocumentsIds.Contains(d.ExpertDocumentId)).ToArray();

      foreach (var docId in documentIdsToAdd)
      {
        vac.VacancyDocuments.Add(new VacancyDocument { ExpertDocumentId = docId });
      }

      foreach (var doc in documentsToDelete)
      {
        vac.VacancyDocuments.Remove(doc);
      }


      var skillsIdsToAdd = req.SkillsIds.Where(id => vac.VacancySkills.All(s => s.SkillId != id)).ToArray();
      var vacSkillsToDelete = vac.VacancySkills.Where(s => !req.SkillsIds.Contains(s.SkillId)).ToArray();

      foreach (var skillId in skillsIdsToAdd)
      {
        vac.VacancySkills.Add(new VacancySkill { SkillId = skillId });
      }

      foreach (var profileSkill in vacSkillsToDelete)
      {
        vac.VacancySkills.Remove(profileSkill);
      }

      if (vac.IsPublished)
      {
        this.ThrowIfPublishingBroken(vac);
      }
    }

    public async Task UpdateAboutVacancy(UpdateAboutVacancyReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = context.Vacancies
        .Single(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);

      vac.AboutVacancyHtml = req.AboutVacancyHtml;

      if (vac.IsPublished)
      {
        this.ThrowIfPublishingBroken(vac);
      }
    }

    public async Task Publish(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = await context.Vacancies
        .SingleAsync(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);

      vac.IsPublished = true;
        this.ThrowIfPublishingBroken(vac);
    }

    public async Task Unpublish(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = await context.Vacancies
        .SingleAsync(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);

      vac.IsPublished = false;
    }

    public async Task MakePublic(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = await context.Vacancies
        .SingleAsync(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);

      vac.IsPublic = true;
    }

    public async Task MakePrivate(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = await context.Vacancies
        .SingleAsync(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);

      vac.IsPublic = false;
    }

    public async Task Delete(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var vac = await context.Vacancies
        .Include(v => v.Connections)
        .SingleAsync(v => v.Id == req.VacancyId && v.Event.Company.UserId == currentUserData.Id);

      vac.IsDeleted = true;

      foreach (var c in vac.Connections)
      {
        c.ConnectionStatus = ConnectionStatuses.Canceled;
      }
    }
  }
}
