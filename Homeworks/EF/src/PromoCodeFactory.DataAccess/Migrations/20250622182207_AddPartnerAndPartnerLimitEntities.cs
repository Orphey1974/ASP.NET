using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerAndPartnerLimitEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerManagerId",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "PromoCodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "PromoCodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreferenceId1",
                table: "PromoCodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId1",
                table: "Employees",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PartnerManagerId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partners_Employees_PartnerManagerId",
                        column: x => x.PartnerManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartnerLimits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Limit = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentCount = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PartnerId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerLimits_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_EmployeeId",
                table: "PromoCodes",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_PartnerId",
                table: "PromoCodes",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_PreferenceId1",
                table: "PromoCodes",
                column: "PreferenceId1");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RoleId1",
                table: "Employees",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLimits_PartnerId",
                table: "PartnerLimits",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_PartnerManagerId",
                table: "Partners",
                column: "PartnerManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Roles_RoleId1",
                table: "Employees",
                column: "RoleId1",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodes_Employees_EmployeeId",
                table: "PromoCodes",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodes_Partners_PartnerId",
                table: "PromoCodes",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCodes_Preferences_PreferenceId1",
                table: "PromoCodes",
                column: "PreferenceId1",
                principalTable: "Preferences",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Roles_RoleId1",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodes_Employees_EmployeeId",
                table: "PromoCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodes_Partners_PartnerId",
                table: "PromoCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoCodes_Preferences_PreferenceId1",
                table: "PromoCodes");

            migrationBuilder.DropTable(
                name: "PartnerLimits");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_EmployeeId",
                table: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_PartnerId",
                table: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_PreferenceId1",
                table: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_Employees_RoleId1",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "PreferenceId1",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "RoleId1",
                table: "Employees");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerManagerId",
                table: "PromoCodes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }
    }
}
