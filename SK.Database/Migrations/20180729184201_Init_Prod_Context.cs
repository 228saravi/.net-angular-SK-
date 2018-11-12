using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Init_Prod_Context : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    AccessFailedCount = table.Column<int>(nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventFormats",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventFormats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Segments",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    LogoImageUrl = table.Column<string>(nullable: true),
                    ThumbnailImageUrl = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpertProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    PhotoImageUrl = table.Column<string>(nullable: true),
                    ThumbnailImageUrl = table.Column<string>(nullable: true),
                    SpecialityId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertProfiles_Specialities_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpertProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    GroupName = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false),
                    SpecialityId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_Specialities_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false),
                    SpecialityId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Specializations_Specialities_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    EstimatedGuestsCount = table.Column<int>(nullable: false),
                    EstimatedAverageCheck = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CompanyId = table.Column<long>(nullable: false),
                    CityId = table.Column<string>(nullable: true),
                    SegmentId = table.Column<string>(nullable: true),
                    EventTypeId = table.Column<string>(nullable: true),
                    EventFormatId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_EventFormats_EventFormatId",
                        column: x => x.EventFormatId,
                        principalTable: "EventFormats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Segments_SegmentId",
                        column: x => x.SegmentId,
                        principalTable: "Segments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpertProfileSkill",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpertProfileId = table.Column<long>(nullable: false),
                    SkillId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertProfileSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertProfileSkill_ExpertProfiles_ExpertProfileId",
                        column: x => x.ExpertProfileId,
                        principalTable: "ExpertProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpertProfileSkill_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<long>(nullable: false),
                    SpecialityId = table.Column<string>(nullable: true),
                    SpecializationId = table.Column<string>(nullable: true),
                    LanguageId = table.Column<string>(nullable: true),
                    ExperienceInYears = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    WorkingHours = table.Column<int>(nullable: false),
                    RatePerHour = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacancies_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vacancies_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vacancies_Specialities_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vacancies_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConnectionType = table.Column<string>(nullable: true),
                    ConnectionStatus = table.Column<string>(nullable: true),
                    ExpertProfileId = table.Column<long>(nullable: false),
                    VacancyId = table.Column<long>(nullable: false),
                    RequestingDate = table.Column<DateTime>(nullable: false),
                    ApprovingDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_ExpertProfiles_ExpertProfileId",
                        column: x => x.ExpertProfileId,
                        principalTable: "ExpertProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Connections_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacancySkill",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VacancyId = table.Column<long>(nullable: false),
                    SkillId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancySkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancySkill_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancySkill_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "Msk", "Москва" },
                    { "Spb", "Санкт-Петербург" }
                });

            migrationBuilder.InsertData(
                table: "EventFormats",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[,]
                {
                    { "Furshet", "Фуршет", 0 },
                    { "Smorgasbord", "Шведский стол", 0 },
                    { "Сocktail", "Велком / коктейль", 0 },
                    { "Barbecue", "Барбекю", 0 }
                });

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[,]
                {
                    { "Formal", "Официальный приём", 0 },
                    { "Graduation", "Выпускной", 0 },
                    { "FamilyCelebration", "Семейное торжество", 0 },
                    { "StagParty", "Мальчишник / Девишник", 0 },
                    { "Training", "Тренинг", 0 },
                    { "Tourists", "Туристы", 0 },
                    { "Birthday", "День Рождения", 0 },
                    { "Wedding", "Свадьба", 0 },
                    { "BusinessMeeting", "Деловая встреча", 0 },
                    { "Sport", "Спортивное мероприятие", 0 },
                    { "Corporative", "Корпоратив", 0 },
                    { "NewYear", "Новый Год", 0 }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "Ru", "Русский" },
                    { "En", "Английский" }
                });

            migrationBuilder.InsertData(
                table: "Segments",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[,]
                {
                    { "Democratic", "Демократичный", 0 },
                    { "Middle", "Средний", 0 }
                });

            migrationBuilder.InsertData(
                table: "Specialities",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[,]
                {
                    { "Waiter", "Официант", 0 },
                    { "Cook", "Повар", 0 },
                    { "Barman", "Бармен", 0 }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "GroupName", "Name", "Rank", "SpecialityId" },
                values: new object[,]
                {
                    { "Cook_RussianCuisine", "Кухня", "Русская", 2000, "Cook" },
                    { "Barman_Coffee", "Напитки", "Кофе", 2000, "Barman" },
                    { "Barman_SoftDrinks", "Напитки", "Безалкогольные напитки", 2000, "Barman" },
                    { "Barman_SoftCocktails", "Напитки", "Безалкогольные коктейли/смузи", 2000, "Barman" },
                    { "Barman_Bear", "Напитки", "Пиво", 2000, "Barman" },
                    { "Barman_Wine", "Напитки", "Вино", 2000, "Barman" },
                    { "Barman_StrongAlcohol", "Напитки", "Крепкий алкоголь", 2000, "Barman" },
                    { "Barman_Tinctures", "Напитки", "Настойки", 2000, "Barman" },
                    { "Barman_Aperitifs", "Напитки", "Апперитивы", 2000, "Barman" },
                    { "Barman_Digestives", "Напитки", "Дижестивы", 2000, "Barman" },
                    { "Barman_Tea", "Напитки", "Чай", 2000, "Barman" },
                    { "Barman_Sommelier", "Напитки", "Навыки сомелье", 2000, "Barman" },
                    { "Cook_NewWorldCuisine", "Кухня", "Нового света", 2000, "Cook" },
                    { "Cook_UsaCuisine", "Кухня", "Американская", 2000, "Cook" },
                    { "Cook_ChinaCuisine", "Кухня", "Китайская", 2000, "Cook" },
                    { "Cook_GermanCuisine", "Кухня", "Немецкая", 2000, "Cook" },
                    { "Cook_BelgiumCuisine", "Кухня", "Бельгийская", 2000, "Cook" },
                    { "Cook_SpainCuisine", "Кухня", "Испанская", 2000, "Cook" },
                    { "Cook_EuropeCuisine", "Кухня", "Европейская", 2000, "Cook" },
                    { "Cook_FranceCuisine", "Кухня", "Французская", 2000, "Cook" },
                    { "Cook_UsbekCuisine", "Кухня", "Узбекская", 2000, "Cook" },
                    { "Barman_Сocktail", "Напитки", "Миксология/коктейли", 2000, "Barman" }
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "Id", "Name", "Rank", "SpecialityId" },
                values: new object[,]
                {
                    { "Barman_Assistant", "Помощник бармена", 0, "Barman" },
                    { "Cook_SousChef", "Су-Шеф", 1000, "Cook" },
                    { "Waiter_Middle", "Официант", 0, "Waiter" },
                    { "Waiter_Assistant", "Помощник официанта", 0, "Waiter" },
                    { "Cook_Chef", "Шеф", 1000, "Cook" },
                    { "Barman_Middle", "Бармен", 0, "Barman" },
                    { "Cook_Foreman", "Бригадир", 1000, "Cook" },
                    { "Cook_Butcher", "Мясник", 1000, "Cook" },
                    { "Cook_Assistant", "Помощник повара", 1000, "Cook" },
                    { "Cook_Confectioner", "Кондитер", 1000, "Cook" },
                    { "Cook_Preparer", "Заготовщик", 1000, "Cook" },
                    { "Cook_AllInOne", "Универсал", 1000, "Cook" },
                    { "Cook_ColdShop", "Повар холодного цеха", 1000, "Cook" },
                    { "Cook_HotShop", "Повар горячего цеха", 1000, "Cook" },
                    { "Waiter_Senior", "Старший официант", 0, "Waiter" },
                    { "Barman_Senior", "Старший бармен", 0, "Barman" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId",
                table: "Companies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_ExpertProfileId",
                table: "Connections",
                column: "ExpertProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_VacancyId",
                table: "Connections",
                column: "VacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CityId",
                table: "Events",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CompanyId",
                table: "Events",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventFormatId",
                table: "Events",
                column: "EventFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTypeId",
                table: "Events",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SegmentId",
                table: "Events",
                column: "SegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfiles_SpecialityId",
                table: "ExpertProfiles",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfiles_UserId",
                table: "ExpertProfiles",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfileSkill_ExpertProfileId",
                table: "ExpertProfileSkill",
                column: "ExpertProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfileSkill_SkillId",
                table: "ExpertProfileSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SpecialityId",
                table: "Skills",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_SpecialityId",
                table: "Specializations",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_EventId",
                table: "Vacancies",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_LanguageId",
                table: "Vacancies",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_SpecialityId",
                table: "Vacancies",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_SpecializationId",
                table: "Vacancies",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancySkill_SkillId",
                table: "VacancySkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancySkill_VacancyId",
                table: "VacancySkill",
                column: "VacancyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "ExpertProfileSkill");

            migrationBuilder.DropTable(
                name: "VacancySkill");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ExpertProfiles");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Vacancies");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "EventFormats");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Segments");

            migrationBuilder.DropTable(
                name: "Specialities");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
