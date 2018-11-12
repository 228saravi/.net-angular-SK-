using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Database
{
  public static class SpecializationIds
  {
    public static string Cook_HotShop => "Cook_HotShop";
    public static string Cook_ColdShop => "Cook_ColdShop";
    public static string Cook_AllInOne => "Cook_AllInOne";
    public static string Cook_Preparer => "Cook_Preparer";
    public static string Cook_Confectioner => "Cook_Confectioner";
    public static string Cook_Assistant => "Cook_Assistant";
    public static string Cook_Butcher => "Cook_Butcher";
    public static string Cook_Foreman => "Cook_Foreman";
    public static string Cook_SousChef => "Cook_SousChef";
    public static string Cook_Chef => "Cook_Chef";
    public static string Cook_SushiMaker => "Cook_SushiMaker";
    public static string Cook_PizzaMaker => "Cook_PizzaMaker";

    public static string Barman_Assistant => "Barman_Assistant";
    public static string Barman_Middle => "Barman_Middle";
    public static string Barman_Senior => "Barman_Senior";

    public static string Waiter_Assistant => "Waiter_Assistant";
    public static string Waiter_Middle => "Waiter_Middle";
    public static string Waiter_Senior => "Waiter_Senior";
  }

  public class Specialization
  {
    public string Id { get; set; }
    public string Name { get; set; }

    public int Rank { get; set; }

    public string SpecialityId { get; set; }
    public Speciality Speciality { get; set; }
  }
}


