using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class Company
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Timestamp]
    public byte[] Timestamp { get; set; }

    public string Name { get; set; }

    public bool IsPublished { get; set; }

    public string CityId { get; set; }
    public City City { get; set; }

    public string LogoImageUrl { get; set; }
    public string ThumbnailImageUrl { get; set; }
    public string AboutCompanyHtml { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }

    public ICollection<Event> Events { get; set; }
  }
}
