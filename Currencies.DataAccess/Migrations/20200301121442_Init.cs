using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Currencies.DataAccess.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsoCode = table.Column<string>(maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyExchangeRate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyExchangeRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyExchangeRate_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currency_IsoCode",
                table: "Currency",
                column: "IsoCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyExchangeRate_Code",
                table: "DailyExchangeRate",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyExchangeRate_CurrencyId",
                table: "DailyExchangeRate",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyExchangeRate");

            migrationBuilder.DropTable(
                name: "Currency");
        }
    }
}
