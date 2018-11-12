using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SK.WebApp.Pages
{
  public class ErrorModel : PageModel
  {
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public void OnGet()
    {
      var f = HttpContext.Features.Get<IExceptionHandlerFeature>();
      RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
  }
}
