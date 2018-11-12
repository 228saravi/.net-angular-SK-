using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SK.Database
{
  public static class SpecialityIds
  {
    public static string Cook => "Cook";
    public static string Waiter => "Waiter";
    public static string Barman => "Barman";
  }

  public class Speciality
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public int Rank { get; set; }

    public ICollection<Specialization> Specializations { get; set; }
    public ICollection<Skill> Skills { get; set; }

    public ICollection<Vacancy> Vacancies { get; set; }
  }
}
