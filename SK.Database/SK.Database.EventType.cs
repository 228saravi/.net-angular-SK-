using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public static class EventTypeIds
  {
    public static string Corporative => "Corporative";
    public static string Sport => "Sport";
    public static string BusinessMeeting => "BusinessMeeting";
    public static string Wedding => "Wedding";
    public static string Birthday => "Birthday";
    public static string NewYear => "NewYear";
    public static string Tourists => "Tourists";
    public static string Training => "Training";
    public static string StagParty => "StagParty";
    public static string FamilyCelebration => "FamilyCelebration";
    public static string Graduation => "Graduation";
    public static string Formal => "Formal";
  }

  public class EventType
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public int Rank { get; set; }
  }
}

