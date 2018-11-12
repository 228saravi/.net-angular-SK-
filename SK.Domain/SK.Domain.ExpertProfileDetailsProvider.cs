using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class ExpertProfileDetailsProvider
  {
    public class Req
    {
      public long ExpertProfileId { get; set; }
    }

    public class Res
    {
      public class LanguageRes
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class CityRes
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Document
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class ClothingSizeRes
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

      public class SpecialityRes
      {
        public string Id { get; set; }
        public string Name { get; set; }
        public Specialization Specialization { get; set; }
        public SkillsGroup[] SkillsGroups { get; set; }
      }

      public class ExpertProfileRes
      {
        public long Id { get; set; }
        public bool IsPublished { get; set; }
        public string Name { get; set; }
        public string PhotoImageUrl { get; set; }
        public double Rating { get; set; }
        public CityRes City { get; set; }
        public LanguageRes[] Languages { get; set; }
        public Document[] Documents { get; set; }
        public SpecialityRes Speciality { get; set; }
        public ClothingSizeRes ClothingSize { get; set; }
        public int? RatePerHour { get; set; }
        public Experience Experience { get; set; }
        public string AboutMeHtml { get; set; }
      }

      public ExpertProfileRes ExpertProfile { get; set; }
    }

    private ICurrentUserService _currentUserService;

    public ExpertProfileDetailsProvider(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public async Task<Res> Get(Req req, DatabaseContext database)
    {
      var res = new Res
      {
        ExpertProfile = await database.ExpertProfiles.Where(p => p.Id == req.ExpertProfileId).Select(p => new Res.ExpertProfileRes
        {
          Id = p.Id,
          IsPublished = p.IsPublished,
          Name = p.User.DisplayName,
          PhotoImageUrl = p.PhotoImageUrl,
          Rating = (4.5 + p.Connections.Where(c => c.FeedbackForExpertId != null).Select(c => c.FeedbackForExpert.Rating).DefaultIfEmpty(0).Sum()) /
            (1 + p.Connections.Count(c => c.FeedbackForExpert != null)),
          City = p.City != null ? new Res.CityRes { Id = p.City.Id, Name = p.City.Name } : null,
          Speciality = new Res.SpecialityRes
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
          Languages = p.ExpertProfileLanguages.Select(pl => new Res.LanguageRes { Id = pl.Language.Id, Name = pl.Language.Name }).ToArray(),
          Documents = p.ExpertProfileDocuments.Select(pd => new Res.Document { Id = pd.ExpertDocument.Id, Name = pd.ExpertDocument.Name }).ToArray(),
          ClothingSize = p.ClothingSize != null ? new Res.ClothingSizeRes { Id = p.ClothingSize.Id, Name = p.ClothingSize.Name } : null,
          RatePerHour = p.RatePerHour,
          Experience = p.ExperienceOptionId != null
            ? new Res.Experience { Id = p.ExperienceOption.Id, Name = p.ExperienceOption.Name }
            : null,
          AboutMeHtml = p.AboutMeHtml,
        }).SingleAsync()
      };

      return res;
    }
  }
}
