using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APCapstoneProject.Migrations
{
    /// <inheritdoc />
    public partial class migration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "SalaryDisbursementDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationAccountNumber",
                table: "SalaryDisbursementDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IFSC",
                table: "SalaryDisbursementDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankName",
                table: "SalaryDisbursementDetails");

            migrationBuilder.DropColumn(
                name: "DestinationAccountNumber",
                table: "SalaryDisbursementDetails");

            migrationBuilder.DropColumn(
                name: "IFSC",
                table: "SalaryDisbursementDetails");
        }
    }
}
