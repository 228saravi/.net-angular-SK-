using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class VacancyDocument
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long VacancyId { get; set; }
    public Vacancy Vacancy { get; set; }

    public string ExpertDocumentId { get; set; }
    public ExpertDocument ExpertDocument { get; set; }
  }
}
