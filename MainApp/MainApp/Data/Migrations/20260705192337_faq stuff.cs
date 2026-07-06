using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class faqstuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FAQConversations",
                columns: table => new
                {
                    FAQConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQConversations", x => x.FAQConversationId);
                    table.ForeignKey(
                        name: "FK_FAQConversations_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAQMessages",
                columns: table => new
                {
                    FAQMessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderType = table.Column<int>(type: "int", nullable: false),
                    FAQConversationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQMessages", x => x.FAQMessageId);
                    table.ForeignKey(
                        name: "FK_FAQMessages_FAQConversations_FAQConversationId",
                        column: x => x.FAQConversationId,
                        principalTable: "FAQConversations",
                        principalColumn: "FAQConversationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FAQConversations_IdentityUserId",
                table: "FAQConversations",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQMessages_FAQConversationId",
                table: "FAQMessages",
                column: "FAQConversationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAQMessages");

            migrationBuilder.DropTable(
                name: "FAQConversations");
        }
    }
}
