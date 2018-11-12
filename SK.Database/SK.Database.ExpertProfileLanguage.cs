using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class ExpertProfileLanguage
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long ExpertProfileId { get; set; }
    public ExpertProfile ExpertProfile { get; set; }

    public string LanguageId { get; set; }
    public Language Language { get; set; }
  }
}
