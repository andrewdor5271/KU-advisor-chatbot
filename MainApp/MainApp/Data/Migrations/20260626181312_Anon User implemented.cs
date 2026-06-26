using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AnonUserimplemented : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_UserId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_UserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Conversations");

            migrationBuilder.AddColumn<int>(
                name: "AnonUserId",
                table: "Conversations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastChangeDatetime",
                table: "Conversations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AnonUsers",
                columns: table => new
                {
                    AnonUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeDatetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonUsers", x => x.AnonUserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_AnonUserId",
                table: "Conversations",
                column: "AnonUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_IdentityUserId",
                table: "Conversations",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AnonUsers_AnonUserId",
                table: "Conversations",
                column: "AnonUserId",
                principalTable: "AnonUsers",
                principalColumn: "AnonUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_IdentityUserId",
                table: "Conversations",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AnonUsers_AnonUserId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_IdentityUserId",
                table: "Conversations");

            migrationBuilder.DropTable(
                name: "AnonUsers");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_AnonUserId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_IdentityUserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "AnonUserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastChangeDatetime",
                table: "Conversations");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UserId",
                table: "Conversations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_UserId",
                table: "Conversations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
