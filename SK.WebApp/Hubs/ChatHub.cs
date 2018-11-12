using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SK.Database;
using SK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SK.Domain.Chat;

namespace SK.WebApp
{
  public class ChatHub : Hub
  {
    private DatabaseContext _context;
    private Chat _chat;

    private async Task OnMessageReadyForDelivery(Chat.Message messageToDelivery, string senderId, string receiverId)
    {
      await Clients.Users(new[] { senderId, receiverId }).SendAsync("MessageReceived", messageToDelivery);
    }

    private async Task OnMessagesDelivered(MessagesRes res, string senderId, string receiverId)
    {
      await Clients.Users(new[] { senderId, receiverId }).SendAsync("MessagesDelivered", res);
    }

    public ChatHub(DatabaseContext databaseContext, Chat chat)
    {
      this._chat = chat;
      this._context = databaseContext;

      this._chat.MessageReadyForDelivery += this.OnMessageReadyForDelivery;
      this._chat.MessagesDelivered += this.OnMessagesDelivered;
    }

    public async Task SendMessage(long connectionId, string message)
    {
      await this._chat.SendMessage(new Chat.SendMessageReq { ConnectionId = connectionId, Body = message }, this._context);
      await this._context.SaveChangesAsync();
    }

    public async Task SendMessageToAll(long connectionId, string message)
    {
      await this._chat.SendMessageToAll(new Chat.SendMessageReq { ConnectionId = connectionId, Body = message }, this._context);
      await this._context.SaveChangesAsync();
    }

    public async Task MarkConnectionAsViewed(long connectionId)
    {
      await this._chat.MarkConnectionAsViewed(connectionId, this._context);
      await this._context.SaveChangesAsync();
    }

    public override async Task OnConnectedAsync()
    {
      await base.OnConnectedAsync();
      await this._chat.MarkOnline(this.Context.UserIdentifier);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      await base.OnDisconnectedAsync(exception);
      await this._chat.MarkOffline(this.Context.UserIdentifier, this._context);
      await this._context.SaveChangesAsync();
    }

    protected override void Dispose(bool disposing)
    {
      this._chat.MessageReadyForDelivery -= this.OnMessageReadyForDelivery;
      this._chat.MessagesDelivered -= this.OnMessagesDelivered;

      base.Dispose(disposing);
    }
  }
}
