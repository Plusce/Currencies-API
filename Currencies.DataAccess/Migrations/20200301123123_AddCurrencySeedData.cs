using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Currencies.DataAccess.Migrations
{
    public partial class AddCurrencySeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "IsoCode" },
                values: new object[] { new Guid("ef936eae-942e-4cef-b3da-4cbb12e224aa"), "EUR" });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "IsoCode" },
                values: new object[] { new Guid("40b58018-0642-46b0-8ffa-c0b54b6673d1"), "USD" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("40b58018-0642-46b0-8ffa-c0b54b6673d1"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("ef936eae-942e-4cef-b3da-4cbb12e224aa"));
        }
    }
}
