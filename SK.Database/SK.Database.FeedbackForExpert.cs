using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SK.Database
{
  public class FeedbackForExpert
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public int Rating { get; set; }
    public string CommentHtml { get; set; }

    public Connection Connection { get; set; }
  }
}
