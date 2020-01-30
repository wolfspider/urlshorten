using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace urlshorten.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UrlViewModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    UrlHash = table.Column<int>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    ShortAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlViewModels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlViewModels");
        }
    }
}
