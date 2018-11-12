using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SK.Database
{

  //public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
  //{
  //  public DatabaseContext CreateDbContext(string[] args)
  //  {
  //    var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
  //    optionsBuilder.UseSqlServer("Data Source=.; Initial Catalog=SK.Database; Integrated Security=true;");
  //    return new DatabaseContext(optionsBuilder.Options);
  //  }
  //}

  public class DatabaseContextFactory
  {
    private IConfiguration _configuration;

    public DatabaseContextFactory(IConfiguration configuration)
    {
      this._configuration = configuration;
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder)
    {
      string connection = this._configuration.GetConnectionString("SK.Database");
      optionsBuilder.UseSqlServer(connection);
    }

    public DatabaseContext Create()
    {
      var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
      this.Configure(optionsBuilder);
      return new DatabaseContext(optionsBuilder.Options);
    }
  }

  public class DatabaseContext : IdentityDbContext<User>
  {
    private void SeedMinimalData(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<City>().HasData(new[] {
        new City { Id = CityIds.Msk, Name = "Москва" },
        new City { Id = CityIds.Spb, Name = "Санкт-Петербург" },
      });

      modelBuilder.Entity<Language>().HasData(new[] {
        new Language{ Id = LanguageIds.Ru, Name = "Русский" },
        new Language{ Id = LanguageIds.En, Name = "Английский" },
      });

      modelBuilder.Entity<ExperienceOption>().HasData(new[] {
        new ExperienceOption{ Id = ExperienceOptionIds.Month6, Name = "до 6 мес." },
        new ExperienceOption{ Id = ExperienceOptionIds.Month6_Year2, Name = "от 6 мес. до 2 лет" },
        new ExperienceOption{ Id = ExperienceOptionIds.Year2, Name = "от 2 лет" },
      });

      modelBuilder.Entity<ExpertDocument>().HasData(new[] {
        new ExpertDocument{ Id = ExpertDocumentIds.Passport, Name = "Паспорт"},
        new ExpertDocument{ Id = ExpertDocumentIds.MedicalBook, Name = "Медицинская книжка"},
        new ExpertDocument{ Id = ExpertDocumentIds.WorkPermit, Name = "Разрешение на работу в РФ"},
      });

      modelBuilder.Entity<ClothingSize>().HasData(new[] {
        new ClothingSize{ Id = ClothingSizeIds.XS, Name = "XS", Rank = 1 },
        new ClothingSize{ Id = ClothingSizeIds.S, Name = "S", Rank = 2 },
        new ClothingSize{ Id = ClothingSizeIds.M, Name = "M", Rank = 3 },
        new ClothingSize{ Id = ClothingSizeIds.L, Name = "L", Rank = 4 },
        new ClothingSize{ Id = ClothingSizeIds.XL, Name = "XL", Rank = 5 },
        new ClothingSize{ Id = ClothingSizeIds.XXL, Name = "XXL", Rank = 6 },
      });

      modelBuilder.Entity<Segment>().HasData(new[] {
        new Segment{ Id = SegmentIds.Democratic, Name = "Демократичный", Rank = 0 },
        new Segment{ Id = SegmentIds.Middle, Name = "Стандарт", Rank = 1, },
        new Segment{ Id = SegmentIds.Premium, Name = "Премиум", Rank = 2 },
      });

      modelBuilder.Entity<EventType>().HasData(new[] {
        new EventType { Id = EventTypeIds.Corporative, Name = "Корпоратив"},
        new EventType { Id = EventTypeIds.Sport, Name = "Спортивное мероприятие"},
        new EventType { Id = EventTypeIds.BusinessMeeting, Name = "Деловая встреча"},
        new EventType { Id = EventTypeIds.Wedding, Name = "Свадьба"},
        new EventType { Id = EventTypeIds.Birthday, Name = "День Рождения"},
        new EventType { Id = EventTypeIds.NewYear, Name = "Новый Год"},
        new EventType { Id = EventTypeIds.Tourists, Name = "Туристы"},
        new EventType { Id = EventTypeIds.Training, Name = "Тренинг"},
        new EventType { Id = EventTypeIds.StagParty, Name = "Мальчишник / Девишник"},
        new EventType { Id = EventTypeIds.FamilyCelebration, Name = "Семейное торжество"},
        new EventType { Id = EventTypeIds.Graduation, Name = "Выпускной"},
        new EventType { Id = EventTypeIds.Formal, Name = "Официальный приём"},
      });

      modelBuilder.Entity<EventFormat>().HasData(new[] {
        new EventFormat { Id = EventFormatIds.Furshet, Name = "Фуршет" },
        new EventFormat { Id = EventFormatIds.MainCourses, Name = "Основные курсы" },
        new EventFormat { Id = EventFormatIds.Smorgasbord, Name = "Шведский стол" },
        new EventFormat { Id = EventFormatIds.Сocktail, Name = "Велком / коктейль" },
        new EventFormat { Id = EventFormatIds.Barbecue, Name = "Барбекю" },
        new EventFormat { Id = EventFormatIds.Replacement, Name = "Подмена в заведение" },
      });

      modelBuilder.Entity<Speciality>().HasData(new[]
      {
        new Speciality
        {
          Id = SpecialityIds.Cook,
          Name = "Повар",
        },
        new Speciality
        {
          Id = SpecialityIds.Waiter,
          Name = "Официант",
        },
        new Speciality
        {
          Id = SpecialityIds.Barman,
          Name = "Бармен"
        }
      });

      modelBuilder.Entity<Specialization>().HasData(new[] {
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_HotShop, Name = "Повар горячего цеха", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_ColdShop, Name = "Повар холодного цеха", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_AllInOne, Name = "Универсал", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_Preparer, Name = "Заготовщик", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_Confectioner, Name = "Кондитер", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_Assistant, Name = "Помощник повара", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_Butcher, Name = "Мясник", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_Foreman, Name = "Бригадир", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_SousChef, Name = "Су-Шеф", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_Chef, Name = "Шеф", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_SushiMaker, Name = "Сушист", Rank = 1000 },
        new Specialization { SpecialityId = SpecialityIds.Cook, Id = SpecializationIds.Cook_PizzaMaker, Name = "Пиццмейкер", Rank = 1000 },

        new Specialization { SpecialityId = SpecialityIds.Barman, Id = SpecializationIds.Barman_Assistant, Name = "Помощник бармена" },
        new Specialization { SpecialityId = SpecialityIds.Barman, Id = SpecializationIds.Barman_Middle, Name = "Бармен" },
        new Specialization { SpecialityId = SpecialityIds.Barman, Id = SpecializationIds.Barman_Senior, Name = "Старший бармен" },

        new Specialization { SpecialityId = SpecialityIds.Waiter, Id = SpecializationIds.Waiter_Assistant, Name = "Помощник официанта" },
        new Specialization { SpecialityId = SpecialityIds.Waiter, Id = SpecializationIds.Waiter_Middle, Name = "Официант" },
        new Specialization { SpecialityId = SpecialityIds.Waiter, Id = SpecializationIds.Waiter_Senior, Name = "Старший официант" },
      });

      modelBuilder.Entity<Skill>().HasData(new[]
      {
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_RussianCuisine, Name = "Русская" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_UsbekCuisine, Name = "Узбекская" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_FranceCuisine, Name = "Французская" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_EuropeCuisine, Name = "Европейская" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_SpainCuisine, Name = "Испанская" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_BelgiumCuisine, Name = "Бельгийская" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_GermanCuisine, Name = "Немецкая" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_ChinaCuisine, Name = "Китайская" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_UsaCuisine, Name = "Американская" },
        new Skill { SpecialityId = SpecialityIds.Cook, GroupName="Кухня", Rank = 2000, Id = SkillIds.Cook_NewWorldCuisine, Name = "Нового света" },

        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Сocktail, Name = "Миксология/коктейли" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Tea, Name = "Чай" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Coffee, Name = "Кофе" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_SoftDrinks, Name = "Безалкогольные напитки" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_SoftCocktails, Name = "Безалкогольные коктейли/смузи" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Bear, Name = "Пиво" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Wine, Name = "Вино" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_StrongAlcohol, Name = "Крепкий алкоголь" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Tinctures, Name = "Настойки" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Aperitifs, Name = "Апперитивы" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Digestives, Name = "Дижестивы" },
        new Skill { SpecialityId = SpecialityIds.Barman, GroupName="Напитки", Rank = 2000, Id = SkillIds.Barman_Sommelier, Name = "Навыки сомелье" },


        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_RussianCuisine, Name = "Русская" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_UsbekCuisine, Name = "Узбекская" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_FranceCuisine, Name = "Французская" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_EuropeCuisine, Name = "Европейская" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_SpainCuisine, Name = "Испанская" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_BelgiumCuisine, Name = "Бельгийская" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_GermanCuisine, Name = "Немецкая" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_ChinaCuisine, Name = "Китайская" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_UsaCuisine, Name = "Американская" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Кухня", Rank = 2000, Id = SkillIds.Waiter_NewWorldCuisine, Name = "Нового света" },

        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Сocktail, Name = "Миксология/коктейли" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Tea, Name = "Чай" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Coffee, Name = "Кофе" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_SoftDrinks, Name = "Безалкогольные напитки" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_SoftCocktails, Name = "Безалкогольные коктейли/смузи" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Bear, Name = "Пиво" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Wine, Name = "Вино" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_StrongAlcohol, Name = "Крепкий алкоголь" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Tinctures, Name = "Настойки" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Aperitifs, Name = "Апперитивы" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Digestives, Name = "Дижестивы" },
        new Skill { SpecialityId = SpecialityIds.Waiter, GroupName="Напитки", Rank = 2000, Id = SkillIds.Waiter_Sommelier, Name = "Навыки сомелье" },
      });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Speciality>().HasMany(s => s.Skills).WithOne(s => s.Speciality).HasForeignKey(s => s.SpecialityId);
      modelBuilder.Entity<User>().HasOne(u => u.ExpertProfile).WithOne(p => p.User).HasForeignKey<ExpertProfile>(p => p.UserId);
      modelBuilder.Entity<User>().HasOne(u => u.Company).WithOne(c => c.User).HasForeignKey<Company>(c => c.UserId);
      modelBuilder.Entity<Connection>().HasOne(c => c.FeedbackForExpert).WithOne(f => f.Connection).HasForeignKey<Connection>(c => c.FeedbackForExpertId);
      modelBuilder.Entity<Connection>().HasOne(c => c.FeedbackForCompany).WithOne(f => f.Connection).HasForeignKey<Connection>(c => c.FeedbackForCompanyId);

      this.SeedMinimalData(modelBuilder);
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    { }

    #region DbSets

    public DbSet<City> Cities { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<ExperienceOption> ExperienceOptions { get; set; }
    public DbSet<ClothingSize> ClothingSizes { get; set; }
    public DbSet<ExpertDocument> ExpertDocuments { get; set; }
    public DbSet<Segment> Segments { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<EventFormat> EventFormats { get; set; }

    public DbSet<Speciality> Specialities { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<Skill> Skills { get; set; }

    public DbSet<ExpertProfile> ExpertProfiles { get; set; }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }

    public DbSet<Connection> Connections { get; set; }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    #endregion
  }
}
