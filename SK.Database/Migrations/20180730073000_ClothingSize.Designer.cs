﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SK.Database;

namespace SK.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20180730073000_ClothingSize")]
    partial class ClothingSize
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SK.Database.City", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Cities");

                    b.HasData(
                        new { Id = "Msk", Name = "Москва" },
                        new { Id = "Spb", Name = "Санкт-Петербург" }
                    );
                });

            modelBuilder.Entity("SK.Database.ClothingSize", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ClothingSizes");

                    b.HasData(
                        new { Id = "XS", Name = "XS" },
                        new { Id = "S", Name = "S" },
                        new { Id = "M", Name = "M" },
                        new { Id = "L", Name = "L" },
                        new { Id = "XL", Name = "XL" },
                        new { Id = "XXL", Name = "XXL" }
                    );
                });

            modelBuilder.Entity("SK.Database.Company", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LogoImageUrl");

                    b.Property<string>("Name");

                    b.Property<string>("ThumbnailImageUrl");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("SK.Database.Connection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ApprovingDate");

                    b.Property<string>("ConnectionStatus");

                    b.Property<string>("ConnectionType");

                    b.Property<long>("ExpertProfileId");

                    b.Property<DateTime>("RequestingDate");

                    b.Property<long>("VacancyId");

                    b.HasKey("Id");

                    b.HasIndex("ExpertProfileId");

                    b.HasIndex("VacancyId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("SK.Database.Event", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<string>("CityId");

                    b.Property<long>("CompanyId");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndTime");

                    b.Property<int>("EstimatedAverageCheck");

                    b.Property<int>("EstimatedGuestsCount");

                    b.Property<string>("EventFormatId");

                    b.Property<string>("EventTypeId");

                    b.Property<string>("Name");

                    b.Property<string>("SegmentId");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("EventFormatId");

                    b.HasIndex("EventTypeId");

                    b.HasIndex("SegmentId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("SK.Database.EventFormat", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Rank");

                    b.HasKey("Id");

                    b.ToTable("EventFormats");

                    b.HasData(
                        new { Id = "Furshet", Name = "Фуршет", Rank = 0 },
                        new { Id = "Smorgasbord", Name = "Шведский стол", Rank = 0 },
                        new { Id = "Сocktail", Name = "Велком / коктейль", Rank = 0 },
                        new { Id = "Barbecue", Name = "Барбекю", Rank = 0 }
                    );
                });

            modelBuilder.Entity("SK.Database.EventType", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Rank");

                    b.HasKey("Id");

                    b.ToTable("EventTypes");

                    b.HasData(
                        new { Id = "Corporative", Name = "Корпоратив", Rank = 0 },
                        new { Id = "Sport", Name = "Спортивное мероприятие", Rank = 0 },
                        new { Id = "BusinessMeeting", Name = "Деловая встреча", Rank = 0 },
                        new { Id = "Wedding", Name = "Свадьба", Rank = 0 },
                        new { Id = "Birthday", Name = "День Рождения", Rank = 0 },
                        new { Id = "NewYear", Name = "Новый Год", Rank = 0 },
                        new { Id = "Tourists", Name = "Туристы", Rank = 0 },
                        new { Id = "Training", Name = "Тренинг", Rank = 0 },
                        new { Id = "StagParty", Name = "Мальчишник / Девишник", Rank = 0 },
                        new { Id = "FamilyCelebration", Name = "Семейное торжество", Rank = 0 },
                        new { Id = "Graduation", Name = "Выпускной", Rank = 0 },
                        new { Id = "Formal", Name = "Официальный приём", Rank = 0 }
                    );
                });

            modelBuilder.Entity("SK.Database.ExpertProfile", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PhotoImageUrl");

                    b.Property<string>("SpecialityId");

                    b.Property<string>("ThumbnailImageUrl");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("ExpertProfiles");
                });

            modelBuilder.Entity("SK.Database.ExpertProfileSkill", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ExpertProfileId");

                    b.Property<string>("SkillId");

                    b.HasKey("Id");

                    b.HasIndex("ExpertProfileId");

                    b.HasIndex("SkillId");

                    b.ToTable("ExpertProfileSkill");
                });

            modelBuilder.Entity("SK.Database.Language", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Languages");

                    b.HasData(
                        new { Id = "Ru", Name = "Русский" },
                        new { Id = "En", Name = "Английский" }
                    );
                });

            modelBuilder.Entity("SK.Database.Segment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Rank");

                    b.HasKey("Id");

                    b.ToTable("Segments");

                    b.HasData(
                        new { Id = "Democratic", Name = "Демократичный", Rank = 0 },
                        new { Id = "Middle", Name = "Средний", Rank = 0 }
                    );
                });

            modelBuilder.Entity("SK.Database.Skill", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GroupName");

                    b.Property<string>("Name");

                    b.Property<int>("Rank");

                    b.Property<string>("SpecialityId");

                    b.HasKey("Id");

                    b.HasIndex("SpecialityId");

                    b.ToTable("Skills");

                    b.HasData(
                        new { Id = "Cook_RussianCuisine", GroupName = "Кухня", Name = "Русская", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_UsbekCuisine", GroupName = "Кухня", Name = "Узбекская", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_FranceCuisine", GroupName = "Кухня", Name = "Французская", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_EuropeCuisine", GroupName = "Кухня", Name = "Европейская", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_SpainCuisine", GroupName = "Кухня", Name = "Испанская", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_BelgiumCuisine", GroupName = "Кухня", Name = "Бельгийская", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_GermanCuisine", GroupName = "Кухня", Name = "Немецкая", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_ChinaCuisine", GroupName = "Кухня", Name = "Китайская", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_UsaCuisine", GroupName = "Кухня", Name = "Американская", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Cook_NewWorldCuisine", GroupName = "Кухня", Name = "Нового света", Rank = 2000, SpecialityId = "Cook" },
                        new { Id = "Barman_Сocktail", GroupName = "Напитки", Name = "Миксология/коктейли", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_Tea", GroupName = "Напитки", Name = "Чай", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_Coffee", GroupName = "Напитки", Name = "Кофе", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_SoftDrinks", GroupName = "Напитки", Name = "Безалкогольные напитки", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_SoftCocktails", GroupName = "Напитки", Name = "Безалкогольные коктейли/смузи", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_Bear", GroupName = "Напитки", Name = "Пиво", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_Wine", GroupName = "Напитки", Name = "Вино", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_StrongAlcohol", GroupName = "Напитки", Name = "Крепкий алкоголь", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_Tinctures", GroupName = "Напитки", Name = "Настойки", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_Aperitifs", GroupName = "Напитки", Name = "Апперитивы", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_Digestives", GroupName = "Напитки", Name = "Дижестивы", Rank = 2000, SpecialityId = "Barman" },
                        new { Id = "Barman_Sommelier", GroupName = "Напитки", Name = "Навыки сомелье", Rank = 2000, SpecialityId = "Barman" }
                    );
                });

            modelBuilder.Entity("SK.Database.Speciality", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Rank");

                    b.HasKey("Id");

                    b.ToTable("Specialities");

                    b.HasData(
                        new { Id = "Cook", Name = "Повар", Rank = 0 },
                        new { Id = "Waiter", Name = "Официант", Rank = 0 },
                        new { Id = "Barman", Name = "Бармен", Rank = 0 }
                    );
                });

            modelBuilder.Entity("SK.Database.Specialization", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Rank");

                    b.Property<string>("SpecialityId");

                    b.HasKey("Id");

                    b.HasIndex("SpecialityId");

                    b.ToTable("Specializations");

                    b.HasData(
                        new { Id = "Cook_HotShop", Name = "Повар горячего цеха", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_ColdShop", Name = "Повар холодного цеха", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_AllInOne", Name = "Универсал", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_Preparer", Name = "Заготовщик", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_Confectioner", Name = "Кондитер", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_Assistant", Name = "Помощник повара", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_Butcher", Name = "Мясник", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_Foreman", Name = "Бригадир", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_SousChef", Name = "Су-Шеф", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Cook_Chef", Name = "Шеф", Rank = 1000, SpecialityId = "Cook" },
                        new { Id = "Barman_Assistant", Name = "Помощник бармена", Rank = 0, SpecialityId = "Barman" },
                        new { Id = "Barman_Middle", Name = "Бармен", Rank = 0, SpecialityId = "Barman" },
                        new { Id = "Barman_Senior", Name = "Старший бармен", Rank = 0, SpecialityId = "Barman" },
                        new { Id = "Waiter_Assistant", Name = "Помощник официанта", Rank = 0, SpecialityId = "Waiter" },
                        new { Id = "Waiter_Middle", Name = "Официант", Rank = 0, SpecialityId = "Waiter" },
                        new { Id = "Waiter_Senior", Name = "Старший официант", Rank = 0, SpecialityId = "Waiter" }
                    );
                });

            modelBuilder.Entity("SK.Database.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("SK.Database.Vacancy", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount");

                    b.Property<long>("EventId");

                    b.Property<int>("ExperienceInYears");

                    b.Property<string>("LanguageId");

                    b.Property<int>("RatePerHour");

                    b.Property<string>("SpecialityId");

                    b.Property<string>("SpecializationId");

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("WorkingHours");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("SpecializationId");

                    b.ToTable("Vacancies");
                });

            modelBuilder.Entity("SK.Database.VacancySkill", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("SkillId");

                    b.Property<long>("VacancyId");

                    b.HasKey("Id");

                    b.HasIndex("SkillId");

                    b.HasIndex("VacancyId");

                    b.ToTable("VacancySkill");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SK.Database.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SK.Database.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SK.Database.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SK.Database.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SK.Database.Company", b =>
                {
                    b.HasOne("SK.Database.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SK.Database.Connection", b =>
                {
                    b.HasOne("SK.Database.ExpertProfile", "ExpertProfile")
                        .WithMany("Connections")
                        .HasForeignKey("ExpertProfileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SK.Database.Vacancy", "Vacancy")
                        .WithMany("Connections")
                        .HasForeignKey("VacancyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SK.Database.Event", b =>
                {
                    b.HasOne("SK.Database.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId");

                    b.HasOne("SK.Database.Company", "Company")
                        .WithMany("Events")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SK.Database.EventFormat", "EventFormat")
                        .WithMany()
                        .HasForeignKey("EventFormatId");

                    b.HasOne("SK.Database.EventType", "EventType")
                        .WithMany()
                        .HasForeignKey("EventTypeId");

                    b.HasOne("SK.Database.Segment", "Segment")
                        .WithMany("Events")
                        .HasForeignKey("SegmentId");
                });

            modelBuilder.Entity("SK.Database.ExpertProfile", b =>
                {
                    b.HasOne("SK.Database.Speciality", "Speciality")
                        .WithMany()
                        .HasForeignKey("SpecialityId");

                    b.HasOne("SK.Database.User", "User")
                        .WithOne("ExpertProfile")
                        .HasForeignKey("SK.Database.ExpertProfile", "UserId");
                });

            modelBuilder.Entity("SK.Database.ExpertProfileSkill", b =>
                {
                    b.HasOne("SK.Database.ExpertProfile", "ExpertProfile")
                        .WithMany("ExpertProfileSkills")
                        .HasForeignKey("ExpertProfileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SK.Database.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId");
                });

            modelBuilder.Entity("SK.Database.Skill", b =>
                {
                    b.HasOne("SK.Database.Speciality", "Speciality")
                        .WithMany("Skills")
                        .HasForeignKey("SpecialityId");
                });

            modelBuilder.Entity("SK.Database.Specialization", b =>
                {
                    b.HasOne("SK.Database.Speciality", "Speciality")
                        .WithMany("Specializations")
                        .HasForeignKey("SpecialityId");
                });

            modelBuilder.Entity("SK.Database.Vacancy", b =>
                {
                    b.HasOne("SK.Database.Event", "Event")
                        .WithMany("Vacancies")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SK.Database.Language", "Language")
                        .WithMany("Vacancies")
                        .HasForeignKey("LanguageId");

                    b.HasOne("SK.Database.Speciality", "Speciality")
                        .WithMany("Vacancies")
                        .HasForeignKey("SpecialityId");

                    b.HasOne("SK.Database.Specialization", "Specialization")
                        .WithMany()
                        .HasForeignKey("SpecializationId");
                });

            modelBuilder.Entity("SK.Database.VacancySkill", b =>
                {
                    b.HasOne("SK.Database.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId");

                    b.HasOne("SK.Database.Vacancy", "Vacancy")
                        .WithMany("VacancySkills")
                        .HasForeignKey("VacancyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
