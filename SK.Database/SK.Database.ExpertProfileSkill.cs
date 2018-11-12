using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class ExpertProfileSkill
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long ExpertProfileId { get; set; }
    public ExpertProfile ExpertProfile { get; set; }

    public string SkillId { get; set; }
    public Skill Skill { get; set; }
  }
}
