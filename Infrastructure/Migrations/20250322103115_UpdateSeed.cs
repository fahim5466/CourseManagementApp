using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email", "email_verification_token_hash", "email_verification_token_hash_expires", "is_email_verified", "name", "password_hash", "refresh_token_expires", "refresh_token_hash" },
                values: new object[,]
                {
                    { new Guid("2e23583f-d118-4295-8a80-09266b7497db"), "student@test.com", null, null, true, "Student", "$2a$13$7ZX.H.wI0yitiqiHnE9D8Om.T6F0boCdYS5EmKviXkl7m8uwefY4e", null, null },
                    { new Guid("ee13b6c6-a98e-440b-93f8-ecf083d51052"), "staff@test.com", null, null, true, "Staff", "$2a$13$902uxeLjnX9nd.78KtdZNOUQNrzoCGvdug1dUiV33NRNNblqe7VA6", null, null }
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "role_id", "user_id" },
                values: new object[,]
                {
                    { new Guid("dcc31107-75b5-4504-8e87-0d654323c56d"), new Guid("2e23583f-d118-4295-8a80-09266b7497db") },
                    { new Guid("214e0c6b-e213-40d1-a1f2-169b0026f882"), new Guid("ee13b6c6-a98e-440b-93f8-ecf083d51052") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { new Guid("dcc31107-75b5-4504-8e87-0d654323c56d"), new Guid("2e23583f-d118-4295-8a80-09266b7497db") });

            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { new Guid("214e0c6b-e213-40d1-a1f2-169b0026f882"), new Guid("ee13b6c6-a98e-440b-93f8-ecf083d51052") });

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("2e23583f-d118-4295-8a80-09266b7497db"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("ee13b6c6-a98e-440b-93f8-ecf083d51052"));
        }
    }
}
