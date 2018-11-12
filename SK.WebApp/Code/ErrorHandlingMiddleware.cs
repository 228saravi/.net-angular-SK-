using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SK.WebApp
{
  public abstract class HttpTransferableException : ApplicationException
  {
    private readonly string _type;
    private readonly HttpStatusCode _httpStatusCode;

    protected virtual object SerializeExtraData()
    {
      return null;
    }

    public HttpTransferableException(string type, string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest) : base(message)
    {
      this._type = type;
      this._httpStatusCode = httpStatusCode;
    }

    public async void WriteToResponse(HttpResponse response)
    {
      response.ContentType = "application/json";
      response.StatusCode = (int)this._httpStatusCode;
      await response.WriteAsync(JsonConvert.SerializeObject(new
      {
        type = this._type,
        message = this.Message,
        extraData = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this.SerializeExtraData(),
          new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
        )
      }));
    }
  }

  public class ApplicationHttpTransferableException : HttpTransferableException
  {
    public ApplicationHttpTransferableException(string message) : base("APPLICATION_EXCEPTION", message) { }
  }

  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
      this.next = next;
    }

    public async Task Invoke(HttpContext context /* other dependencies */)
    {
      try
      {
        await next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex);
      }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      if (exception is HttpTransferableException)
      {
        (exception as HttpTransferableException).WriteToResponse(context.Response);
      }
      else
      {
        throw new ApplicationException("Exception!", exception);
      }
    }
  }
}
