using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public static class CityIds
  {
    public static string Msk => "Msk";
    public static string Spb => "Spb";
  }

  public class City
  {
    public string Id { get; set; }
    public string Name { get; set; }
  }
}
