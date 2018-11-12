using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public static class EventFormatIds
  {
    public static string Furshet => "Furshet";
    public static string MainCourses => "MainCourses";
    public static string Smorgasbord => "Smorgasbord";
    public static string Сocktail => "Сocktail";
    public static string Barbecue => "Barbecue";
    public static string Replacement => "Replacement";
  }

  public class EventFormat
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public int Rank { get; set; }
  }
}

