using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class ExpertProfileDetailsUpdator
  {
    public class UpdateHeaderReq
    {
      public long ExpertProfileId { get; set; }
      public string Name { get; set; }
      public int? RatePerHour { get; set; }
      public string SpecialityId { get; set; }
      public string SpecializationId { get; set; }
    }

    public class UpdateMainInfoReq
    {
      public long ExpertProfileId { get; set; }
      public string CityId { get; set; }
      public string ExperienceOptionId { get; set; }
      public string[] LanguagesIds { get; set; }
      public string[] SkillsIds { get; set; }
    }

    public class UpdateExtraInfoReq
    {
      public long ExpertProfileId { get; set; }
      public string ClothingSizeId { get; set; }
      public string[] ExpertDocumentsIds { get; set; }
    }

    public class UpdateAboutMeReq
    {
      public long ExpertProfileId { get; set; }
      public string AboutMeHtml { get; set; }
    }

    public class UpdatePhotoReq
    {
      public long ExpertProfileId { get; set; }
      public string PhotoUrl { get; set; }
    }

    public class PublishReq
    {
      public long ExpertProfileId { get; set; }
    }

    public class ProfileCheckBeforePublish
    {
      public ProfileCheckBeforePublish(ExpertProfile profile)
      {
        this.NameSet = !String.IsNullOrWhiteSpace(profile.User.DisplayName);
        this.PhotoSet = profile.PhotoImageUrl != null;
        this.CitySet = profile.CityId != null;
        this.RatePerHourSet = profile.RatePerHour != null;
        this.SpecialitySet = profile.SpecialityId != null;
        this.SpecializationSet = profile.SpecializationId != null;
        this.ExperienceSet = profile.ExperienceOptionId != null;
      }

      public bool NameSet { get; private set; }
      public bool PhotoSet { get; private set; }
      public bool CitySet { get; private set; }
      public bool RatePerHourSet { get; private set; }
      public bool SpecialitySet { get; private set; }
      public bool SpecializationSet { get; private set; }
      public bool ExperienceSet { get; private set; }

      public bool IsReadyForPublish
      {
        get
        {
          return this.NameSet &&
            this.PhotoSet &&
            this.CitySet &&
            this.RatePerHourSet &&
            this.SpecialitySet &&
            this.SpecializationSet &&
            this.ExperienceSet;
        }
      }
    }

    public class ProfileIsNotPublishedException : ApplicationException
    {
      private ProfileCheckBeforePublish _profileCheck;

      public ProfileIsNotPublishedException(ProfileCheckBeforePublish profileCheck) : base("Can not publish!")
      {
        this._profileCheck = profileCheck;
      }

      public ProfileCheckBeforePublish ProfileCheck
      {
        get
        {
          return this._profileCheck;
        }
      }
    }

    private ICurrentUserService _currentUserService;

    public ExpertProfileDetailsUpdator(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public void ThrowIfPublishingBroken(ExpertProfile p)
    {
      var check = new ProfileCheckBeforePublish(p);
      var isOk = p.IsPublished && check.IsReadyForPublish;
      if (!isOk)
      {
        throw new ProfileIsNotPublishedException(check);
      }
    }

    public async Task UpdateHeader(UpdateHeaderReq req, DatabaseContext context)
    {
      var expertProfile = await context.ExpertProfiles
        .Include(profile => profile.User)
        .Include(profile => profile.Speciality)
        .Include(profile => profile.Specialization)
        .Include(profile => profile.ExpertProfileSkills)
        .SingleAsync(profile => profile.Id == req.ExpertProfileId);

      if (expertProfile.SpecialityId != req.SpecialityId)
      {
        expertProfile.SpecializationId = null;
        expertProfile.Specialization = null;
        expertProfile.ExpertProfileSkills.Clear();
      }

      expertProfile.User.DisplayName = req.Name;
      expertProfile.RatePerHour = req.RatePerHour;
      expertProfile.SpecialityId = req.SpecialityId;
      expertProfile.SpecializationId = req.SpecializationId;

      if (expertProfile.IsPublished)
      {
        this.ThrowIfPublishingBroken(expertProfile);
      }
    }

    public async Task UpdateMainInfo(UpdateMainInfoReq req, DatabaseContext context)
    {
      var expertProfile = await context.ExpertProfiles
        .Include(p => p.User)
        .Include(p => p.ExpertProfileLanguages)
        .Include(p => p.ExpertProfileSkills)
        .SingleAsync(profile => profile.Id == req.ExpertProfileId);

      expertProfile.CityId = req.CityId;
      expertProfile.ExperienceOptionId = req.ExperienceOptionId;

      var languagesIdsToAdd = req.LanguagesIds.Where(id => expertProfile.ExpertProfileLanguages.All(l => l.LanguageId != id)).ToArray();
      var profileLanguagesToDelete = expertProfile.ExpertProfileLanguages.Where(lang => !req.LanguagesIds.Contains(lang.LanguageId)).ToArray();

      foreach (var profileLang in profileLanguagesToDelete)
      {
        expertProfile.ExpertProfileLanguages.Remove(profileLang);
      }

      foreach (var langId in languagesIdsToAdd)
      {
        expertProfile.ExpertProfileLanguages.Add(new ExpertProfileLanguage { LanguageId = langId });
      }

      var skillsIdsToAdd = req.SkillsIds.Where(id => expertProfile.ExpertProfileSkills.All(s => s.SkillId != id)).ToArray();
      var profileSkillsToDelete = expertProfile.ExpertProfileSkills.Where(s => !req.SkillsIds.Contains(s.SkillId)).ToArray();

      foreach (var skillId in skillsIdsToAdd)
      {
        expertProfile.ExpertProfileSkills.Add(new ExpertProfileSkill { SkillId = skillId });
      }

      foreach (var profileSkill in profileSkillsToDelete)
      {
        expertProfile.ExpertProfileSkills.Remove(profileSkill);
      }

      if (expertProfile.IsPublished)
      {
        this.ThrowIfPublishingBroken(expertProfile);
      }
    }

    public async Task UpdateExtraInfo(UpdateExtraInfoReq req, DatabaseContext context)
    {
      var expertProfile = await context.ExpertProfiles
        .Include(p => p.User)
        .Include(p => p.ExpertProfileDocuments)
        .SingleAsync(profile => profile.Id == req.ExpertProfileId);

      expertProfile.ClothingSizeId = req.ClothingSizeId;

      var documentIdsToAdd = req.ExpertDocumentsIds.Where(id => expertProfile.ExpertProfileDocuments.All(d => d.ExpertDocumentId != id)).ToArray();
      var documentsToDelete = expertProfile.ExpertProfileDocuments.Where(d => !req.ExpertDocumentsIds.Contains(d.ExpertDocumentId)).ToArray();

      foreach (var docId in documentIdsToAdd)
      {
        expertProfile.ExpertProfileDocuments.Add(new ExpertProfileDocument { ExpertDocumentId = docId });
      }

      foreach (var doc in documentsToDelete)
      {
        expertProfile.ExpertProfileDocuments.Remove(doc);
      }

      if (expertProfile.IsPublished)
      {
        this.ThrowIfPublishingBroken(expertProfile);
      }
    }

    public async Task UpdateAboutMe(UpdateAboutMeReq req, DatabaseContext context)
    {
      var expertProfile = await context.ExpertProfiles
        .Include(profile => profile.User)
        .SingleAsync(profile => profile.Id == req.ExpertProfileId);

      expertProfile.AboutMeHtml = req.AboutMeHtml;

      if (expertProfile.IsPublished)
      {
        this.ThrowIfPublishingBroken(expertProfile);
      }
    }

    public async Task UpdatePhoto(UpdatePhotoReq req, DatabaseContext context)
    {
      var expertProfile = await context.ExpertProfiles
        .Include(p => p.User)
        .SingleAsync(profile => profile.Id == req.ExpertProfileId);

      expertProfile.PhotoImageUrl = req.PhotoUrl;
      expertProfile.ThumbnailImageUrl = req.PhotoUrl;

      if (expertProfile.IsPublished)
      {
        this.ThrowIfPublishingBroken(expertProfile);
      }
    }

    public async Task Publish(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var profile = await context.ExpertProfiles
        .Include(p => p.User)
        .SingleAsync(p => p.Id == req.ExpertProfileId);

      profile.IsPublished = true;
      this.ThrowIfPublishingBroken(profile);
    }

    public async Task Unpublish(PublishReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var profile = await context.ExpertProfiles
        .Include(p => p.User)
        .SingleAsync(p => p.Id == req.ExpertProfileId);

      profile.IsPublished = false;
    }
  }
}
