using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class ExpertProfileDocument
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long ExpertProfileId { get; set; }
    public ExpertProfile ExpertProfile { get; set; }

    public string ExpertDocumentId { get; set; }
    public ExpertDocument ExpertDocument { get; set; }
  }
}
