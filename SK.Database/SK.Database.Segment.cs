using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class SegmentIds
  {
    public static string Democratic => "Democratic";
    public static string Middle => "Middle";
    public static string Premium => "Premium";
  }

  public class Segment
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public int Rank { get; set; }

    public ICollection<Event> Events { get; set; }
  }
}
