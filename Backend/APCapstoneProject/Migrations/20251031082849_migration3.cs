using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APCapstoneProject.Migrations
{
    /// <inheritdoc />
    public partial class migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Success",
                table: "SalaryDisbursementDetails",
                newName: "IsSuccessful");

            migrationBuilder.AddColumn<int>(
                name: "FailedCount",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartialSuccess",
                table: "Transactions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuccessfulCount",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalEmployees",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "SalaryDisbursementDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedCount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsPartialSuccess",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SuccessfulCount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TotalEmployees",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "IsSuccessful",
                table: "SalaryDisbursementDetails",
                newName: "Success");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "SalaryDisbursementDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
