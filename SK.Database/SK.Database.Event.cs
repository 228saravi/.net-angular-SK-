using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class Event
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Timestamp]
    public byte[] Timestamp { get; set; }

    public string Name { get; set; }

    public bool IsPublic { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }

    public string Address { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? EstimatedGuestsCount { get; set; }
    public int? EstimatedAverageCheck { get; set; }
    public bool WithtDelivery { get; set; }
    public bool WithAccomodation { get; set; }
    public string AboutEventHtml { get; set; }

    public string LogoImageUrl { get; set; }

    public long CompanyId { get; set; }
    public Company Company { get; set; }

    public string CityId { get; set; }
    public City City { get; set; }

    public string SegmentId { get; set; }
    public Segment Segment { get; set; }

    public string EventTypeId { get; set; }
    public EventType EventType { get; set; }

    public string EventFormatId { get; set; }
    public EventFormat EventFormat { get; set; }

    public ICollection<Vacancy> Vacancies { get; set; }
  }
}
