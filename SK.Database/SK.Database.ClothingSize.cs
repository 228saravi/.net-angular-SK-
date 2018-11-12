using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public static class ClothingSizeIds
  {
    public static string XS => "XS";
    public static string S => "S";
    public static string M => "M";
    public static string L => "L";
    public static string XL => "XL";
    public static string XXL => "XXL";
  }

  public class ClothingSize
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public int Rank { get; set; }
  }
}
