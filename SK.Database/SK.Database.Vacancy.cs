using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class Vacancy
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Timestamp]
    public byte[] Timestamp { get; set; }

    public bool IsPublic { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }

    public long EventId { get; set; }
    public Event Event { get; set; }

    public string SpecialityId { get; set; }
    public Speciality Speciality { get; set; }

    public string SpecializationId { get; set; }
    public Specialization Specialization { get; set; }

    public ICollection<VacancySkill> VacancySkills { get; set; }
    public ICollection<VacancyLanguage> VacancyLanguages { get; set; }
    public ICollection<VacancyDocument> VacancyDocuments { get; set; }

    public string ExperienceOptionId { get; set; }
    public ExperienceOption ExperienceOption { get; set; }

    public DateTime? StartTime { get; set; }
    public int? WorkingHours { get; set; }
    public int? RatePerHour { get; set; }

    public int? Amount { get; set; }
    public string AboutVacancyHtml { get; set; }

    public ICollection<Connection> Connections { get; set; }
  }
}
