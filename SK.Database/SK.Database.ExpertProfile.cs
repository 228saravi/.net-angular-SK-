using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class ExpertProfile
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Timestamp]
    public byte[] Timestamp { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }

    public bool IsPublished { get; set; }

    public string PhotoImageUrl { get; set; }
    public string ThumbnailImageUrl { get; set; }

    public string CityId { get; set; }
    public City City { get; set; }

    public string SpecialityId { get; set; }
    public Speciality Speciality { get; set; }

    public string SpecializationId { get; set; }
    public Specialization Specialization { get; set; }

    public ICollection<ExpertProfileSkill> ExpertProfileSkills { get; set; }

    public ICollection<ExpertProfileLanguage> ExpertProfileLanguages { get; set; }
    public ICollection<ExpertProfileDocument> ExpertProfileDocuments { get; set; }

    public string ClothingSizeId { get; set; }
    public ClothingSize ClothingSize { get; set; }

    public string ExperienceOptionId { get; set; }
    public ExperienceOption ExperienceOption { get; set; }

    public int? RatePerHour { get; set; }
    public string AboutMeHtml { get; set; }

    public ICollection<Connection> Connections { get; set; }
  }
}
