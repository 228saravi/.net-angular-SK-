using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class VacancyLanguage
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long VacancyId { get; set; }
    public Vacancy Vacancy { get; set; }

    public string LanguageId { get; set; }
    public Language Language { get; set; }
  }
}
