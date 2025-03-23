using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("2e23583f-d118-4295-8a80-09266b7497db"),
                column: "password_hash",
                value: "$2a$13$IiHfZUPeaA5zu22M57YZB.riJVHfuTwMpw4z9sVvpvdqstDRtt21O");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b8c35758-1881-47ad-8994-0529df7c70ed"),
                column: "password_hash",
                value: "$2a$13$IiHfZUPeaA5zu22M57YZB.riJVHfuTwMpw4z9sVvpvdqstDRtt21O");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("ee13b6c6-a98e-440b-93f8-ecf083d51052"),
                column: "password_hash",
                value: "$2a$13$IiHfZUPeaA5zu22M57YZB.riJVHfuTwMpw4z9sVvpvdqstDRtt21O");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("2e23583f-d118-4295-8a80-09266b7497db"),
                column: "password_hash",
                value: "$2a$13$7ZX.H.wI0yitiqiHnE9D8Om.T6F0boCdYS5EmKviXkl7m8uwefY4e");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b8c35758-1881-47ad-8994-0529df7c70ed"),
                column: "password_hash",
                value: "$2a$13$GFEm2Ijr3Jd4GsbRXp8Yn.YVrCAoatoFjfm458GFMpd5UFzyShqCq");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("ee13b6c6-a98e-440b-93f8-ecf083d51052"),
                column: "password_hash",
                value: "$2a$13$902uxeLjnX9nd.78KtdZNOUQNrzoCGvdug1dUiV33NRNNblqe7VA6");
        }
    }
}
