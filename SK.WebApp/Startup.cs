using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SK.Database;
using SK.Domain;
using SK.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SK.WebApp
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
      Configuration = configuration;
      Env = env;
    }

    public IConfiguration Configuration { get; }
    public IHostingEnvironment Env { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      //string connection = Configuration.GetConnectionString("SK.Database");

      var databaseContextFactory = new DatabaseContextFactory(this.Configuration);

      Action<DbContextOptionsBuilder> sqlDatabaseContextConfigAction = (DbContextOptionsBuilder options) =>
      {
        databaseContextFactory.Configure(options);
      };

      services.AddDbContext<DatabaseContext>(sqlDatabaseContextConfigAction);

      services.AddIdentity<User, IdentityRole>(options =>
      {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
      })
      .AddEntityFrameworkStores<DatabaseContext>()
      .AddDefaultTokenProviders();

      services.ConfigureApplicationCookie(options =>
      {
        options.Events.OnRedirectToAccessDenied = async (context) =>
        {
          if (context.Request.Path.Value.StartsWith("/api"))
          {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          }
        };

        options.Events.OnRedirectToLogin = async (context) =>
        {
          if (context.Request.Path.Value.StartsWith("/api"))
          {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          }
        };
      });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      services.AddSignalR();

      // In production, the Angular files will be served from this directory
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ng/dist/sk-web-app";
      });

      services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory>();

      services.AddSingleton(databaseContextFactory);
      services.AddSingleton<Domain.Chat>();
      services.AddTransient<Domain.IEmailSender, EmailSender>();
      services.AddTransient<Domain.ICurrentUserService, CurrentUserService>();
      services.AddTransient<Domain.CitiesDirectory>();
      services.AddTransient<Domain.LanguagesDirectory>();
      services.AddTransient<Domain.SegmentsDirectory>();
      services.AddTransient<Domain.EventTypesDirectory>();
      services.AddTransient<Domain.EventFormatsDirectory>();
      services.AddTransient<Domain.SkillsMatrixDirectory>();
      services.AddTransient<Domain.ClothingSizesDirectory>();
      services.AddTransient<Domain.ExpertDocumentsDirectory>();
      services.AddTransient<Domain.ExperienceOptionsDirectory>();
      services.AddTransient<Domain.VacanciesSearcher>();
      services.AddTransient<Domain.VacancyDetailsProvider>();
      services.AddTransient<Domain.VacancyDetailsUpdator>();
      services.AddTransient<Domain.EventDetailsProvider>();
      services.AddTransient<Domain.EventDetailsUpdator>();
      services.AddTransient<Domain.CompanyDetailsProvider>();
      services.AddTransient<Domain.CompanyDetailsUpdator>();
      services.AddTransient<Domain.ExpertsSearcher>();
      services.AddTransient<Domain.ConnectionsManager>();
      services.AddTransient<Domain.ExpertProfileDetailsProvider>();
      services.AddTransient<Domain.ExpertProfileDetailsUpdator>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
    {
      if (env.IsDevelopment() || env.IsStaging())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/error");
        app.UseHsts();
      }

      app.UseMiddleware<ErrorHandlingMiddleware>();
      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller}/{action=Index}/{id?}");
      });

      app.UseSignalR(routes =>
      {
        routes.MapHub<ChatHub>("/chatHub");
      });

      app.UseSpa(spa =>
      {
        // To learn more about options for serving an Angular SPA from ASP.NET Core,
        // see https://go.microsoft.com/fwlink/?linkid=864501

        spa.Options.SourcePath = "ng";

        spa.UseSpaPrerendering(options =>
        {
          options.BootModulePath = $"{spa.Options.SourcePath}/dist-server/main.js";
          options.ExcludeUrls = new[]
          {
              "/",
              "/error-page",
              "/experts",
              "/connections",
              "/dialogs",
              "/login",
          };
        });

      });

      // Prepare db.
      using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {
        var context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
        await context.Database.MigrateAsync();
      }

      using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {
        var rolesManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
        var roleNamesToCreate = new[] { "expert", "company-admin" };

        foreach (var roleName in roleNamesToCreate)
        {
          var role = await rolesManager.FindByNameAsync(roleName);
          if (role == null)
          {
            await rolesManager.CreateAsync(new IdentityRole(roleName));
          }
        }
      }
    }
  }
}
