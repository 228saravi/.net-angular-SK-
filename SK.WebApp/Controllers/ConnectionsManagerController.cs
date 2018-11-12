using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SK.Domain;
using SK.Database;

namespace SK.WebApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ConnectionsManagerController : ControllerBase
  {
    public class AmooutIsFullException : HttpTransferableException
    {
      public AmooutIsFullException(ConnectionsManager.AmooutIsFullException e)
        : base("AMOUNT_IS_FULL", e.Message)
      { }
    }

    public class AlreadyConnectedException : HttpTransferableException
    {
      public AlreadyConnectedException(ConnectionsManager.AlreadyConnectedException e)
        : base("ALREADY_CONNECTED", e.Message)
      { }
    }

    public class NotYourProfileException : HttpTransferableException
    {
      public NotYourProfileException(ConnectionsManager.NotYourProfileException e)
        : base("NOT_YOUR_PROFILE", e.Message)
      { }
    }

    public class NotYourCompanyException : HttpTransferableException
    {
      public NotYourCompanyException(ConnectionsManager.NotYourCompanyException e)
        : base("NOT_YOUR_COMPANY", e.Message)
      { }
    }

    public class NotYourConnectionException : HttpTransferableException
    {
      public NotYourConnectionException(ConnectionsManager.NotYourConnectionException e)
        : base("NOT_YOUR_CONNECTION", e.Message)
      { }
    }

    public class TooLateToCancelConnectionException : HttpTransferableException
    {
      public TooLateToCancelConnectionException(ConnectionsManager.TooLateToCancelConnectionException e)
        : base("TOO_LATE_TO_CANCEL_CONNECTION", e.Message)
      { }
    }

    private DatabaseContext _context;
    private ConnectionsManager _connectionsManager;

    public ConnectionsManagerController(DatabaseContext context, ConnectionsManager connectionsManager)
    {
      this._context = context;
      this._connectionsManager = connectionsManager;
    }

    [HttpGet("[action]")]
    [Authorize()]
    public async Task<ConnectionsManager.ExpertConnectionsRes> GetExpertConnections([FromQuery]ConnectionsManager.ExpertConnectionsReq req)
    {
      try
      {
        var res = await this._connectionsManager.GetExpertConnections(req, this._context);
        return res;
      }
      catch (Exception e)
      {
        var notYourProfileException = e as ConnectionsManager.NotYourProfileException;
        if (notYourProfileException != null)
        {
          throw new NotYourProfileException(notYourProfileException);
        }

        throw e;
      }
    }

    [HttpGet("[action]")]
    [Authorize()]
    public async Task<ConnectionsManager.VacancyConnectionsRes> GetVacancyConnections([FromQuery]ConnectionsManager.VacancyConnectionsReq req)
    {
      try
      {
        var res = await this._connectionsManager.GetVacancyConnecctions(req, this._context);
        return res;
      }
      catch (Exception e)
      {
        var notYourCompanyException = e as ConnectionsManager.NotYourCompanyException;
        if (notYourCompanyException != null)
        {
          throw new NotYourCompanyException(notYourCompanyException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "expert")]
    public async Task RegisterExpertToVacancyConnection([FromBody]ConnectionsManager.RegisterExpertToVacancyConnectionReq req)
    {
      try
      {
        await this._connectionsManager.RegisterExpertToVacancyConnection(this._context, req);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        var amountIsFullException = e as ConnectionsManager.AmooutIsFullException;
        if (amountIsFullException != null)
        {
          throw new AmooutIsFullException(amountIsFullException);
        }

        var alreadyConnectedException = e as ConnectionsManager.AlreadyConnectedException;
        if (alreadyConnectedException != null)
        {
          throw new AlreadyConnectedException(alreadyConnectedException);
        }

        var notYourProfileException = e as ConnectionsManager.NotYourProfileException;
        if (notYourProfileException != null)
        {
          throw new NotYourProfileException(notYourProfileException);
        }

        var profileIsNotPublishedException = e as ExpertProfileDetailsUpdator.ProfileIsNotPublishedException;
        if (profileIsNotPublishedException != null)
        {
          throw new ExpertProfileDetailsUpdatorController.ProfileIsNotPublishedException(profileIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "company-admin")]
    public async Task RegisterVacancyToExpertConnection([FromBody]ConnectionsManager.RegisterVacancyToExpertConnectionReq req)
    {
      try
      {
        await this._connectionsManager.RegisterVacancyToExpertConnection(this._context, req);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        var amountIsFullException = e as ConnectionsManager.AmooutIsFullException;
        if (amountIsFullException != null)
        {
          throw new AmooutIsFullException(amountIsFullException);
        }

        var alreadyConnectedException = e as ConnectionsManager.AlreadyConnectedException;
        if (alreadyConnectedException != null)
        {
          throw new AlreadyConnectedException(alreadyConnectedException);
        }

        var notYourCompanyException = e as ConnectionsManager.NotYourCompanyException;
        if (notYourCompanyException != null)
        {
          throw new NotYourCompanyException(notYourCompanyException);
        }

        var companyIsNotPublishedException = e as CompanyDetailsUpdator.CompanyIsNotPublishedException;
        if (companyIsNotPublishedException != null)
        {
          throw new CompanyDetailsUpdatorController.CompanyIsNotPublishedException(companyIsNotPublishedException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    [Authorize()]
    public async Task CancelConnection([FromBody]ConnectionsManager.CancelConnectionReq req)
    {
      try
      {
        await this._connectionsManager.CancelConnection(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        var notYourConnectionException = e as ConnectionsManager.NotYourConnectionException;
        if (notYourConnectionException != null)
        {
          throw new NotYourConnectionException(notYourConnectionException);
        }

        var tooLateToCancelConnectionException = e as ConnectionsManager.TooLateToCancelConnectionException;
        if (tooLateToCancelConnectionException != null)
        {
          throw new TooLateToCancelConnectionException(tooLateToCancelConnectionException);
        }

        throw e;
      }
    }

    [HttpPost("[action]")]
    [Authorize()]
    public async Task ApproveConnection([FromBody]ConnectionsManager.ApproveConnectionReq req)
    {
      try
      {
        await this._connectionsManager.ApproveConnection(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    [HttpPost("[action]")]
    [Authorize()]
    public async Task PostFeedback([FromBody]ConnectionsManager.PostFeedbackReq req)
    {
      try
      {
        await this._connectionsManager.PostFeedback(req, this._context);
        await this._context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }
}