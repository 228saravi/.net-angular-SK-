using Microsoft.Extensions.Configuration;
using SK.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SK.Infrastructure
{
  public class EmailSender : IEmailSender
  {
    private class SendingSettings
    {
      public string SmtpHost { get; set; }
      public string User { get; set; }
      public string Password { get; set; }
      public int Port { get; set; }
    }

    private SendingSettings _settings;
    private SmtpClient _smtp;

    public EmailSender(IConfiguration configuration)
    {
      var settings = configuration.GetSection("Email").Get<SendingSettings>();

      this._settings = settings;

      this._smtp = new SmtpClient(settings.SmtpHost, settings.Port)
      {
        Credentials = new NetworkCredential(settings.User, settings.Password),
        EnableSsl = true,
      };
    }

    public async Task SendAsync(string body, string subject, IReadOnlyCollection<string> emails)
    {
      var message = new MailMessage() { IsBodyHtml = true };

      message.From = new MailAddress(this._settings.User); // Пока что это совпадает.
      message.Body = body;
      message.Subject = subject;

      foreach (var to in emails)
      {
        message.To.Add(to);
      }

      await this._smtp.SendMailAsync(message);
    }

    public void Dispose()
    {
      this._smtp.Dispose();
    }
  }
}
