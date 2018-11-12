using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public static class ExperienceOptionIds
  {
    public static string Month6 => "Month6";
    public static string Month6_Year2 => "Month6_Year2";
    public static string Year2 => "Year2";
  }

  public class ExperienceOption
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public int Rank { get; set; }
  }
}

