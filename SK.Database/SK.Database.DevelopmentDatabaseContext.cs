using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SK.Database
{
  public class DevelopmentDatabaseContext : DatabaseContext
  {
    private void SeedVacanciesData(ModelBuilder modelBuilder)
    {
      var company = new Database.Company
      {
        Id = 1,
        Name = "Крутая компания",
        LogoImageUrl = "https://www.designevo.com/res/templates/thumb_small/colorful-centripetal-circle-company.png",
        ThumbnailImageUrl = "https://www.designevo.com/res/templates/thumb_small/colorful-centripetal-circle-company.png",
        Events = new[]
        {
          new Database.Event()
          {
            Id = 1,
            Name = "Презентация нового авто",
            SegmentId = SegmentIds.Democratic,
            EventFormatId = EventFormatIds.Smorgasbord,
            EventTypeId = EventTypeIds.Graduation,
            CityId = CityIds.Spb,
            Address = "Маршала Жукова 47/3",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
            LogoImageUrl = "https://event.ru/wp-content/uploads/2013/12/11-12-10_021a.jpg",
            Vacancies = new List<Vacancy>
            {
              new Vacancy
              {
                Id = 1,
                SpecialityId = SpecialityIds.Cook,
                VacancyLanguages = new List<VacancyLanguage>{ new VacancyLanguage { Id = 1, VacancyId = 1, LanguageId = LanguageIds.En } },
                VacancyDocuments = new List<VacancyDocument>{ new VacancyDocument { Id = 1, VacancyId = 1, ExpertDocumentId = ExpertDocumentIds.MedicalBook } },
                ExperienceOptionId = ExperienceOptionIds.Month6,
                StartTime = DateTime.Now.AddHours(-4),
                WorkingHours = 6,
                RatePerHour = 500,
                Amount = 10,
                AboutVacancyHtml = "<p>Очень хорошая вакансия</p>",
                VacancySkills = new List<VacancySkill>
                {
                  new VacancySkill{ Id = 2, VacancyId = 1, SkillId = SkillIds.Cook_RussianCuisine },
                  new VacancySkill{ Id = 3, VacancyId = 1, SkillId = SkillIds.Cook_ChinaCuisine },
                }
              },
              new Vacancy
              {
                Id = 2,
                SpecialityId = SpecialityIds.Cook,
                SpecializationId = SpecializationIds.Cook_ColdShop,
                VacancyLanguages = new List<VacancyLanguage>{ new VacancyLanguage { Id = 2, VacancyId = 2, LanguageId = LanguageIds.En } },
                VacancyDocuments = new List<VacancyDocument>{ new VacancyDocument { Id = 2, VacancyId = 2, ExpertDocumentId = ExpertDocumentIds.MedicalBook } },
                ExperienceOptionId = ExperienceOptionIds.Month6,
                StartTime = DateTime.Now.AddHours(-3),
                WorkingHours = 8,
                RatePerHour = 1000,
                Amount = 4,
                VacancySkills = new List<VacancySkill>
                {
                  new VacancySkill{ Id = 6, VacancyId = 2, SkillId = SkillIds.Cook_RussianCuisine },
                }
              },
              new Vacancy
              {
                Id = 3,
                SpecialityId = SpecialityIds.Waiter,
                SpecializationId = SpecializationIds.Waiter_Senior,
                VacancyLanguages = new List<VacancyLanguage>{ new VacancyLanguage { Id = 3, VacancyId = 3, LanguageId = LanguageIds.En } },
                VacancyDocuments = new List<VacancyDocument>{ new VacancyDocument { Id = 3, VacancyId = 3, ExpertDocumentId = ExpertDocumentIds.MedicalBook } },
                ExperienceOptionId = ExperienceOptionIds.Month6,
                Amount = 50,
                VacancySkills = new List<VacancySkill>()
              },
              new Vacancy
              {
                Id = 4,
                SpecialityId = SpecialityIds.Barman,
                VacancyLanguages = new List<VacancyLanguage>{ new VacancyLanguage { Id = 4, VacancyId = 4, LanguageId = LanguageIds.En } },
                VacancyDocuments = new List<VacancyDocument>{ new VacancyDocument { Id = 4, VacancyId = 4, ExpertDocumentId = ExpertDocumentIds.MedicalBook } },
                ExperienceOptionId = ExperienceOptionIds.Month6,
                SpecializationId = SpecializationIds.Barman_Senior,
                Amount = 15,
                VacancySkills = new List<VacancySkill>
                {
                  new VacancySkill { Id = 7, VacancyId = 4, SkillId = SkillIds.Barman_StrongAlcohol },
                  new VacancySkill { Id = 8, VacancyId = 4, SkillId = SkillIds.Barman_Sommelier }
                },
              },
            }
          }
        }
      };

      modelBuilder.Entity<Company>().HasData(new Company
      {
        Id = company.Id,
        Name = company.Name,
        LogoImageUrl = company.LogoImageUrl,
        ThumbnailImageUrl = company.ThumbnailImageUrl
      });

      modelBuilder.Entity<Event>().HasData(company.Events.Select(e =>
        new Event
        {
          Id = e.Id,
          Name = e.Name,
          CompanyId = company.Id,
          SegmentId = e.SegmentId,
          EventTypeId = e.EventTypeId,
          EventFormatId = e.EventFormatId,
          CityId = e.CityId,
          Address = e.Address,
          StartTime = e.StartTime,
          EndTime = e.EndTime,
          LogoImageUrl = e.LogoImageUrl,
        }
      ).ToArray());

      modelBuilder.Entity<Vacancy>().HasData(company.Events.SelectMany(e => e.Vacancies.Select(v =>
        new Vacancy
        {
          Id = v.Id,
          EventId = e.Id,
          SpecialityId = v.SpecialityId,
          SpecializationId = v.SpecializationId,
          ExperienceOptionId = v.ExperienceOptionId,
          StartTime = v.StartTime,
          WorkingHours = v.WorkingHours,
          RatePerHour = v.RatePerHour,
          Amount = v.Amount,
          AboutVacancyHtml = v.AboutVacancyHtml,
        }
      )).ToArray());

      modelBuilder.Entity<VacancySkill>().HasData(company.Events.SelectMany(e => e.Vacancies).SelectMany(v => v.VacancySkills).Select(vs =>
        new VacancySkill
        {
          Id = vs.Id,
          VacancyId = vs.VacancyId,
          SkillId = vs.SkillId,
        }
      ).ToArray());

      modelBuilder.Entity<VacancyLanguage>().HasData(company.Events.SelectMany(e => e.Vacancies).SelectMany(v => v.VacancyLanguages).Select(vl =>
        new VacancyLanguage
        {
          Id = vl.Id,
          VacancyId = vl.VacancyId,
          LanguageId = vl.LanguageId,
        }
      ).ToArray());

      modelBuilder.Entity<VacancyDocument>().HasData(company.Events.SelectMany(e => e.Vacancies).SelectMany(v => v.VacancyDocuments).Select(vd =>
        new VacancyDocument
        {
          Id = vd.Id,
          VacancyId = vd.VacancyId,
          ExpertDocumentId = vd.ExpertDocumentId,
        }
      ).ToArray());
    }

    private void SeedProfileData(ModelBuilder modelBuilder)
    {
      var profile = new Database.ExpertProfile
      {
        Id = 1,
        PhotoImageUrl = "https://cdn.the-village.ru/the-village.ru/post-cover/08raV4meydmVvKJQpR3VwA-default.jpg",
        CityId = CityIds.Msk,
        SpecialityId = SpecialityIds.Cook,
        SpecializationId = SpecializationIds.Cook_AllInOne,
        ExpertProfileSkills = new List<ExpertProfileSkill>
        {
          new ExpertProfileSkill{ Id = 10, ExpertProfileId = 1, SkillId = SkillIds.Cook_EuropeCuisine },
          new ExpertProfileSkill{ Id = 20, ExpertProfileId = 1, SkillId = SkillIds.Cook_SpainCuisine },
        },
        ExpertProfileLanguages = new List<ExpertProfileLanguage>
        {
          new ExpertProfileLanguage{ Id = 1, ExpertProfileId = 1, LanguageId = LanguageIds.Ru },
          new ExpertProfileLanguage{ Id = 2, ExpertProfileId = 1, LanguageId = LanguageIds.En },
        },
        ExpertProfileDocuments = new List<ExpertProfileDocument>
        {
          new ExpertProfileDocument{ Id = 1, ExpertProfileId = 1, ExpertDocumentId = ExpertDocumentIds.Passport },
          new ExpertProfileDocument{ Id = 2, ExpertProfileId = 1, ExpertDocumentId = ExpertDocumentIds.MedicalBook },
        },
        ClothingSizeId = ClothingSizeIds.XL,
        RatePerHour = 500,
        ExperienceOptionId = ExperienceOptionIds.Month6_Year2,
        AboutMeHtml = "<strong>Крутой повар.</strong>"
      };

      modelBuilder.Entity<ExpertProfile>().HasData(new ExpertProfile
      {
        Id = profile.Id,
        PhotoImageUrl = profile.PhotoImageUrl,
        CityId = profile.CityId,
        SpecialityId = profile.SpecialityId,
        SpecializationId = profile.SpecializationId,
        ClothingSizeId = profile.ClothingSizeId,
        RatePerHour = profile.RatePerHour,
        ExperienceOptionId = profile.ExperienceOptionId,
        AboutMeHtml = profile.AboutMeHtml,
      });

      modelBuilder.Entity<ExpertProfileSkill>().HasData(profile.ExpertProfileSkills.Select(s => new ExpertProfileSkill
      {
        Id = s.Id,
        ExpertProfileId = s.ExpertProfileId,
        SkillId = s.SkillId,
      }).ToArray());

      modelBuilder.Entity<ExpertProfileLanguage>().HasData(profile.ExpertProfileLanguages.Select(l => new ExpertProfileLanguage
      {
        Id = l.Id,
        ExpertProfileId = l.ExpertProfileId,
        LanguageId = l.LanguageId
      }).ToArray());

      modelBuilder.Entity<ExpertProfileDocument>().HasData(profile.ExpertProfileDocuments.Select(d => new ExpertProfileDocument
      {
        Id = d.Id,
        ExpertProfileId = d.ExpertProfileId,
        ExpertDocumentId = d.ExpertDocumentId,
      }).ToArray());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      this.SeedVacanciesData(modelBuilder);
      //this.SeedProfileData(modelBuilder);
    }

    public DevelopmentDatabaseContext(DbContextOptions options) : base(options) { }
  }
}

