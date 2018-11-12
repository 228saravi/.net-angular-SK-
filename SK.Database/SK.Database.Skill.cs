using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Database
{
  public static class SkillIds
  {
    public static string Cook_RussianCuisine => "Cook_RussianCuisine";
    public static string Cook_UsbekCuisine => "Cook_UsbekCuisine";
    public static string Cook_FranceCuisine => "Cook_FranceCuisine";
    public static string Cook_EuropeCuisine => "Cook_EuropeCuisine";
    public static string Cook_SpainCuisine => "Cook_SpainCuisine";
    public static string Cook_BelgiumCuisine => "Cook_BelgiumCuisine";
    public static string Cook_GermanCuisine => "Cook_GermanCuisine";
    public static string Cook_ChinaCuisine => "Cook_ChinaCuisine";
    public static string Cook_UsaCuisine => "Cook_UsaCuisine";
    public static string Cook_NewWorldCuisine => "Cook_NewWorldCuisine";

    public static string Barman_Сocktail => "Barman_Сocktail";
    public static string Barman_Tea => "Barman_Tea";
    public static string Barman_Coffee => "Barman_Coffee";
    public static string Barman_SoftDrinks => "Barman_SoftDrinks";
    public static string Barman_SoftCocktails => "Barman_SoftCocktails";
    public static string Barman_Bear => "Barman_Bear";
    public static string Barman_Wine => "Barman_Wine";
    public static string Barman_StrongAlcohol => "Barman_StrongAlcohol";
    public static string Barman_Tinctures => "Barman_Tinctures";
    public static string Barman_Aperitifs => "Barman_Aperitifs";
    public static string Barman_Digestives => "Barman_Digestives";
    public static string Barman_Sommelier => "Barman_Sommelier";

    public static string Waiter_RussianCuisine => "Waiter_RussianCuisine";
    public static string Waiter_UsbekCuisine => "Waiter_UsbekCuisine";
    public static string Waiter_FranceCuisine => "Waiter_FranceCuisine";
    public static string Waiter_EuropeCuisine => "Waiter_EuropeCuisine";
    public static string Waiter_SpainCuisine => "Waiter_SpainCuisine";
    public static string Waiter_BelgiumCuisine => "Waiter_BelgiumCuisine";
    public static string Waiter_GermanCuisine => "Waiter_GermanCuisine";
    public static string Waiter_ChinaCuisine => "Waiter_ChinaCuisine";
    public static string Waiter_UsaCuisine => "Waiter_UsaCuisine";
    public static string Waiter_NewWorldCuisine => "Waiter_NewWorldCuisine";

    public static string Waiter_Сocktail => "Waiter_Сocktail";
    public static string Waiter_Tea => "Waiter_Tea";
    public static string Waiter_Coffee => "Waiter_Coffee";
    public static string Waiter_SoftDrinks => "Waiter_SoftDrinks";
    public static string Waiter_SoftCocktails => "Waiter_SoftCocktails";
    public static string Waiter_Bear => "Waiter_Bear";
    public static string Waiter_Wine => "Waiter_Wine";
    public static string Waiter_StrongAlcohol => "Waiter_StrongAlcohol";
    public static string Waiter_Tinctures => "Waiter_Tinctures";
    public static string Waiter_Aperitifs => "Waiter_Aperitifs";
    public static string Waiter_Digestives => "Waiter_Digestives";
    public static string Waiter_Sommelier => "Waiter_Sommelier";
  }

  public class Skill
  {
    public string Id { get; set; }
    public string Name { get; set; }

    public string GroupName { get; set; }
    public int Rank { get; set; }

    public string SpecialityId { get; set; }
    public Speciality Speciality { get; set; }
  }
}


