using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutMeHtml",
                table: "ExpertProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "ExpertProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClothingSizeId",
                table: "ExpertProfiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExperienceInYears",
                table: "ExpertProfiles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "ExpertProfiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RatePerHour",
                table: "ExpertProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecializationId",
                table: "ExpertProfiles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpertDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpertProfileLanguage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpertProfileId = table.Column<long>(nullable: false),
                    LanguageId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertProfileLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertProfileLanguage_ExpertProfiles_ExpertProfileId",
                        column: x => x.ExpertProfileId,
                        principalTable: "ExpertProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpertProfileLanguage_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpertProfileDocument",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpertProfileId = table.Column<long>(nullable: false),
                    ExpertDocumentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertProfileDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertProfileDocument_ExpertDocuments_ExpertDocumentId",
                        column: x => x.ExpertDocumentId,
                        principalTable: "ExpertDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpertProfileDocument_ExpertProfiles_ExpertProfileId",
                        column: x => x.ExpertProfileId,
                        principalTable: "ExpertProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExpertDocuments",
                columns: new[] { "Id", "Name" },
                values: new object[] { "Passport", "Паспорт" });

            migrationBuilder.InsertData(
                table: "ExpertDocuments",
                columns: new[] { "Id", "Name" },
                values: new object[] { "MedicalBook", "Медицинская книжка" });

            migrationBuilder.InsertData(
                table: "ExpertDocuments",
                columns: new[] { "Id", "Name" },
                values: new object[] { "WorkPermit", "Разрешение на работу в РФ" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfiles_CityId",
                table: "ExpertProfiles",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfiles_ClothingSizeId",
                table: "ExpertProfiles",
                column: "ClothingSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfiles_SpecializationId",
                table: "ExpertProfiles",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfileDocument_ExpertDocumentId",
                table: "ExpertProfileDocument",
                column: "ExpertDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfileDocument_ExpertProfileId",
                table: "ExpertProfileDocument",
                column: "ExpertProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfileLanguage_ExpertProfileId",
                table: "ExpertProfileLanguage",
                column: "ExpertProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfileLanguage_LanguageId",
                table: "ExpertProfileLanguage",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpertProfiles_Cities_CityId",
                table: "ExpertProfiles",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpertProfiles_ClothingSizes_ClothingSizeId",
                table: "ExpertProfiles",
                column: "ClothingSizeId",
                principalTable: "ClothingSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpertProfiles_Specializations_SpecializationId",
                table: "ExpertProfiles",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpertProfiles_Cities_CityId",
                table: "ExpertProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpertProfiles_ClothingSizes_ClothingSizeId",
                table: "ExpertProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpertProfiles_Specializations_SpecializationId",
                table: "ExpertProfiles");

            migrationBuilder.DropTable(
                name: "ExpertProfileDocument");

            migrationBuilder.DropTable(
                name: "ExpertProfileLanguage");

            migrationBuilder.DropTable(
                name: "ExpertDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ExpertProfiles_CityId",
                table: "ExpertProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ExpertProfiles_ClothingSizeId",
                table: "ExpertProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ExpertProfiles_SpecializationId",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "AboutMeHtml",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "ClothingSizeId",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "ExperienceInYears",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "RatePerHour",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "ExpertProfiles");
        }
    }
}
