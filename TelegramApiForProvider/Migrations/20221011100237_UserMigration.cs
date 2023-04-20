using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramApiForProvider.Migrations
{
    public partial class UserMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PartnerName = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    OrderNumber = table.Column<string>(nullable: true),
                    CreateDatetime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    ChatId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MessageId = table.Column<int>(nullable: true),
                    ChatId = table.Column<long>(nullable: false),
                    IsAccept = table.Column<bool>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderMessages_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderMessages_OrderId",
                table: "OrderMessages",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderMessages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
