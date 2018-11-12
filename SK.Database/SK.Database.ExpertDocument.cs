using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public static class ExpertDocumentIds
  {
    public static string Passport => "Passport";
    public static string MedicalBook => "MedicalBook";
    public static string WorkPermit => "WorkPermit";
  }

  public class ExpertDocument
  {
    public string Id { get; set; }
    public string Name { get; set; }
  }
}
