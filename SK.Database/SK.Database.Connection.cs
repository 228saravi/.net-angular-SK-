using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public static class ConnectionTypes
  {
    public static string ExpertToVacancy => "ExpertToVacancy";
    public static string VacancyToExpert => "VacancyToExpert";
  }

  public static class ConnectionStatuses
  {
    public static string Initiated => "Initiated"; // Отклик или приглашение отправлено
    public static string Connected => "Connected"; // Обратная сторона приняля отклик или приглашение. После этого можно писать.
    public static string Canceled => "Canceled"; // Кто-то из сторон отменил. Отменять можно за день до начала работы по вакансии.
  }

  public class Connection
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Timestamp]
    public byte[] Timestamp { get; set; }

    public string ConnectionType { get; set; }

    public string ConnectionStatus { get; set; }

    public long ExpertProfileId { get; set; }
    public ExpertProfile ExpertProfile { get; set; }

    public long VacancyId { get; set; }
    public Vacancy Vacancy { get; set; }

    public DateTime RequestingDate { get; set; }
    public DateTime? ApprovingDate { get; set; }

    public long? FeedbackForExpertId { get; set; }
    public FeedbackForExpert FeedbackForExpert { get; set; }

    public long? FeedbackForCompanyId { get; set; }
    public FeedbackForCompany FeedbackForCompany { get; set; }

    public ICollection<ChatMessage> ChatMessages { get; set; }
  }
}
