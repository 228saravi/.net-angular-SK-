using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public static class LanguageIds
  {
    public static string Ru => "Ru";
    public static string En => "En";
  }

  public class Language
  {
    public string Id { get; set; }
    public string Name { get; set; }

    public ICollection<Vacancy> Vacancies { get; set; }
  }
}
