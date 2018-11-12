using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class Chat
  {
    public class ActualExpertConnection
    {
      public long Id { get; set; }
    }

    public class ActualCompanyConnection
    {
      public long Id { get; set; }
    }

    public class SendMessageReq
    {
      public string Body { get; set; }
      public long ConnectionId { get; set; }
    }

    public class ExpertConnectionsRes
    {
      public class Specialization
      {
        public string Id { get; set; }
        public string Name { get; set; }
      }

      public class Speciality
      {
        public string Id { get; set; }
        public string Name { get; set; }
        public Specialization Specialization { get; set; }
      }

      public class Company
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public bool IsOnline { get; set; }
        public DateTimeOffset? LastSeenTime { get; set; }
      }

      public class Event
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public string LogoImageUrl { get; set; }
        public Company Company { get; set; }
      }

      public class Vacancy
      {
        public long Id { get; set; }
        public Speciality Speciality { get; set; }
        public Event Event { get; set; }
      }

      public class LastMessage
      {
        public long Id { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Time { get; set; }
      }

      public class Feedback
      {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string CommentHtml { get; set; }
      }

      public class Connection
      {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public Vacancy Vacancy { get; set; }
        public Feedback FeedbackForExpert { get; set; }
        public Feedback FeedbackForCompany { get; set; }
        public int NewIncomingMessagesCount { get; set; }
        public LastMessage LastMessage { get; set; }
      }

      public Connection[] Connections { get; set; }
    }

    public class CompanyConnectionsRes
    {
      public class Expert
      {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailImageUrl { get; set; }
      }

      public class Vacancy
      {
        public long Id { get; set; }
      }

      public class LastMessage
      {
        public long Id { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Time { get; set; }
      }

      public class Feedback
      {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string CommentHtml { get; set; }
      }

      public class Connection
      {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public Expert Expert { get; set; }
        public Vacancy Vacancy { get; set; }
        public Feedback FeedbackForExpert { get; set; }
        public Feedback FeedbackForCompany { get; set; }
        public int NewIncomingMessagesCount { get; set; }
        public LastMessage LastMessage { get; set; }
      }

      public Connection[] Connections { get; set; }
    }

    public class Message
    {
      public long Id { get; set; }
      public string Direction { get; set; }
      public string Body { get; set; }
      public DateTimeOffset SendTime { get; set; }
      public DateTimeOffset? ReceiveTime { get; set; }

      public long ConnectionId { get; set; }
      public string ConnectionType { get; set; }

      public long ExpertId { get; set; }
      public string ExpertName { get; set; }

      public long CompanyId { get; set; }
      public string CompanyName { get; set; }
    }

    public class MessagesReq
    {
      public long ConnectionId { get; set; }
    }

    public class MessagesRes
    {
      public Message[] Messages { get; set; }
    }

    public class NotYourMessagesException : ApplicationException
    {
      public NotYourMessagesException() : base("You can reqd only your messages!") { }
    }

    private ICurrentUserService _currentUserService;
    private HashSet<string> _onlineUserIdsSet = new HashSet<string>(100);

    private string GetOppositeUserId(Connection connection, string userId)
    {
      if (connection.ExpertProfile.UserId == userId)
      {
        return connection.Vacancy.Event.Company.UserId;
      }

      if (connection.Vacancy.Event.Company.UserId == userId)
      {
        return connection.ExpertProfile.UserId;
      }

      throw new ApplicationException("Wrong user id!");
    }

    public Chat(ICurrentUserService currentUserService)
    {
      this._currentUserService = currentUserService;
    }

    public delegate Task MessageReadyForDeliveryHandler(Message message, string senderId, string receiverId);

    public delegate Task MessagesDeliveredHandler(MessagesRes messagesRes, string senderUserId, string receiverId);

    public event MessageReadyForDeliveryHandler MessageReadyForDelivery;

    public event MessagesDeliveredHandler MessagesDelivered;

    public async Task<ExpertConnectionsRes> GetActiveExpertConnections(DatabaseContext context)
    {
      var currentUserId = this._currentUserService.GetCurrentUserData().Id;

      var res = new ExpertConnectionsRes
      {
        Connections = await context.Connections
        .Where(c => c.ExpertProfile.UserId == currentUserId)
        .Where(c => c.ConnectionStatus == ConnectionStatuses.Connected)
        .Where(c => c.FeedbackForCompanyId == null)
        .Select(c => new ExpertConnectionsRes.Connection
        {
          Id = c.Id,
          Type = c.ConnectionType,
          Status = c.ConnectionStatus,
          FeedbackForExpert = c.FeedbackForExpert != null
            ? new ExpertConnectionsRes.Feedback
            {
              Id = c.FeedbackForExpert.Id,
              Rating = c.FeedbackForExpert.Rating,
              CommentHtml = c.FeedbackForExpert.CommentHtml,
            }
            : null,
          FeedbackForCompany = c.FeedbackForCompany != null
            ? new ExpertConnectionsRes.Feedback
            {
              Id = c.FeedbackForCompany.Id,
              Rating = c.FeedbackForCompany.Rating,
              CommentHtml = c.FeedbackForCompany.CommentHtml,
            }
            : null,
          NewIncomingMessagesCount = c.ChatMessages.Count(m => m.ReceiveTime == null && m.Direction == ChatMessageDirections.VacancyToExpert),
          LastMessage = c.ChatMessages
            .Select(m => new ExpertConnectionsRes.LastMessage
            {
              Id = m.Id,
              Body = m.Body,
              Time = m.ReceiveTime ?? m.SendTime
            })
            .OrderByDescending(m => m.Time)
            .FirstOrDefault(),
          Vacancy = new ExpertConnectionsRes.Vacancy
          {
            Id = c.Vacancy.Id,
            Speciality = c.Vacancy.SpecialityId != null
              ? new ExpertConnectionsRes.Speciality
              {
                Id = c.Vacancy.Speciality.Id,
                Name = c.Vacancy.Speciality.Name,
                Specialization = c.Vacancy.SpecializationId != null
                  ? new ExpertConnectionsRes.Specialization
                  {
                    Id = c.Vacancy.Specialization.Id,
                    Name = c.Vacancy.Specialization.Name,
                  }
                  : null,
              }
              : null,
            Event = new ExpertConnectionsRes.Event
            {
              Id = c.Vacancy.Event.Id,
              Name = c.Vacancy.Event.Name,
              LogoImageUrl = c.Vacancy.Event.LogoImageUrl,
              Company = new ExpertConnectionsRes.Company
              {
                Id = c.Vacancy.Event.Company.Id,
                Name = c.Vacancy.Event.Company.Name,
                ThumbnailImageUrl = c.Vacancy.Event.Company.ThumbnailImageUrl,
                IsOnline = this.IsOnline(c.Vacancy.Event.Company.UserId),
                LastSeenTime = c.Vacancy.Event.Company.User.LastSeenTime,
              }
            }
          }
        }).ToArrayAsync()
      };

      return res;
    }

    public async Task<CompanyConnectionsRes> GetActiveCompanyConnections(DatabaseContext context)
    {
      var currentUserId = this._currentUserService.GetCurrentUserData().Id;
      return new CompanyConnectionsRes
      {
        Connections = await context.Connections
        .Where(c => c.Vacancy.Event.Company.UserId == currentUserId)
        .Where(c => c.ConnectionStatus == ConnectionStatuses.Connected)
        .Where(c => c.FeedbackForExpertId == null)
        .Select(c => new CompanyConnectionsRes.Connection
        {
          Id = c.Id,
          Type = c.ConnectionType,
          Status = c.ConnectionStatus,
          FeedbackForExpert = c.FeedbackForExpert != null
            ? new CompanyConnectionsRes.Feedback
            {
              Id = c.FeedbackForExpert.Id,
              Rating = c.FeedbackForExpert.Rating,
              CommentHtml = c.FeedbackForExpert.CommentHtml,
            }
            : null,
          FeedbackForCompany = c.FeedbackForCompany != null
            ? new CompanyConnectionsRes.Feedback
            {
              Id = c.FeedbackForCompany.Id,
              Rating = c.FeedbackForCompany.Rating,
              CommentHtml = c.FeedbackForCompany.CommentHtml,
            }
            : null,
          Expert = new CompanyConnectionsRes.Expert
          {
            Id = c.ExpertProfile.Id,
            Name = c.ExpertProfile.User.DisplayName,
            ThumbnailImageUrl = c.ExpertProfile.ThumbnailImageUrl,
          },
          Vacancy = new CompanyConnectionsRes.Vacancy
          {
            Id = c.VacancyId
          },
          NewIncomingMessagesCount = c.ChatMessages.Count(m => m.ReceiveTime == null && m.Direction == ChatMessageDirections.ExpertToCompany),
          LastMessage = c.ChatMessages
            .Select(m => new CompanyConnectionsRes.LastMessage
            {
              Id = m.Id,
              Body = m.Body,
              Time = m.ReceiveTime ?? m.SendTime
            })
            .OrderByDescending(m => m.Time)
            .FirstOrDefault()
        }).ToArrayAsync()
      };
    }

    public async Task<MessagesRes> GetMessaages(MessagesReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      var res = new MessagesRes
      {
        Messages = await context.ChatMessages
          .Where(m => m.ConnectionId == req.ConnectionId)
          .Select(m => new Message
          {
            Id = m.Id,
            Direction = m.Direction,
            Body = m.Body,
            SendTime = m.SendTime,
            ReceiveTime = m.ReceiveTime,
            ConnectionId = m.Connection.Id,
            ConnectionType = m.Connection.ConnectionType,
            CompanyId = m.Connection.Vacancy.Event.Company.Id,
            CompanyName = m.Connection.Vacancy.Event.Company.Name,
            ExpertId = m.Connection.ExpertProfile.Id,
            ExpertName = m.Connection.ExpertProfile.User.DisplayName,
          }).ToArrayAsync()
      };

      if (!res.Messages.All(m => m.CompanyId == currentUserData.CompanyId || m.ExpertId == currentUserData.ExpertProfileId))
      {
        throw new NotYourMessagesException();
      }

      return res;
    }

    public async Task<MessagesRes> GetNewIncomingMessages(DatabaseContext context)
    {
      var currentUserId = this._currentUserService.GetCurrentUserData().Id;

      var res = new MessagesRes
      {
        Messages = await context.ChatMessages
          .Where(m =>
            m.Connection.ConnectionStatus == ConnectionStatuses.Connected &&
            (
              m.Direction == ChatMessageDirections.VacancyToExpert && m.Connection.ExpertProfile.UserId == currentUserId && m.ReceiveTime == null ||
              m.Direction == ChatMessageDirections.ExpertToCompany && m.Connection.Vacancy.Event.Company.UserId == currentUserId && m.ReceiveTime == null
            )
          )
          .Select(m => new Message
          {
            Id = m.Id,
            Body = m.Body,
            SendTime = m.SendTime,
            ReceiveTime = m.ReceiveTime,
            ConnectionId = m.Connection.Id,
            ConnectionType = m.Connection.ConnectionType,
            CompanyId = m.Connection.Vacancy.Event.Company.Id,
            CompanyName = m.Connection.Vacancy.Event.Company.Name,
            ExpertId = m.Connection.ExpertProfile.Id,
            ExpertName = m.Connection.ExpertProfile.User.DisplayName,
          }).ToArrayAsync()
      };

      return res;
    }

    public async Task MarkOnline(string userId)
    {
      this._onlineUserIdsSet.Add(userId);
    }

    public async Task MarkOffline(string userId, DatabaseContext context)
    {
      this._onlineUserIdsSet.Remove(userId);
      var user = await context.Users.SingleAsync(u => u.Id == userId);
      user.LastSeenTime = DateTimeOffset.Now;
    }

    public bool IsOnline(string userId)
    {
      return this._onlineUserIdsSet.Contains(userId);
    }

    public async Task MarkConnectionAsViewed(long connectionId, DatabaseContext context)
    {
      var currentUserId = this._currentUserService.GetCurrentUserData().Id;

      var connection = await context.Connections
        .Include(c => c.ExpertProfile.User)
        .Include(c => c.Vacancy.Event.Company)
        .Include(c => c.ChatMessages)
        .SingleAsync(c => c.Id == connectionId);

      var oppositeUserId = this.GetOppositeUserId(connection, currentUserId);

      var messages = connection.ChatMessages
        .Where(m => m.ReceiveTime == null && (
          (m.Connection.ExpertProfile.UserId == currentUserId && m.Direction == ChatMessageDirections.VacancyToExpert) ||
          (m.Connection.Vacancy.Event.Company.UserId == currentUserId && m.Direction == ChatMessageDirections.ExpertToCompany)
        )).ToArray();

      foreach (var message in messages)
      {
        message.ReceiveTime = DateTimeOffset.Now;
      }

      if (this.MessagesDelivered != null)
      {
        var res = new MessagesRes
        {
          Messages = messages.Select(m => new Message
          {
            Id = m.Id,
            Direction = m.Direction,
            Body = m.Body,
            SendTime = m.SendTime,
            ReceiveTime = m.ReceiveTime,
            ConnectionId = m.Connection.Id,
            ConnectionType = m.Connection.ConnectionType,
            CompanyId = m.Connection.Vacancy.Event.Company.Id,
            CompanyName = m.Connection.Vacancy.Event.Company.Name,
            ExpertId = m.Connection.ExpertProfile.Id,
            ExpertName = m.Connection.ExpertProfile.User.DisplayName,
          }).ToArray()
        };

        await this.MessagesDelivered(res, this.GetOppositeUserId(connection, currentUserId), currentUserId);
      }
    }

    public async Task SendMessage(SendMessageReq req, DatabaseContext context)
    {
      var currentUserData = this._currentUserService.GetCurrentUserData();

      string senderId;
      string receiverId;

      var connection = await context.Connections
        .Include(c => c.ExpertProfile.User)
        .Include(c => c.Vacancy.Event.Company)
        .SingleAsync(c => c.Id == req.ConnectionId);

      var expertUserId = connection.ExpertProfile.UserId;
      var companyUserId = connection.Vacancy.Event.Company.UserId;

      if (expertUserId == currentUserData.Id)
      {
        senderId = expertUserId;
        receiverId = companyUserId;
      }
      else if (companyUserId == currentUserData.Id)
      {
        senderId = companyUserId;
        receiverId = expertUserId;
      }
      else
      {
        throw new Exception();
      }

      var message = new ChatMessage
      {
        ConnectionId = req.ConnectionId,
        Body = req.Body,
        SendTime = DateTime.Now,
        Direction = senderId == expertUserId ? ChatMessageDirections.ExpertToCompany : ChatMessageDirections.VacancyToExpert
      };
      context.ChatMessages.Add(message);
      await context.SaveChangesAsync();

      await this.MessageReadyForDelivery.Invoke(
        new Message
        {
          Id = message.Id,
          Direction = message.Direction,
          Body = req.Body,
          ConnectionId = req.ConnectionId,
          ConnectionType = connection.ConnectionType,
          ExpertId = connection.ExpertProfileId,
          ExpertName = connection.ExpertProfile.User.DisplayName,
          CompanyId = connection.Vacancy.Event.CompanyId,
          CompanyName = connection.Vacancy.Event.Company.Name,
          SendTime = DateTime.Now,
        },
        senderId,
        receiverId
      );
    }

    public async Task SendMessageToAll(SendMessageReq req, DatabaseContext context)
    {
      var connections = await context.Connections
        .Where(c => c.Vacancy.Connections.Any(cc => cc.Id == req.ConnectionId))
        .ToArrayAsync();

      foreach (var c in connections)
      {
        await this.SendMessage(new SendMessageReq { ConnectionId = c.Id, Body = req.Body }, context);
      }

    }
  }
}
