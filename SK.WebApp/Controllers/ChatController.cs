using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SK.Domain;
using SK.Database;

namespace SK.WebApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ChatController : ControllerBase
  {
    public class NotYourMessagesException : HttpTransferableException
    {
      public NotYourMessagesException(Chat.NotYourMessagesException e)
        : base("NOT_YOUR_MESSAGES", e.Message)
      { }
    }

    private DatabaseContext _context;
    private Chat _chat;

    public ChatController(DatabaseContext context, Chat chat)
    {
      this._context = context;
      this._chat = chat;
    }

    [HttpGet("[action]")]
    public async Task<Chat.ExpertConnectionsRes> GetActiveExpertConnections()
    {
      return await this._chat.GetActiveExpertConnections(this._context);
    }

    [HttpGet("[action]")]
    public async Task<Chat.CompanyConnectionsRes> GetActiveCompanyConnections()
    {
      return await this._chat.GetActiveCompanyConnections(this._context);
    }

    [HttpGet("[action]")]
    public async Task<Chat.MessagesRes> GetMessaages([FromQuery]Chat.MessagesReq req)
    {
      try
      {
        var res = await this._chat.GetMessaages(req, this._context);
        await this._context.SaveChangesAsync();
        return res;
      }
      catch (Exception e)
      {
        var notYourMessagesException = e as Chat.NotYourMessagesException;
        if (notYourMessagesException != null)
        {
          throw new NotYourMessagesException(notYourMessagesException);
        }

        throw e;
      }

      
    }

    [HttpGet("[action]")]
    public async Task<Chat.MessagesRes> GetNewIncomingMessages()
    {
      var res = await this._chat.GetNewIncomingMessages(this._context);
      return res;
    }
  }
}