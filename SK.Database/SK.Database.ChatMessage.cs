using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class ChatMessageDirections
  {
    public static string ExpertToCompany => "ExpertToVacancy";
    public static string VacancyToExpert => "VacancyToExpert";
  }

  public class ChatMessage
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Direction { get; set; }

    public string Body { get; set; }

    public long ConnectionId { get; set; }
    public Connection Connection { get; set; }

    public DateTimeOffset SendTime { get; set; }
    public DateTimeOffset? ReceiveTime { get; set; }
  }
}
