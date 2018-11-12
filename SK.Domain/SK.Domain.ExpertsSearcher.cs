using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class ExpertsSearcher
  {
    public class Req
    {
      public long? ExpertProfileId { get; set; }
      public string CityId { get; set; }
      public int? MaxRatePerHour { get; set; }
      public string[] SpecialityIds { get; set; } = new string[0];
      public string[] SpecializationIds { get; set; } = new string[0];
      public string[] SkillIds { get; set; } = new string[0];

      public int? Skip { get; set; }
      public int? Take { get; set; }
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

      public class Document
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class ClothingSize
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

      public class Expert
      {
        public long Id { get; set; }
        public bool IsPublished { get; set; }
        public string Name { get; set; }
        public string PhotoImageUrl { get; set; }
        public double Rating { get; set; }
        public City City { get; set; }
        public Language[] Languages { get; set; }
        public Document[] Documents { get; set; }
        public Speciality Speciality { get; set; }
        public ClothingSize ClothingSize { get; set; }
        public int? RatePerHour { get; set; }
        public Experience Experience { get; set; }
        public string AboutMeHtml { get; set; }
      }

      public Expert[] Experts { get; set; }

      public int TotalCount { get; set; }
    }

    private ICurrentUserService _currentUserService;

    public ExpertsSearcher(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public async Task<Res> Search(Req req, DatabaseContext database)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var profiles = database.ExpertProfiles.AsQueryable();

      profiles = profiles.Where(p => p.IsPublished);

      if (req.ExpertProfileId != null)
      {
        profiles = profiles.Where(p => p.Id == req.ExpertProfileId);
      }

      if (req.CityId != null)
      {
        profiles = profiles.Where(p => p.CityId == req.CityId);
      }

      if (req.MaxRatePerHour != null)
      {
        profiles = profiles.Where(p => p.RatePerHour <= req.MaxRatePerHour);
      }

      if (req.SpecialityIds.Any())
      {
        profiles = profiles.Where(p => req.SpecialityIds.Contains(p.SpecialityId));
      }

      if (req.SpecializationIds.Any())
      {
        profiles = profiles.Where(p => req.SpecializationIds.All(id => p.SpecializationId == id)); // Если передать несколько параметров, то ничего не найдёт. Осторожно!
      }

      if (req.SkillIds.Any())
      {
        profiles = profiles.Where(p => req.SkillIds.All(id => p.ExpertProfileSkills.Any(ps => ps.SkillId == id)));
      }

      if (req.SkillIds.Count() > 0)
      {
        profiles = profiles.OrderByDescending(p => p.ExpertProfileSkills.Count(ps => req.SkillIds.Contains(ps.SkillId)));
      }


      var totalCount = await profiles.CountAsync();


      if (req.Skip != null)
      {
        profiles = profiles.Skip(req.Skip.Value);
      }

      if (req.Take != null)
      {
        profiles = profiles.Take(req.Take.Value);
      }

      return new Res
      {
        Experts = await profiles.Select(p => new Res.Expert
        {
          Id = p.Id,
          IsPublished = p.IsPublished,
          Name = p.User.DisplayName,
          PhotoImageUrl = p.PhotoImageUrl,
          Rating = (4.5 + p.Connections.Where(c => c.FeedbackForExpertId != null).Select(c => c.FeedbackForExpert.Rating).DefaultIfEmpty(0).Sum()) /
            (1 + p.Connections.Count(c => c.FeedbackForExpert != null)),
          City = p.City != null ? new Res.City { Id = p.City.Id, Name = p.City.Name } : null,
          Speciality = new Res.Speciality
          {
            Id = p.Speciality.Id,
            Name = p.Speciality.Name,
            Specialization = p.Specialization != null
              ? new Res.Specialization
              {
                Id = p.Specialization.Id,
                Name = p.Specialization.Name,
              }
              : null,
            SkillsGroups = p.ExpertProfileSkills
              .Select(ps => ps.Skill)
              .Where(s => s.SpecialityId == p.SpecialityId)
              .GroupBy(s => s.GroupName)
              .Select(g => new Res.SkillsGroup
              {
                Name = g.Key,
                Skills = g.Select(s => new Res.Skill { Id = s.Id, Name = s.Name }).ToArray()
              }).ToArray()
          },
          Languages = p.ExpertProfileLanguages.Select(pl => new Res.Language { Id = pl.Language.Id, Name = pl.Language.Name }).ToArray(),
          Documents = p.ExpertProfileDocuments.Select(pd => new Res.Document { Id = pd.ExpertDocument.Id, Name = pd.ExpertDocument.Name }).ToArray(),
          ClothingSize = p.ClothingSize != null ? new Res.ClothingSize { Id = p.ClothingSize.Id, Name = p.ClothingSize.Name } : null,
          RatePerHour = p.RatePerHour,
          Experience = p.ExperienceOptionId != null
            ? new Res.Experience { Id = p.ExperienceOption.Id, Name = p.ExperienceOption.Name }
            : null,
          AboutMeHtml = p.AboutMeHtml,
        }).ToArrayAsync(),
        TotalCount = totalCount,
      };
    }
  }
}
